namespace XUnitTestProject1
{
    public static class Api
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