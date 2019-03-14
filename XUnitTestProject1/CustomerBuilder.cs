using ClassLibrary1;

namespace XUnitTestProject1
{
    public class CustomerBuilder
    {
        private readonly Customer _customer;

        public CustomerBuilder()
        {
            _customer = new Customer();
        }

        public CustomerBuilder WithName(string name)
        {
            _customer.Name = name;
            return this;
        }

        public CustomerBuilder WithId(int id)
        {
            _customer.Id = id;
            return this;
        }

        public static implicit operator Customer(CustomerBuilder builder)
        {
            return builder._customer;
        }
    }
}