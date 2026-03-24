namespace CarMarketplace.Api.Common;

public abstract class AppException(string message) : Exception(message);

public sealed class NotFoundException(string message) : AppException(message);

public sealed class ConflictException(string message) : AppException(message);

public sealed class ForbiddenException(string message) : AppException(message);

public sealed class BadRequestException(string message) : AppException(message);

public sealed class UnauthorizedAppException(string message) : AppException(message);
