using Bogus;
using CosmosStarter.Entities;

namespace CosmosStarter.Seeders
{
    public class AddressFaker: Faker<Address>
    {
        public AddressFaker()
        {
            RuleFor(a => a.StreetAddress, f => f.Address.StreetAddress());
            RuleFor(a => a.City, f => f.Address.City());
            RuleFor(a => a.State, f => f.Address.State());
            RuleFor(a => a.County, f => f.Address.County());
            RuleFor(a => a.ZipCode, f => f.Address.ZipCode());        
        }
        
    }
}