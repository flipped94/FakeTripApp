using FakeTrip.StartupConfig;

var builder = WebApplication.CreateBuilder(args);

builder.AddStandard();

builder.AddDatabaseContext();

builder.AddCustomService();

var app = builder.Build();

app.MapControllers();

app.Run();
