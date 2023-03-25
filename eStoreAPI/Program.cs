using BusinessObject;
using BusinessObject.Models;
using DataAccess.UnitOfWork;
using eStoreAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(
          options => {
              options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
          });
//start add odata
builder.Services.AddControllers().AddOData(o => o.Select().Filter().Count().OrderBy().Expand().SetMaxTop(10).AddRouteComponents("odata", StartupExtension.GetEdmModel()));
//end add odata

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer = builder.Configuration.GetRequiredSection("Jwt")["Issuer"],
        ValidAudience = builder.Configuration.GetRequiredSection("Jwt")["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetRequiredSection("Jwt")["Key"])),      
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey= true,
        ValidateLifetime= true
    };
});
builder.Services.AddAuthorization();
builder.Services.AddDbContext<eStoreDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

var admin = new Member()
{
    Email = builder.Configuration.GetSection("eStoreAccount")["Email"],
    Password = builder.Configuration.GetSection("eStoreAccount")["Password"]
};

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//start add odata
app.UseODataBatching();
app.UseRouting();
//end add odata
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
