using System.Data;
using System.Text;
using CarMarketplace.Api.Common;
using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Entities;
using Dapper;

namespace CarMarketplace.Api.Repositories;

public class CarRepository(IDbConnection dbConnection) : ICarRepository
{
    public async Task<(IReadOnlyCollection<CarListing> Items, int TotalCount)> SearchPublicAsync(CarSearchRequestDto request, CancellationToken cancellationToken)
    {
        var filterSql = new StringBuilder("""
            FROM CarListings cl
            INNER JOIN Users u ON u.Id = cl.SellerId
            WHERE cl.Status = @ActiveStatus
              AND cl.IsLocked = 0
              AND (@Keyword IS NULL OR cl.Title LIKE @KeywordLike OR cl.Brand LIKE @KeywordLike OR cl.Model LIKE @KeywordLike)
              AND (@Brand IS NULL OR cl.Brand = @Brand)
              AND (@Model IS NULL OR cl.Model = @Model)
              AND (@MinPrice IS NULL OR cl.Price >= @MinPrice)
              AND (@MaxPrice IS NULL OR cl.Price <= @MaxPrice)
              AND (@Year IS NULL OR cl.Year = @Year)
              AND (@FuelType IS NULL OR cl.FuelType = @FuelType)
              AND (@Transmission IS NULL OR cl.Transmission = @Transmission)
            """);

        var parameters = new
        {
            ActiveStatus = AppConstants.ListingStatuses.Active,
            request.Keyword,
            KeywordLike = string.IsNullOrWhiteSpace(request.Keyword) ? null : $"%{request.Keyword.Trim()}%",
            Brand = request.Brand?.Trim(),
            Model = request.Model?.Trim(),
            request.MinPrice,
            request.MaxPrice,
            request.Year,
            FuelType = request.FuelType?.Trim(),
            Transmission = request.Transmission?.Trim(),
            Offset = (request.Page - 1) * request.PageSize,
            request.PageSize
        };

        var countSql = $"SELECT COUNT(1) {filterSql};";
        var totalCount = await dbConnection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

        var listSql = $"""
            SELECT cl.Id, cl.SellerId, CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   cl.Title, cl.Brand, cl.Model, cl.Year, cl.Price, cl.Mileage, cl.FuelType,
                   cl.Transmission, cl.Description, cl.Status, cl.IsLocked, cl.CreatedAt, cl.UpdatedAt
            {filterSql}
            ORDER BY cl.CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var cars = (await dbConnection.QueryAsync<CarListing>(new CommandDefinition(listSql, parameters, cancellationToken: cancellationToken))).ToArray();
        await LoadImagesAsync(cars, cancellationToken);

        return (cars, totalCount);
    }

    public async Task<CarListing?> GetPublicByIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP 1 cl.Id, cl.SellerId, CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   cl.Title, cl.Brand, cl.Model, cl.Year, cl.Price, cl.Mileage, cl.FuelType,
                   cl.Transmission, cl.Description, cl.Status, cl.IsLocked, cl.CreatedAt, cl.UpdatedAt
            FROM CarListings cl
            INNER JOIN Users u ON u.Id = cl.SellerId
            WHERE cl.Id = @Id
              AND cl.Status = @ActiveStatus
              AND cl.IsLocked = 0;
            """;

        var car = await dbConnection.QuerySingleOrDefaultAsync<CarListing>(new CommandDefinition(
            sql,
            new { Id = id, ActiveStatus = AppConstants.ListingStatuses.Active },
            cancellationToken: cancellationToken));

        if (car is not null)
        {
            car.Images = (await GetImagesAsync(car.Id, cancellationToken)).ToList();
        }

        return car;
    }

    public async Task<CarListing?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP 1 cl.Id, cl.SellerId, CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   cl.Title, cl.Brand, cl.Model, cl.Year, cl.Price, cl.Mileage, cl.FuelType,
                   cl.Transmission, cl.Description, cl.Status, cl.IsLocked, cl.CreatedAt, cl.UpdatedAt
            FROM CarListings cl
            INNER JOIN Users u ON u.Id = cl.SellerId
            WHERE cl.Id = @Id;
            """;

        var car = await dbConnection.QuerySingleOrDefaultAsync<CarListing>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        if (car is not null)
        {
            car.Images = (await GetImagesAsync(car.Id, cancellationToken)).ToList();
        }

        return car;
    }

    public async Task<int> CreateAsync(CarListing carListing, CancellationToken cancellationToken)
    {
        await EnsureOpenAsync(cancellationToken);
        using var transaction = dbConnection.BeginTransaction();

        try
        {
            const string insertListingSql = """
                INSERT INTO CarListings (SellerId, Title, Brand, Model, Year, Price, Mileage, FuelType, Transmission, Description, Status, IsLocked, CreatedAt, UpdatedAt)
                VALUES (@SellerId, @Title, @Brand, @Model, @Year, @Price, @Mileage, @FuelType, @Transmission, @Description, @Status, 0, SYSUTCDATETIME(), SYSUTCDATETIME());
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;

            var carId = await dbConnection.ExecuteScalarAsync<int>(new CommandDefinition(
                insertListingSql,
                carListing,
                transaction,
                cancellationToken: cancellationToken));

            const string insertImageSql = """
                INSERT INTO CarImages (CarListingId, ImageUrl, SortOrder, CreatedAt)
                VALUES (@CarListingId, @ImageUrl, @SortOrder, SYSUTCDATETIME());
                """;

