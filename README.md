# Application Gestion de Tournois d'Echecs

Application fullstack de gestion de tournois d'échecs, composée d'une API REST ASP.NET Core et d'un frontend Angular.

---

## Structure du projet

```
ApplicationTournoiEchec/
├── BackEnd_ChessTournamentProject/     # API ASP.NET Core 10
└── FrontEnd_ChessTournamentProject/    # Frontend Angular 21
```

---

## Backend — ASP.NET Core 10

### Architecture

L'API suit une architecture en 4 couches :

| Couche | Dossier | Rôle |
|--------|---------|------|
| API | `AppliChess/` | Controllers, DTOs, Mappers |
| BLL | `BLL/` | Services métier |
| DAL | `DAL/` | Repositories ADO.NET |
| DOMAIN | `DOMAIN/Entities/` | Entités (POCOs) |

Flux de dépendances : `API → BLL → DAL → DOMAIN`

### Entités

- `ChessClub` — Clubs d'échecs
- `Player` — Joueurs (Elo, genre, club)
- `Tournament` — Tournois (lieu, fourchette Elo, réservé femmes, deadline)
- `Registration` — Inscriptions joueur/tournoi avec score
- `Match_` — Matchs (joueur blanc/noir, résultat)
- `Category` — Catégories
- `TournamentCategory` — Association tournoi/catégorie

### Technologies

- ASP.NET Core 10
- ADO.NET (pas d'ORM) avec `SqlConnection` / `SqlCommand` paramétrés
- SQL Server
- Swagger (mode Development)

### Prérequis

- .NET 10 SDK
- SQL Server
- La base de données `asp_api_ChessTournament`

### Configuration

La connection string est gérée via les **User Secrets** (ne jamais la mettre dans le code) :

```bash
cd BackEnd_ChessTournamentProject/2.Application_API_Web_Asp.Net_Core/ChessAppli

dotnet user-secrets init --project AppliChess/API.csproj

dotnet user-secrets set "CHESS_CONNECTION_STRING" "<VOTRE_CONNECTION_STRING>" --project AppliChess/API.csproj
```

### Lancer l'API

```bash
dotnet run --project AppliChess/API.csproj
```

Swagger disponible sur : `http://localhost:5083/swagger`

### Endpoints principaux

| Méthode | Route | Description |
|---------|-------|-------------|
| GET | `/api/tournament` | Liste des tournois en cours |
| GET | `/api/tournament/{id}` | Détail d'un tournoi |
| POST | `/api/tournament` | Créer un tournoi |
| PATCH | `/api/tournament/{id}/start` | Démarrer un tournoi |
| PATCH | `/api/tournament/{id}/next-round` | Passer au round suivant |
| GET | `/api/tournament/{id}/winner` | Obtenir le vainqueur |
| DELETE | `/api/tournament/{id}` | Supprimer un tournoi |
| GET | `/api/player` | Liste des joueurs |
| POST | `/api/player` | Créer un joueur |
| POST | `/api/registration` | Inscrire un joueur à un tournoi |
| GET | `/api/category` | Liste des catégories |
| PATCH | `/api/match/{id}` | Mettre à jour le résultat d'un match |

---

## Frontend — Angular 21

### Technologies

- Angular 21
- Bootstrap 5
- Server-Side Rendering (SSR)
- Lazy loading des modules

### Pages

| Route | Description |
|-------|-------------|
| `/tournaments` | Liste des tournois en cours |
| `/tournaments/:id` | Détail d'un tournoi |
| `/tournaments/create` | Créer un tournoi |
| `/players/create` | Créer un joueur |
| `/categories/create` | Créer une catégorie |
| `/chessclubs/create` | Créer un club |
| `/registrations/create` | Inscrire un joueur |
| `/matches/update` | Mettre à jour un match |
| `/registrations/scores` | Classement d'un tournoi |

### Configuration

L'URL de l'API est définie dans `src/environments/environment.ts` :

```ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5083/api'
};
```

### Lancer le frontend

```bash
cd FrontEnd_ChessTournamentProject

npm install

npm start
```

Application disponible sur : `http://localhost:4200`

---

## Lancer le projet complet

1. Démarrer l'API backend :
```bash
cd BackEnd_ChessTournamentProject/2.Application_API_Web_Asp.Net_Core/ChessAppli
dotnet run --project AppliChess/API.csproj
```

2. Démarrer le frontend Angular :
```bash
cd FrontEnd_ChessTournamentProject
npm start
```

3. Ouvrir `http://localhost:4200` dans le navigateur.
