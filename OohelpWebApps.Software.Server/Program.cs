using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OohelpWebApps.Software.Server;
using OohelpWebApps.Software.Server.Common.Interfaces;
using OohelpWebApps.Software.Server.Configurations;
using OohelpWebApps.Software.Server.Database;
using OohelpWebApps.Software.Server.Endpoints;
using OohelpWebApps.Software.Server.Services;
using OohelpWebApps.Software.Server.Services.UploadService;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterSerilog();

builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString(nameof(AppDbContext))));
builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection(MinioOptions.Key))
    .AddSingleton(s => s.GetRequiredService<IOptions<MinioOptions>>().Value);
builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<ApplicationsService>();

builder.Services.AddScoped<IUploadService, MinioUploadService>();

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
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("InAdminRole", options =>
    options.RequireAuthenticatedUser().
    RequireRole("Admin"));
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("https://oohelp.net", permanent:true));
app.MapApplicationEndpoints();
app.MapReleaseEndpoints();
app.MapDetailEndpoints();
app.MapFileEndpoints();
app.MapUpdateEndpoints();

app.Run();
