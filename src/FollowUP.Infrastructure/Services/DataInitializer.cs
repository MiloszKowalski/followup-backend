﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public class DataInitializer : IDataInitializer
    {
        private readonly IUserService _userService;

        public DataInitializer(IUserService userService)
        {
            _userService = userService;
        }

        public async Task SeedAsync()
        {
            var users = await _userService.BrowseAsync();
            if (users.Any())
            {
                Console.WriteLine("Data was already initialized");

                return;
            }
            Console.WriteLine("Initializing data...");
            var tasks = new List<Task>();
            for (var i = 1; i <= 10; i++)
            {
                var userId = Guid.NewGuid();
                var username = $"user{i}";
                await _userService.RegisterAsync(userId, $"user{i}@test.com",
                                                 username, "secret", "user");
                Console.WriteLine($"Adding user: '{username}'.");
            }
            for (var i = 1; i <= 3; i++)
            {
                var userId = Guid.NewGuid();
                var username = $"admin{i}";
                Console.WriteLine($"Adding admin: '{username}'.");
                await _userService.RegisterAsync(userId, $"admin{i}@test.com",
                    username, "secret", "admin");
            }
            Console.WriteLine("Data was initialized.");
        }
    }
}
