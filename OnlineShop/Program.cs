using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
//Database doesn't exist: Scaffold-DbContext "Server=.;Database=OnlineShop;Integrated Security=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
//Database already exists: Scaffold-DbContext "Server=.;Database=OnlineShop;Integrated Security=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force