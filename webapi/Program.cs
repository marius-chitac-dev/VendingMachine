using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using webapi.Core.Interfaces;
using webapi.Infrastructure;
using webapi.Infrastructure.Extensions;
using webapi.Infrastructure.Options;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); ;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection(AppSettingsOptions.Name).Get<AppSettingsOptions>()!.SigningKey))
    };

    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents()
    {
        OnAuthenticationFailed = c =>
        {
            var x = c.Principal;
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddTransient<IJwtGenerator, JwtGenerator>();
builder.Services.AddTransient<IChangeCalculator, ChangeCalculator>();
builder.Services.AddDbContext<VendingMachineContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("VendingMachineContext")));
builder.Services.AddOptions<AppSettingsOptions>()
            .Bind(builder.Configuration.GetSection(AppSettingsOptions.Name))
            .ValidateDataAnnotations()
            .ValidateOnStart();
builder.Services.AddOptions<VendingOptions>()
            .Bind(builder.Configuration.GetSection(VendingOptions.Name))
            .ValidateDataAnnotations()
            .ValidateOnStart();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.EnsureDatabaseMigration();

app.Run();
public partial class Program
{ }