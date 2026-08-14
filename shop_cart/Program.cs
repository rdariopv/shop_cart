using Blazored.LocalStorage;
using shop_cart.Services;
using shop_cart;
using Microsoft.AspNetCore.Components.Server.Circuits;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<CartState>();
builder.Services.AddScoped<CartStorage>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<FilterPanelService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => options.DetailedErrors = true); // ← aquí va DetailedErrors;
//builder.Services.AddServerSideBlazor().AddCircuitOptions(options => options.DetailedErrors = true);
builder.Services.AddHttpClient<ProductService>(client =>
{
    // client.BaseAddress = new Uri("https://localhost:7179/");
   // client.BaseAddress = new Uri("https://store-api-bhab.onrender.com/");
    client.BaseAddress = new Uri(
       builder.Configuration["ApiSettings:BaseUrl"]!
    );
});
builder.Services.AddHttpClient<OrderService>(client =>
{
    //client.BaseAddress = new Uri("https://localhost:7179/");
    //client.BaseAddress = new Uri("https://store-api-bhab.onrender.com/");
    client.BaseAddress = new Uri(
      builder.Configuration["ApiSettings:BaseUrl"]!
   );
});

builder.Services.AddScoped<CircuitHandler, DiagnosticCircuitHandler>();

builder.Services.AddSignalR(e => {
    e.MaximumReceiveMessageSize = 102400000;
});
var app = builder.Build();
 // ✅ 2. Pipeline estándar
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// ✅ 3. MapRazorComponents AL FINAL
app.MapRazorComponents<shop_cart.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();