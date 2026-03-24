/*
Run this script against SQL Server.
Update the database name if needed before execution.
*/

IF DB_ID('CarMarketplaceDb') IS NULL
BEGIN
    CREATE DATABASE CarMarketplaceDb;
END;
GO

USE CarMarketplaceDb;
GO

IF OBJECT_ID('dbo.CarImages', 'U') IS NOT NULL DROP TABLE dbo.CarImages;
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.CarListings', 'U') IS NOT NULL DROP TABLE dbo.CarListings;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO

CREATE TABLE dbo.Users
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Phone NVARCHAR(30) NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    IsLocked BIT NOT NULL CONSTRAINT DF_Users_IsLocked DEFAULT (0),
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_UpdatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT CK_Users_Role CHECK (Role IN ('Buyer', 'Seller', 'Admin'))
);
GO

CREATE TABLE dbo.CarListings
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SellerId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Brand NVARCHAR(100) NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    [Year] INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Mileage INT NOT NULL,
    FuelType NVARCHAR(50) NOT NULL,
    Transmission NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Status NVARCHAR(20) NOT NULL,
    IsLocked BIT NOT NULL CONSTRAINT DF_CarListings_IsLocked DEFAULT (0),
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_CarListings_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_CarListings_UpdatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_CarListings_Users FOREIGN KEY (SellerId) REFERENCES dbo.Users(Id),
    CONSTRAINT CK_CarListings_Status CHECK (Status IN ('Active', 'Inactive', 'Sold'))
);
GO

CREATE TABLE dbo.CarImages
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CarListingId INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    SortOrder INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_CarImages_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_CarImages_CarListings FOREIGN KEY (CarListingId) REFERENCES dbo.CarListings(Id) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.Orders
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CarListingId INT NOT NULL,
    BuyerId INT NOT NULL,
    SellerId INT NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    Notes NVARCHAR(1000) NULL,
    BookedAt DATETIME2 NOT NULL CONSTRAINT DF_Orders_BookedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_Orders_UpdatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_Orders_CarListings FOREIGN KEY (CarListingId) REFERENCES dbo.CarListings(Id),
    CONSTRAINT FK_Orders_Buyer FOREIGN KEY (BuyerId) REFERENCES dbo.Users(Id),
    CONSTRAINT FK_Orders_Seller FOREIGN KEY (SellerId) REFERENCES dbo.Users(Id),
    CONSTRAINT CK_Orders_Status CHECK (Status IN ('Pending', 'Confirmed', 'Rejected', 'Cancelled'))
);
GO

CREATE INDEX IX_CarListings_SellerId ON dbo.CarListings(SellerId);
CREATE INDEX IX_CarListings_Status_IsLocked ON dbo.CarListings(Status, IsLocked);
CREATE INDEX IX_CarListings_Brand_Model ON dbo.CarListings(Brand, Model);
CREATE INDEX IX_CarListings_Price ON dbo.CarListings(Price);
CREATE INDEX IX_CarListings_Year ON dbo.CarListings([Year]);
CREATE INDEX IX_Orders_BuyerId ON dbo.Orders(BuyerId);
CREATE INDEX IX_Orders_SellerId ON dbo.Orders(SellerId);
CREATE INDEX IX_Orders_CarListingId ON dbo.Orders(CarListingId);
GO

INSERT INTO dbo.Users (FirstName, LastName, Email, Phone, PasswordHash, Role, IsLocked)
VALUES
('System', 'Admin', 'admin@carmarketplace.local', '+10000000000', '100000.1z2qjD7m6Jw8x7Z3wM8zjg==.BTvMJs9+gIK6BffA1M0/9KWC+2lvd0cJ5M+m7evVdJ0=', 'Admin', 0),
('Ava', 'Buyer', 'buyer@carmarketplace.local', '+10000000001', '100000.1z2qjD7m6Jw8x7Z3wM8zjg==.BTvMJs9+gIK6BffA1M0/9KWC+2lvd0cJ5M+m7evVdJ0=', 'Buyer', 0),
('Sam', 'Seller', 'seller@carmarketplace.local', '+10000000002', '100000.1z2qjD7m6Jw8x7Z3wM8zjg==.BTvMJs9+gIK6BffA1M0/9KWC+2lvd0cJ5M+m7evVdJ0=', 'Seller', 0);
GO

INSERT INTO dbo.CarListings (SellerId, Title, Brand, Model, [Year], Price, Mileage, FuelType, Transmission, Description, Status, IsLocked)
VALUES
(3, '2019 Honda Civic VTi', 'Honda', 'Civic', 2019, 15500.00, 42000, 'Petrol', 'Automatic', 'Single owner with full service history.', 'Active', 0),
(3, '2021 Hyundai Creta SX', 'Hyundai', 'Creta', 2021, 19800.00, 21000, 'Diesel', 'Manual', 'Well maintained SUV with insurance.', 'Active', 0),
(3, '2018 Maruti Baleno Delta', 'Maruti', 'Baleno', 2018, 8900.00, 55000, 'Petrol', 'Manual', 'City driven hatchback in good condition.', 'Inactive', 0);
GO

INSERT INTO dbo.CarImages (CarListingId, ImageUrl, SortOrder)
VALUES
(1, 'https://example.com/images/civic-front.jpg', 1),
(1, 'https://example.com/images/civic-side.jpg', 2),
(2, 'https://example.com/images/creta-front.jpg', 1),
(3, 'https://example.com/images/baleno-front.jpg', 1);
GO

INSERT INTO dbo.Orders (CarListingId, BuyerId, SellerId, Status, Notes)
VALUES
(1, 2, 3, 'Pending', 'Interested in a weekend inspection.');
GO
