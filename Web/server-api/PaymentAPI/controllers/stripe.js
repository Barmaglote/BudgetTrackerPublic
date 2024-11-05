import dotenv from 'dotenv'
dotenv.config('')
import { createLogger } from '../helpers/logger.js'
import Stripe from 'stripe';
import Payment from './../models/payment/model.js'
import { sendToQueue } from '../helpers/rabbitmq.js';

const logger = createLogger(process.env.SEQ_LOG_ADDR, process.env.SEQ_LOG_KEY);
const stripe = Stripe(process.env.STRIPE_PRIVATE_KEY);

const plans = {
  'T01': {
    id: 'T01',
    amount: 1900,
    currency: 'usd',
    recurring: { interval: 'month' },   // monthly recurring
    trialDays: 3,                       // 3 days from now of trial period
    priceId: 'T01-1900-usd',
    recurring: 'month',
    productName: 'Premium subscribtion',
  }
}

export async function Subscribe(req, res) {
  const { planId, paymentMethodId, email, userId, provider } = req.body;

  if (!planId || !paymentMethodId || !email || !userId || !provider) {
    logger.error('Error creating subscribtion',{ planId, paymentMethodId, email, userId });
    res.status(500).send(`Error creating payment intent`);
    return;
  }

  const plan = plans[planId];
  try {
    const priceId = await createOrRetrievePrice(plan.priceId, plan.amount, plan.currency, plan.recurring, plan.productName);
    const result = await createSubscribtion(email, paymentMethodId, priceId);

    if (result) {
      await sendToQueue("subcribtionsResults", { email, planId, subscribtionId: result, status: 'subscribed', date: new Date(), userId, provider });
    }

    res.status(result ? 200 : 500).send(result != null);
  } catch (error) {
    console.log(error)
    res.status(500).send(`Error creating Subscription: ${error.message}`);
  }
}

export async function CreateIntentToken(req, res) {
  const { planId, userId, email } = req.body;

  if (!planId || !userId || !email) {
    logger.error('Error creating payment intent',{ planId, userId });
    res.status(500).send(`Error creating payment intent`);
    return;
  }

  const plan = plans[planId];
  const trialEnd = new Date();
  trialEnd.setDate(trialEnd.getDate() + plan?.trialDays);

  try {
    const paymentIntent = await stripe.paymentIntents.create({
      amount: plan?.amount,
      currency: plan?.currency,
      payment_method_types: ['card'],
      metadata: {
        planId,
        userId,
        amount: plan?.amount,
        currency: plan?.currency,
        email: email
      },
      setup_future_usage: 'off_session'
    });

    res.json({ clientSecret: paymentIntent.client_secret });
  } catch (error) {
    console.log(error)
    res.status(500).send(`Error creating PaymentIntent: ${error.message}`);
  }
}

export async function RegisterPayment(request, response) {
  const sig = request.headers['stripe-signature'];
  let event;

  try {
    event = stripe.webhooks.constructEvent(request.body, sig, process.env.STRIPE_WEBHOOK_SECRET);
  } catch (err) {
    console.log(`⚠️  Webhook signature verification failed.`, err);
    return response.sendStatus(400);
  }

  if (event.type === 'payment_intent.succeeded') {
    const paymentIntent = event.data.object;
    console.log("metadata", paymentIntent.metadata);
    try {
      const payment = await Payment.create({ paymentIntent, createUserId: paymentIntent.metadata.userId, createDate: new Date() });
      await payment.save()
      console.log(`PaymentIntent ${paymentIntent.id} for ${paymentIntent.amount} was successful!`);
      response.status(200).send(true);
    } catch {
      response.status(500).send(false);
    }
  } else {
    console.log(`Unhandled event type ${event.type}.`);
    response.status(500).send(false);
  }
}

async function findOrCreateCustomer(email, paymentMethodId) {
  const customers = await stripe.customers.list({
    email: email,
    limit: 1
  });

  let customer;
  if (customers.data.length > 0) {
    customer = customers.data[0];
  } else {
    customer = await stripe.customers.create({
      email: email,
      payment_method: paymentMethodId
    });
    await stripe.paymentMethods.attach(paymentMethodId, {
      customer: customer.id
    });
    await stripe.customers.update(customer.id, {
      invoice_settings: {
        default_payment_method: paymentMethodId
      }
    });
  }

  const test = await stripe.customers.list({
    email: email,
    limit: 1
  });

  return customer;
}

async function createSubscribtion(email, paymentMethodId, priceId) {
  try {

    const customer = await findOrCreateCustomer(email, paymentMethodId);

    if (!customer.invoice_settings.default_payment_method || customer.invoice_settings.default_payment_method !== paymentMethodId) {
      await stripe.paymentMethods.attach(paymentMethodId, { customer: customer.id });
      const updatedcustomer = await stripe.customers.update(customer.id, {
        invoice_settings: {
          default_payment_method: paymentMethodId
        }
      });
    }

    const subscription = await stripe.subscriptions.create({
      customer: customer.id,
      items: [{ price: priceId }],
      default_payment_method: paymentMethodId,
    });

    console.log(`Subscription ${subscription.id} created for user ${email}`);
    return subscription.id;
  } catch (error) {
    console.error(`Error creating subscription: ${error.message}`);
    return null;
  }
}

export async function CancelSubscribtion(req, res) {
  const { subscribtionId, userId, provider } = req.body;

  if (!subscribtionId || !userId) {
    logger.error('Error canceling subscription', { subscribtionId, userId, provider });
    res.status(400).send('Invalid request');
    return;
  }

  try {
    console.log(req.body, subscribtionId);
    const cancellation = await stripe.subscriptions.cancel(subscribtionId);
    console.log(cancellation);

    if (cancellation.status === 'canceled') {
      logger.info('Subscription canceled successfully', { subscribtionId, userId, provider });
      await sendToQueue("subcribtionsResults", { userId, subscribtionId, status: 'canceled', date: new Date(), provider });

      res.status(200).send({Result: 'Subscription canceled successfully'});
    } else {
      throw new Error('Failed to cancel subscription');
    }
  } catch (error) {
    logger.error('Error canceling subscription', { subscribtionId, userId, error: error.message });
    res.status(500).send(`Error canceling subscription: ${error.message}`);
  }
}

async function createOrRetrievePrice(priceId, unitAmount, currency, recurringInterval, productName) {
  try {
    let price;

    try {
      price = await stripe.prices.retrieve(priceId);
    } catch (error) {

      if (error.statusCode === 404) {
        price = await stripe.prices.create({
          unit_amount: unitAmount,
          currency: currency,
          recurring: {
            interval: recurringInterval,
          },
          product_data: {
            name: productName,  // Название вашего продукта
          },
        });
      } else {
        throw error;  // Если возникла другая ошибка, выбрасываем ее дальше
      }
    }

    return price.id;  // Возвращаем идентификатор созданной или существующей цены
  } catch (error) {
    console.error('Error creating or retrieving price:', error);
    throw error;
  }
}