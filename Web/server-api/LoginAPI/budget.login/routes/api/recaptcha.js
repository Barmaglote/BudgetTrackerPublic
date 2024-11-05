import { $fetch } from 'ohmyfetch/node'
import { Send } from '../../helpers/response.js'

import { createLogger } from '../../helpers/logger.js'

const logger = createLogger(process.env.SEQ_LOG_ADDR, process.env.SEQ_LOG_KEY);

export async function recaptcha (req, res, next) {
  logger.info("recaptcha 01");
  res.setHeader('Content-Type', 'application/json')

  logger.info("recaptcha 02");

  if (process.env.NODE_ENV === 'test') {
    next();
    return;
  }

  try {
    const { recaptcha } = req.body
    logger.info("recaptcha 03");
    if (!recaptcha) {
      Send(res, 500, { status: 'failed' })
      return;
    }

    logger.info("recaptcha 04");

    const response = await $fetch(
      `https://www.google.com/recaptcha/api/siteverify?secret=${process.env.RECAPTCHA_SECRET_KEY}&response=${recaptcha}`
    )

    if (response.success || process.env.NODE_ENV === 'test') {
      logger.info("recaptcha 05");
      next()
    } else {
      Send(res, 500, { status: 'failed', info: "recaptcha is wrong" })
      return;
    }
  } catch (e) {
    console.log('ReCaptcha error:', e)
    Send(res, 500, { status: 'failed', info: "ReCaptcha error" })
    return;
  }
}
