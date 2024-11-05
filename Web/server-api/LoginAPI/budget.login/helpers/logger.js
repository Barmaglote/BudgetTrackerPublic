import winston from 'winston';
import { SeqTransport } from '@datalust/winston-seq';

export function createLogger(logAddr, logKey) { return winston.createLogger({
    level: 'info',  
    format: winston.format.combine( 
        winston.format.errors({ stack: true }),
        winston.format.json(),    
    ),
    defaultMeta: { application: 'loginapi' },
    transports: [
      new winston.transports.Console({
          format: winston.format.simple(),        
      }),
      new SeqTransport({
        serverUrl: logAddr,
        apiKey: logKey,
        onError: (e => { console.error(e) }),
        handleExceptions: true,
        handleRejections: true,
      })
    ]
    });
  }