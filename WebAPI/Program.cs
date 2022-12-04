using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DataAccessLayer.Entities;
using WebAPI.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http.Features;
using BuisnessLogicLayer;
using WebAPI.Filters;
using AutoMapper;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using BuisnessLogicLayer.Interfaces;
using BuisnessLogicLayer.Services;
using Microsoft.OpenApi.Models;


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200");
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
});


builder.Services.AddControllers(config => config.Filters.Add(new AppExceptionFilterAttribute(builder.Environment)));

//setup EF 
var connectionString = builder.Configuration.GetConnectionString("FileStorage");
builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseSqlServer(connectionString);
});

//set limit for the length of each multipart body 256MB
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 268435456;
});

//setup Identity
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    options.SignIn.RequireConfirmedPhoneNumber = false; //confirm phone number
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<AppDbContext>();

//setup JWT service
builder.Services.AddScoped<JwtHandler>();

//setup JWTBearerMiddleware
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        RequireExpirationTime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecurityKey"]))
    };
});

//setup Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.MSSqlServer(connectionString: 
        ctx.Configuration.GetConnectionString("FileStorage"),
        restrictedToMinimumLevel: LogEventLevel.Information,
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "LogEvents",
            AutoCreateSqlTable = true,
        }
        )
    .WriteTo.Console()
);

//setup AutoMapper
var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
builder.Services.AddScoped<IMapper, Mapper>(i => new Mapper(mapperConfig));

//setup services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IShortLinkService, ShortLinkService>();
builder.Services.AddScoped<IUserService, UserService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "File Storage", Version = "v1" });
    /*option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
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
    });*/
});

var app = builder.Build();

//uncomment, if want logging HTTP requests
//app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
