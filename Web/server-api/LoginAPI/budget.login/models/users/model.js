import mongoose from 'mongoose'
import { userSchema } from './schema.js'

export default mongoose.model('users', userSchema)
