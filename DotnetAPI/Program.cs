// Create a WebApplicationBuilder that provides the necessary services and configuration for the application.
using System.Text;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


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

//This registers UserRepository as the implementation for IUserRepository, and when the UserEFController 
//is instantiated(a new instance is created when there is a request.), the IUserRepository parameter is injected automatically by the dependency injection (DI) system.
builder.Services.AddScoped<IUserRepository, UserRepository>();

/*JWT configuration: Retrieves the TokenKey from configuration settings and sets it as the signing key, use 
TokenValidationParameters to validate that the token was signed with the provided TokenKey
This configuration simply ensures that any token presented in the Authorization header is 
signed with the correct TokenKey. (It does not inherently make use of a UserId or any user-specific claim.)*/
string? tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters() 
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    tokenKeyString != null ? tokenKeyString : ""
                )),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }
        );



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

//order is important, otherwise resulting "401unauthorized"
/*calling app.UseAuthentication() first ensures that the request’s token is validated and the 
user's identity is established. Then, app.UseAuthorization() can properly assess the user's 
permissions based on their claims.*/
app.UseAuthentication(); // Checks and validates the JWT token in the request to authenticate the user.
app.UseAuthorization();  // Enforces authorization policies, assuming the user is authenticated.


// Map controller routes to handle HTTP requests.
app.MapControllers();

// Run the application, listening for incoming HTTP requests.
app.Run();
