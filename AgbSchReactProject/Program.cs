using Infrastructure.Accounts;
using ApplicationCore.Utility;
using Infrastructure.Data;
//using Infrastructure.Entity;
using AgbSchReactProject.MiddlewareServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Accounts;
using System.Text;
//using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Bind Database connection

var connectionStr = builder.Configuration.GetConnectionString("DefaultConn");

builder.Services.AddDbContext<ApplicationDbContext>(Options => Options.UseSqlServer(connectionStr));
//for authentication, this will inject the user manager into the sol
//to enable use use roles, we need change AddDefaultIdentity to AddIdentity and add IdentityRole
// services.AddDefaultIdentity<IdentityUser>(otp =>
builder.Services.AddIdentity<AdminRegistration, AdminRole>(otp =>
{
    otp.SignIn.RequireConfirmedAccount = false;
    otp.Password.RequireNonAlphanumeric = false;
    otp.Password.RequiredLength = 6;
}

)//IdentityUser allow creation of uers
    .AddRoleManager<RoleManager<AdminRole>>()
    .AddUserManager<UserManager<AdminRegistration>>()
    .AddSignInManager<SignInManager<AdminRegistration>>()//this is to allow sign-n
.AddEntityFrameworkStores<ApplicationDbContext>();




//Authentication
// var key = "1010101010101010290877";
var expectedKey = builder.Configuration.GetSection("SecretKey");
builder.Services.Configure<SecretKeys>(expectedKey);//map the appsettings section to the Object
var appSettings = expectedKey.Get<SecretKeys>();

var key = Encoding.ASCII.GetBytes(appSettings.Secrete);


builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}

).AddJwtBearer(x =>

{

    x.RequireHttpsMetadata = false;//set to true if using https

    x.SaveToken = true;

    x.TokenValidationParameters = new TokenValidationParameters

    {

        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = false,

        ValidateAudience = false

    };

});



// This is an extension method to keep this startup class clean.
builder.Services.AddScopedConfig();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDocSwagger();  // Swagger Configuration

builder.Services.AddAutoMapper(typeof(MappingProfile));




var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InternalCRM"));
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InternalCRM"));
}


//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // use context

    if (context.Database.EnsureCreated())
    {
        context.Database.Migrate();
    }
    else
    {
        context.Database.EnsureCreated();
    }

}

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


//Create Deafult Admin User //to seed data
IServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
CreateDafaultUserRoles(serviceProvider).Wait();

//await CreateDafaultUserRoles(serviceProvider);

async Task CreateDafaultUserRoles(IServiceProvider serviceProvider)
{
    var RoleManager = serviceProvider.GetRequiredService<RoleManager<AdminRole>>();
    var UserManager = serviceProvider.GetRequiredService<UserManager<AdminRegistration>>();

    //var user = new User() { Email = "admin@gmail.com", UserName = "admin" };

    var user = new AdminRegistration
    {
        FirstName = "Godwin",
        LastName = "Agbon",
        Email = "agbonwinn1@yahoo.com",
        NormalizedEmail = "AGBONWINN1@YAHOO.COM",
        UserName = "Godwin",
        NormalizedUserName = "GODWIN",
        PhoneNumber = "07034539975",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true,
        SecurityStamp = Guid.NewGuid().ToString("D"),
        UserType ="Non Student",
        UserRole= "Admin"
    };
    var result = await UserManager.CreateAsync(user, "Pa$$word123");
    IdentityResult result2;
    //Check if role exist, else create role and add user
    var roleCheck = await RoleManager.RoleExistsAsync(user.UserRole);
    var role = new AdminRole();
    role.Name = user.UserRole;// "Admin";
    if (!roleCheck)
    {
        //create the roles and seed them to the database  
        //await RoleManager.CreateAsync(role.RoleName);
        result2 = await RoleManager.CreateAsync(role);
    }
    await UserManager.AddToRoleAsync(user, user.UserRole);
}

app.Run();
