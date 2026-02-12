using System.Text;
using System.Text.Json;
using Bogus;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using ProdutorXP.Model;
using ProdutorXP.Persistence;
using RabbitMQ.Client;

await using var context = new AppDbContext();
await context.Database.EnsureCreatedAsync(); 

var faker = new Faker<Transacao>()
    .RuleFor(t => t.Data, f => f.Date.Past(1))
    .RuleFor(t => t.Valor, f => f.Finance.Amount(1, 20000))
    .RuleFor(t => t.Tipo, f => f.PickRandom<TypeTransaction>())
    .RuleFor(t => t.ContaOrigem, f => f.Finance.Account())
    .RuleFor(t => t.ContaDestino, f => f.Finance.Account())
    .RuleFor(t => t.Suspeita, (f, t) => t.Tipo == TypeTransaction.Saque && t.Valor > 10000);

var range = Enumerable.Range(0, 10001);
var options = new ParallelOptions { MaxDegreeOfParallelism = 7 };

await Parallel.ForEachAsync(range, options, async (i, token) =>
{
    await using var context = new AppDbContext();
    var transacoes = faker.Generate(100);

    await context.BulkInsertAsync(transacoes, cancellationToken: token); 
    Console.WriteLine($"[{Environment.CurrentManagedThreadId}] Inserção {i} concluída!");

    await PublicarNaFila(transacoes, token);
    Console.WriteLine($"[{Environment.CurrentManagedThreadId}] Publicação {i} concluída!");
});

return;


static async Task PublicarNaFila(List<Transacao> transacoes, CancellationToken token)
{
    var factory = new ConnectionFactory { HostName = "localhost" };
    await using var connection = await factory.CreateConnectionAsync(token);
    await using var channel = await connection.CreateChannelAsync(cancellationToken: token);

    await channel.QueueDeclareAsync(queue: "transacoes", 
        durable: true, 
        exclusive: false, 
        autoDelete: false, 
        arguments: null,
        cancellationToken: token);

    foreach (var transacao in transacoes)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(transacao);
        var properties = new BasicProperties { Persistent = true };

        await channel.BasicPublishAsync(exchange: string.Empty, 
            routingKey: "transacoes", 
            mandatory: true, 
            basicProperties: properties, 
            body: body, 
            cancellationToken: token);
    }
}