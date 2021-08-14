using EfSqlServer.Docker;
using EfSqlServer.Ef;
using EfSqlServer.Helpers;
using EfSqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EfSqlServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();
            var loggerFactory = new LoggerFactory(new[] { new SerilogLoggerProvider() });

            var dockerSetup = new DockerSetup();
            dockerSetup.RunDockerContainer();
            dockerSetup.SetupDatabase();

            dockerSetup.GetDbConnectionString();
            var dbOptionsBuilder = new DbContextOptionsBuilder<EfSqlServerDbContext>();
            dbOptionsBuilder.UseLoggerFactory(loggerFactory)
                .EnableSensitiveDataLogging()
                .UseSqlServer(dockerSetup.GetDbConnectionString(),
                    options => options.EnableRetryOnFailure(2));
            var ctx = new EfSqlServerDbContext(dbOptionsBuilder.Options);

            var timestampFrom = DateTime.UtcNow.AddYears(-2);

            var query = ctx.Foods.Where(a => a.Timestamp > timestampFrom).Select(FoodResultHelpers.Map());

            // Suppose multiple OR conditions cannot be combined into a single Where clause.
            // In this case, create a list with expressions of multiple OR conditions
            var orExpressions = new List<Expression<Func<FoodResultModel, bool>>>();
            orExpressions.Add(a => a.Code == 246018);
            orExpressions.Add(a => a.Ingredient == "Onion");
            orExpressions.Add(a => a.Code == 246020);

            query.WhereAny(orExpressions);

            var result = await query.ToListAsync();
        }
    }
}
