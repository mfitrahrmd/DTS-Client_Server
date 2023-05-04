using System.Net.Mime;
using System.Text;
using System.Text.Json.Serialization;
using API.Data;
using API.Extensions.Filters;
using API.Profiles;
using API.Repositories.Contracts;
using API.Repositories.Implementations;
using API.Services;
using API.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<Tugas6Context>();
builder.Services.AddScoped<IUniversityRepository, UniversityRepository<Tugas6Context>>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository<Tugas6Context>>();
builder.Services.AddScoped<IRoleRepository, RoleRepository<Tugas6Context>>();
builder.Services.AddScoped<IEducationRepository, EducationRepository<Tugas6Context>>();
builder.Services.AddScoped<IProfilingRepository, ProfilingRepository<Tugas6Context>>();
builder.Services.AddScoped<IAccountRepository, AccountRepository<Tugas6Context>>();
builder.Services.AddScoped<IAccountRoleRepository, AccountRoleRepository<Tugas6Context>>();
builder.Services.AddSingleton<JwtUtil>();
builder.Services.AddSingleton<BcryptUtil>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddControllers(options =>
{
}).ConfigureApiBehaviorOptions(options => {  }).AddJsonOptions(
    options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull; // omit properties with null
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles; // ignore cardinality include cycle
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configure CORS
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5209");
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    }));

builder.Services.AddAutoMapper(expression =>
{
    expression.AddProfile<EmployeeProfile>();
    expression.AddProfile<EducationProfile>();
    expression.AddProfile<UniversityProfile>();
    expression.AddProfile<AccountProfile>();
    expression.AddProfile<RoleProfile>();
    expression.AddProfile<ProfilingProfile>();
    expression.AddProfile<AccountRoleProfile>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["jwt:Issuer"],
            ValidAudience = builder.Configuration["jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    // only admin can access resource with all methods
    // user can only access resource with get method
    options.AddPolicy("ViewOnlyUser", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
        {
            var httpContext = context.Resource as HttpContext;

            switch (httpContext?.Request.Method)
            {
                case "GET":
                    return true;
                default:
                    return context.User.IsInRole("admin");
            }
        });
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Access Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
});

builder.Services.AddRouting(options => { options.LowercaseUrls = true; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(applicationBuilder =>
    {
        applicationBuilder.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync("{\"message\":\"unexpected server error\"}");
        });
    });
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();