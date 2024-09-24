using BookSale.Management.DataAccess;
using BookSale.Management.DataAccess.Configuration;
using BookSale.Management.UI.Ulity;

var builder = WebApplication.CreateBuilder(args);
var builderRazer = builder.Services.AddRazorPages();




builder.Services.ConfigureIdentity(builder.Configuration);

builder.Services.AddDependencyInjection();

builder.Services.AddAutoMapper();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews(options => {
  options.Conventions.Add(new SiteAreaConvention());
});

builder.Services.AddAuthorizationGlobal();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromMinutes(30);
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// Nếu bạn muốn chỉ dùng trong môi trường Development:
if (builder.Environment.IsDevelopment())
{
  builder.Services.AddControllersWithViews()
      .AddRazorRuntimeCompilation();
}

var app = builder.Build();


app.AutoMigration().GetAwaiter().GetResult();

app.SeedData(builder.Configuration).GetAwaiter().GetResult();

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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();



app.MapAreaControllerRoute(
    name: "AdminRouting",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}")
.RequireAuthorization("AuthorizedAdminPolicy");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Shop}/{action=Index}/{id?}");

app.Run();
