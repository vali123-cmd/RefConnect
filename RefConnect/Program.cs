using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using RefConnect.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models; 
using RefConnect.Models;
using RefConnect.Services.Implementations;
using RefConnect.Services.Interfaces;
using System.Net.Http.Headers;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;


var builder = WebApplication.CreateBuilder(args);

// HTTPS/HTTP endpoints (dev-friendly defaults)
// - HTTPS uses the ASP.NET Core dev-certificate ("dotnet dev-certs https")
// - Ports match `Properties/launchSettings.json`

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000);
    options.ListenLocalhost(7016, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});


if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5000);
        options.ListenLocalhost(7016, listenOptions => listenOptions.UseHttps());
    });
}


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDevClient", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()    
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Conexiunea la baza de date nu a fost gasita.");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        serverVersion
    )
);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true; //Email MUST be unique, .NET only defaults username as unique

    
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
   
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

builder.Services.AddHttpClient<RefConnect.Services.Interfaces.IRefinePostTextAI, RefConnect.Services.Implementations.RefinePostTextAIService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OpenAI:ApiUrl"] ?? "https://api.openai.com/v1/");
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", builder.Configuration["OpenAI:ApiKey"] ?? string.Empty);
});


builder.Services.AddControllers();
// Register application services
builder.Services.AddScoped<RefConnect.Services.Interfaces.IProfileService, RefConnect.Services.Implementations.ProfileService>();
builder.Services.AddScoped<IFollowRequestService, FollowRequestService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatJoinRequestService, ChatJoinRequestService>();

// Replace the AddAWSService call with an explicit registration:
var awsAccessKey = builder.Configuration["AWS:AccessKey"];
var awsSecretKey = builder.Configuration["AWS:SecretKey"];
var awsRegion = builder.Configuration["AWS:Region"] ?? "us-east-1";

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var regionEndpoint = RegionEndpoint.GetBySystemName(awsRegion);
    var s3Config = new AmazonS3Config { RegionEndpoint = regionEndpoint };

    
    try
    {
        if (!string.IsNullOrWhiteSpace(awsAccessKey) && !string.IsNullOrWhiteSpace(awsSecretKey))
        {
            
            var credentials = new BasicAWSCredentials(awsAccessKey.Trim(), awsSecretKey.Trim());
            return new AmazonS3Client(credentials, s3Config);
        }
    }
    catch
    {
        
    }

    return new AmazonS3Client(s3Config);
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User", "Moderator" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}


await SeedData.SeedAdminAsync(app.Services, app.Configuration);

    
    app.UseStaticFiles();
    app.UseCors("AllowReactDevClient");
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
