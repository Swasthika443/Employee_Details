using Employee_Details.Models;

using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Logging;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EmployeeContext>(OptionsBuilderConfigurationExtensions => OptionsBuilderConfigurationExtensions.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Hangfire services and configure SQL Server storage
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"));
});


builder.Logging.AddConsole();

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=employees}/{action=Index}/{id?}");


app.UseHangfireServer(); // Start Hangfire server

// Enqueue the file processing job to run every 5 minutes
RecurringJob.AddOrUpdate<EmployeeFlatFileprocess>(x => x.ProcessFile("C:\\Users\\EI13150\\source\\repos\\E_D - Copy\\Employee_Details\\Flatfile\\employeedetails.txt"), Cron.MinuteInterval(10));

// Inside the Configure method
app.UseHangfireDashboard();

app.Run();

