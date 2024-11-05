// TODO: Move to whitelist to .env
var whitelist = ['https://localhost:4200', 'https://budget-web-app:4200', 'http://budget-web-app:4200'];

var corsOptionsDelegate = function (req, callback) {

  const origin = req.header('Origin');
  const isWhitelisted = (whitelist.indexOf(origin) !== -1);

  const corsOptions = {
    origin: isWhitelisted,
    methods: ['GET', 'HEAD', 'PUT', 'PATCH', 'POST', 'DELETE'],
    credentials: true,
    //allowedHeaders: '*', // Разрешить все заголовки
    allowedHeaders: ['Content-Type', 'Authorization', 'Custom-Origin', 'Language', 'x-csrf-token', 'Accept', 'Referer', 'Sec-Ch-Ua', 'Sec-Ch-Ua-Mobile', 'Sec-Ch-Ua-Platform', 'X-Csrf-Token', 'User-Agent', 'Provider', 'X-Access-Token']
  };

  callback(null, corsOptions)
}

export default corsOptionsDelegate;