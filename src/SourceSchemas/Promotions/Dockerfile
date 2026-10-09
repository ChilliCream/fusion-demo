# Build stage
FROM node:24-alpine AS builder

WORKDIR /app

# Copy package files
COPY .npmrc package.json package-lock.json ./

# Install dependencies
RUN npm ci

# Copy source files
COPY . .

# Build the application
RUN npm run build

# Production stage
FROM node:24-alpine

WORKDIR /app

ENV NODE_ENV=production
ENV PORT=8080

COPY .npmrc package.json package-lock.json ./

RUN npm ci --omit=dev

COPY --from=builder /app/dist ./dist

# The SQL migrations are applied on startup
COPY migrations ./migrations

# Expose port 8080 (Container Apps standard)
EXPOSE 8080

CMD ["node", "--enable-source-maps", "dist/main.js"]
