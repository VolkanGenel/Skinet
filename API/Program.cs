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

app.Run();
