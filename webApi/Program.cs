//using System;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.CodeAnalysis.Options;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.OpenApi.Models;
//using webApi.Models.Data;
//using WebAPIDemo.Services;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// Database connection
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//builder.Services.AddSingleton<TokenService>();
//builder.Services.AddDbContext<AppDbcontext>(
//    options =>
//      options.UseSqlServer(connectionString)) ;

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowReactApp",
//        builder =>
//        {
//            builder
//                .WithOrigins("http://localhost:5173", "http://localhost:5174")
//                // Your React app's URL
//                .AllowAnyMethod()
//                .AllowAnyHeader()
//                .AllowCredentials();
//        });
//});

//// Configure JWT Authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.RequireHttpsMetadata = false;
//        options.SaveToken = true;
//        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//        };
//    });

//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
////builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(opt =>
//{
//    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
//    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "Please enter token",
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        BearerFormat = "JWT",
//        Scheme = "bearer"
//    });
//    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type=ReferenceType.SecurityScheme,
//                    Id="Bearer"
//                }
//            },
//            new string[]{}
//        }
//    });

//});

//var app = builder.Build();
//// Swagger UI
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseCors("AllowReactApp");
//app.UseHttpsRedirection();

//app.UseAuthorization();
//app.MapControllers();

//app.Run();


using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using webApi.Models.Data;
using WebAPIDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger configuration with JWT support
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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
            new string[] {}
        }
    });
});

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbcontext>(options =>
    options.UseSqlServer(connectionString));

// Dependency injection
builder.Services.AddSingleton<TokenService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:5173", "http://localhost:5174") // your frontend dev ports
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// JWT Authentication configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

// Swagger UI (only in dev)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware order matters!
app.UseHttpsRedirection();
app.UseRouting();                    // ✅ REQUIRED before CORS/auth
app.UseCors("AllowReactApp");        // ✅ CORS after routing
app.UseAuthentication();             // ✅ Add this line if using JWT
app.UseAuthorization();

app.MapControllers();

app.Run();
