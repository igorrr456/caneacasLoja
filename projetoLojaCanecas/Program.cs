using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Sessao: guarda quem esta logado (cliente e/ou administrador) e o carrinho de compras
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);   // sessao expira apos 60 min sem uso
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Cultura brasileira: precos como "R$ 49,90", datas dd/MM/yyyy e decimais com virgula
var cultura = new CultureInfo("pt-BR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = new List<CultureInfo> { cultura },
    SupportedUICultures = new List<CultureInfo> { cultura }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();      // serve tambem as fotos enviadas pelo painel (wwwroot/img/produtos)
app.UseRouting();

app.UseSession();          // precisa vir ANTES dos controllers

app.UseAuthorization();

app.MapStaticAssets();

// Area administrativa: /Admin/Produtos, /Admin/Pedidos ...
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// Loja publica
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
