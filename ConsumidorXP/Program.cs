using ConsumidorXP.Background;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// 1. Registre a classe como Singleton explicitamente
builder.Services.AddSingleton<TransacaoConsumer>();

// 2. Registre o HostedService apontando para a instância Singleton acima
builder.Services.AddHostedService(sp => sp.GetRequiredService<TransacaoConsumer>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", ([FromServices]TransacaoConsumer consumer) =>
{
    var transacoes = consumer.GetTransacoes().TakeLast(10);
    return Results.Ok(transacoes);
});


await app.RunAsync(CancellationToken.None);