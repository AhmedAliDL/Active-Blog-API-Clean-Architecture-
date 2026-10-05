using App.Application;
using App.Application.Behaviors;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using App.Infrastructure.BackgroundServices.Likes;
using App.Infrastructure.Hubs;
using App.Infrastructure.Interceptors;
using App.Infrastructure.Persistance;
using App.Infrastructure.Persistance.Seed;
using App.Infrastructure.Services;
using App.Infrastructure.UnitOfWork;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace Active_Blog_Service_API
{
    public class Program
    {
        private static readonly string _documentName = "v1";
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                            .AddJsonOptions(opts =>
                            {
                                opts.JsonSerializerOptions.Converters.Add(
                                    new JsonStringEnumConverter());
                            });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(
                   swagger =>
                   {
                       swagger.SwaggerDoc(_documentName, new Microsoft.OpenApi.Models.OpenApiInfo
                       {
                           Version = "v1",
                           Title = "Active API",
                           Description = "A blog website"
                       });
                       swagger.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                       {
                           Name = "Authorization",
                           Type = SecuritySchemeType.Http,
                           Scheme = "Bearer",
                           BearerFormat = "JWT",
                           In = ParameterLocation.Header,
                           Description = "Enter your valid token"

                       });
                       var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                       var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                       swagger.IncludeXmlComments(xmlPath);
                   }

               );

            // configure SQLDB
            var config = builder.Configuration;

            builder.Services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseSqlServer(config.GetConnectionString("constr"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                           maxRetryCount: 5,
                           maxRetryDelay: TimeSpan.FromSeconds(10),
                           errorNumbersToAdd: null);
                    });
                options.AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>());
                options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
            }
             );

            builder.Services
                .AddIdentity<User, Role>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 4;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(5);
            });
            builder.Services.AddHttpContextAccessor();

            //services
            var serviceAssembly = typeof(RoleService).Assembly;
            builder.Services.Scan(s => s
            .FromAssemblies(serviceAssembly)
            .AddClasses(c => c.AssignableTo<IScopedServiceMarker>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<SoftDeleteInterceptor>();

            //authentication using JWT
            builder.Services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
                ).AddJwtBearer(
                        options =>
                        {
                            options.SaveToken = true;
                            options.RequireHttpsMetadata = false;

                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = config["JWT:issuer"],

                                ValidateAudience = true,
                                ValidAudience = config["JWT:audience"],

                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(config["JWT:SecurityKey"]!)
                                ),

                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.Zero
                            };
                        }
                );
            //mediatr
            builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(IApplicationMarker).Assembly));
            //fluent validation
            builder.Services.AddValidatorsFromAssembly(typeof(IApplicationMarker).Assembly);
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));
            //global exceptions
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            //global rate limiter
            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>
                (
                    context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 50,
                            Window = TimeSpan.FromMinutes(1)
                        }
                        )
                );
            });
            //memory service
            builder.Services.AddMemoryCache();
            //background services
            builder.Services.AddHostedService<SaveLikesToDatabaseBackgroundService>();
            builder.Services.AddHostedService<DeleteLikesFromDatabaseBackgroundService>();
            //SignalR
            builder.Services.AddSignalR();
            //interceptors
            builder.Services.AddScoped(typeof(AuditInterceptor));
            //configure serilog
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .CreateBootstrapLogger();

            builder.Services.AddSerilog((services, lc) =>
            {
                lc.ReadFrom.Configuration(config)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
            });
            try
            {
                var app = builder.Build();

                // Configure the HTTP request pipeline.
                //if (app.Environment.IsDevelopment())
                //{
                app.MapSwagger();
                app.MapScalarApiReference(options =>
                {
                    options
                        .WithTitle("Active API")
                        .WithOpenApiRoutePattern($"/swagger/{_documentName}/swagger.json");
                });

                //}
                app.UseRateLimiter();

                app.UseExceptionHandler();

                app.UseSerilogRequestLogging();

                app.UseHttpsRedirection();

                app.UseAuthentication();
                app.UseAuthorization();


                app.MapControllers();

                app.MapHub<NotificationHub>("/notify");

                await DatabaseSeeder.SeedAsync(app.Services);

                app.Run();
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }
    }
}
