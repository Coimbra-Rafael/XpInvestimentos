using Microsoft.EntityFrameworkCore;
using ProdutorXP.Model;

namespace ProdutorXP.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Transacao> Transacoes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=localhost,1433;Database=XPTransacoesDB;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;");
    }
}