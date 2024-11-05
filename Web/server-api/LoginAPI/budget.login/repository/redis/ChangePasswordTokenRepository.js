import { Entity, Schema } from 'redis-om';
import client from "./client.js";

class ChangePasswordToken extends Entity {}

export const changePasswordTokenSchema = new Schema( ChangePasswordToken, {
  token: { type: 'string' },
  login: { type: 'string' }
});

export const ChangePasswordTokenRepository = client.fetchRepository(changePasswordTokenSchema);

try {
  await ChangePasswordTokenRepository.createIndex();
} catch (error) {
  console.log(error);
}
