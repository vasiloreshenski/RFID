FROM node:11.6.0-alpine AS builder
ARG CACHEBUST=1
COPY /src /app
COPY nginx.conf /app
WORKDIR /app
ARG CACHEBUST=1
RUN npm i
ARG CACHEBUST=1
RUN $(npm bin)/ng build --prod

FROM nginx:1.15.8-alpine
ARG CACHEBUST=1
COPY --from=builder /app/dist/rfid /usr/share/nginx/htm
COPY --from=builder /app/nginx.conf /etc/nginx/conf.d/default.conf