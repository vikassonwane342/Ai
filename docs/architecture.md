# Architecture Overview

## Tech Stack
- Backend: ASP.NET Core Web API (.NET 8)
- Frontend: Angular (if applicable)
- Database: SQL Server
- Data Access: Dapper

## Architecture Pattern
Clean Architecture

### Layers

1. API Layer
- Controllers
- Handles HTTP requests
- Calls Application layer

2. Application Layer
- Business logic
- Services
- DTOs

3. Domain Layer
- Entities
- Interfaces
- Core business rules

4. Infrastructure Layer
- Database access
- Dapper queries
- External services

## Flow

Request → Controller → Service → Repository → Database

## Rules

- Controllers must be thin
- Business logic only in Application layer
- No direct DB calls from controllers
- Use DTOs for all API communication

## Naming Conventions

- Controllers: `ProductController`
- Services: `ProductService`
- Repositories: `ProductRepository`
- DTOs: `ProductDto`

## Error Handling

- Use centralized error handling middleware
- Return standard response format

## Logging

- Use structured logging
- Log errors and important actions