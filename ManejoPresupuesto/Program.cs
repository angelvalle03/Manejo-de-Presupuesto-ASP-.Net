using ManejoPresupuesto.Services;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRepostiroyTiposCuentas, RepositoryTiposCuentas>();
builder.Services.AddTransient<IServicioUsuario, ServicioUsuarios>();
builder.Services.AddTransient<IRepositoryCuentas, RepositoryCuentas>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddTransient<IrepositoryCategorias, RepositoryCategorias>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
