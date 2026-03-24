# Coding Conventions

## General Rules

- Use clean, readable code
- Follow SOLID principles
- Avoid duplicate code
- Keep methods small

## Naming

- Classes: PascalCase
- Methods: PascalCase
- Variables: camelCase
- Interfaces: prefix with I (IProductService)

## API Rules

- Always use DTOs
- Do not expose domain entities directly
- Validate all inputs
- Return proper HTTP status codes

## Async

- Always use async/await for I/O operations
- Do not block threads

## Validation

- Validate request models
- Return meaningful error messages

## Exception Handling

- Do not use try-catch everywhere
- Use global exception middleware

## Database

- Use Dapper for queries
- Write optimized SQL
- Avoid unnecessary joins

## Folder Structure

- Controllers → API layer
- Services → Application layer
- Repositories → Infrastructure layer