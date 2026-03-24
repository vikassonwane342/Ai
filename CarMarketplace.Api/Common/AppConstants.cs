namespace CarMarketplace.Api.Common;

public static class AppConstants
{
    public static class Roles
    {
        public const string Buyer = "Buyer";
        public const string Seller = "Seller";
        public const string Admin = "Admin";

        public static readonly HashSet<string> AllowedRegistrationRoles =
        [
            Buyer,
            Seller
        ];
    }

    public static class ListingStatuses
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Sold = "Sold";

        public static readonly HashSet<string> Allowed =
        [
            Active,
            Inactive,
            Sold
        ];
    }

    public static class OrderStatuses
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Rejected = "Rejected";
        public const string Cancelled = "Cancelled";

        public static readonly HashSet<string> AllowedUpdates =
        [
            Confirmed,
            Rejected,
            Cancelled
        ];
    }
}
