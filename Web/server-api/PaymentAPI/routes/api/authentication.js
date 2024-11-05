import jwt from 'jsonwebtoken'
import { OAuth2Client } from 'google-auth-library';

const googleClient = new OAuth2Client(process.env.GOOGLE_CLIENT_ID);

export async function authenticateToken(req, res, next) {
  const authHeader = req.headers.authorization?.split(' ')[1];
  const token = authHeader && authHeader.trim();
  if (token == null) return res.sendStatus(401);

  // Если токен Google, верифицируем с помощью google-auth-library
  try {
    const decodedToken = jwt.decode(token, { complete: true });

    if (decodedToken?.payload?.iss?.includes("accounts.google.com")) {
      const ticket = await googleClient.verifyIdToken({
        idToken: token,
        audience: process.env.GOOGLE_CLIENT_ID,
      });
      const payload = ticket.getPayload();
      req.user = payload;
      return next();
    }
  } catch (error) {
    console.error('Google token verification error:', error);
    return res.sendStatus(401);
  }

  // Верификация локального токена
  jwt.verify(token, process.env.ACCESS_TOKEN_SECRET, (err, user) => {
    if (err) return res.sendStatus(401);
    req.user = user;
    next();
  });
};
