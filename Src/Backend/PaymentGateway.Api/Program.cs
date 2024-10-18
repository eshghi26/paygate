using Common.Helper.Helper;
using PaymentGateway.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceRegistry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

ConfigurationHelper.Configure(app.Configuration);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
