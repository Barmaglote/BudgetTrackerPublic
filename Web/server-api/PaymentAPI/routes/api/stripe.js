import express from 'express'
import bodyParser from 'body-parser';
import { authenticateToken } from './authentication.js'
import { CreateIntentToken, RegisterPayment, Subscribe, CancelSubscribtion } from '../../controllers/stripe.js'
const router = express.Router()

router.use('/webhook', bodyParser.raw({ type: 'application/json' }));
router.post('/intent', express.json(), authenticateToken, CreateIntentToken);
router.post('/subscribe', express.json(), authenticateToken, Subscribe);
router.post('/unsubscribe', express.json(), authenticateToken, CancelSubscribtion);
router.post('/webhook', RegisterPayment);

export function getRoutesAPIStripe () {
  return router
}

