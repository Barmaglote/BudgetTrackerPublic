using budget.core.DB;
using budget.core.DB.Interfaces;
using budget.core.Factories;
using budget.core.Factories.Interfaces;
using budget.core.Repositories;
using budget.core.Repositories.Interfaces;
using budget.core.Services;
using budget.core.Services.Interfaces;
using budget.webapi.Middlewares;
using Microsoft.OpenApi.Models;
using budget.webapi.Security;
using Asp.Versioning;
using budget.core.Configurations;
using Serilog;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using budget.utils.ApiFilters;
using budget.core.Models.Interfaces;
using budget.core.Models;
using budget.webapi.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Prometheus;
using budget.webapi.Services;
using Stripe;
using budget.webapi.Factories;

var CORS_POLICY_NAME = "_MyOriginPolicy";

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("budget.webapi\\appsettings.json", optional: true)
      .AddEnvironmentVariables()
      .Build();

var apiSettingsSection = builder.Configuration.GetSection("ApiSettings");
var apiSettings = apiSettingsSection.Get<ApiSettings>();


#region Serilog

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Logging.AddSerilog();
Log.Information("Serilog works!");

#endregion

// Add services to the container.
builder.Services.AddControllers(options => {
  options.Filters.Add<UnhandledExceptionFilterAttribute>();
  options.Filters.Add<AddCustomHeaderResultFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// "'v'major[.minor][-status]"
builder.Services.AddApiVersioning(o => {
  o.ApiVersionReader = new HeaderApiVersionReader("api-version");
  o.ReportApiVersions = true;
  o.DefaultApiVersion = new ApiVersion(1, 0);
  o.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddSwaggerGen(c => {
  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
    Description = "JWT Authorization header using the Bearer scheme.",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
  });

  c.OperationFilter<AddRequiredProviderHeaderParameter>();

  c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
      {
          new OpenApiSecurityScheme
          {
              Reference = new OpenApiReference
              {
                  Type = ReferenceType.SecurityScheme,
                  Id = "Bearer",
              },
          },
          new string[] {}
      },
  });
});

#region Antiforgery
builder.Services.AddSession(options => {
  options.IdleTimeout = TimeSpan.FromMinutes(20);
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
  options.Cookie.SameSite = SameSiteMode.None;
  options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"./keys"))    // DATA - переключить на REDIS
    .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration {
      EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
      ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
    })
    .SetApplicationName("BudgetTracker")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

builder.Services.AddAuthentication(options => {
  options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

builder.Services.AddAuthorization(options => {
  options.AddPolicy(IdentityData.AdminUserPolicyName, p => p.RequireClaim(IdentityData.AdminUserClaimName, "true"));
  options.AddPolicy(IdentityData.ClientUserPolicyName, p => p.RequireClaim(IdentityData.ClientUserClaimName, "true"));
  options.AddPolicy(IdentityData.ServiceUserPolicyName, p => p.RequireClaim(IdentityData.ServiceUserClaimName, "true"));
});
#endregion

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // Can be used for quick access to API version

if (apiSettings != null && apiSettings.CorsOrigin != null) {
  builder.Services.AddCors(options => {
    options.AddPolicy(CORS_POLICY_NAME, builder => {
      builder.WithOrigins(apiSettings.CorsOrigin)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("X-Xsrf-Token");
    });
  });
}

#region DI-container
builder.Services.Configure<ApiSettings>(apiSettingsSection);
builder.Services.AddScoped<IncomeService>();
builder.Services.AddScoped<CreditsService>();
builder.Services.AddScoped<ExpensesService>();
builder.Services.AddScoped<PlanningService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<UserSettingsService>();
builder.Services.AddTransient<IDBClient, DBClient>();
builder.Services.AddScoped(typeof(IEnumerableRepository<>), typeof(EnumerableRepository<>));
builder.Services.AddScoped(typeof(IAggregateRepository<>), typeof(AggregateRepository<>));
builder.Services.AddScoped(typeof(IFilteringRepository<>), typeof(FilteringRepository<>));
builder.Services.AddScoped(typeof(IPostingRepository<>), typeof(PostingRepository<>));
builder.Services.AddScoped(typeof(IEnumerableService<,>), typeof(EnumerableService<,>));
builder.Services.AddScoped(typeof(IFilteringService<,>), typeof(FilteringService<,>));
builder.Services.AddScoped(typeof(IPostingService<,>), typeof(PostingService<,>));
builder.Services.AddScoped(typeof(IDeletingService<,>), typeof(DeletingService<,>));
builder.Services.AddScoped(typeof(IFilterBuilder<>), typeof(FilterBuilder<>));
builder.Services.AddScoped(typeof(IStatisticsService<,>), typeof(StatisticsService<,>));
builder.Services.AddScoped<ITransferService, budget.core.Services.TransferService>();
builder.Services.AddScoped<IAccountsService, AccountsService>();
builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();
builder.Services.AddSingleton<UserSettingsServiceFactory>();
builder.Services.AddScoped<IDataValidator, DataValidator>();
builder.Services.AddScoped<ITransactionManager, MongoTransactionManager>();
builder.Services.AddTransient(typeof(IFactory<,>), typeof(Factory<,>));
builder.Services.AddTransient<IUserFactory, UserFactory>();
builder.Services.AddTransient<UnhandledExceptionFilterAttribute>();
builder.Services.AddTransient<AddCustomHeaderResultFilter>();
builder.Services.AddSingleton<RabbitMqConsumerService>();
#endregion

#region Kestrel
builder.WebHost.UseKestrel(options => {
  options.ListenAnyIP(5001, listenOptions => {
    var certificate = new X509Certificate2(Path.Combine(apiSettings?.PathToSSL ?? "", "localhost.pfx"), "qwertzuiop"); // TODO: Пароль нужно сохранить в valet
    listenOptions.UseHttps(certificate);
    listenOptions.Protocols = HttpProtocols.Http1;
  });
});
#endregion

builder.Services.AddControllersWithViews(options => {
  options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddAntiforgery(options => {
  options.FormFieldName = "AntiforgeryFieldname";
  options.HeaderName = "X-CSRF-TOKEN";
  options.SuppressXFrameOptionsHeader = false;
  options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
  options.Cookie.HttpOnly = false;
  options.Cookie.SameSite = SameSiteMode.None;
  options.Cookie.Expiration = TimeSpan.FromDays(14);
  options.SuppressXFrameOptionsHeader = false;
});

builder.Services.AddLogging(builder => {
  builder.SetMinimumLevel(LogLevel.Debug);
  builder.AddFilter("Microsoft.AspNetCore.DataProtection", LogLevel.Debug);
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
  app.UseDeveloperExceptionPage();
  app.UseSwagger();
  app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Budget Tracker API"));
}

  app.UseMetricServer(5001, "/metrics");

try {
  Log.Information("Starting web application (webapi)");

  app.UseHttpsRedirection();
  app.UseCors(CORS_POLICY_NAME);
  app.UseMiddleware<TokenValidationMiddleware>();

  if (app.Environment.IsDevelopment()) {
    app.UseMiddleware<AntiForgeryTokenLoggingMiddleware>();
  }

  app.UseHttpMetrics();
  app.UseSession();
  app.UseAuthentication();
  app.UseAuthorization();
  app.MapControllers();

  var consumerService = app.Services.GetRequiredService<RabbitMqConsumerService>();
  consumerService.StartListening();

  app.Run();

} catch (Exception ex) {
  Log.Fatal(ex, "Application terminated unexpectedly");
} finally {
  Log.CloseAndFlush();
}

