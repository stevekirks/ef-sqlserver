## To generate the EF DB Context models
1.   Install the EF Core tools - see https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet
2.   Generate the models by running the following console command from the project folder:
	 `dotnet ef dbcontext scaffold "Server=tcp:localhost,41707;Database=testdb;User ID=sa;Password=thisStrong(!)Password;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;MultipleActiveResultSets=True;" Microsoft.EntityFrameworkCore.SqlServer --context EfSqlServerDbContext --data-annotations --output-dir Ef --force -t "dbo.Food"`
4.   In the generated context file `EfSqlServerDbContext.cs`, 
	 -   remove the `OnConfiguring(DbContextOptionsBuilder optionsBuilder)` method.
	 -   update the base class to be fully qualified, ie. : Microsoft.EntityFrameworkCore.DbContext