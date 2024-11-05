import { Send } from './../helpers/response.js'

export async function Health (req, res) {
    Send(res, 500, { status: 'ok', message: 'OK' })
};