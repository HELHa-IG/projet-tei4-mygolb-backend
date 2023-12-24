using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyGolb.Data;
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:8000", "https://localhost:8080");
builder.Services.AddDbContext<MyGolbContext>(options =>
    // Use this one for MS SQL Server
    //options.UseSqlServer(builder.Configuration.GetConnectionString("MyGolbContext") ?? throw new InvalidOperationException("Connection string 'MyGolbContext' not found.")));
    // Use this one for SQLite
    options.UseSqlite(builder.Configuration.GetConnectionString("MyGolbContextLite") ?? throw new InvalidOperationException("Connection string 'MyGolbContext' not found.")));

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyGolbPolicy",
        policy =>
        {
            policy.WithOrigins("*")
                .WithMethods("*")
                .WithHeaders("*");
        });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
    
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyGolbPolicy"");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
