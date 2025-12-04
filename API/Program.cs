using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// AddScoped lifetime HTTP request olduğu müddetçe nesne dursun anlamındadır. Request bitince yok olur. Örn Veritabanı işlemleri (DbContext genellikle scoped’tır)
// AddTransient Metot seviyesindedir. Sık yaratılıp yok edilmesi sorun olmayan servisler. Hemen yok olur.
// Add Transient tek kullanımlıktır. Örn: EmailSender, küçük helper servisler. Her kullanımda yeni bir obje oluşur.
// AddSingleton Servis ayağa kalkınca bir adet oluşturur. Bütün uygulamada o nesneyi kullanır. Uygulama kapanınca yok olur. Config, Cache, Logger gibi global şeyler.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
// Controller artık IProductRepository’nin sunduğu metotlardan fazlasını göremez. Yani ProductRepository’ye özel metotları göremez.

// BU SATIRIN ÜSTÜNDEKİ HER SATIR SERVİS OLARAK GEÇER
var app = builder.Build();
//BU SATIRIN ALTINDAKİ HER ALAN İSE MIDDLEWARE OLARAK GEÇER

// Middeleware ile request ile etkileşime geçilebilir. örnek olarak isteği authorize etme, yönlendirme, doğrulama gibi

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

try
{
    using var scope = app.Services.CreateScope() ; // Controller endpoint'lerini ekle
    var services = scope.ServiceProvider; // DbContext elde etmek için manuel scope aç. Fake request gönder.
    var context = services.GetRequiredService<StoreContext>(); // DbContext al
    await context.Database.MigrateAsync(); // Migration'ları otomatik uygula
    await StoreContextSeed.SeedAsync(context); // Örnek verileri veritabanına doldur
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

app.Run();
