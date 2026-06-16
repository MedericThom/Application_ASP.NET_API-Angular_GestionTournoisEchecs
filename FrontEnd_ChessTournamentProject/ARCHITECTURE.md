# Architecture complète — Chess Tournament Front

## Vue d'ensemble

Application Angular **21.2.0** de gestion de tournois d'échecs.  
Architecture standalone components (Angular 17+ moderne), sans NgModule.  
Rendu SSR activé via Angular Universal + Express.

---

## Stack technique

| Élément | Version |
|---|---|
| Angular | 21.2.0 |
| TypeScript | 5.9.2 |
| RxJS | 7.8.0 |
| Bootstrap | 5.3.8 |
| Express (SSR) | 5.1.0 |
| Vitest (tests) | 4.0.8 |

Scripts npm :
- `npm start` → `ng serve` (dev)
- `npm run build` → `ng build` (prod)
- `npm run test` → `ng test` (Vitest)
- `npm run serve:ssr:chess-tournament-front` → `node dist/.../server.mjs`

---

## Structure des dossiers

```
src/
├── app/
│   ├── app.config.ts            ← config Angular globale
│   ├── app.config.server.ts     ← config SSR
│   ├── app.routes.ts            ← routes racine
│   ├── app.routes.server.ts     ← stratégie de rendu SSR par route
│   ├── app.ts                   ← composant racine
│   ├── app.html                 ← template racine
│   ├── app.scss                 ← styles racine (vide)
│   │
│   ├── core/
│   │   ├── services/
│   │   │   ├── tournament/tournament.service.ts
│   │   │   ├── player/player.service.ts
│   │   │   ├── registration/registration.service.ts
│   │   │   ├── category/category.service.ts
│   │   │   ├── chessclub/chessclub.service.ts
│   │   │   └── match/match.service.ts
│   │   └── interceptors/
│   │       └── error.interceptor.ts
│   │
│   ├── models/
│   │   ├── tournament/
│   │   │   ├── tournament-create.interface.ts
│   │   │   ├── tournament-response.interface.ts
│   │   │   └── tournament-detail.interface.ts
│   │   ├── player/
│   │   │   ├── player-create.interface.ts
│   │   │   └── player-response.interface.ts
│   │   ├── registration/
│   │   │   ├── registration-create.interface.ts
│   │   │   ├── registration-response.interface.ts
│   │   │   └── score.interface.ts
│   │   ├── category/
│   │   │   ├── category-create.interface.ts
│   │   │   └── category-response.interface.ts
│   │   ├── chessclub/
│   │   │   ├── chessclub-create.interface.ts
│   │   │   └── chessclub-response.interface.ts
│   │   └── match/
│   │       ├── match-response.interface.ts
│   │       └── match-update.interface.ts
│   │
│   ├── features/
│   │   └── tournament/
│   │       ├── tournament.routes.ts
│   │       ├── pages/
│   │       │   ├── tournament-list/
│   │       │   │   ├── tournament-list.ts
│   │       │   │   ├── tournament-list.html
│   │       │   │   └── tournament-list.scss
│   │       │   └── tournament-detail/
│   │       │       ├── tournament-detail.ts
│   │       │       ├── tournament-detail.html
│   │       │       └── tournament-detail.scss
│   │       └── components/
│   │           └── tournament-card/
│   │               ├── tournament-card.ts
│   │               ├── tournament-card.html
│   │               └── tournament-card.scss
│   │
│   └── shared/
│       └── components/
│           └── navbar/
│               ├── navbar.ts
│               ├── navbar.html
│               └── navbar.scss
│
├── environments/
│   ├── environment.ts           ← dev : apiUrl = http://localhost:5083/api
│   └── environments.prod.ts    ← prod : apiUrl = https://localhost:7108/api
│
├── main.ts                      ← bootstrap navigateur
├── main.server.ts               ← bootstrap SSR
├── server.ts                    ← serveur Express SSR
├── styles.scss                  ← styles globaux (vide)
└── index.html                   ← shell HTML (<app-root>)
```

---

## Configuration Angular (angular.json)

- Système de build : `@angular/build:application`
- Langage de styles : SCSS
- Assets : dossier `public/`
- Styles globaux : Bootstrap CSS + `src/styles.scss`
- Entry SSR : `src/server.ts`
- Output mode : `server` (SSR activé)
- Optimisation : activée en prod, désactivée en dev

