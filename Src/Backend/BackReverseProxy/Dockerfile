# Use Nginx base image
FROM nginx:stable-alpine

# Copy the Nginx configuration file to the container
COPY Src/Backend/BackReverseProxy/nginx.conf /etc/nginx/nginx.conf

# Expose port 80
EXPOSE 9412 7586 9736

# Start Nginx server
CMD ["nginx", "-g", "daemon off;"]
