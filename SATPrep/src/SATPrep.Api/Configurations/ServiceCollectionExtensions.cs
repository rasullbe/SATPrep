using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SATPrep.Api.Configurations.Settings;
using SATPrep.Api.Data;
using SATPrep.Api.HealthChecks;
using SATPrep.Api.Repositories;
using SATPrep.Api.Services;

namespace SATPrep.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSatPrepDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    public static IServiceCollection AddSatPrepCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>() ?? new CorsOptions();
        var appOptions = configuration.GetSection(AppOptions.SectionName).Get<AppOptions>() ?? new AppOptions();

        var origins = new List<string>();
        if (corsOptions.Origins is not null)
            origins.AddRange(corsOptions.Origins.Where(o => !string.IsNullOrWhiteSpace(o)));
        if (!string.IsNullOrWhiteSpace(appOptions.FrontendUrl))
            origins.Add(appOptions.FrontendUrl);

        var frontendOrigin = !string.IsNullOrWhiteSpace(appOptions.FrontendUrl)
            ? appOptions.FrontendUrl
            : $"http://localhost:{appOptions.FrontendPort}";
        if (!origins.Contains(frontendOrigin))
            origins.Add(frontendOrigin);

        services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy.WithOrigins(origins.Distinct().ToArray())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddSatPrepAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        if (jwtOptions.Secret.Length < 32)
            throw new InvalidOperationException("Jwt:Secret must be at least 32 characters. Configure it via user-secrets (dotnet user-secrets set \"Jwt:Secret\" \"...\").");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = System.Security.Claims.ClaimTypes.Name,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessJwtOptions = context.HttpContext.RequestServices
                            .GetRequiredService<IOptions<JwtOptions>>().Value;
                        var accessToken = context.HttpContext.Request.Cookies[accessJwtOptions.CookieName];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
            options.AddPolicy(AuthPolicies.AdminOnly, policy =>
                policy.RequireRole("Admin", "SuperAdmin"));
            options.AddPolicy(AuthPolicies.SuperAdminOnly, policy =>
                policy.RequireRole("SuperAdmin"));
        });

        return services;
    }

    public static IServiceCollection AddSatPrepServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IFlashcardRepository, FlashcardRepository>();
        services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
        services.AddScoped<IUserProgressRepository, UserProgressRepository>();
        services.AddScoped<IStudySessionRepository, StudySessionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IQuizService, QuizService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<ITopicService, TopicService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IFlashcardService, FlashcardService>();
        services.AddScoped<IQuizAttemptService, QuizAttemptService>();
        services.AddScoped<IStudyService, StudyService>();

        // Auth services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICookieAuthWriter, CookieAuthWriter>();

        return services;
    }

    public static IServiceCollection AddSatPrepHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database");

        return services;
    }

    public static IServiceCollection AddSatPrepOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }
}

public static class AuthPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string SuperAdminOnly = "SuperAdminOnly";
}