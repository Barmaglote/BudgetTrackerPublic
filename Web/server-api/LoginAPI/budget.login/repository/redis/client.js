import dotenv from 'dotenv'
dotenv.config('')
import { createLogger } from '../../helpers/logger.js'
import { Client } from 'redis-om';
const logger = createLogger(process.env.SEQ_LOG_ADDR, process.env.SEQ_LOG_KEY);
const client = await new Client().open(process.env.REDIS_URL);
logger.info("🚀 Redis connection status: ", client.isOpen());


export default client;