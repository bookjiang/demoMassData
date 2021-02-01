using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace demoMassData.Models
{
    class DemoDbContext:DbContext
    {
        //public DemoDbContext(DbContextOptions<DemoDbContext> options):base(options)
        //{

        //}
        public DbSet<Company> company { get; set; }
        public DbSet<test1> test1  { get; set; }
        private IConfiguration configuration;
        //private readonly IHostingEnvironment _hostingEnvironment;
        public DemoDbContext()
        {
            configuration = new ConfigurationBuilder().SetBasePath("E:\\software\\Microsoft Visual Studio\\source\\repos\\demoMassData\\").AddJsonFile("appsettings.json").Build();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection"));
        }

    }
}
