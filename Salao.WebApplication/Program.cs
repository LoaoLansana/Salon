using Salao.WebApplication;

var builder = WebApplication.CreateBuilder(args);

// Criar uma instância da classe Startup
var startup = new Startup(builder.Configuration);

// Adicionar serviços à instância da classe Startup
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configurar o pipeline de solicitação HTTP
startup.Configure(app, builder.Environment);

app.Run();

