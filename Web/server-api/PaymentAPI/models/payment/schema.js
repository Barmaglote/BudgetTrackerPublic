import mongoose from 'mongoose'

export const userSchema = new mongoose.Schema({
  paymentIntent: {
    type: Object,
    required: true,
  },
  createUserId: {
    type: String,
    required: true,
    maxlength: 50
  },
  createDate: {
    type: Date,
    required: true,
    immutable: true,
    default: () => Date.now()
  }
})
