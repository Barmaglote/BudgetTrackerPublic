import { $fetch } from 'ohmyfetch/node'
import { Send } from '../../helpers/response.js'

export async function recaptcha (req, res, next) {
  res.setHeader('Content-Type', 'application/json')

  if (process.env.NODE_ENV === 'test') {
    next();
    return;
  }

  try {
    const { recaptcha } = req.body

    if (!recaptcha) {
      Send(res, 500, { status: 'failed' })
      return;
    }

    const response = await $fetch(
      `https://www.google.com/recaptcha/api/siteverify?secret=${process.env.RECAPTCHA_SECRET_KEY}&response=${recaptcha}`
    )

    if (response.success || process.env.NODE_ENV === 'test') {
      next()
    } else {
      Send(res, 500, { status: 'failed' })
      return;
    }
  } catch (e) {
    console.log('ReCaptcha error:', e)
    Send(res, 500, { status: 'failed' })
    return;
  }
}
