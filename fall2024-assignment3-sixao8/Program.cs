using fall2024_assignment3_sixao8.Data;
using fall2024_assignment3_sixao8.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using fall2024_assignment3_sixao8.Extensions;
using static fall2024_assignment3_sixao8.Const;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<fall2024_assignment3_sixao8Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("fall2024_assignment3_sixao8Context") ?? throw new InvalidOperationException("Connection string 'fall2024_assignment3_sixao8Context' not found.")));

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));

// Add services to the container.
//var configConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(configConnectionString));
//var conStrBuilder = new SqlConnectionStringBuilder(configConnectionString);
//conStrBuilder.UserID = builder.Configuration["DbUsername"];
//conStrBuilder.Password = builder.Configuration["DbPassword"];
//string connectionString = conStrBuilder.ConnectionString;

string configConnectionString = builder.Configuration.GetConnectionStringOrError(Config.ConnectionStrings.DefaultConnection);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configConnectionString));
var conStrBuilder = new SqlConnectionStringBuilder(configConnectionString);
conStrBuilder.UserID = builder.Configuration.GetOrError("DbUsername");
conStrBuilder.Password = builder.Configuration.GetOrError("DbPassword");
string connectionString = conStrBuilder.ConnectionString;

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
