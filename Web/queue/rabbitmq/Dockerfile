# Official image
FROM rabbitmq:latest

# ENV
ENV RABBITMQ_DEFAULT_USER=${RABBITMQ_DEFAULT_USER}
ENV RABBITMQ_DEFAULT_PASS=${RABBITMQ_DEFAULT_PASS}
ENV RABBITMQ_SERVICE_USER=${RABBITMQ_SERVICE_USER}
ENV RABBITMQ_SERVICE_PASS=${RABBITMQ_SERVICE_PASS}
ENV RABBITMQ_ADMIN_USER=${RABBITMQ_ADMIN_USER}
ENV RABBITMQ_ADMIN_PASS=${RABBITMQ_ADMIN_PASS}
ENV RABBITMQ_PID_FILE /var/lib/rabbitmq/mnesia/rabbitmq

# Ports
EXPOSE 5672
EXPOSE 15672

# Copy init script
COPY init-rabbitmq.sh /docker-entrypoint-initdb.d/

# Permissions
RUN rabbitmq-plugins enable rabbitmq_management
RUN chmod +x /docker-entrypoint-initdb.d/init-rabbitmq.sh

# Run
CMD ["/docker-entrypoint-initdb.d/init-rabbitmq.sh"]
