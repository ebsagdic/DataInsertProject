using DataInsertProject;
//using DataInsertProject.Consumers;
using DataInsertProject.Context;
using DataInsertProject.HangfireJob;
using DataInsertProject.Models;
using DataInsertProject.Repositoy;
using DataInsertProject.Service;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddFluentValidation(x =>
    x.RegisterValidatorsFromAssemblyContaining<DataModel>()); ;

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
    {
        EndPoints = { "localhost:6379" },
        ConnectTimeout = 10000, // Baðlantý zaman aþýmýný 10 saniyeye çýkarýr
    };
    options.InstanceName = "SampleProject_";
});

builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<DataService>();
builder.Services.AddScoped<DataRepository>();

//Hangfire Job ile tetiklemek için
builder.Services.AddScoped<DataConsumer>();

//Bu Consumer IHostedService ile triggirlanmasý içindi
//builder.Services.AddHostedService<DataConsumer>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHangfireDashboard();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

RecurringJob.AddOrUpdate<DataConsumer>(
    consumer => consumer.ConsumeDataFromQueueAsync(),
    "*/5 * * * *");

app.Run();
