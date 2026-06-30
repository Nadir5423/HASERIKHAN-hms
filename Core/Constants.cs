namespace HMS.Core
{
    public static class Constants
    {
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string Manager = "Manager";
            public const string Receptionist = "Receptionist";
            public const string Customer = "Customer";
        }

        public static class SessionKeys
        {
            public const string UserId = "UserId";
            public const string Username = "Username";
            public const string Role = "Role";
        }

        public static class RoomStatus
        {
            public const string Available = "Available";
            public const string Occupied = "Occupied";
            public const string Maintenance = "Maintenance";
            public const string Reserved = "Reserved";
        }

        public static class BookingStatus
        {
            public const string Pending = "Pending";
            public const string Confirmed = "Confirmed";
            public const string CheckedIn = "CheckedIn";
            public const string CheckedOut = "CheckedOut";
            public const string Cancelled = "Cancelled";
        }

        public static class PaymentStatus
        {
            public const string Pending = "Pending";
            public const string Paid = "Completed";
            public const string Failed = "Failed";
            public const string Refunded = "Refunded";
        }

        public static class PaymentMethods
        {
            public const string Cash = "Cash";
            public const string CreditCard = "Credit Card";
            public const string DebitCard = "Debit Card";
            public const string Online = "Online";
        }
    }
}
