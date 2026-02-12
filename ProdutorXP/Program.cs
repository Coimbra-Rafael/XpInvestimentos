using System.Text;
using System.Text.Json;
using Bogus;
using EFCore.BulkExtensions;
using ProdutorXP.Model;
using ProdutorXP.Persistence;
using RabbitMQ.Client;

await using var context = new AppDbContext();
await context.Database.EnsureCreatedAsync();  

var faker = new Faker<Transacao>()
    .RuleFor(t => t.Data, f => f.Date.Past(1))
    .RuleFor(t => t.Valor, f => f.Finance.Amount(1, 20000))
    .RuleFor(t => t.Tipo, f => f.PickRandom("Deposito", "Saque", "Trade"))
    .RuleFor(t => t.ContaOrigem, f => f.Finance.Account())
    .RuleFor(t => t.ContaDestino, f => f.Finance.Account())
    .RuleFor(t => t.Suspeita, (f, t) => t.Tipo == "Saque" && t.Valor > 10000);


var transacoes = faker.Generate(1000001);  

await context.BulkInsertAsync(transacoes);
Console.WriteLine("Inserção de 1M+ registros concluída!");

await PublicarNaFila(transacoes);
Console.WriteLine("Publicação na fila concluída!");
return;


async Task PublicarNaFila(List<Transacao> transacoes)
{
    var factory = new ConnectionFactory { HostName = "localhost" };
    await using var connection = await factory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();
    await channel.QueueDeclareAsync(queue: "transacoes", durable: true, exclusive: false, autoDelete: false, arguments: null);

    foreach (var transacao in transacoes)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(transacao);
        var properties = new BasicProperties { Persistent = true };
        await channel.BasicPublishAsync(
            exchange: string.Empty, 
            routingKey: "transacoes", 
            mandatory: true, 
            basicProperties: properties, 
            body: body);
    }
}