---

## Point d'entrée de l'application

### app.config.ts
Fournit la configuration globale Angular :
- `provideRouter(appRoutes)` — injection du routeur
- `provideHttpClient(withFetch())` — HttpClient en mode Fetch API
- `withInterceptors([errorInterceptor])` — intercepteur d'erreurs global

### app.config.server.ts
Configuration SSR : fusionne `appConfig` + `serverConfig` Angular Universal.

### app.ts
Composant racine standalone.
- Imports : `RouterOutlet`, `Navbar`
- Template : `app.html`
- Template HTML :
  ```html
  <app-navbar />
  <router-outlet />
  ```
  La navbar est affichée sur toutes les pages ; `router-outlet` rend la page active.

---

## Routing

### app.routes.ts (routes racine)
| Chemin | Destination |
|---|---|
| `` (vide) | redirige vers `/tournaments` |
| `tournaments` | lazy-load du module feature `tournament.routes.ts` |

### app.routes.server.ts (stratégie SSR)
| Chemin | RenderMode |
|---|---|
| `/tournaments/:id` | `RenderMode.Server` |
| `**` (tout le reste) | `RenderMode.Prerender` |

### tournament.routes.ts (routes du feature)
| Chemin relatif | Composant |
|---|---|
| `` | `TournamentList` |
| `:id` | `TournamentDetail` |

---

## Couche Core

### Services

Tous les services sont `@Injectable({ providedIn: 'root' })` — singleton global.  
Tous injectent `HttpClient` via `inject(HttpClient)`.  
Tous retournent des `Observable<T>`.

#### TournamentService
Fichier : `core/services/tournament/tournament.service.ts`

| Méthode | HTTP | Endpoint |
|---|---|---|
| `getAll()` | GET | `/api/tournament` |
| `getTournamentById(id)` | GET | `/api/tournament/{id}` |
| `createTournament(tournament)` | POST | `/api/tournament` |
| `deleteTournament(id)` | DELETE | `/api/tournament/{id}` |
| `startTournament(id)` | PATCH | `/api/tournament/{id}/start` |
| `nextRound(id)` | PATCH | `/api/tournament/{id}/next-round` |

#### PlayerService
Fichier : `core/services/player/player.service.ts`

| Méthode | HTTP | Endpoint |
|---|---|---|
| `getAll()` | GET | `/api/player` |
| `create(player)` | POST | `/api/player` |

#### RegistrationService
Fichier : `core/services/registration/registration.service.ts`

| Méthode | HTTP | Endpoint |
|---|---|---|
| `registerPlayer(registration)` | POST | `/api/registration` |
| `unregisterPlayer(playerId, tournamentId)` | DELETE | `/api/registration/{playerId}/{tournamentId}` |
| `getScores(tournamentId, round)` | GET | `/api/registration/{tournamentId}/{round}` |

#### CategoryService
Fichier : `core/services/category/category.service.ts`

| Méthode | HTTP | Endpoint |
|---|---|---|
| `createCategory(category)` | POST | `/api/category` |
| `getAllCategories()` | GET | `/api/category` |

#### ChessClubService
Fichier : `core/services/chessclub/chessclub.service.ts`

| Méthode | HTTP | Endpoint |
|---|---|---|
| `createChessClub(chessclub)` | POST | `/api/chessclub` |
| `getAllChessClub()` | GET | `/api/chessclub` |

#### MatchService
Fichier : `core/services/match/match.service.ts`

| Méthode | HTTP | Endpoint |
|---|---|---|
| `updateMatch(matchId, match)` | PATCH | `/api/match/{matchId}` |

---

### Intercepteur

#### ErrorInterceptor
Fichier : `core/interceptors/error.interceptor.ts`

Intercepteur fonctionnel (non-class) enregistré dans `app.config.ts`.  
Intercepte toutes les erreurs HTTP et les mappe en messages lisibles :

| Code HTTP | Message |
|---|---|
| 400 | Données invalides |
| 404 | Ressource introuvable |
| 500 | Erreur serveur |
| autre | Message d'erreur générique |

