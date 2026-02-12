using ConsumidorXP.Background;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<TransacaoConsumer>();

builder.Services.AddHostedService(sp => sp.GetRequiredService<TransacaoConsumer>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", ([FromServices]TransacaoConsumer consumer) =>
{
    var transacoes = consumer.GetTransacoes().TakeLast(100);
    return Results.Ok(transacoes);
});


await app.RunAsync(CancellationToken.None);