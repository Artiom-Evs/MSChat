# MSChat.NotificationWorker

Background notification service for MSChat that sends email notifications about unread messages.

## Overview

This service consumes `ChatMessageSent` events from RabbitMQ and sends email notifications to offline users who have unread messages. It integrates with Chat API and Presence API via gRPC to check user presence and retrieve chat memberships.

## Features

- Event-driven message consumption via MassTransit/RabbitMQ
- gRPC integration with Chat API and Presence API
- Email notifications using MailKit SMTP
- JWT-based machine-to-machine authentication
- User presence checking to avoid notifying online users
- Configurable SMTP settings for email delivery

## Technologies

- .NET 9.0
- MassTransit + RabbitMQ
- gRPC clients
- MailKit for email sending
- Duende.IdentityModel for JWT authentication
- Machine-to-machine OAuth2 flow

## Message Flow

1. Consumes `ChatMessageSent` events from RabbitMQ
2. Retrieves chat memberships from Chat API (gRPC)
3. Checks user presence status via Presence API (gRPC)
4. Fetches user info from Auth API (HTTP)
5. Sends email notifications to offline users with unread messages

## Configuration

Configure the following in `appsettings.json`:

- `RabbitMQSettings` - RabbitMQ connection details
- `ServicesSettings` - Chat API, Presence API, and Auth API endpoints
- `EmailSettings` - SMTP configuration for email notifications
- `M2MAuthSettings` - Machine-to-machine authentication settings