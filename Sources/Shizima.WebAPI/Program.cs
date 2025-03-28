using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Shizima.Business.Services;
using Shizima.Providers;
using Shizima.WebAPI.Authentication;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var corsPolicy = "corsPolicy";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

// Add services to the container
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(connectionString, m => m.MigrationsAssembly("Shizima.WebAPI")));

builder.Services.AddTransient<UnitOfWork>();
builder.Services.AddTransient<TokenProvider>();
builder.Services.AddTransient<MusicService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<AdministratorService>();

builder.Services.AddCors(options => options.AddPolicy(name: corsPolicy, builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"))
                .AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext().CreateLogger());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

/*
 * This API is currently deploy in a container.
 * https is handle outside of it. (Through traefik)
 *
 * Therefore, HTTPS redirection within the application is unnecessary and should remain disabled.
 * Add `app.UseHttpsRedirection();` only if HTTPS is handled directly within the application.
 */
if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            var dbContext = services.GetRequiredService<DatabaseContext>();
            dbContext.Database.Migrate();
            logger.LogInformation("Migrating database ...");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    } 
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors(corsPolicy);

app.Run();