Logue en console, relance l'erreur pour que les composants puissent la gérer.

---

## Couche Models (interfaces TypeScript)

### Tournament
```typescript
// TournamentCreate
{
  nameTournament: string;
  place: string;
  minNbPlayer: number;
  maxNbPlayer: number;
  minElo?: number;
  maxElo?: number;
  womenOnly: boolean;
  registrationDeadline: Date;
  categoryIds: number[];
}

// TournamentResponse
{
  tournament_Id: number;
  nameTournament: string;
  place: string;
  minNbPlayer: number;
  maxNbPlayer: number;
  minElo?: number;
  maxElo?: number;
  statusTournament: string;
  currentRound: number;
  womenOnly: boolean;
  registrationDeadline: Date;
  creationDate: Date;
  updateDate: Date;
}

// TournamentDetail (extends TournamentResponse)
{
  // ...tous les champs de TournamentResponse
  players: PlayerResponse[];
  matches: MatchResponse[];
}
```

### Player
```typescript
// PlayerCreate
{
  pseudo: string;
  email: string;
  birthDate: Date;
  gender: string;
  elo: number;
  chessClub_Id?: number;
}

// PlayerResponse
{
  player_Id: number;
  pseudo: string;
  email: string;
  birthDate: Date;
  gender: string;
  elo: number;
  chessClub_Id?: number;
}
```

### Registration
```typescript
// RegistrationCreate
{
  player_Id: number;
  tournament_Id: number;
}

// RegistrationResponse
{
  registration_Id: number;
  player_Id: number;
  tournament_Id: number;
  wins: number;
  losses: number;
  draws: number;
  score: number;
  matchesPlayed: number;
  registrationDate: Date;
}

// Score
{
  pseudo: string;
  wins: number;
  losses: number;
  draws: number;
  score: number;
  matchesPlayed: number;
}
```

### Category
```typescript
// CategoryCreate
{ nameCategory: string; minAge: number; maxAge: number; }

// CategoryResponse
{ category_Id: number; nameCategory: string; minAge: number; maxAge: number; }
```

### ChessClub
```typescript
// ChessClubCreate
{ nameChessClub: string; }

// ChessClubResponse
{ chessClub_Id: number; nameChessClub: string; }
```

### Match
```typescript
// MatchResponse
{
  match_Id: number;
  roundNumber: number;
  result?: string;
  tournament_Id: number;
  whitePlayer_Id: number;
  blackPlayer_Id: number;
}

// MatchUpdate
{ result: string; }
```

---

## Couche Features

### Feature : Tournament

#### TournamentList (page)
Fichier : `features/tournament/pages/tournament-list/tournament-list.ts`

Composant standalone, route `/tournaments`.

**État géré avec Signals :**
```typescript
tournaments = signal<TournamentResponse[]>([]);
isLoading   = signal<boolean>(false);
error       = signal<string | null>(null);
```

**Cycle de vie :** `ngOnInit()` → appelle `loadTournaments()`

**Méthodes :**
- `loadTournaments()` : appelle `TournamentService.getAll()`, met à jour les signals
- `onTournamentSelected(id)` : navigue vers `/tournaments/{id}`

**Template :**
- Spinner Bootstrap si `isLoading()`
- Alerte d'erreur si `error()`
- Grille Bootstrap 3 colonnes avec `@for` (track tournament_Id)
- Un `<app-tournament-card>` par tournoi
- Message "aucun tournoi" si liste vide

---

#### TournamentDetail (page)
Fichier : `features/tournament/pages/tournament-detail/tournament-detail.ts`

Composant standalone, route `/tournaments/:id`.

**État géré avec Signals :**
```typescript
tournament = signal<TournamentDetail | null>(null);
isLoading  = signal<boolean>(false);
error      = signal<string | null>(null);
```

**Cycle de vie :** `ngOnInit()` → lit le paramètre `id` de l'URL, appelle `loadTournament(id)`

**Méthodes :**
- `loadTournament(id)` : appelle `TournamentService.getTournamentById(id)`, hydrate le signal
- `goBack()` : navigue vers `/tournaments`

**Template :**
- Infos du tournoi (nom, lieu, statut, round courant, dates, etc.)
- Tableau des joueurs inscrits
- Tableau des matchs
- Utilise `@if` et `@for` (syntaxe control flow Angular 17+)

