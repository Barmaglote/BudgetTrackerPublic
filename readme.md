DEMO
  
## Technologies

1. SEQ
2. Dockers
3. Angular
4. Golang
5. Cypress
6. RabbitMQ
7. Prometeus
8. Stripe
8. C#
8. MongoDB

## Создание пользователя для работы базы данных  

1. Запускаем кластер  
docker-compose up  
docker-compose --env-file development.env --file docker-compose.yaml up --build  


2. Инициализация (Имя контейнера, а не сервиса)  
docker exec mongo-config-01 sh -c "/scripts/init-configserver.sh"  
docker exec mongo-shard-01-arbiter sh -c "/scripts/init-shard01.sh"  
docker exec mongo-shard-02-arbiter sh -c "/scripts/init-shard02.sh"  

3. Инициализация роутера
docker exec mongo-router-01 sh -c "/scripts/init-router.sh"  

4. Создаем пароль
#docker exec -e MONGO_ADMIN_USER="mongoadmin" -e MONGO_ADMIN_PASSWORD="mongopassword" mongo-config-01 sh -c "/scripts/auth.sh"  
#docker exec -e MONGO_ADMIN_USER="mongoadmin" -e MONGO_ADMIN_PASSWORD="mongopassword" mongo-shard-01-arbiter sh -c "/scripts/auth.sh"  
#docker exec -e MONGO_ADMIN_USER="mongoadmin" -e MONGO_ADMIN_PASSWORD="mongopassword" mongo-shard-02-arbiter sh -c "/scripts/auth.sh"  

docker exec mongo-config-01 sh -c "/scripts/auth.sh"
docker exec mongo-shard-01-arbiter sh -c "/scripts/auth.sh"
docker exec mongo-shard-02-arbiter sh -c "/scripts/auth.sh"

5. Инициализируем базу данных
docker exec mongo-router-01 sh -c "/scripts/init-databases.sh"

6. Проверить, что кластер создан
docker exec -it mongo-config-01 bash -c "echo 'rs.status()' | mongosh --port 27017 -u 'mongoadmin' -p 'mongopassword' --authenticationDatabase admin"  
docker exec -it mongo-config-01 bash -c "echo 'db.getUsers()' | mongosh --port 27017 -u 'mongoadmin' -p 'mongopassword' --authenticationDatabase admin"  
docker exec -it mongo-shard-01-arbiter bash -c "echo 'rs.status()' | mongosh --port 27017 -u 'mongoadmin' -p 'mongopassword' --authenticationDatabase admin"  
docker exec -it mongo-shard-01-arbiter bash -c "echo 'db.getUsers()' | mongosh --port 27017 -u 'mongoadmin' -p 'mongopassword' --authenticationDatabase admin"  
docker exec -it mongo-shard-02-arbiter bash -c "echo 'rs.status()' | mongosh --port 27017 -u 'mongoadmin' -p 'mongopassword' --authenticationDatabase admin"  
docker exec -it mongo-shard-02-arbiter bash -c "echo 'db.getUsers()' | mongosh --port 27017 -u 'mongoadmin' -p 'mongopassword' --authenticationDatabase admin"  
docker exec -it mongo-shard-01-arbiter bash -c "echo 'rs.printReplicationInfo()' | mongosh --port 27017 -u 'mongoadmin' -p 'mongopassword' --authenticationDatabase admin"  
docker exec -it mongo-shard-01-arbiter bash -c "echo 'rs.printSecondaryReplicationInfo()' | mongosh --port 27017 -u 'mongoadmin' -p 'mongopassword' --authenticationDatabase admin"  
docker exec -it mongo-shard-01-arbiter bash -c "echo 'rs.help()' | mongosh --port 27017 -u 'mongoadmin' -p 'mongopassword' --authenticationDatabase admin"  
  
7. Создание индексов  
docker exec mongo-router-01 sh -c "/scripts/init-indexes.sh"  
  
8.Подключение снаружи  
mongodb://mongoadmin:mongopassword@localhost:27030,localhost:27031/?authMechanism=DEFAULT  
mongodb://mongoadmin:mongopassword@localhost:27040/  
mongodb://mongoadmin:mongopassword@localhost:27041/  
  
## CYPRESS  
1. Запустить loginapi в режиме тестирования  
 
    npm run start:test 
 
2. Запустить клиентскую часть в тестовом режиме 
  
    npm run start:test 
   
3. Запустить CYPRESS: 
  
    npm run cypress:open