            foreach (var image in carListing.Images)
            {
                image.CarListingId = carId;
                await dbConnection.ExecuteAsync(new CommandDefinition(insertImageSql, image, transaction, cancellationToken: cancellationToken));
            }

            transaction.Commit();
            return carId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task UpdateAsync(CarListing carListing, CancellationToken cancellationToken)
    {
        await EnsureOpenAsync(cancellationToken);
        using var transaction = dbConnection.BeginTransaction();

        try
        {
            const string updateListingSql = """
                UPDATE CarListings
                SET Title = @Title,
                    Brand = @Brand,
                    Model = @Model,
                    Year = @Year,
                    Price = @Price,
                    Mileage = @Mileage,
                    FuelType = @FuelType,
                    Transmission = @Transmission,
                    Description = @Description,
                    Status = @Status,
                    UpdatedAt = SYSUTCDATETIME()
                WHERE Id = @Id;
                """;

            await dbConnection.ExecuteAsync(new CommandDefinition(updateListingSql, carListing, transaction, cancellationToken: cancellationToken));

            const string deleteImagesSql = "DELETE FROM CarImages WHERE CarListingId = @CarListingId;";
            await dbConnection.ExecuteAsync(new CommandDefinition(deleteImagesSql, new { CarListingId = carListing.Id }, transaction, cancellationToken: cancellationToken));

            const string insertImageSql = """
                INSERT INTO CarImages (CarListingId, ImageUrl, SortOrder, CreatedAt)
                VALUES (@CarListingId, @ImageUrl, @SortOrder, SYSUTCDATETIME());
                """;

            foreach (var image in carListing.Images)
            {
                image.CarListingId = carListing.Id;
                await dbConnection.ExecuteAsync(new CommandDefinition(insertImageSql, image, transaction, cancellationToken: cancellationToken));
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            DELETE FROM CarListings
            WHERE Id = @Id;
            """;

        await dbConnection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<CarListing>> GetBySellerAsync(int sellerId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT cl.Id, cl.SellerId, CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   cl.Title, cl.Brand, cl.Model, cl.Year, cl.Price, cl.Mileage, cl.FuelType,
                   cl.Transmission, cl.Description, cl.Status, cl.IsLocked, cl.CreatedAt, cl.UpdatedAt
            FROM CarListings cl
            INNER JOIN Users u ON u.Id = cl.SellerId
            WHERE cl.SellerId = @SellerId
            ORDER BY cl.CreatedAt DESC;
            """;

        var cars = (await dbConnection.QueryAsync<CarListing>(new CommandDefinition(sql, new { SellerId = sellerId }, cancellationToken: cancellationToken))).ToArray();
        await LoadImagesAsync(cars, cancellationToken);
        return cars;
    }

    public async Task<IReadOnlyCollection<CarListing>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT cl.Id, cl.SellerId, CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   cl.Title, cl.Brand, cl.Model, cl.Year, cl.Price, cl.Mileage, cl.FuelType,
                   cl.Transmission, cl.Description, cl.Status, cl.IsLocked, cl.CreatedAt, cl.UpdatedAt
            FROM CarListings cl
            INNER JOIN Users u ON u.Id = cl.SellerId
            ORDER BY cl.CreatedAt DESC;
            """;

        var cars = (await dbConnection.QueryAsync<CarListing>(new CommandDefinition(sql, cancellationToken: cancellationToken))).ToArray();
        await LoadImagesAsync(cars, cancellationToken);
        return cars;
    }

    public async Task SetLockedAsync(int id, bool isLocked, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE CarListings
            SET IsLocked = @IsLocked,
                UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @Id;
            """;

        await dbConnection.ExecuteAsync(new CommandDefinition(sql, new { Id = id, IsLocked = isLocked }, cancellationToken: cancellationToken));
    }

    private async Task EnsureOpenAsync(CancellationToken cancellationToken)
    {
        if (dbConnection is not null && dbConnection.State != ConnectionState.Open)
        {
            if (dbConnection is System.Data.Common.DbConnection connection)
            {
                await connection.OpenAsync(cancellationToken);
            }
            else
            {
                dbConnection.Open();
            }
        }
    }

    private async Task LoadImagesAsync(IReadOnlyCollection<CarListing> cars, CancellationToken cancellationToken)
    {
        var carIds = cars.Select(car => car.Id).ToArray();
        if (carIds.Length == 0)
        {
            return;
        }

        var images = await dbConnection.QueryAsync<CarImage>(new CommandDefinition(
            """
            SELECT Id, CarListingId, ImageUrl, SortOrder, CreatedAt
            FROM CarImages
            WHERE CarListingId IN @CarIds
            ORDER BY SortOrder;
            """,
            new { CarIds = carIds },
            cancellationToken: cancellationToken));

        var imageMap = images.GroupBy(image => image.CarListingId).ToDictionary(group => group.Key, group => group.ToList());
        foreach (var car in cars)
        {
            car.Images = imageMap.GetValueOrDefault(car.Id, []);
        }
    }

    private async Task<IReadOnlyCollection<CarImage>> GetImagesAsync(int carId, CancellationToken cancellationToken)
    {
        var images = await dbConnection.QueryAsync<CarImage>(new CommandDefinition(
            """
            SELECT Id, CarListingId, ImageUrl, SortOrder, CreatedAt
            FROM CarImages
            WHERE CarListingId = @CarListingId
            ORDER BY SortOrder;
            """,
            new { CarListingId = carId },
            cancellationToken: cancellationToken));

        return images.ToArray();
    }
}
