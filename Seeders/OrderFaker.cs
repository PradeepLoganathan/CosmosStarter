﻿using System;
using System.Security.Cryptography;
using Bogus;
using CosmosStarter.Entities;

namespace CosmosStarter.Seeders
{
    public class OrderFaker : Faker<Order>
    {
        private readonly string[] _systems = new[] { "XPS", "Precision", "Optiplex", "Alienware", "Latitude" };

        public OrderFaker()
        {
            var random = RandomNumberGenerator.Create();
            var bytes = new byte[sizeof(int)]; // 4 bytes 
            random.GetNonZeroBytes(bytes);
            var result = BitConverter.ToInt32(bytes);

            RuleFor(o => o.OrderId, Guid.NewGuid().ToString());
            RuleFor(o => o.Date, f => f.Date.Past(3));
            RuleFor(o => o.OrderValue, f => f.Finance.Amount(0, 10000));
            RuleFor(o => o.Config, f => f.PickRandom<string>(_systems));
            RuleFor(o => o.Shipped, f => f.Random.Bool(0.9f));
        }
    }
}