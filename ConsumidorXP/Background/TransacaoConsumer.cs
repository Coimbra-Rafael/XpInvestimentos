using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using ConsumidorXP.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConsumidorXP.Background;

public class TransacaoConsumer: BackgroundService
{
    private readonly ConcurrentBag<Transacao> _transacoesProcessadas = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = await factory.CreateConnectionAsync(stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(queue: "transacoes", durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var transacao = JsonSerializer.Deserialize<Transacao>(body);
            if (transacao != null) _transacoesProcessadas.Add(transacao);
            
            await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
        };

        await channel.BasicConsumeAsync(queue: "transacoes", autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public IEnumerable<Transacao> GetTransacoes() => _transacoesProcessadas;
}