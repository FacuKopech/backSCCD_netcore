using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Repositories;
using Data.Contracts;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;
using Model.State;
using SCCD.Command.Ausencia;
using SCCD.Services.Interfaces;
using SCCD.Services.Entities;

var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString)); ;

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connString).EnableSensitiveDataLogging());
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers().AddJsonOptions(x =>
x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

//Services added
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUsuarioRepositorie, UsuarioRepositorio>();
builder.Services.AddScoped<IPersonaRepositorie, PersonaRepositorio>();
builder.Services.AddScoped<IInstitucionRepositorie, InstitucionRepositorio>();
builder.Services.AddScoped<INotaRepositorie, NotaRepositorio>();
builder.Services.AddScoped<IGrupoRepositorie, GrupoRepositorio>();
builder.Services.AddScoped<IAulaRepositorie, AulaRepositorio>();
builder.Services.AddScoped<IHistorialRepositorie, HistorialRepositorio>();
builder.Services.AddScoped<IAusenciaRepositorie, AusenciaRepositorie>();
builder.Services.AddScoped<IAsistenciaRepositorie, AsistenciaRepositorio>();
builder.Services.AddScoped<ILoginAuditRepositorie, LoginAuditRepositorio>();
builder.Services.AddScoped<IHistorialAuditRepositorie, HistorialAuditRepositorio>();
builder.Services.AddScoped<IReportesRepositorie, ReportesRepositorio>();
builder.Services.AddScoped<IEventoRepositorie, EventoRepositorie>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IArchivosService, ArchivosService>();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAusenciaCommand, AgregarAusenciaCommand>();
builder.Services.AddScoped<IAusenciaDataLayerRepo, AusenciaRepositorie>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policyBuilder =>
    {
        policyBuilder.WithOrigins().AllowAnyOrigin();
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
    });
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//builder.Services.AddAutoMapper(typeof(Program));
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
    //options.MemoryBufferThreshold = Int32.MaxValue;-
    //options.MultipartBodyLengthLimit = 10485760; // Set your desired limit in bytes
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();

}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAllOrigins");
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
