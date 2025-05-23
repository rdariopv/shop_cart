using shop_cart.Components;
using shop_cart.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<CartService>();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


//builder.WebHost.UseUrls("http://*:80");

var app = builder.Build();
//app.MapRazorComponents<App>()
//    .AddInteractiveServerRenderMode(); // ? Este tambi�n

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
