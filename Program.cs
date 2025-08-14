using System.Text;
using System.Text.Json.Serialization;
using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };


});



builder.Services.AddControllers()
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
})
.AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
});

builder.Services.AddDbContext<BlogDataContext>();
builder.Services.AddTransient<TokenService>();
builder.Services.AddTransient<EmailService>();

var app = builder.Build();

Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");
Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
Configuration.ApiKey = app.Configuration.GetValue<string>("ApiKey");

var smtp = new Configuration.SmtpConfiguration();
app.Configuration.GetSection("Smtp").Bind(smtp);
Configuration.Smtp = smtp;


app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();
app.Run();
