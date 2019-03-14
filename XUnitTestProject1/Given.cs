using ClassLibrary1;

namespace XUnitTestProject1
{
    public static class Given
    {
        public static class A
        {
            public static Customer CustomerWithName()
            {
                return CustomerObjectMother.CustomerWithName();
            }
        }
    }
}