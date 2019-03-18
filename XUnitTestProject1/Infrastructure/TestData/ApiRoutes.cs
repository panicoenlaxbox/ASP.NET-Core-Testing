namespace XUnitTestProject1.Infrastructure.TestData
{
    // Use it if we don't use Acheve
    public static class ApiRoutes
    {
        public static class Get
        {
            public static string Customer()
            {
                return "api/customers";
            }
            public static string Customer(int id)
            {
                return $"{Customer()}/{id}";
            }
        }
    }
}