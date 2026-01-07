using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UserRolePermission.Authentication;
using UserRolePermission.HUB;
using UserRolePermission.Middlewares;
using UserRolePermission.Repository.Data;
using UserRolePermission.Repository.Implementation;
using UserRolePermission.Repository.Interface;
using UserRolePermission.Service.Implementation;
using UserRolePermission.Service.Interface;
using UserRolePermission.SignalR;



    var builder = WebApplication.CreateBuilder(args);

    // Clear default logging providers and use NLog
    builder.Logging.ClearProviders();
    // Add services to the container
    builder.Services.AddControllers();
    // Add HttpContext
    builder.Services.AddHttpContextAccessor();

    // DbContext
    builder.Services.AddDbContext<AppDBContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper Profiles
builder.Services.AddAutoMapper(config => config.AddMaps(typeof(AutoMapperProfile).Assembly));

// Memory Cache
builder.Services.AddMemoryCache();
    // . Add Azure Communication Services Clients --
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IStatusRepository, StatusRepository>();
    builder.Services.AddScoped<IScreenRepository, ScreenRepository>();
    builder.Services.AddScoped<IScreenActionRepository, ScreenActionRepository>();
    builder.Services.AddScoped<IMenuStructureRepository, MenuStructureRepository>();
    builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
    builder.Services.AddScoped<IUPORepository, UserPermissionOverrideRepository>();




builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IScreenService, ScreenService>();
builder.Services.AddScoped<IScreenActionService, ScreenActionService>();
builder.Services.AddScoped<IMenuStructureService, MenuStructureService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IUPOService, UPOService>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();

    builder.Services.AddScoped<IUserPublisherService, SignalRUserPublisher>();

    builder.Services.AddHttpClient();



// Services
builder.Services.AddSignalR();

//  builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();

// JWT Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
    });

    builder.Services.AddScoped<JwtTokenGenerator>();

    // Authorization
    builder.Services.AddSingleton<IAuthorizationPolicyProvider, UserRolePermission.Authorization.HasPermissionPolicyProvider>();
    builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, UserRolePermission.Authorization.HasPermissionHandler>();

    // Logging
    builder.Services.AddLogging(logging =>
    {
        logging.AddConsole();
        logging.AddDebug();
    });

    // Global Exception Handling
    builder.Services.AddProblemDetails();

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
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
            new string[]{}
        }
        });
    });
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp",
            builder => builder.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials());
    });

    var app = builder.Build();

    // Middleware - Request & Response Logging

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }    //CORS Policy

    app.UseCors("AllowAngularApp");
    app.UseAuthentication();
    app.UseMiddleware<CurrentUserMiddleware>();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<SignalRHub>("/signalRHub");


    app.Run();
