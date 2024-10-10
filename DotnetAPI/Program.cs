// Create a WebApplicationBuilder that provides the necessary services and configuration for the application.
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add support for controllers, which handle HTTP requests and return responses.
builder.Services.AddControllers();

// Add services to the container.

// Adds support for API endpoint exploration and generation of metadata for API endpoints, such as OpenAPI documentation.
builder.Services.AddEndpointsApiExplorer();

// Adds Swagger, which generates documentation and an interactive UI for testing API endpoints.
builder.Services.AddSwaggerGen();

builder.Services.AddCors((options) => 
{
    options.AddPolicy("DevCors", (corsBuilder) => 
    {
        corsBuilder.WithOrigins("http://localhost:4200","http://localhost:8000", "http://localhost:3000")//specifies the allowed domains
            .AllowAnyMethod()//means any HTTP method (GET, POST, PUT, DELETE, etc.) can be used in requests.
            .AllowAnyHeader()// allows any HTTP headers in the request.
            .AllowCredentials();//permits sending credentials (cookies, authorization headers, etc.) with cross-origin requests.
    } );

    options.AddPolicy("ProdCors", (corsBuilder) => 
    {
        corsBuilder.WithOrigins("http://myProductionSite.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    } );
});


// Build the application with the configured services.
var app = builder.Build();

// Configure the HTTP request pipeline.
// If the application is in the development environment, enable Swagger and its UI for API testing and exploration.
if (app.Environment.IsDevelopment())
{   
    // Use the DevCors policy for development
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Use the ProdCors policy for production
    app.UseCors("ProdCors");
    // In production, use HTTPS redirection to ensure secure communication.
    app.UseHttpsRedirection();
}

// Map controller routes to handle HTTP requests.
app.MapControllers();

// Run the application, listening for incoming HTTP requests.
app.Run();
