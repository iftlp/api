using Microsoft.EntityFrameworkCore;
using Serilog;
using Shizima.Business.Services;
using Shizima.Providers;

var builder = WebApplication.CreateBuilder(args);
var corsPolicy = "corsPolicy";

// Add services to the container
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(connectionString, m => m.MigrationsAssembly("Shizima.WebAPI")));

builder.Services.AddTransient<UnitOfWork>();
builder.Services.AddTransient<MusicService>();

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(corsPolicy);

app.Run();
