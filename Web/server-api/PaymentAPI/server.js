import dotenv from 'dotenv'
dotenv.config('')
import cors from 'cors'
import corsOptionsDelegate from './helpers/cors.js'
import express from 'express'
import { createLogger } from './helpers/logger.js'
import { connectDB } from './models/db.js'
import { getRoutesAPIStripe } from './routes/api/stripe.js'
import { getRoutesAPIHealth } from './routes/api/health.js'
import swaggerUi from 'swagger-ui-express'
import swaggerDocument from './swagger.js'
import { connectRabbitMQ } from './helpers/rabbitmq.js'

const logger = createLogger(process.env.SEQ_LOG_ADDR, process.env.SEQ_LOG_KEY);
const app = express();

logger.info(`Starting PaymentAPI Server at ${process.env.PORT}`);
console.log(process.env.MONGO_URI)
connectDB(process.env.MONGO_URI);
connectRabbitMQ();
app.use(cors(corsOptionsDelegate));
app.use('/api/stripe', getRoutesAPIStripe());
app.use('/health', getRoutesAPIHealth());
app.use('/api/docs', swaggerUi.serve, swaggerUi.setup(swaggerDocument));

app.listen(process.env.PORT, () => {
    logger.info(`🚀 PaymentAPI Server is started, port: ${process.env.PORT}`);
});
