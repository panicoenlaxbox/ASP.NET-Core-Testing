using ClassLibrary1;

namespace XUnitTestProject1
{
    public static class CustomerObjectMother
    {
        public static Customer CustomerWithName()
        {
            return Build.Customer()
                .WithName("Customer 1");
        }
    }
}