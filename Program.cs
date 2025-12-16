using SistemaWorkspace.Middlewares;
using SistemaWorkspace.Servicos;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<ServicoSeguranca>();
builder.Services.AddSingleton<ServicoUsuarios>();
builder.Services.AddSingleton<ServicoMaquinas>();
builder.Services.AddSingleton<ServicoGoogleWorkspace>();

var app = builder.Build();

app.UseStaticFiles();

app.UseSession();

app.UseMiddleware<MiddlewareFiltroMaquina>();

app.MapRazorPages();

app.Run();
