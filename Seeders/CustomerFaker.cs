using Bogus;
using CosmosStarter.Entities;

namespace CosmosStarter.Seeders
{
    public class CustomerFaker : Faker<Customer>
    {
        public CustomerFaker()
        {
            CustomInstantiator(f => new Customer(new Randomizer().Replace("CU#-##-####")));
            RuleFor(cust => cust.ModifiedDate, f => f.Date.Recent(10));
            RuleFor(cust => cust.Title, f => f.Name.Prefix(f.Person.Gender));
            RuleFor(cust => cust.Suffix, f => f.Name.Suffix());
            RuleFor(cust => cust.MiddleName, f => f.Name.FirstName());
            RuleFor(cust => cust.SalesPerson, f => f.Name.FullName());
            RuleFor(cust => cust.CompanyName, f => f.Company.CompanyName());
            RuleFor(cust => cust.CreditLimit, f => f.Finance.Amount(100, 1000));
            RuleFor(cust => cust.Address, f => new AddressFaker().Generate());
            RuleFor(cust => cust.Contact, f => new ContactFaker().Generate());        
        }
        
    }
}