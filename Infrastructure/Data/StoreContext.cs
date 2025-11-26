using Core.Entities;
using Infrastructure.Config;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

// public class StoreContext : DbContext
// {
//     public StoreContext(DbContextOptions options) : base(options)
//     {
//     }
// }
// Aşağısı ile aynı, aşağısı temiz koda uygun
// DbContext(options) connection string gönderilir.
public class StoreContext(DbContextOptions options) : DbContext(options)
{
    // Bu şekilde migration oluşturulduktan sonra Products Tablosu oluşturulur.
    public DbSet<Product> Products { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        // Burada yapılabildiği gibi configuration dosyasında da yapılabilir. Daha temiz kod için Configuratioom dosyas daha mantıklı.
        // Bir sürü entity olursa her ayarı burada yapmak karmaşıklığa sebep olur.
        // Bu yüzden ProductConfiguration dosyası oluşturuldu.
        
        base.OnModelCreating(modelBuilder);
        // ProductConfiguration dosyası oluşturulduğu için bu satır eklenmeli
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);
    }
}