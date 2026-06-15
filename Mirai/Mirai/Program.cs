using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mirai.Infastructure;
using Mirai.MiddleWare;
using Mirai.Supabase;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SupabaseSettings>(
    builder.Configuration.GetSection("Supabase")
);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mirai API",
        Version = "v1",
        Description = "Includes Admin APIs at /api/admin (requires JWT with Role = 1)."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("JWT Key is missing");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

var supabaseIssuer = builder.Configuration["Supabase:Issuer"];
var jwksUrl = builder.Configuration["Supabase:JwksUrl"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)
            ),

            ValidateIssuer = true,

            ValidIssuers = new[]
            {
                jwtIssuer,
                supabaseIssuer
            },

            ValidateAudience = true,

            ValidAudiences = new[]
            {
                jwtAudience,
                "authenticated"
            },

            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero,

            NameClaimType = ClaimTypes.NameIdentifier
        };

        options.ConfigurationManager =
            new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{supabaseIssuer}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever()
            );

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var identity = context.Principal?.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    var email = context.Principal?.FindFirst("email")?.Value;

                    if (!string.IsNullOrEmpty(email))
                    {
                        identity.AddClaim(
                            new Claim(ClaimTypes.Name, email)
                        );
                    }
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mirai API v1");
        c.RoutePrefix = "swagger";
    });


app.UseCors("AllowAll");
app.UseStaticFiles();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<JwtBlacklistMiddleware>();
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

