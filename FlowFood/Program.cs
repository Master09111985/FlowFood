using FlowFood.Data;
using FlowFood.Repositorio;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// 1. DbContext - SQL Server
// ==============================
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(/*"FlowFoodConnection"*/"FlowFoodCloud"))
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging());


// ==============================================
// 2. Inyeccion de dependencias - Repositorio
// ==============================================
builder.Services.AddScoped<IAlmacenRepositorio, AlmacenRepositorio>();
builder.Services.AddScoped<IAsistenciaRepositorio, AsistenciaRepositorio>();
builder.Services.AddScoped<IBajaRepositorio, BajaRepositorio>();
builder.Services.AddScoped<ICajaRepositorio,  CajaRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IComandaRepositorio, ComandaRepositorio>();
builder.Services.AddScoped<IEmpleadoRepositorio, EmpleadoRepositorio>();
builder.Services.AddScoped<IInventarioRepositorio, InventarioRepositorio>();
builder.Services.AddScoped<IMetodoPagoRepositorio, MetodoPagoRepositorio>();
builder.Services.AddScoped<IMotivoBajaRepositorio, MotivoBajaRepositorio>();
builder.Services.AddScoped<INominaRepositorio, NominaRepositorio>();
builder.Services.AddScoped<IPermisoRepositorio, PermisoRepositorio>();
builder.Services.AddScoped<IPlataformaRepositorio, PlataformaRepositorio>();
builder.Services.AddScoped<IPlatilloRepositorio, PlatilloRepositorio>();
builder.Services.AddScoped<IProductoRepositorio, ProductoRepositorio>();
builder.Services.AddScoped<IProveedorRepositorio, ProveedorRepositorio>();
builder.Services.AddScoped<IPuestoRepositorio, PuestoRepositorio>();
builder.Services.AddScoped<IRolPermisoRepositorio, RolPermisoRepositorio>();
builder.Services.AddScoped<IRolRepositorio, RolRepositorio>();
builder.Services.AddScoped<IUnidadMedidaRepositorio, UnidadMedidaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

// =========================================
// 3. CORS - Para consumir desde Angular
// =========================================
const string PoliticaAngular = "PoliticaAngular";

builder.Services.AddCors(options =>
{
  options.AddPolicy(name: PoliticaAngular, policy =>
  {
    policy.WithOrigins(
            "http://localhost:4200",   // ng serve por defecto
            "https://localhost:4200",
            "https://CamaronServer:9001",
            "https://camaronserver:9001"
          )
          .AllowAnyHeader()
          .AllowAnyMethod();
    // .AllowCredentials(); // para usar cookies/autenticacion con credenciales
  });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 1. Definimos la ruta
var carpetaFotos = @"C:\LosDeCamaron\empleados";

// 2. Revisamos si existe, y si no, la creamos automáticamente
if (!Directory.Exists(carpetaFotos))
{
  Directory.CreateDirectory(carpetaFotos);
}

app.UseStaticFiles();

// 3. Ahora sí, inicializamos el proveedor de archivos con toda seguridad
app.UseStaticFiles(new StaticFileOptions
{
  FileProvider = new PhysicalFileProvider(carpetaFotos),
  RequestPath = "/fotos-empleados"
});

app.UseCors(PoliticaAngular);

app.UseAuthorization();

app.MapControllers();

app.Run();
