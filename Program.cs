using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using TaskCircle.AuthentcationApi.Infrastructure.Repositories;
using TaskCircle.AuthentcationApi.Infrastructure.Repositories.Iterfaces;
using TaskCircle.AuthentcationApi.Infrastructure.Services;
using TaskCircle.AuthentcationApi.Infrastructure.Services.Interfaces;
using TaskCircle.AuthentcationApi.Infrastructure.Setting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AuthApi",
        Version = "v1",
        Description = "Authentication Api With OpenAPI documentation",
        Contact = new OpenApiContact
        {
            Name = "Henrique Araújo Neto 2023/24",
            Email = "anhenrique93@gmail.com",
            Url = new Uri("http://anhenrique.netlify.app/")
        },
    });

    var filePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "AuthentcationApi.xml");
    options.IncludeXmlComments(filePath);

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer Scheme (\"bearer  {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

//Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
            ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("Jwt:Key").Value))
        };
    });

// Connection DB
builder.Services.Configure<ConnectionSetting>(builder.Configuration.GetSection("ConnectionSetting"));

// AutoMapper for DTOs
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Repoitories and Services DI
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthApi v1");
    });
} else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthApi v1");
    });
}

// Configuração CORS - deve vir após app.UseEndpoints
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
