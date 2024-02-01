using Microsoft.EntityFrameworkCore;
using Solar.Services.Abstraction;
using Solar.Services.Implementation;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Explicitly set the default culture to invariant culture
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;


var connectionString = builder.Configuration.GetConnectionString("SolarApplicationDbContext");
Console.WriteLine(connectionString);

// connect to database here - and check if the connection is successful
/*try
{
    using var dbContext = new DiscomDbContext(new DbContextOptionsBuilder<DiscomDbContext>()
        .UseSqlServer(connectionString)
        .Options);
    Console.WriteLine("Database connection successful.");
}
catch (Exception ex)
{
    Console.WriteLine($"Database connection failed: {ex.Message}");
}*/

builder.Services.AddDbContext<DiscomDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});


builder.Services.AddScoped<IConsumerService, ConsumerService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
