using Npgsql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Queue_Management_System.Hubs;

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

  builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole",
         policy => policy.RequireRole("super"));
});

// Add services to the container
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();


builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<IServicePointService, ServicePointService>();

builder.Services.AddScoped<IServiceProviderService, ServiceProviderService>();

builder.Services.AddScoped<IReportService, ReportService>();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");



var initializer = new DatabaseInitializer(connectionString);
initializer.InitializeDatabase();



var app = builder.Build();


// app.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping.Register(() => {
   
//    initializer.DatabaseCleanup();
// });


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

app.MapHub<TicketHub>("/ticketHub");

app.Run();