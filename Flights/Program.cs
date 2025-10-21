using System.Reflection;
using Flights.Application.Abstractions;
using Flights.Application.Common.Behaviors;
using Flights.Infrastructure.Auth;
using Flights.Infrastructure.Cache;
using Flights.Infrastructure.Logging;
using Flights.Infrastructure.Persistence;
using Flights.Infrastructure.Persistence.Seed;
using FluentValidation;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Flights.Application.Abstractions.Repositories;
using Flights.Infrastructure.Persistence.Repositories;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Flights.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);


// Serilog
builder.AddSerilogLogging();


string? currentUser = null;

// EF Core (SQLite)
builder.Services.AddDbContext<AppDbContext>(opt =>
opt.AddInterceptors(new ChangeLoggingInterceptor(() => currentUser))
.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


// MediatR + FluentValidation
builder.Services.AddMediatR(Assembly.Load("Flights.Application"));
builder.Services.AddValidatorsFromAssembly(Assembly.Load("Flights.Application"));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IFlightCache, FlightMemoryCache>();


// JWT Auth
builder.Services.AddSingleton<JwtTokenService>();
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddAuthorization();


builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Flights API", Version = "v1" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, new string[] { } } });
});


var app = builder.Build();


app.Use(async (ctx, next) =>
{
    currentUser = ctx.User?.Identity?.Name;
    await next();
});

app.Use(async (ctx, next) =>
{
    using (Serilog.Context.LogContext.PushProperty("Username", ctx.User?.Identity?.Name ?? "anonymous"))
        await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


// Ensure DB + seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db, CancellationToken.None);
}


app.Run();