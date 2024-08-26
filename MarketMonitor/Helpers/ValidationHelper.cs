namespace MarketMonitorApp.Helpers
{
    public static class ValidationHelper
    {
       public static void ValidateProductName(string productName)
        {
            if (string.IsNullOrEmpty(productName))
            {
                throw new NullReferenceException("Product name node is null or empty.");
            }
       }
    }
}
