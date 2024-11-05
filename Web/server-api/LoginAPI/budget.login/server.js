import dotenv from 'dotenv'
dotenv.config('')
import cors from 'cors'
import corsOptionsDelegate from './helpers/cors.js'
import express from 'express'
import { createLogger } from './helpers/logger.js'
import { connectDB } from './models/db.js'
import { getRoutesAPIUsers } from './routes/api/users.js'
import { getRoutesAPIHealth } from './routes/api/health.js'
import swaggerUi from 'swagger-ui-express'
import swaggerDocument from './swagger.js'

const logger = createLogger(process.env.SEQ_LOG_ADDR, process.env.SEQ_LOG_KEY);
const app = express()

logger.info(`Starting LoginAPI Server at ${process.env.PORT}`)
connectDB(process.env.MONGO_URI)
app.use(cors(corsOptionsDelegate))
app.use(express.json())
app.use('/api/auth', getRoutesAPIUsers())
app.use('/health', getRoutesAPIHealth())
app.use('/api/docs', swaggerUi.serve, swaggerUi.setup(swaggerDocument))
app.listen(process.env.PORT, () => {
    logger.info(`🚀 LoginAPI Server is started, port: ${process.env.PORT}`)
})