---

#### TournamentCard (composant réutilisable)
Fichier : `features/tournament/components/tournament-card/tournament-card.ts`

Composant standalone, utilisé dans `TournamentList`.

**Inputs/Outputs :**
```typescript
@Input({ required: true }) tournament: TournamentResponse;
@Output() tournamentSelected = new EventEmitter<number>(); // émet tournament_Id
```

**Badges de statut (CSS conditionnel) :**
| Statut | Couleur Bootstrap |
|---|---|
| En cours | `success` (vert) |
| En attente de joueurs | `warning` (orange) |
| Terminé | `secondary` (gris) |

**Template :** Carte Bootstrap — nom, lieu, statut, round courant. Clic → émet l'ID.

---

## Couche Shared

### Navbar
Fichier : `shared/components/navbar/navbar.ts`

Composant standalone, importé dans `App`.  
Imports : `RouterLink`, `RouterLinkActive`.

**Liens de navigation :**
| Label | Route |
|---|---|
| Tournois | `/tournaments` |
| Joueurs | `/players` |
| Catégories | `/categories` |
| Clubs d'échecs | `/chessclubs` |

Utilise `routerLinkActive="active"` pour mettre en évidence la page courante.  
Navbar Bootstrap dark avec support du toggle mobile (hamburger).

---

## Environnements

| Variable | Développement | Production |
|---|---|---|
| `production` | `false` | `true` |
| `apiUrl` | `http://localhost:5083/api` | `https://localhost:7108/api` |

Utilisés dans les services via `environment.apiUrl`.

---

## SSR — Server-Side Rendering

### server.ts (Express)
- Sert les fichiers statiques depuis le dossier `/browser`
- Utilise `AngularNodeAppEngine` pour le rendu côté serveur
- Écoute sur `process.env.PORT` (défaut : 4000)
- Toutes les routes non-statiques passent par Angular SSR

### main.server.ts
Bootstrap SSR avec la config serveur fusionnée.

---

## Patterns architecturaux clés

| Pattern | Détail |
|---|---|
| **Standalone Components** | Zéro NgModule — chaque composant déclare ses propres imports |
| **Lazy Loading** | Le feature `tournament` se charge à la demande via `loadChildren` |
| **Signals** | Gestion d'état réactive (Angular 17+) dans les composants pages |
| **Service Layer** | Un service par ressource API, singleton, retourne des Observables |
| **HTTP Interceptor** | Gestion centralisée des erreurs HTTP (fonctionnel, non-class) |
| **Control Flow** | `@if`, `@for` (nouvelle syntaxe Angular 17+, pas `*ngIf`/`*ngFor`) |
| **SSR** | Rendu serveur complet via Angular Universal + Express |
| **TypeScript strict** | Interfaces typées pour tous les contrats API |
| **Bootstrap** | UI entièrement basée sur les classes Bootstrap 5 |

---

## API Backend — Résumé des endpoints

Base URL (dev) : `http://localhost:5083/api`

| Resource | GET | POST | PATCH | DELETE |
|---|---|---|---|---|
| `/tournament` | liste tous | crée | `/tournament/{id}/start`, `/tournament/{id}/next-round` | `/tournament/{id}` |
| `/player` | liste tous | crée | — | — |
| `/registration` | `/registration/{tournamentId}/{round}` (scores) | inscrit | — | `/registration/{playerId}/{tournamentId}` |
| `/category` | liste toutes | crée | — | — |
| `/chessclub` | liste tous | crée | — | — |
| `/match` | — | — | `/match/{matchId}` | — |

---

## Ce qui n'est pas encore implémenté (routes navbar déclarées mais sans page)

Les liens de la navbar pointent vers `/players`, `/categories`, `/chessclubs` mais les routes, pages et composants correspondants **n'existent pas encore** dans le code. Seul le feature `tournament` est implémenté à ce jour.

---

## Remarques sur l'état actuel du code

- Tous les fichiers SCSS des composants sont **vides** — le style vient entièrement de Bootstrap
- `styles.scss` global est **vide**
- `app.spec.ts` a été **supprimé** (visible dans le git status)
- Aucun test n'est écrit pour l'instant malgré Vitest configuré
