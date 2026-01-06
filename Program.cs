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
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddSingleton<ServicoSeguranca>();
builder.Services.AddSingleton<ServicoUsuarios>();
builder.Services.AddSingleton<ServicoMaquinas>();
builder.Services.AddSingleton<ServicoGoogleWorkspace>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});

app.UseStaticFiles();

app.UseSession();

app.UseMiddleware<MiddlewareFiltroMaquina>();

app.MapGet("/", context =>
{
    context.Response.Redirect("/login");
    return Task.CompletedTask;
});

app.MapRazorPages();

app.Run();
