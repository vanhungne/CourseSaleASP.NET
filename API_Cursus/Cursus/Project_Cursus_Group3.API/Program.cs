using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Project_Cursus_Group3.Data;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Mapping;
using Project_Cursus_Group3.Data.Repository;
using Project_Cursus_Group3.Data.ViewModels.ForgotDTO;
using Project_Cursus_Group3.Service;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

namespace Project_Cursus_Group3.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // send email resetpass
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
            builder.Services.Configure<SmtpResetPassword>(builder.Configuration.GetSection("SmtpResetPassword"));

            //upload file service
            builder.Services.AddScoped<UploadFile>();

            // Load appsettings.json configuration
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Add CursusDbContext with SQL Server configuration
            builder.Services.AddDbContext<CursusDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CursusDB")));

            // Add repositories and services
            builder.Services
                    .AddRepository() // Assuming AddRepository is an extension method to add repositories
                    .AddServices();   // Assuming AddServices is an extension method to add your services

            // Configure authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
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
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Cookies["authToken"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };

            })
            .AddCookie(options =>
            {
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.SlidingExpiration = true;
            })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
    });

            // Add CORS policy to allow specific origins (e.g., localhost for development)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "https://coursev1.vercel.app")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Add session management
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");
                var cultures = new List<CultureInfo> { new CultureInfo("en-US") };
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });

            // Configure JSON serialization to handle reference cycles
            builder.Services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // Add AutoMapper for mapping between objects
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            // Add Firebase configuration
            var firebaseConfig = builder.Configuration.GetSection("FireBase:Credentials").Get<Dictionary<string, object>>();
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(JsonConvert.SerializeObject(firebaseConfig)),
            });

            // Add Swagger for API documentation
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cursus", Version = "v1.0" });

                // Add security definitions for JWT Bearer and Cookie-based authentication
                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    In = ParameterLocation.Header,
                //    Description = "Please enter a valid JWT token",
                //    Name = "Authorization",
                //    Type = SecuritySchemeType.Http,
                //    BearerFormat = "JWT",
                //    Scheme = "Bearer"
                //});

                c.AddSecurityDefinition("Cookie", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Cookie,
                    Description = "Please enter a valid cookie",
                    Name = "authToken",
                    Type = SecuritySchemeType.ApiKey
                });

                // Add security requirements for both JWT and Cookie
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
                new string[] { }
            },
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Cookie"
                    }
                },
                new string[] { }
            }
        });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cursus API V1");
                    c.RoutePrefix = string.Empty;
                    c.InjectJavascript("/swagger/custom-swagger.js");
                });
            }

            app.UseStaticFiles();

            // Use middleware for Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cursus API V1");
            });

            // Enable CORS
            app.UseCors("AllowSpecificOrigins");

            app.UseHttpsRedirection();

            // Use authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Use session management middleware
            app.UseSession();

            // Map controllers to endpoints
            app.MapControllers();

            app.Run();
        }
    }
}