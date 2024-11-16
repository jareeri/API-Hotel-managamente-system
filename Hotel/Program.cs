using Hotel.Context;
using Hotel.Data;
using Hotel.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using static System.Net.WebRequestMethods;
using System.Text;
using Hotel.Generic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<DataContext>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IAcountService, AcountService>();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGuestService, GuestService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped(typeof(IGenric<>) , typeof(Genric<>));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
    option =>
    {
        option.Password.RequiredLength = 5;
        option.Password.RequireNonAlphanumeric = false;
        option.Password.RequireDigit = false;
        option.Password.RequireLowercase = false;
        option.Password.RequireUppercase = false;
    }
    ).AddEntityFrameworkStores<DataContext>();

builder.Services.AddCors(opt =>

{

    opt.AddPolicy("_loginOrgin", builder =>

    {

        builder.AllowAnyOrigin();

        builder.AllowAnyMethod();

        builder.AllowAnyHeader();

    });

});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = "User",
        ValidIssuer = "http://localhost",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("5ea0b74f736081455e758a02bf2fdbb8ae94900a0b28fcdb3713eea1acefe02b"))
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("_loginOrgin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
