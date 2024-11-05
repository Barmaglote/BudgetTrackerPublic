import amqp from 'amqplib';

let channel = null;

export async function connectRabbitMQ() {
    try {
        const connection = await amqp.connect(process.env.RABBITMQ_URL);
        channel = await connection.createChannel();
        console.log('Connected to RabbitMQ');
    } catch (error) {
        console.error('Failed to connect to RabbitMQ', error);
    }
}

export async function sendToQueue(queue, message) {
    if (!channel) {
        throw new Error('RabbitMQ channel is not established');
    }

    try {
        await channel.assertQueue(queue, { durable: true });
        await channel.sendToQueue(queue, Buffer.from(JSON.stringify(message)), { persistent: true });
    } catch (error) {
        console.error('Failed to send message to RabbitMQ', error);
    }
}