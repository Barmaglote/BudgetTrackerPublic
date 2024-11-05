21. Создать ключи при помощи команды. Нужно конечно указать свою организацию и правильный host (очевидно не localhost в случае с кластером kubernetes)

1.1. Создать сертификат  (самоподписанный или получить от CA)

openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -subj "/C=US/ST=State/L=Locality/O=Organization/OU=Organizational Unit/CN=localhost" -addext "subjectAltName = DNS:localhost" -addext "keyUsage = keyEncipherment, dataEncipherment" -addext "extendedKeyUsage = serverAuth"

1.2. Получить pfx-файл:
openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt

Указать пароль, который потом передать в Kerstel

1.3. Поскольку сертификат самоподписанный, то его надо установить в системе самостоятельно - в Trusted Root Sertificates

*Возможно будет ругаться, что нет файла конфигураций для openssl. Надо указать его через переменную окружения.
set OPENSSL_CONF=c:\Program Files\OpenSSL-Win64\bin\openssl.cnf


2. В angular.json  надо указать

"serve": {
          "options": {
            "browserTarget": "ideationapp:build",
            "ssl": true,
            "sslKey": "ssl/localhost.key",
            "sslCert": "ssl/localhost.crt"
          },

3. Путь к сертификатам указывает в настройках приложения. Для разарботки это путь к локальной папке, для кластера Kuberternetes нужно сначала создать секрет

Ключи для Production можно получить в Let's Encrypt - бесплатный на первое время

kubectl create secret generic my-tls-secret \
  --from-file=ssl/localhost.crt \
  --from-file=ssl/localhost.key
  
4. А потом этот секрет подключить к поду

apiVersion: apps/v1
kind: Deployment
metadata:
  name: my-app
spec:
  replicas: 1
  selector:
    matchLabels:
      app: my-app
  template:
    metadata:
      labels:
        app: my-app
    spec:
      containers:
      - name: my-app
        image: my-app-image:latest
        ports:
        - containerPort: 5001
      volumes:
      - name: tls-secret
        secret:
          secretName: my-tls-secret
		  
4. И тогда в настройка приложения путь к папке должен выглядеть как app/tls-secret
