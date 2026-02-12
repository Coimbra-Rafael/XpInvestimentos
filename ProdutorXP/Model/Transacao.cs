namespace ProdutorXP.Model;

public class Transacao
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public TypeTransaction Tipo { get; set; }  // Ex.: "Deposito", "Saque"
    public string ContaOrigem { get; set; }
    public string ContaDestino { get; set; }
    public bool Suspeita { get; set; }
}

public enum TypeTransaction
{
    Deposito,
    Saque,
    Trade
}