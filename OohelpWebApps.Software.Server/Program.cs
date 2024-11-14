using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OohelpWebApps.Software.Server.Database;
using OohelpWebApps.Software.Server.Endpoints;
using OohelpWebApps.Software.Server.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(builder.Configuration.GetValue<string>("ConnectionString")));

builder.Services.AddScoped<ApplicationsService>();
builder.Services.AddSingleton<FileSystemService>();

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

//app.MapControllers();
app.MapApplicationEndpoints();
app.MapReleaseEndpoints();
app.MapDetailEndpoints();
app.MapFileEndpoints();
app.MapUpdaterEndpoints();

app.Run();
