# Car Marketplace API and Database v1

## Summary
Convert the current template API in `CarMarketplace.Api/Program.cs` into a real car marketplace backend that follows `AGENT.md` and the docs in `docs/project.md`, `docs/architecture.md`, `docs/flows.md`, and `docs/convensions.md`.

Keep the implementation inside the existing `CarMarketplace.Api` project and add the full SQL Server schema in one root file, `script.sql`.

## Implementation Changes
- Keep a single-project structure for v1 and add folders for `Controllers`, `Services`, `Repositories`, `Dtos`, `Entities`, `Middleware`, `Common`, and `Configuration`.
- Replace the sample weather endpoint in `CarMarketplace.Api/Program.cs` with:
  - controller support
  - Swagger/OpenAPI
  - JWT bearer authentication
  - authorization
  - service and repository DI
  - Dapper/SQL connection registration
  - centralized exception middleware
- Add the required NuGet dependencies during implementation for:
  - Dapper
  - SQL Server client
  - JWT bearer auth
- Use DTOs for all API contracts and a single response wrapper with `Success`, `Message`, `Data`, and `Errors`.
- Use paged responses for listing endpoints with `Items`, `Page`, `PageSize`, and `TotalCount`.

### Auth APIs
- `POST /api/auth/register`
  - Request: `FirstName`, `LastName`, `Email`, `Phone`, `Password`, `Role`
  - Only allow `Buyer` and `Seller`
  - Hash password before storage
  - Reject duplicate email
- `POST /api/auth/login`
  - Request: `Email`, `Password`
  - Return JWT token, expiry, and user summary
  - Reject invalid credentials and locked users
- `GET /api/auth/me`
  - Authenticated current-user profile endpoint

### Car listing APIs
- `GET /api/cars`
  - Public endpoint
  - Filters: `Keyword`, `Brand`, `Model`, `MinPrice`, `MaxPrice`, `Year`, `FuelType`, `Transmission`, `Page`, `PageSize`
  - Return only active and unlocked listings
- `GET /api/cars/{id}`
  - Public details endpoint for active and unlocked listings, including images
- `POST /api/cars`
  - Seller-only create
  - Request: `Title`, `Brand`, `Model`, `Year`, `Price`, `Mileage`, `FuelType`, `Transmission`, `Description`, `ImageUrls`
- `PUT /api/cars/{id}`
  - Seller can update own listing
  - Admin can update any listing
- `DELETE /api/cars/{id}`
  - Seller can delete own listing
  - Admin can delete any listing
- `GET /api/cars/mine`
  - Seller-only endpoint for own listings

### Order APIs
- `POST /api/orders`
  - Buyer-only booking request
  - Request: `CarListingId`, `Notes`
  - Create with `Pending` status
  - Reject booking own listing
  - Reject inactive or locked listing
- `GET /api/orders/mine`
  - Buyer-only booking history
- `GET /api/orders/seller`
  - Seller-only bookings for own listings
- `PATCH /api/orders/{id}/status`
  - Seller or admin updates status
  - Allowed target statuses: `Confirmed`, `Rejected`, `Cancelled`
  - Seller may update only orders for their own listings

### Admin APIs
- `GET /api/admin/users`
  - Admin-only
  - Optional filters: `Role`, `IsLocked`
- `PATCH /api/admin/users/{id}/lock`
- `PATCH /api/admin/users/{id}/unlock`
- `GET /api/admin/listings`
  - Admin-only all-listings endpoint
- `PATCH /api/admin/listings/{id}/lock`
- `PATCH /api/admin/listings/{id}/unlock`

## Database and Public Types
### Tables in `script.sql`
- `Users`
  - `Id`, `FirstName`, `LastName`, `Email`, `Phone`, `PasswordHash`, `Role`, `IsLocked`, `CreatedAt`, `UpdatedAt`
- `CarListings`
  - `Id`, `SellerId`, `Title`, `Brand`, `Model`, `Year`, `Price`, `Mileage`, `FuelType`, `Transmission`, `Description`, `Status`, `IsLocked`, `CreatedAt`, `UpdatedAt`
- `CarImages`
  - `Id`, `CarListingId`, `ImageUrl`, `SortOrder`, `CreatedAt`
- `Orders`
  - `Id`, `CarListingId`, `BuyerId`, `SellerId`, `Status`, `Notes`, `BookedAt`, `UpdatedAt`

### Constraints and indexes
- Unique constraint on `Users.Email`
- Check constraints for:
  - `Users.Role`: `Buyer`, `Seller`, `Admin`
  - `CarListings.Status`: `Active`, `Inactive`, `Sold`
  - `Orders.Status`: `Pending`, `Confirmed`, `Rejected`, `Cancelled`
- Foreign keys between users, listings, images, and orders
- Indexes for:
  - `CarListings(SellerId)`
  - `CarListings(Status, IsLocked)`
  - `CarListings(Brand, Model)`
  - `CarListings(Price)`
  - `CarListings(Year)`
  - `Orders(BuyerId)`
  - `Orders(SellerId)`
  - `Orders(CarListingId)`

### Seed data
- Seed one admin, one buyer, one seller
- Seed a few active listings and images
- Seed only hashed passwords

## Test Plan
- Register buyer and seller successfully
- Reject duplicate email registration
- Reject admin self-registration
- Login returns JWT with correct role claim
- Locked user cannot log in
- Seller can create, update, delete, and view own listings
- Buyer cannot create or edit listings
- Public search returns only active unlocked listings and applies filters
- Buyer can create booking for another seller’s listing
- Buyer cannot book own listing
- Seller can view bookings for own listings only
- Invalid order status transitions are rejected
- Admin can lock and unlock users and listings
- Non-admin users are forbidden from admin endpoints
- `script.sql` executes cleanly on an empty SQL Server database

## Assumptions
- JWT bearer auth with hashed passwords is the required auth model.
- Roles and statuses stay as constrained string values rather than separate lookup tables.
- Listings are created as `Active`; draft workflow is out of scope.
- Images are stored as URL strings only.
- Orders represent booking requests only; payment and checkout are out of scope.
