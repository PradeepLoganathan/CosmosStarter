using Bogus;
using CosmosStarter.Entities;

namespace CosmosStarter.Seeders
{
    public class ContactFaker : Faker<Contact>
    {
        public ContactFaker()
        {
            RuleFor(cust => cust.FirstName, f => f.Name.FirstName());
            RuleFor(cust => cust.LastName, f => f.Name.LastName());
            RuleFor(o => o.Phone, f => f.Person.Phone);
            RuleFor(o => o.EmailAddress, (f, u) => f.Internet.Email());
        }


    }
}