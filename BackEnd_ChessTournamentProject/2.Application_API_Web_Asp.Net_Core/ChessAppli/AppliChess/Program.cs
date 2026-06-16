using BLL.Interfaces;
using BLL.Services;
using DAL.Connection;
using DAL.Interfaces;
using DAL.Repositories;
using API.Middlewares;

var builder = WebApplication.CreateBuilder(args);


//SERVICES DE BASE
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//GESTION D'ERREUR GLOBALE
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

//CONNEXION DB
builder.Services.AddScoped<DbConnection>();

//REPOSITORIES (DAL)
builder.Services.AddScoped<IChessClubRepository, ChessClubRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

//SERVICES (BLL)
builder.Services.AddScoped<IChessClubService, ChessClubService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IMatchService, MatchService>();

//CORS - Autorise Angular (-http://localhost:4200-) � appeler l'API
builder.Services.AddCors(options =>
{
	options.AddPolicy("Angular", policy => 
	{
		policy.WithOrigins("http://localhost:4200")
		.AllowAnyMethod()
		.AllowAnyHeader();
	});
});

//------------------------
var app = builder.Build(); //Moment o� on assemble tout ce qu'on a configur� et on cr�e l'application 
//------------------------


//MIDDLEWARES


//Middleware 1
app.UseExceptionHandler(); //Attrape les erreurs
app.UseStatusCodePages();

//Middleware 2
app.UseCors("Angular"); //Autorise Angular � appeler l'API

//Middleware 3
if (app.Environment.IsDevelopment())
{
	app.UseSwagger(); //Active la doc API
	app.UseSwaggerUI();
}

//Middleware 4
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection(); //Redirige HTTP vers HTTPS
}

//Middleware 5
app.UseAuthorization(); //V�rifie les droits d'acc�s


//DESTINATION FINALE !
app.MapControllers(); //Dirige vers le bon Controller

app.Run();

//Rappel de la requ�te HTTP :
//1.Requ�te HTTP traverse tous les middlewares
//2.Arrive au Controller
//3.Repart en sens inverse
