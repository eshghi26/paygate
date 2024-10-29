using Common.Helper.Helper;
using PaymentGateway.PaymentApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.AddServiceRegistry();
builder.AddFactoryServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

ConfigurationHelper.Configure(app.Configuration);

app.UseAuthorization();

app.MapControllers();

app.Run();
