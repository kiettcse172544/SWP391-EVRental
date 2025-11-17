using EVRenter_Data;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Service.Mapping;
using EVRenter_Service.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Database Context

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Dependency Injection (DI Container)

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<IModelService, ModelService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IRentalPriceService, RentalPriceService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAmenitiesService, AmenitiesService>();
builder.Services.AddScoped<IHandoverService, HandoverService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IRenterProfileService, RenterProfileService>();


// Email
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBookingEmailService, BookingEmailService>();

// Payment-related
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<VnPayService>();

// Email
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBookingEmailService, BookingEmailService>();


// AutoMapper Configuration

builder.Services.AddAutoMapper(typeof(UserMapping));
builder.Services.AddAutoMapper(typeof(StationMapping));
builder.Services.AddAutoMapper(typeof(ModelMapping));
builder.Services.AddAutoMapper(typeof(VehicleMapping));
builder.Services.AddAutoMapper(typeof(PriceMapping));
builder.Services.AddAutoMapper(typeof(BookingMapping));
builder.Services.AddAutoMapper(typeof(PaymentMapping));
builder.Services.AddAutoMapper(typeof(AmenitiesMapping));


// JWT Authentication

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// VNPAY CONFIGURATION (Payment Gateway)


// Bind section "VnPay" từ appsettings.json vào class VnPayOptions
builder.Services.Configure<VnPayOptions>(
    builder.Configuration.GetSection("VnPay"));


builder.Services.AddHttpContextAccessor();


// Swagger Documentation

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EVRenter API",
        Version = "v1",
        Description = "Backend API for EVRenter Electric Vehicle Rental System"
    });

    // Cho phép nhập JWT token vào Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token vào đây. Ví dụ: Bearer {token}"
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
            new string[] {}
        }
    });
});


// Controllers

builder.Services.AddControllers();

var app = builder.Build();

app.UseDeveloperExceptionPage();

// Middleware Pipeline

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseDeveloperExceptionPage();


app.Run();
