using BlazorApp1.Client;
using BlazorApp1.Client.AuthProviders;
using BlazorApp1.Client.Services;
using Blazored.SessionStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mjc2MjQ3N0AzMjMzMmUzMDJlMzBpd3NuNzhObzM1QXdGSndrZzU2K1VLWWxzL2hteTZndHVlbjFLRXJ1eExJPQ==");
builder.Services.AddSyncfusionBlazor();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<AuthenticationStateProvider, TestAuthStateProvider>();
builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();
