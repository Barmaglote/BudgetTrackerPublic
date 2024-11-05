import { Send } from './../helpers/response.js'
import { createLogger } from '../helpers/logger.js'

const logger = createLogger(process.env.SEQ_LOG_ADDR, process.env.SEQ_LOG_KEY);

export async function Health (req, res) {
    logger.info(`Health`);
    Send(res, 500, { status: 'ok', message: 'OK' })
};