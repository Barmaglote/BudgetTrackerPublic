import express from 'express'

import { Health } from '../../controllers/health.js'
const router = express.Router()

router.get('/', Health)

export function getRoutesAPIHealth() {
  return router
}

