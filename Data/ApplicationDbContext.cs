﻿using AuthenticationServer.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationServer.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
