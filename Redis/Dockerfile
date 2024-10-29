# Use the official Redis image as the base image
FROM redis

# Set the working directory to /data
WORKDIR /data

# Optionally, copy custom Redis configuration if needed
COPY Redis/redis.conf /usr/local/etc/redis/redis.conf

# Declare a volume for Redis data persistence
VOLUME ["/data"]
# Command to start Redis with default configuration
# Use this line if you are using a custom config
# CMD ["redis-server", "/usr/local/etc/redis/redis.conf"]

# Expose the default Redis port
EXPOSE 6379

# Start Redis with default settings
CMD ["redis-server", "--save", "60", "1", "--loglevel", "warning", "--dir", "/data"]
