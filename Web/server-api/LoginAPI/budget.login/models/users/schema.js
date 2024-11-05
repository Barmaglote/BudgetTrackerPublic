import mongoose from 'mongoose'
const EMAIL = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i

export const userSchema = new mongoose.Schema({
  login: {
    type: String,
    required: true,
    lowercase: true,
    minlength: 5,
    validate: {
      validator: v => EMAIL.test(v),
      message: props => `${props.value} is not correct email`
    }
  },
  username: {
    type: String,
    required: true,
    maxlength: 50
  },
  password: {
    type: String,
    required: false
  },
  newpassword: {
    type: String,
    required: false
  },
  confirmed: Boolean,
  confirmation: String,
  createdAt: {
    type: Date,
    required: true,
    immutable: true,
    default: () => Date.now()
  },
  updatedAt: {
    type: Date,
    required: true,
    default: () => Date.now()
  }
})
