using BankAccountManager.CustomPolicies;
using BankAccountManager.Data;
using BankAccountManager.Enums;
using BankAccountManager.ExtensionMethods;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "RequireCurrentAccountVipUser",
        policy => policy.Requirements.Add(new VipAuthorizationRequirement(AccountTypes.Current))
    );
});

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>();

builder
    .Services.AddIdentityCore<AppUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 7;
        options.Password.RequireNonAlphanumeric = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (await roleManager.RoleExistsAsync(AppUserRoles.User.ToString()))
    {
        await roleManager.CreateAsync(new IdentityRole(AppUserRoles.User.ToString()));
    }
    if (await roleManager.RoleExistsAsync(AppUserRoles.VipUser.ToString()))
    {
        await roleManager.CreateAsync(new IdentityRole(AppUserRoles.VipUser.ToString()));
    }
    if (await roleManager.RoleExistsAsync(AppUserRoles.PlatinumUser.ToString()))
    {
        await roleManager.CreateAsync(new IdentityRole(AppUserRoles.PlatinumUser.ToString()));
    }
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
