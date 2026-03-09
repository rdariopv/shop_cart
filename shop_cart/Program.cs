using Blazored.LocalStorage;
using shop_cart.Services;
using shop_cart;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<CartService>();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<ProductService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7179/");
});


//builder.WebHost.UseUrls("http://*:80");

var app = builder.Build();
app.MapRazorComponents<shop_cart.Components.App>()
    .AddInteractiveServerRenderMode(); // ? Este también

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


app.Run();
