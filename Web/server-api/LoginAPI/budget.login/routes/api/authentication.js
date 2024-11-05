import jwt from 'jsonwebtoken'

export function authenticateToken (req, res, next) {
  const authHeader = req.headers.authorization?.split(' ')[1]
  const token = authHeader && authHeader.trim()
  if (token == null) return res.sendStatus(401)

  jwt.verify(token, process.env.ACCESS_TOKEN_SECRET, (err, user) => {
    if (err) return res.sendStatus(401)
    req.user = user
    next()
  })
};
