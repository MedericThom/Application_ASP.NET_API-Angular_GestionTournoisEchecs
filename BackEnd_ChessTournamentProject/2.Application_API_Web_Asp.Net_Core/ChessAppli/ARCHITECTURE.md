# Architecture complète — Chess Tournament API (ASP.NET Core 10)

## Vue d'ensemble

Application API REST en 4 couches, solution `ChessAppli.slnx`.

```
API (ChessAppli/) → BLL (BLL/) → DAL (DAL/) → DOMAIN (DOMAIN/)
```

Toutes les couches référencent DOMAIN. BLL référence les interfaces DAL. API référence les interfaces BLL.  
Injection de dépendances via `AddScoped` dans `Program.cs`.  
Base de données : SQL Server, ADO.NET pur (pas d'ORM).

---

## Démarrage / Configuration

### Variables d'environnement

```
CHESS_CONNECTION_STRING=Data Source=<SERVER>\TFTIC;Initial Catalog=asp_api_ChessTournament;Integrated Security=True;TrustServerCertificate=True;
```

### Commandes

```bash
dotnet build
dotnet run --project ChessAppli/API.csproj
dotnet run --project ChessAppli/API.csproj --launch-profile https
```

### URLs

- HTTP : `http://localhost:5083`
- HTTPS : `https://localhost:7108`
- Swagger UI : `/swagger` (mode Development uniquement)

### CORS

Politique nommée `"Angular"` — autorise toutes les requêtes depuis `http://localhost:4200`.

---

## DOMAIN — Entités (POCOs)

Namespace : `DOMAIN.Entities`  
Projet : `DOMAIN.csproj` (net10.0, aucune dépendance externe)

```csharp
public class ChessClub
{
    public int ChessClub_Id { get; set; }
    public string NameChessClub { get; set; }
}

public class Category
{
    public int Category_Id { get; set; }
    public string NameCategory { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
}

public class Player
{
    public int Player_Id { get; set; }
    public string Pseudo { get; set; }
    public string Email { get; set; }
    public string Pwd { get; set; }          // hashé BCrypt en BLL
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }       // "Male" | "Female"
    public int Elo { get; set; } = 1200;
    public int? ChessClub_Id { get; set; }   // nullable
}

public class Tournament
{
    public int Tournament_Id { get; set; }
    public string NameTournament { get; set; }
    public string Place { get; set; }
    public int MinNbPlayer { get; set; }
    public int MaxNbPlayer { get; set; }
    public int? MinElo { get; set; }         // nullable
    public int? MaxElo { get; set; }         // nullable
    public string StatusTournament { get; set; } // "En attente de joueurs" | "En cours" | "Clôturé"
    public int CurrentRound { get; set; } = 0;
    public bool WomenOnly { get; set; } = false;
    public DateTime RegistrationDeadline { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime UpdateDate { get; set; }
}

public class Registration
{
    public int Registration_Id { get; set; }
    public int Player_Id { get; set; }
    public int Tournament_Id { get; set; }
    public int Wins { get; set; } = 0;
    public int Losses { get; set; } = 0;
    public int Draws { get; set; } = 0;
    public decimal Score { get; set; } = 0;
    public int MatchesPlayed { get; set; } = 0;
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
}

public class Match_
{
    public int Match_Id { get; set; }
    public int RoundNumber { get; set; }
    public int? Result { get; set; }  // null=non joué, 0=nul, 1=blancs gagnent, 2=noirs gagnent
    public int Tournament_Id { get; set; }
    public int WhitePlayer_Id { get; set; }
    public int BlackPlayer_Id { get; set; }
}

public class TournamentCategory
{
    public int Tournament_Id { get; set; }
    public int Category_Id { get; set; }
}
```

---

## DAL — Data Access Layer

Namespace : `DAL.Connection`, `DAL.Interfaces`, `DAL.Repositories`  
Projet : `DAL.csproj` (net10.0)  
Dépendance NuGet : `Microsoft.Data.SqlClient v7.0.0`

### DbConnection

```csharp
// DAL/Connection/DbConnection.cs
public class DbConnection
{
    private readonly string _connectionString;

    public DbConnection()
    {
        _connectionString = Environment.GetEnvironmentVariable("CHESS_CONNECTION_STRING")
            ?? throw new Exception("Connection string non trouvée");
    }

    public SqlConnection GetConnection() => new SqlConnection(_connectionString);
}
```

---

### IChessClubRepository / ChessClubRepository

```csharp
public interface IChessClubRepository
{
    Task CreateChessClubAsync(ChessClub chessclub);
    Task<List<ChessClub>> GetAllAsync();
}
```

Requêtes SQL :
```sql
-- CreateChessClubAsync
INSERT INTO ChessClub (NameChessClub) VALUES (@NameChessClub)

-- GetAllAsync
SELECT * FROM ChessClub
```

---

### ICategoryRepository / CategoryRepository

```csharp
public interface ICategoryRepository
{
    Task CreateCategoryAsync(Category category);
    Task<List<Category>> GetCategoriesByTournamentIdAsync(int tournamentId);
    Task<List<Category>> GetAllAsync();
}
```

Requêtes SQL :
```sql
-- CreateCategoryAsync
INSERT INTO Category (NameCategory, MinAge, MaxAge) VALUES (@NameCategory, @MinAge, @MaxAge)

-- GetCategoriesByTournamentIdAsync
SELECT c.* FROM Category c
JOIN TournamentCategory tc ON c.Category_Id = tc.Category_Id
WHERE tc.Tournament_Id = @Tournament_Id

-- GetAllAsync
SELECT * FROM Category
```

---

### IPlayerRepository / PlayerRepository

```csharp
public interface IPlayerRepository
{
    Task CreatePlayerAsync(Player player);
    Task<Player> GetByPlayerIdAsync(int playerId);
    Task<List<Player>> GetPlayersByTournamentIdAsync(int tournamentId);
    Task<Player> GetByEmailAsync(string email);
    Task<Player> GetByPseudoAsync(string pseudo);
    Task<List<Player>> GetAllAsync();
}
```

Requêtes SQL :
```sql
-- CreatePlayerAsync
INSERT INTO Player (Pseudo, Email, Pwd, BirthDate, Gender, Elo, ChessClub_Id)
VALUES (@Pseudo, @Email, @Pwd, @BirthDate, @Gender, @Elo, @ChessClub_Id)

-- GetByPlayerIdAsync
SELECT * FROM Player WHERE Player_Id = @Player_Id

-- GetPlayersByTournamentIdAsync
SELECT p.* FROM Player p
JOIN Registration r ON p.Player_Id = r.Player_Id
WHERE r.Tournament_Id = @Tournament_Id

-- GetByEmailAsync
SELECT Player_Id, Email FROM Player WHERE Email = @Email

-- GetByPseudoAsync
SELECT Player_Id, Pseudo FROM Player WHERE Pseudo = @Pseudo

-- GetAllAsync
SELECT * FROM Player
```

---

### ITournamentRepository / TournamentRepository

```csharp
public interface ITournamentRepository
{
    Task CreateTournamentAsync(Tournament tournament);
    Task AddCategoriesToTournamentAsync(int tournamentId, List<int> categoryIds);
    Task<List<Tournament>> GetLastTenTournamentInProgressAsync();
    Task<Tournament?> GetByIdAsync(int tournamentId);
    Task UpdateTournamentAsync(Tournament tournament);
    Task DeleteTournamentAsync(int tournamentId);
}
```

Requêtes SQL :
```sql
-- CreateTournamentAsync
INSERT INTO Tournament (NameTournament, Place, MinNbPlayer, MaxNbPlayer, MinElo, MaxElo,
    StatusTournament, CurrentRound, WomenOnly, RegistrationDeadline, CreationDate, UpdateDate)
VALUES (...)

-- AddCategoriesToTournamentAsync (boucle sur chaque categoryId)
INSERT INTO TournamentCategory (Tournament_Id, Category_Id) VALUES (@Tournament_Id, @Category_Id)

-- GetLastTenTournamentInProgressAsync
SELECT TOP 10 * FROM Tournament
WHERE StatusTournament != 'Clôturé'
ORDER BY UpdateDate DESC

-- GetByIdAsync
SELECT * FROM Tournament WHERE Tournament_Id = @Tournament_Id

-- UpdateTournamentAsync
UPDATE Tournament
SET StatusTournament = @StatusTournament, CurrentRound = @CurrentRound, UpdateDate = @UpdateDate
WHERE Tournament_Id = @Tournament_Id

-- DeleteTournamentAsync
DELETE FROM Tournament WHERE Tournament_Id = @Tournament_Id
```

---

### IRegistrationRepository / RegistrationRepository

```csharp
public interface IRegistrationRepository
{
    Task CreateRegistrationAsync(Registration registration);
    Task<bool> IsPlayerRegisteredAsync(int playerId, int tournamentId);
    Task<int> GetRegisteredPlayersCountAsync(int tournamentId);
    Task<List<Registration>> GetScoresByTournamentAsync(int tournamentId, int round);
    Task DeleteRegistrationAsync(int playerId, int tournamentId);
}
```

Requêtes SQL :
```sql
-- CreateRegistrationAsync
INSERT INTO Registration (Player_Id, Tournament_Id, Wins, Losses, Draws, Score, MatchesPlayed, RegistrationDate)
VALUES (...)

-- IsPlayerRegisteredAsync (retourne bool via SqlDataReader)
SELECT * FROM Registration WHERE Player_Id = @Player_Id AND Tournament_Id = @Tournament_Id

-- GetRegisteredPlayersCountAsync (ExecuteScalarAsync)
SELECT COUNT(*) FROM Registration WHERE Tournament_Id = @Tournament_Id

-- GetScoresByTournamentAsync
SELECT r.* FROM Registration r
JOIN Tournament t ON r.Tournament_Id = t.Tournament_Id
JOIN Player p ON r.Player_Id = p.Player_Id
WHERE r.Tournament_Id = @Tournament_Id AND t.CurrentRound = @Round
ORDER BY r.Score DESC

-- DeleteRegistrationAsync
DELETE FROM Registration WHERE Player_Id = @Player_Id AND Tournament_Id = @Tournament_Id
```

---

### IMatchRepository / MatchRepository

```csharp
public interface IMatchRepository
{
    Task CreateMatchesAsync(List<Match_> matches);
    Task<Match_?> GetMatchByIdAsync(int matchId);
    Task<List<Match_>> GetMatchesByTournamentAndRoundAsync(int tournamentId, int round);
    Task UpdateMatchAsync(Match_ match);
}
```

Requêtes SQL :
```sql
-- CreateMatchesAsync (boucle sur chaque match)
INSERT INTO Match_ (Tournament_Id, WhitePlayer_Id, BlackPlayer_Id, RoundNumber, Result)
VALUES (...)

-- GetMatchByIdAsync
SELECT * FROM Match_ WHERE Match_Id = @Match_Id

-- GetMatchesByTournamentAndRoundAsync
SELECT m.* FROM Match_ m
WHERE m.Tournament_Id = @Tournament_Id AND m.RoundNumber = @RoundNumber

-- UpdateMatchAsync
UPDATE Match_ SET Result = @Result WHERE Match_Id = @Match_Id
```

---

## BLL — Business Logic Layer

Namespace : `BLL.Interfaces`, `BLL.Services`  
Projet : `BLL.csproj` (net10.0)  
Dépendance NuGet : `BCrypt.Net-Next v4.1.0`

---

### IChessClubService / ChessClubService

```csharp
public interface IChessClubService
{
    Task CreateChessClubAsync(ChessClub chessclub);
    Task<List<ChessClub>> GetAllAsync();
}
```

Logique `CreateChessClubAsync` :
- Vérifie que `NameChessClub` n'est pas vide → `ArgumentException("Le nom du club ne peut être vide !")`

---

### ICategoryService / CategoryService

```csharp
public interface ICategoryService
{
    Task CreateCategoryAsync(Category category);
    Task<List<Category>> GetAllAsync();
}
```

Logique `CreateCategoryAsync` :
- `NameCategory` non vide → `ArgumentException("Indiquez un nom de categorie")`
- `MinAge >= 0` → sinon `ArgumentException("L'âge minimum doit être supérieur ou égal à 0")`
- `MaxAge <= 110` → sinon `ArgumentException("L'âge maximum doit être inférieur ou égal à 110")`
- `MinAge <= MaxAge` → sinon `ArgumentException("L'âge minimum doit être inférieur ou égal à l'âge maximum")`

---

### IPlayerService / PlayerService

```csharp
public interface IPlayerService
{
    Task CreatePlayerAsync(Player player);
    Task<Player> GetByPlayerIdAsync(int playerId);
    Task<List<Player>> GetPlayersByTournamentIdAsync(int tournamentId);
    Task<List<Player>> GetAllAsync();
}
```

Logique `CreatePlayerAsync` :
1. Si `ChessClub_Id == 0` → met à `null`
2. Vérifie unicité email → `ArgumentException("Cette adresse e-mail est déjà utilisé !")`
3. Vérifie unicité pseudo → `ArgumentException("Ce pseudo est déjà utilisé !")`
4. Hash le mot de passe : `BCrypt.Net.BCrypt.HashPassword(player.Pwd)`
5. Si `Elo == 0` → force à `1200`

---

### IRegistrationService / RegistrationService

Dépend de : `IRegistrationRepository`, `ITournamentRepository`, `IPlayerRepository`, `ICategoryRepository`

```csharp
public interface IRegistrationService
{
    Task RegisterPlayerAsync(Registration registration);
    Task<List<Registration>> GetScoresAsync(int tournamentId, int round);
    Task UnregisterPlayerAsync(int playerId, int tournamentId);
}
```

Logique `RegisterPlayerAsync` (10 validations dans l'ordre) :
1. Tournoi existe → `KeyNotFoundException("Tournoi introuvable !")`
2. Joueur existe → `KeyNotFoundException("Joueur introuvable !")`
3. Status = `"En attente de joueurs"` → sinon `ArgumentException("Le tournoi a déjà commencé")`
4. `DateTime.Now <= RegistrationDeadline` → sinon `ArgumentException("La date de fin des inscriptions est dépassée")`
5. Joueur pas déjà inscrit → `ArgumentException("Le joueur est déjà inscrit !")`
6. Nombre inscrits < MaxNbPlayer → `ArgumentException("Le tournoi a atteint son nombre maximum de joueurs !")`
7. Âge du joueur correspond à au moins une catégorie du tournoi → `ArgumentException("L'âge du joueur ne correspond à aucune catégorie autorisée !")`
8. `Elo >= MinElo` (si défini) → `ArgumentException("L'ELO du joueur est inférieur à l'ELO minimum du tournoi !")`
9. `Elo <= MaxElo` (si défini) → `ArgumentException("L'ELO du joueur est supérieur à l'ELO maximum du tournoi !")`
10. Si `WomenOnly` → `Gender == "Female"` → sinon `ArgumentException("Ce tournoi est réservé aux femmes !")`

Logique `UnregisterPlayerAsync` :
1. Tournoi existe → `KeyNotFoundException("Tournoi introuvable !")`
2. Status = `"En attente de joueurs"` → sinon `ArgumentException("Le tournoi a déjà commencé")`
3. Joueur inscrit → sinon `ArgumentException("Le joueur n'est pas inscrit !")`

---

### IMatchService / MatchService

Dépend de : `IMatchRepository`, `ITournamentRepository`

```csharp
public interface IMatchService
{
    Task UpdateMatchAsync(int matchId, int result);
    Task<List<Match_>> GetMatchesByTournamentAndRoundAsync(int tournamentId, int round);
}
```

Logique `UpdateMatchAsync` :
1. Match existe → `KeyNotFoundException("Match introuvable !")`
2. `match.RoundNumber == tournament.CurrentRound` → sinon `ArgumentException("Seuls les matchs de la ronde courante peuvent être modifiés !")`
3. Met à jour `match.Result` et sauvegarde

---

### ITournamentService / TournamentService

Dépend de : `ITournamentRepository`, `IRegistrationRepository`, `IPlayerRepository`, `IMatchRepository`

```csharp
public interface ITournamentService
{
    Task CreateTournamentAsync(Tournament tournament);
    Task AddCategoriesAsync(int tournamentId, List<int> categoryIds);
    Task<List<Tournament>> GetTournamentsAsync();
    Task<Tournament?> GetTournamentByIdAsync(int tournamentId);
    Task StartTournamentAsync(int tournamentId);
    Task NextRoundAsync(int tournamentId);
    Task DeleteTournamentAsync(int tournamentId);
}
```

Logique `CreateTournamentAsync` :
1. `MinNbPlayer <= MaxNbPlayer` → sinon `ArgumentException`
2. `MinElo <= MaxElo` (si les deux sont définis) → sinon `ArgumentException`
3. `RegistrationDeadline > DateTime.Now.AddDays(MinNbPlayer)` → sinon `ArgumentException`
4. Initialise : `CurrentRound = 0`, `StatusTournament = "En attente de joueurs"`, `CreationDate = UpdateDate = DateTime.Now`

Logique `StartTournamentAsync` :
1. Tournoi existe, status = `"En attente de joueurs"` → sinon erreurs
2. `RegistrationDeadline < DateTime.Now` → sinon `ArgumentException("La date de fin des inscriptions n'est pas encore dépassée !")`
3. Inscrits >= `MinNbPlayer` → sinon `ArgumentException("Le nombre minimum de participants n'est pas atteint !")`
4. Génère les matchs aller-retour round-robin pour la ronde 1 :
   ```
   Pour chaque paire (i, j) avec i < j :
     - Match blanc=players[i], noir=players[j], ronde=1
     - Match blanc=players[j], noir=players[i], ronde=1
   ```
5. `CurrentRound = 1`, `StatusTournament = "En cours"`, `UpdateDate = DateTime.Now`
6. Sauvegarde tous les matchs puis le tournoi

Logique `NextRoundAsync` :
1. Tournoi existe
2. Tous les matchs de la ronde courante ont un résultat (non null) → sinon `ArgumentException("Tous les matchs doivent avoir un résultat !")`
3. `CurrentRound++`, `UpdateDate = DateTime.Now`, sauvegarde

Logique `DeleteTournamentAsync` :
1. Tournoi existe
2. Status = `"En attente de joueurs"` → sinon `ArgumentException("Impossible de supprimer un tournoi qui a déjà commencé !")`

---

## API — Controllers, DTOs, Mappers

Namespace : `API.Controllers`, `API.DTO`, `API.Mappers`  
Projet : `ChessAppli/API.csproj` (net10.0)  
Dépendances NuGet : `Swashbuckle.AspNetCore v10.1.7`, `Microsoft.AspNetCore.OpenApi v10.0.5`  
Route de base : `api/[controller]`

---

### DTOs

#### ChessClub
```csharp
public class ChessClubCreateDTO
{
    public string NameChessClub { get; set; }
}

public class ChessClubResponseDTO
{
    public int ChessClub_Id { get; set; }
    public string NameChessClub { get; set; }
}
```

#### Category
```csharp
public class CategoryCreateDTO
{
    [Required] public string NameCategory { get; set; }
    [Required][Range(0, 110)] public int MinAge { get; set; }
    [Required][Range(0, 110)] public int MaxAge { get; set; }
}

public class CategoryResponseDTO
{
    public int Category_Id { get; set; }
    public string NameCategory { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
}
```

#### Player
```csharp
public class PlayerCreateDTO
{
    [Required] public string Pseudo { get; set; }
    [Required][EmailAddress] public string Email { get; set; }
    [Required] public string Password { get; set; }   // mappé vers Player.Pwd
    [Required] public DateTime BirthDate { get; set; }
    [Required] public string Gender { get; set; }     // "Male" | "Female"
    [Range(0, 3000)] public int Elo { get; set; } = 1200;
}

public class PlayerResponseDTO
{
    public int Player_Id { get; set; }
    public string Pseudo { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public int Elo { get; set; }
    public int? ChessClub_Id { get; set; }
    // Pwd n'est jamais exposé
}
```

#### Tournament
```csharp
public class TournamentCreateDTO
{
    [Required][MinLength(3)][MaxLength(50)] public string NameTournament { get; set; }
    [Required] public string Place { get; set; }
    [Required][Range(2, 100)] public int MinNbPlayer { get; set; }
    [Required][Range(2, 100)] public int MaxNbPlayer { get; set; }
    [Range(0, 3000)] public int? MinElo { get; set; }
    [Range(0, 3000)] public int? MaxElo { get; set; }
    public bool WomenOnly { get; set; } = false;
    [Required] public DateTime RegistrationDeadline { get; set; }
    [Required] public List<int> CategoryIds { get; set; }
}

public class TournamentResponseDTO
{
    public int Tournament_Id { get; set; }
    public string NameTournament { get; set; }
    public string Place { get; set; }
    public int MinNbPlayer { get; set; }
    public int MaxNbPlayer { get; set; }
    public int? MinElo { get; set; }
    public int? MaxElo { get; set; }
    public string StatusTournament { get; set; }
    public int CurrentRound { get; set; }
    public bool WomenOnly { get; set; }
    public DateTime RegistrationDeadline { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime UpdateDate { get; set; }
}

public class TournamentDetailDTO  // retourné par GET /api/tournament/{id}
{
    public int Tournament_Id { get; set; }
    public string NameTournament { get; set; }
    public string Place { get; set; }
    public int MinNbPlayer { get; set; }
    public int MaxNbPlayer { get; set; }
    public int? MinElo { get; set; }
    public int? MaxElo { get; set; }
    public string StatusTournament { get; set; }
    public int CurrentRound { get; set; }
    public bool WomenOnly { get; set; }
    public DateTime RegistrationDeadline { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public List<PlayerResponseDTO> Players { get; set; }
    public List<MatchResponseDTO> Matches { get; set; }
}
```

#### Registration
```csharp
public class RegistrationCreateDTO
{
    [Required] public int Player_Id { get; set; }
    [Required] public int Tournament_Id { get; set; }
}

public class RegistrationResponseDTO
{
    public int Registration_Id { get; set; }
    public int Player_Id { get; set; }
    public int Tournament_Id { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public decimal Score { get; set; }
    public int MatchesPlayed { get; set; }
    public DateTime RegistrationDate { get; set; }
}

public class ScoreDTO
{
    public string Pseudo { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public decimal Score { get; set; }
    public int MatchesPlayed { get; set; }
}
```

#### Match
```csharp
public class MatchUpdateDTO
{
    [Required][Range(0, 2)] public int Result { get; set; }
    // 0 = nul, 1 = blancs gagnent, 2 = noirs gagnent
}

public class MatchResponseDTO
{
    public int Match_Id { get; set; }
    public int RoundNumber { get; set; }
    public int? Result { get; set; }
    public int Tournament_Id { get; set; }
    public int WhitePlayer_Id { get; set; }
    public int BlackPlayer_Id { get; set; }
}
```

---

### Mappers

Chaque mapper est une classe statique avec des méthodes d'extension.

| Mapper | Méthodes |
|---|---|
| `ChessClubMapper` | `ToEntity(ChessClubCreateDTO)`, `ToResponse(ChessClub)` |
| `CategoryMapper` | `ToEntity(CategoryCreateDTO)`, `ToResponse(Category)` |
| `PlayerMapper` | `ToEntity(PlayerCreateDTO)` *(Password → Pwd)*, `ToResponse(Player)` |
| `MatchMapper` | `ToEntity(MatchUpdateDTO)`, `ToResponse(Match_)` |
| `RegistrationMapper` | `ToEntity(RegistrationCreateDTO)`, `ToResponse(Registration)`, `ToScore(Registration, Player)` |
| `TournamentMapper` | `ToEntity(TournamentCreateDTO)`, `ToResponse(Tournament)`, `ToDetail(Tournament, List<PlayerResponseDTO>, List<MatchResponseDTO>)` |

---

### Controllers & Routes

#### GET /api/chessclub
Retourne : `List<ChessClubResponseDTO>` — 200 OK

#### POST /api/chessclub
Body : `ChessClubCreateDTO`  
Retourne : `ChessClubResponseDTO` — 201 Created

---

#### GET /api/category
Retourne : `List<CategoryResponseDTO>` — 200 OK

#### POST /api/category
Body : `CategoryCreateDTO`  
Retourne : `CategoryResponseDTO` — 201 Created

---

#### GET /api/player
Retourne : `List<PlayerResponseDTO>` — 200 OK

#### POST /api/player
Body : `PlayerCreateDTO`  
Retourne : `PlayerResponseDTO` — 201 Created

---

#### POST /api/tournament
Body : `TournamentCreateDTO` *(inclut `CategoryIds: List<int>`)*  
Retourne : `TournamentResponseDTO` — 201 Created  
Note : crée le tournoi puis appelle `AddCategoriesAsync` en interne.

#### GET /api/tournament
Retourne : `List<TournamentResponseDTO>` — 200 OK  
*(les 10 derniers tournois non clôturés, triés par UpdateDate DESC)*

#### GET /api/tournament/{id}
Retourne : `TournamentDetailDTO` — 200 OK | 404 Not Found  
*(inclut la liste des joueurs inscrits et les matchs de la ronde courante)*

#### PATCH /api/tournament/{id}/start
Démarre le tournoi (génère les matchs round-robin)  
Retourne : 204 No Content

#### PATCH /api/tournament/{id}/next-round
Passe à la ronde suivante (valide que tous les matchs ont un résultat)  
Retourne : 204 No Content

#### DELETE /api/tournament/{id}
Supprime un tournoi (uniquement si status = "En attente de joueurs")  
Retourne : 204 No Content

---

#### POST /api/registration
Body : `RegistrationCreateDTO`  
Retourne : `RegistrationResponseDTO` — 201 Created

#### GET /api/registration/{tournamentId}/{round}
Retourne : `List<ScoreDTO>` — 200 OK *(classement trié par score DESC)*

#### DELETE /api/registration/{playerId}/{tournamentId}
Désinscrit un joueur (uniquement si tournoi en attente)  
Retourne : 204 No Content

---

#### PATCH /api/match/{id}
Body : `MatchUpdateDTO`  
Retourne : 204 No Content

---

### Tableau récapitulatif des routes

| Méthode | Route | Body | Retour |
|---|---|---|---|
| GET | /api/chessclub | — | `List<ChessClubResponseDTO>` |
| POST | /api/chessclub | `ChessClubCreateDTO` | `ChessClubResponseDTO` 201 |
| GET | /api/category | — | `List<CategoryResponseDTO>` |
| POST | /api/category | `CategoryCreateDTO` | `CategoryResponseDTO` 201 |
| GET | /api/player | — | `List<PlayerResponseDTO>` |
| POST | /api/player | `PlayerCreateDTO` | `PlayerResponseDTO` 201 |
| POST | /api/tournament | `TournamentCreateDTO` | `TournamentResponseDTO` 201 |
| GET | /api/tournament | — | `List<TournamentResponseDTO>` |
| GET | /api/tournament/{id} | — | `TournamentDetailDTO` |
| PATCH | /api/tournament/{id}/start | — | 204 |
| PATCH | /api/tournament/{id}/next-round | — | 204 |
| DELETE | /api/tournament/{id} | — | 204 |
| POST | /api/registration | `RegistrationCreateDTO` | `RegistrationResponseDTO` 201 |
| GET | /api/registration/{tournamentId}/{round} | — | `List<ScoreDTO>` |
| DELETE | /api/registration/{playerId}/{tournamentId} | — | 204 |
| PATCH | /api/match/{id} | `MatchUpdateDTO` | 204 |

---

## Gestion des erreurs

### GlobalExceptionHandler

Middleware `IExceptionHandler` enregistré via `AddExceptionHandler<GlobalExceptionHandler>()`.

```csharp
// Mapping exception → HTTP status
ArgumentException    → 400 Bad Request
KeyNotFoundException → 404 Not Found
Tout autre           → 500 Internal Server Error

// Réponse JSON
{ "StatusCode": <int>, "Message": "<message de l'exception>" }
```

---

## Injection de dépendances (Program.cs)

```csharp
// DbConnection
builder.Services.AddScoped<DbConnection>();

// Repositories
builder.Services.AddScoped<IChessClubRepository, ChessClubRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

// Services
builder.Services.AddScoped<IChessClubService, ChessClubService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IMatchService, MatchService>();
```

---

## Schéma de base de données (SQL Server)

```sql
ChessClub        (ChessClub_Id PK, NameChessClub)
Category         (Category_Id PK, NameCategory, MinAge, MaxAge)
Player           (Player_Id PK, Pseudo UNIQUE, Email UNIQUE, Pwd, BirthDate, Gender, Elo DEFAULT 1200, ChessClub_Id FK NULLABLE)
Tournament       (Tournament_Id PK, NameTournament, Place, MinNbPlayer, MaxNbPlayer,
                  MinElo NULLABLE, MaxElo NULLABLE, StatusTournament, CurrentRound DEFAULT 0,
                  WomenOnly DEFAULT 0, RegistrationDeadline, CreationDate, UpdateDate)
TournamentCategory (Tournament_Id FK, Category_Id FK, PK composite)
Registration     (Registration_Id PK, Player_Id FK, Tournament_Id FK,
                  Wins DEFAULT 0, Losses DEFAULT 0, Draws DEFAULT 0,
                  Score DECIMAL DEFAULT 0, MatchesPlayed DEFAULT 0, RegistrationDate)
Match_           (Match_Id PK, RoundNumber, Result NULLABLE, Tournament_Id FK,
                  WhitePlayer_Id FK, BlackPlayer_Id FK)
```

---

## Statuts possibles d'un tournoi

| Valeur | Signification |
|---|---|
| `"En attente de joueurs"` | Tournoi créé, inscriptions ouvertes |
| `"En cours"` | Tournoi démarré, matchs en cours |
| `"Clôturé"` | Tournoi terminé |

---

## Résultat d'un match

| Valeur | Signification |
|---|---|
| `null` | Match non joué |
| `0` | Partie nulle |
| `1` | Les blancs gagnent |
| `2` | Les noirs gagnent |
