using Npgsql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);


  builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
     })
    .AddCookie(options =>
    {
        options.Cookie.Name = "MYAppAuthCookie";
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

// Add services to the container
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");



var initializer = new DatabaseInitializer(connectionString);
initializer.InitializeDatabase();



var app = builder.Build();


app.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping.Register(() => {
   
   initializer.DatabaseCleanup();
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();


}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();