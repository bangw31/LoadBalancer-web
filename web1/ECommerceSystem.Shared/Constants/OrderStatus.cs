using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceSystem.Shared.Constants
{
    public static class OrderStatus
    {
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Processing = "Processing";
        public const string Delivered = "Delivered";
        public const string Failed = "Failed";
    }

    public static class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
    public static class AppConstants
    {
        public const string ApiBaseUrl = "https://localhost:7068";
        public const string HubPattern = "/hubs/social-hub";
        public const string HubFullUrl = ApiBaseUrl + HubPattern;
    }
}
