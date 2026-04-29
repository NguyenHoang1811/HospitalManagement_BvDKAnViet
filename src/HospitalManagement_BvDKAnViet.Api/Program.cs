using HospitalManagement_BvDKAnViet.Api.BackgroundServices;
using HospitalManagement_BvDKAnViet.Core.IServices;
using HospitalManagement_BvDKAnViet.Data.Context;
using HospitalManagement_BvDKAnViet.Data.Repositories;
using HospitalManagement_BvDKAnViet.Data.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Add services to the container.
        builder.Services.AddControllers();

        // Register AutoMapper (scans assembly for Profile classes)
        builder.Services.AddAutoMapper(typeof(MappingProfile));

        // Register EF Core DbContext (SQL Server)
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        
        // Register repositories / services
        builder.Services.AddScoped<IPatientRepository, PatientRepository>();
        builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
        builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
        builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
        builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var jwtSettings = configuration.GetSection("Jwt");

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

                ValidIssuer = jwtSettings["ValidIssuer"],
                ValidAudience = jwtSettings["ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)
                )
            };

            // 🔥 CUSTOM RESPONSE 401 + 403
            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse(); // VERY IMPORTANT

                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        responseCode = 401,
                        responseMessage = "Vui lòng đăng nhập để sử dụng chức năng",
                        data = (object?)null
                    });

                    return context.Response.WriteAsync(result);
                },

                OnForbidden = context =>
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        responseCode = 403,
                        responseMessage = "Bạn không có quyền truy cập chức năng này",
                        data = (object?)null
                    });

                    return context.Response.WriteAsync(result);
                }
            };
        });

        // Authorization
        builder.Services.AddAuthorization();
        // Thêm vào trước builder.Build()
        builder.Services.AddHostedService<AppointmentCancelService > ();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // Use authentication before authorization and endpoint routing
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}