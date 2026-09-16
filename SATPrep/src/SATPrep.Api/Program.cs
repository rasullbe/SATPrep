using SATPrep.Api.Configurations;
using SATPrep.Api.Data;
using SATPrep.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Controller services
builder.Services.AddControllers();

// Database
builder.Services.AddSatPrepDatabase(builder.Configuration);

// CORS
builder.Services.AddSatPrepCors(builder.Configuration);

// JWT Auth + Authorization policies
builder.Services.AddSatPrepAuth(builder.Configuration);

// Domain services + repositories (DI)
builder.Services.AddSatPrepServices();

// Health checks
builder.Services.AddSatPrepHealthChecks();

// Swagger / OpenAPI
builder.Services.AddSatPrepOpenApi();

var app = builder.Build();

// Apply EF Core migrations on startup
await DatabaseMigrator.ApplyAsync(app.Services);

// Seed sample data (idempotent — skips if data already exists)
using (var scope = app.Services.CreateScope())
{
    await SATPrep.Api.Data.DataSeeder.DataSeeder.SeedAllAsync(scope.ServiceProvider.GetRequiredService<SATPrep.Api.Data.AppDbContext>());
}

// Global exception handling (outermost)
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseMiddleware<SecurityHeadersMiddleware>();

// CSRF double-submit cookie protection (before auth, but after static/CORS)
app.UseMiddleware<CsrfMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health").AllowAnonymous();

// Configure the HTTP request pipeline for development tooling.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();