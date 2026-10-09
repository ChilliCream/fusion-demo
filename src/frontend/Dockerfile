# Build stage
FROM node:24-alpine AS builder

WORKDIR /app

# Copy package files
COPY .npmrc package.json package-lock.json ./

# Install dependencies
RUN npm ci

# Copy source files
COPY . .

# Build argument for GraphQL endpoint
ARG VITE_GRAPHQL_ENDPOINT
ENV VITE_GRAPHQL_ENDPOINT=$VITE_GRAPHQL_ENDPOINT

ARG VITE_GRAPHQL_CLIENT_ID
ENV VITE_GRAPHQL_CLIENT_ID=$VITE_GRAPHQL_CLIENT_ID

ARG VITE_GRAPHQL_CLIENT_VERSION
ENV VITE_GRAPHQL_CLIENT_VERSION=$VITE_GRAPHQL_CLIENT_VERSION

ARG VITE_ENABLE_PERSISTED_OPERATIONS
ENV VITE_ENABLE_PERSISTED_OPERATIONS=$VITE_ENABLE_PERSISTED_OPERATIONS

RUN if [ "$VITE_ENABLE_PERSISTED_OPERATIONS" = "true" ]; then \
  npm run relay:persist; \
  fi

# Build the application
RUN npm run build

# Persisted operations export stage
FROM scratch AS operations
COPY --from=builder /app/operations.json /operations.json

# Production stage
FROM nginx:alpine

# Copy built files from builder stage
COPY --from=builder /app/dist /usr/share/nginx/html

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose port 8080 (Container Apps standard)
EXPOSE 8080

CMD ["nginx", "-g", "daemon off;"]
