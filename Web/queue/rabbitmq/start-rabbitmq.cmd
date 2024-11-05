docker build -t barmaglote/rabbitmq:0.0.2 .
docker pull barmaglote/rabbitmq:0.0.2
docker run -d --name barmaglote-rabbitmq-container -p 5672:5672 -p 15672:15672 barmaglote/rabbitmq
