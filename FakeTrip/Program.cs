using FakeTrip.StartupConfig;

var builder = WebApplication.CreateBuilder(args);

builder.AddStandard();

builder.AddAuthServices();

builder.AddDatabaseContext();

builder.AddCustomServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
