using Demo.DbValuesChangeMonitoring.Data;
using Demo.DbValuesChangeMonitoring.DatabaseOptionsProvider;
using Demo.DbValuesChangeMonitoring.UI.Code;
using Demo.DbValuesChangeMonitoring.UI.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddDatabaseOptionsProvider();

builder.Services.AddDbContextFactory<ConfigurationContext>(opt => 
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ValuesChangedMonitoring")));

builder.Services.AddOptions<DisplayOptions>()
    .Bind(builder.Configuration.GetSection("Display"));

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
