namespace MarketMonitorApp.Helpers
{
    public static class ValidationHelper
    {
        public static void ValidateProductName(string productName)
        {
            if (string.IsNullOrEmpty(productName))
            {
                throw new ArgumentException("Product name cannot be null or empty.", nameof(productName));
            }
        }
    }
}
