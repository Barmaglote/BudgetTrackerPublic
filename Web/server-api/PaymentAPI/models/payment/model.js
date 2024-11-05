import mongoose from 'mongoose'
import { userSchema } from './schema.js'

export default mongoose.model('payment', userSchema)
