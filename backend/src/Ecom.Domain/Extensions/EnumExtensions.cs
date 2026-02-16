using Ecom.Domain.Enums;

namespace Ecom.Domain.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets all valid order status values as strings
        /// </summary>
        public static string[] GetAllOrderStatusValues()
        {
            return Enum.GetNames<OrderStatusEnum>();
        }

        /// <summary>
        /// Gets all valid payment status values as strings
        /// </summary>
        public static string[] GetAllPaymentStatusValues()
        {
            return Enum.GetNames<PaymentStatusEnum>();
        }

        /// <summary>
        /// Tries to parse a string to OrderStatusEnum
        /// </summary>
        public static bool TryParseOrderStatus(string value, out OrderStatusEnum status)
        {
            return Enum.TryParse(value, true, out status);
        }

        /// <summary>
        /// Tries to parse a string to PaymentStatusEnum
        /// </summary>
        public static bool TryParsePaymentStatus(string value, out PaymentStatusEnum status)
        {
            return Enum.TryParse(value, true, out status);
        }
    }
}
