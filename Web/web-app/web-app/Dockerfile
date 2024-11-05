# Stage 1: Сборка приложения
FROM node:18.20.4-alpine AS build
# WORKDIR /app
WORKDIR /usr/src/app

COPY ./src ./src
COPY ./src/environments ./src/
COPY ./src/environments/environment.prod.ts ./src/environments
COPY ./ssl ./
COPY ./*.json ./
RUN npm install
RUN npm version minor
RUN node_modules/.bin/ng build --configuration production

# Stage 2: Запуск nginx
FROM nginx

# Копируем SSL-сертификаты в контейнер
COPY ./ssl /etc/nginx/ssl

# Меняем конфиг nginx-а на собственный
COPY nginx.conf /etc/nginx/nginx.conf

# Копируем собранное приложение из предыдущего этапа в рабочую директорию nginx
COPY --from=build /usr/src/app/dist/web-app /usr/share/nginx/html
EXPOSE 8080

# Запускаем nginx
CMD ["nginx", "-g", "daemon off;"]
