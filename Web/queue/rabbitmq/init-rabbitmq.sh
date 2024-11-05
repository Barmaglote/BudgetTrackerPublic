#!/bin/sh

# Установка переменной NODENAME
echo 'NODENAME=rabbit@localhost' > /etc/rabbitmq/rabbitmq-env.conf

# Запуск RabbitMQ в фоновом режиме
rabbitmq-server &

# Функция для ожидания запуска RabbitMQ
wait_for_rabbitmq() {
    echo "Ожидание запуска RabbitMQ..."
    # Ожидание готовности RabbitMQ (с указанием PID файла)
    rabbitmqctl wait --timeout 60 $RABBITMQ_PID_FILE
    echo "RabbitMQ запущен и готов к работе."
}

# Ожидание запуска RabbitMQ
wait_for_rabbitmq

# Функция для добавления пользователя, если его нет
add_user_if_not_exists() {
    local username=$1
    local password=$2
    if rabbitmqctl list_users | grep -q "^${username}"; then
        echo "Пользователь '${username}' уже существует."
    else
        echo "Добавление пользователя '${username}'..."
        rabbitmqctl add_user ${username} ${password}
    fi
}

# Функция для установки прав
set_permissions() {
    local username=$1
    echo "Установка прав для пользователя '${username}' в виртуальном хосте '/' ..."
    rabbitmqctl set_permissions -p / ${username} ".*" ".*" ".*"
}

# Функция для установки тегов
set_user_tags() {
    local username=$1
    local tags=$2
    echo "Установка тегов для пользователя '${username}'..."
    rabbitmqctl set_user_tags ${username} ${tags}
}

# Добавление и настройка пользователей
add_user_if_not_exists ${RABBITMQ_SERVICE_USER} ${RABBITMQ_SERVICE_PASS}
set_permissions ${RABBITMQ_SERVICE_USER}

add_user_if_not_exists ${RABBITMQ_ADMIN_USER} ${RABBITMQ_ADMIN_PASS}
set_user_tags ${RABBITMQ_ADMIN_USER} administrator
set_permissions ${RABBITMQ_ADMIN_USER}

# Включаем статистикуЮ
rabbitmqctl enable_management_stats
rabbitmq-plugins enable rabbitmq_shovel rabbitmq_shovel_management

# Держим контейнер активным
tail -f /dev/null
