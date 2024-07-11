using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<DeezerService>();
builder.Services.AddSingleton<ArtistManager>();
builder.Services.AddHttpClient<DeezerService>();
builder.Services.AddLogging();

// Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

// Use the CORS policy
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Urls.Add("http://*:5204");
app.Urls.Add("https://*:7136");

app.Run();
