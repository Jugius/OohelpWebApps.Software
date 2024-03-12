global using Microsoft.AspNetCore.Components.Authorization;
global using Blazored.LocalStorage;
using OohelpWebApps.Software.Client.SoftwareManagerWeb.Components;
using OohelpWebApps.Software.Client.SoftwareManagerWeb.Services;
using Microsoft.AspNetCore.Authorization;
using OohelpWebApps.Software.Client.SoftwareManagerWeb.Components.Middleware;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient());
builder.Services.AddScoped<SoftwareApiService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProvider>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
