using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Threading;
using System;
using UrlShortener.Context;
using UrlShortener.Helper;
using UrlShortener.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PgSql");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Rotas cMn72B

app.MapGet("/urls", async (AppDbContext appDbContext, CancellationToken cancellationToken) =>
{
    var urls = await appDbContext.Urls.ToListAsync(cancellationToken);
    return urls;
});

app.MapGet("/{id}", async (string Id, AppDbContext appDbContext,
    CancellationToken cancellationToken) =>
{
    var url = await appDbContext.Urls.FirstOrDefaultAsync(x => x.CodigoEncurtador == Id, cancellationToken);
    if (url is null) return Results.BadRequest("Url não encontrada");
    url.QuantidadeDeAcessos = url.QuantidadeDeAcessos + 1;
    appDbContext.Entry(url).State = EntityState.Modified;
    await appDbContext.SaveChangesAsync(cancellationToken);
    return Results.Redirect(url.UrlDestino, permanent: true);
});

app.MapPost("/urls", async (Urls urls, AppDbContext appDbContext, 
    CancellationToken cancellationToken) =>
{
    Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    var novaUrl = new Urls
    {
        Id = Guid.NewGuid(),
        UrlDestino = urls.UrlDestino,
        CodigoEncurtador = Encurtador.Encode(unixTimestamp),
        QuantidadeDeAcessos = 0
    };
    appDbContext.Urls.Add(novaUrl);
    await appDbContext.SaveChangesAsync(cancellationToken);
    return $"https://localhost:7147/{novaUrl.CodigoEncurtador}";
});

app.MapPut("urls/{id}", async (Guid id, Urls urls, AppDbContext appDbContext, 
    CancellationToken cancellationToken) =>
{
    var url = await appDbContext.Urls.FirstOrDefaultAsync(x => x.Id == id);

    if (url != null)
    {
        url.DataAtualizacao = DateTimeOffset.UtcNow;
        url.UrlDestino = urls.UrlDestino;
        appDbContext.Entry(url).State = EntityState.Modified;
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok("Url atualizada!");
    }
    return Results.BadRequest("Url não encontrada!");
});

app.MapDelete("/urls/{id}", async (Guid id, AppDbContext appDbContext, 
    CancellationToken cancellationToken) =>
{
    var url = await appDbContext.Urls.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    if(url != null)
    {
        appDbContext.Urls.Remove(url);
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }
    return Results.BadRequest("Url não encontrada");
});

app.Run();

