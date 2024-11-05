import dotenv from 'dotenv'
dotenv.config('')
import mongoose from 'mongoose'
import { createLogger } from '../helpers/logger.js'
const logger = createLogger(process.env.SEQ_LOG_ADDR, process.env.SEQ_LOG_KEY);

mongoose.connection.on('connected', () => {
  logger.info('Mongoose connected to MongoDB')
})

mongoose.connection.on('error', (error) => {
  logger.error('Mongoose connected error ' + error)
})

mongoose.connection.on('disconnected', () => {
  logger.info('Mongoose disconnected')
})

export function connectDB (url) {
  try {
    logger.info('Connecting to DB')
    mongoose.connect(url, { useUnifiedTopology: true})
  } catch (error) {
    logger.error('Unable to connect to DB: ' + error)
    logger.error(error)
  }
}
