# Use the official PostgreSQL image as the base image
FROM postgres

# Set environment variables for the database
ENV POSTGRES_DB=PaymentGatewayDb
ENV POSTGRES_USER=admin
ENV POSTGRES_PASSWORD=Systemmige1

# Ensure a persistent volume for database data
VOLUME ["/var/lib/postgresql/data"]

# Copy any initialization SQL scripts if needed (optional)
# Uncomment the following lines if you have an init script
# COPY ./init.sql /docker-entrypoint-initdb.d/
# Copy custom configuration files
COPY Postgres/postgresql.conf /usr/share/postgresql/postgresql.conf.sample
COPY Postgres/pg_hba.conf /var/lib/postgresql/data/pg_hba.conf

# Expose the PostgreSQL port
EXPOSE 5432
