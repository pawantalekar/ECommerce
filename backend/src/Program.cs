options.UseSqlServer(connectionString)
       .EnableSensitiveDataLogging() // optional, shows parameter values
       .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);