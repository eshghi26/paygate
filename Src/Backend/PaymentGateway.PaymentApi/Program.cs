using Common.Helper.Helper;
using PaymentGateway.PaymentApi;

var builder = WebApplication.CreateBuilder(args);

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
