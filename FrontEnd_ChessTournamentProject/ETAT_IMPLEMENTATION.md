# État de l'implémentation — Chess Tournament Front

---

## PARTIE 1 — Ce qui est déjà implémenté

---

### Racine de l'application

#### `src/index.html`
Shell HTML minimal. Contient uniquement `<app-root>` et les balises `<head>` de base (title, charset, viewport). C'est Angular qui injecte le reste.

#### `src/main.ts`
Point d'entrée navigateur. Appelle `bootstrapApplication(App, appConfig)`. Un seul rôle : démarrer l'application côté client.

#### `src/main.server.ts`
Point d'entrée SSR. Bootstrap avec la config serveur fusionnée. Appelé par Express pour le rendu côté serveur.

#### `src/server.ts`
Serveur Express pour le SSR.
- Sert les fichiers statiques depuis le dossier `/browser`
- Délègue toutes les routes à `AngularNodeAppEngine`
- Écoute sur `process.env.PORT` (défaut : 4000)

#### `src/styles.scss`
Fichier de styles globaux. **Vide pour l'instant.** Bootstrap est chargé via `angular.json`.

---

### Configuration Angular (`src/app/`)

#### `app.config.ts`
Configuration globale de l'application Angular.
- Active le routeur (`provideRouter`)
- Active HttpClient en mode Fetch API (`provideHttpClient(withFetch())`)
- Enregistre l'`ErrorInterceptor` globalement (`withInterceptors`)

#### `app.config.server.ts`
Fusionne `appConfig` avec `serverConfig` pour le SSR. Rien à modifier ici normalement.

#### `app.routes.ts`
Routes racine de l'application.
- Chemin vide `''` → redirige vers `/tournaments`
- Chemin `tournaments` → charge le feature `tournament` en **lazy loading** via `loadChildren`

> Les routes `/players`, `/categories`, `/chessclubs` **ne sont pas encore déclarées ici**. Seul `tournaments` existe.

#### `app.routes.server.ts`
Stratégie de rendu SSR par route.
- `/tournaments/:id` → `RenderMode.Server` (rendu à la demande)
- `**` → `RenderMode.Prerender` (rendu à la build)

#### `app.ts`
Composant racine. Importe `RouterOutlet` et `Navbar`. Template minimal :
```html
<app-navbar />
<router-outlet />
```

#### `app.html`
Template du composant racine. Contient uniquement `<app-navbar />` et `<router-outlet />`.

---

### Couche Core

#### `core/interceptors/error.interceptor.ts`
Intercepteur HTTP fonctionnel (pas une classe), enregistré dans `app.config.ts`.
Mappe les codes d'erreur HTTP en messages lisibles :
- `400` → message d'erreur de données invalides
- `404` → ressource introuvable
- `500` → erreur serveur
- autre → message générique

Logue en console et relance l'erreur pour que les composants puissent la traiter.

#### `core/services/tournament/tournament.service.ts`
Service HTTP pour les tournois. 6 méthodes implémentées :
- `getAll()` → `GET /api/tournament`
- `getById(id)` → `GET /api/tournament/{id}`
- `createTournament(tournament)` → `POST /api/tournament`
- `deleteTournament(id)` → `DELETE /api/tournament/{id}`
- `startTournament(id)` → `PATCH /api/tournament/{id}/start`
- `nextRound(id)` → `PATCH /api/tournament/{id}/next-round`

> `createTournament`, `deleteTournament`, `startTournament`, `nextRound` sont écrits dans le service mais **aucun composant ne les appelle encore**.

#### `core/services/player/player.service.ts`
Service HTTP pour les joueurs.
- `getAll()` → `GET /api/player`
- `create(player)` → `POST /api/player`

> Les deux méthodes sont prêtes mais **aucun composant ne les utilise encore**.

#### `core/services/registration/registration.service.ts`
Service HTTP pour les inscriptions.
- `registerPlayer(registration)` → `POST /api/registration`
- `unregisterPlayer(playerId, tournamentId)` → `DELETE /api/registration/{playerId}/{tournamentId}`
- `getScores(tournamentId, round)` → `GET /api/registration/{tournamentId}/{round}`

> Toutes les méthodes sont prêtes mais **aucun composant ne les utilise encore**.

#### `core/services/category/category.service.ts`
Service HTTP pour les catégories.
- `createCategory(category)` → `POST /api/category`
- `getAllCategories()` → `GET /api/category`

> Prêt mais **non utilisé**.

#### `core/services/chessclub/chessclub.service.ts`
Service HTTP pour les clubs d'échecs.
- `createChessClub(chessclub)` → `POST /api/chessclub`
- `getAllChessClub()` → `GET /api/chessclub`

> Prêt mais **non utilisé**.

#### `core/services/match/match.service.ts`
Service HTTP pour les matchs.
- `updateMatch(matchId, match)` → `PATCH /api/match/{matchId}`

> Prêt mais **non utilisé**.

---

### Couche Models (interfaces TypeScript)

Toutes les interfaces sont écrites. Elles servent de contrat de typage entre le front et l'API backend.

| Fichier | Interface | Rôle |
|---|---|---|
| `models/tournament/tournament-create.interface.ts` | `TournamentCreate` | Corps du POST pour créer un tournoi |
| `models/tournament/tournament-response.interface.ts` | `TournamentResponse` | Réponse de l'API pour un tournoi (liste) |
| `models/tournament/tournament-detail.interface.ts` | `TournamentDetail` | Réponse détaillée (inclut players[] et matches[]) |
| `models/player/player-create.interface.ts` | `PlayerCreate` | Corps du POST pour créer un joueur |
| `models/player/player-response.interface.ts` | `PlayerResponse` | Réponse de l'API pour un joueur |
| `models/registration/registration-create.interface.ts` | `RegistrationCreate` | Corps du POST pour inscrire un joueur |
| `models/registration/registration-response.interface.ts` | `RegistrationResponse` | Réponse de l'API pour une inscription |
| `models/registration/score.interface.ts` | `Score` | Réponse de l'API pour les scores d'une ronde |
| `models/category/category-create.interface.ts` | `CategoryCreate` | Corps du POST pour créer une catégorie |
| `models/category/category-response.interface.ts` | `CategoryResponse` | Réponse de l'API pour une catégorie |
| `models/chessclub/chessclub-create.interface.ts` | `ChessClubCreate` | Corps du POST pour créer un club |
| `models/chessclub/chessclub-response.interface.ts` | `ChessClubResponse` | Réponse de l'API pour un club |
| `models/match/match-response.interface.ts` | `MatchResponse` | Réponse de l'API pour un match |
| `models/match/match-update.interface.ts` | `MatchUpdate` | Corps du PATCH pour mettre à jour un match |

---

### Feature : Tournament

#### `features/tournament/tournament.routes.ts`
Routing interne du feature. Deux routes :
- `''` → `TournamentList`
- `':id'` → `TournamentDetail`

#### `features/tournament/pages/tournament-list/` (3 fichiers)

**`tournament-list.ts`**
- Injecte `TournamentService` et `Router`
- 3 signals : `tournaments`, `isLoading`, `error`
- `ngOnInit()` → appelle `loadTournaments()`
- `loadTournaments()` → subscribe à `getAll()`, hydrate les signals
- `onTournamentSelected(id)` → navigue vers `/tournaments/{id}`
- Utilise la **nouvelle syntaxe `input.required` / `output`** (Angular signals API)

**`tournament-list.html`**
- Spinner Bootstrap pendant le chargement
- Alerte danger si erreur
- Grille Bootstrap 3 colonnes avec `@for` (track `tournament_Id`)
- Un `<app-tournament-card>` par tournoi
- Message "Aucun tournoi disponible" si liste vide

**`tournament-list.scss`** — vide.

---

#### `features/tournament/pages/tournament-detail/` (3 fichiers)

**`tournament-detail.ts`**
- Injecte `ActivatedRoute`, `Router`, `TournamentService`
- 3 signals : `tournament`, `isLoading`, `error`
- `ngOnInit()` → lit l'`id` dans l'URL via `route.snapshot.params['id']`, appelle `loadTournament(id)`
- `loadTournament(id)` → subscribe à `getById(id)`, hydrate le signal
- `goBack()` → navigue vers `/tournaments`
- Importe `CommonModule` (pour `@if`/`@for` en mode Angular classique)

**`tournament-detail.html`**
- Bouton "← Retour" qui appelle `goBack()`
- Spinner pendant le chargement
- Alerte danger si erreur
- Carte Bootstrap avec :
  - Infos générales : lieu, statut, ronde courante, min/max joueurs, women only
  - Tableau des joueurs inscrits : pseudo, ELO, genre
  - Tableau des matchs : whitePlayer_Id, blackPlayer_Id, résultat (ou "En cours")
- Utilise `@if` / `@for` (nouveau control flow Angular 17+)

> **Limitation actuelle** : le tableau des matchs affiche les IDs des joueurs, pas leurs pseudos. Pas de boutons d'action (start, next round, saisir résultat, inscrire/désinscrire).

**`tournament-detail.scss`** — vide.

---

#### `features/tournament/components/tournament-card/` (3 fichiers)

**`tournament-card.ts`**
- Composant réutilisable standalone
- `tournament = input.required<TournamentResponse>()` — reçoit un tournoi du parent
- `tournamentSelected = output<number>()` — émet l'ID quand on clique sur "Voir le détail"
- `onSelect()` → émet `tournament().tournament_Id`
- Importe `NgClass` pour les badges de statut

**`tournament-card.html`**
- Carte Bootstrap avec nom, lieu, badge de statut coloré, ronde actuelle, bouton "Voir le détail"
- Badge conditionnel via `[ngClass]` :
  - `'En cours'` → `bg-success` (vert)
  - `'En attente de joueurs'` → `bg-warning` (orange)
  - `'Terminé'` → `bg-secondary` (gris)

**`tournament-card.scss`** — vide.

---

### Couche Shared

#### `shared/components/navbar/` (3 fichiers)

**`navbar.ts`**
- Composant standalone
- Importe `RouterLink` et `RouterLinkActive`
- Aucune logique, que du template

**`navbar.html`**
- Navbar Bootstrap dark avec toggle mobile (hamburger)
- Logo : "Tournois d'échecs"
- 4 liens : `/tournaments`, `/players`, `/categories`, `/chessclubs`
- `routerLinkActive="active"` pour surligner la page courante

> Les liens `/players`, `/categories`, `/chessclubs` **sont dans la navbar mais n'ont pas de page derrière**.

**`navbar.scss`** — vide.

---

### Environnements

#### `src/environments/environment.ts`
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5083/api'
};
```

#### `src/environments/environments.prod.ts`
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://localhost:7108/api'
};
```

---

---

## PARTIE 2 — Ce qui reste à implémenter

---

### 1. Feature : Players (toute la feature est à créer)

**Routes à ajouter dans `app.routes.ts` :**
```
/players → PlayerList
/players/:id → PlayerDetail  (optionnel)
```

**Fichiers à créer :**

```
features/player/
├── player.routes.ts
├── pages/
│   ├── player-list/
│   │   ├── player-list.ts       ← liste tous les joueurs, GET /api/player
│   │   ├── player-list.html
│   │   └── player-list.scss
│   └── player-form/             ← formulaire de création, POST /api/player
│       ├── player-form.ts
│       ├── player-form.html
│       └── player-form.scss
└── components/
    └── player-card/             ← optionnel, carte réutilisable
        ├── player-card.ts
        ├── player-card.html
        └── player-card.scss
```

**Ce que `PlayerList` doit faire :**
- Appeler `PlayerService.getAll()` au `ngOnInit`
- Afficher la liste avec les colonnes : pseudo, ELO, genre, club
- Bouton "Ajouter un joueur" qui ouvre le formulaire ou navigue vers `/players/new`

**Ce que `PlayerForm` doit faire :**
- Formulaire avec : pseudo, email, date de naissance, genre, ELO, club (optionnel)
- Appeler `PlayerService.create(player)` à la soumission
- Utiliser `ReactiveFormsModule` (`FormGroup`, `FormControl`, `Validators`)
- Rediriger vers `/players` après succès

---

### 2. Feature : Categories (toute la feature est à créer)

**Routes à ajouter dans `app.routes.ts` :**
```
/categories → CategoryList
```

**Fichiers à créer :**

```
features/category/
├── category.routes.ts
└── pages/
    ├── category-list/
    │   ├── category-list.ts     ← GET /api/category
    │   ├── category-list.html
    │   └── category-list.scss
    └── category-form/           ← POST /api/category
        ├── category-form.ts
        ├── category-form.html
        └── category-form.scss
```

**Ce que `CategoryList` doit faire :**
- Appeler `CategoryService.getAllCategories()` au `ngOnInit`
- Afficher : nom, âge min, âge max
- Bouton "Créer une catégorie"

**Ce que `CategoryForm` doit faire :**
- Formulaire : nom de la catégorie, âge minimum, âge maximum
- Appeler `CategoryService.createCategory(category)` à la soumission

---

### 3. Feature : ChessClubs (toute la feature est à créer)

**Routes à ajouter dans `app.routes.ts` :**
```
/chessclubs → ChessClubList
```

**Fichiers à créer :**

```
features/chessclub/
├── chessclub.routes.ts
└── pages/
    ├── chessclub-list/
    │   ├── chessclub-list.ts    ← GET /api/chessclub
    │   ├── chessclub-list.html
    │   └── chessclub-list.scss
    └── chessclub-form/          ← POST /api/chessclub
        ├── chessclub-form.ts
        ├── chessclub-form.html
        └── chessclub-form.scss
```

**Ce que `ChessClubList` doit faire :**
- Appeler `ChessClubService.getAllChessClub()` au `ngOnInit`
- Afficher : nom du club
- Bouton "Créer un club"

---

### 4. Actions manquantes dans `TournamentDetail`

La page de détail affiche les données mais n'a **aucun bouton d'action**. Tout le reste est à brancher :

#### 4a. Démarrer le tournoi
- Bouton "Démarrer le tournoi" visible si `statusTournament === 'En attente de joueurs'`
- Appelle `TournamentService.startTournament(id)`
- Recharge les données après succès

#### 4b. Passer à la ronde suivante
- Bouton "Ronde suivante" visible si `statusTournament === 'En cours'`
- Appelle `TournamentService.nextRound(id)`
- Recharge les données après succès

#### 4c. Saisir le résultat d'un match
- Dans le tableau des matchs, ajouter un champ ou bouton par ligne
- Appelle `MatchService.updateMatch(matchId, { result: '1-0' | '0-1' | '0.5-0.5' })`
- Recharge les données après succès

#### 4d. Afficher les pseudos dans le tableau des matchs
- Actuellement les colonnes "Blancs" et "Noirs" affichent les IDs (`whitePlayer_Id`, `blackPlayer_Id`)
- Il faut les résoudre en pseudos en croisant avec `tournament().players`

#### 4e. Inscrire un joueur au tournoi
- Bouton ou section "Inscrire un joueur"
- Liste déroulante des joueurs existants (via `PlayerService.getAll()`)
- Appelle `RegistrationService.registerPlayer({ player_Id, tournament_Id })`
- Recharge les données après succès

#### 4f. Désinscrire un joueur
- Bouton "Désinscrire" sur chaque ligne du tableau des joueurs
- Appelle `RegistrationService.unregisterPlayer(playerId, tournamentId)`
- Recharge les données après succès

#### 4g. Afficher le score / classement de la ronde
- Appelle `RegistrationService.getScores(tournamentId, round)`
- Afficher un tableau classement : pseudo, victoires, défaites, nuls, score total

#### 4h. Supprimer un tournoi
- Bouton "Supprimer" (avec confirmation)
- Appelle `TournamentService.deleteTournament(id)`
- Redirige vers `/tournaments` après succès

---

### 5. Formulaire de création de tournoi

Le service `createTournament()` existe mais il n'y a **aucun formulaire** dans l'application.

**Fichiers à créer :**
```
features/tournament/pages/tournament-form/
├── tournament-form.ts
├── tournament-form.html
└── tournament-form.scss
```

**Route à ajouter dans `tournament.routes.ts` :**
```
'new' → TournamentForm
```

**Ce que `TournamentForm` doit faire :**
- Formulaire complet : nom, lieu, min/max joueurs, min/max ELO (optionnel), women only, deadline d'inscription, catégories
- Charger les catégories disponibles (`CategoryService.getAllCategories()`) pour une liste de cases à cocher
- Appelle `TournamentService.createTournament(tournament)`
- Redirige vers `/tournaments` après succès

---

### 6. `app.routes.server.ts` — mettre à jour les règles SSR

Quand les nouvelles features seront créées, ajouter les routes SSR appropriées pour les pages dynamiques (ex: `/players/:id` → `RenderMode.Server`).

---

### Récapitulatif visuel

```
Implémenté ✅        À faire ❌
─────────────────    ──────────────────────────────────────────
TournamentList       TournamentForm (création)
TournamentDetail*    TournamentDetail → boutons d'action
TournamentCard       TournamentDetail → résolution pseudos matchs
Navbar               TournamentDetail → inscription/désinscription
Tous les services    TournamentDetail → scores/classement
Tous les models      PlayerList
ErrorInterceptor     PlayerForm
SSR                  CategoryList
Environments         CategoryForm
                     ChessClubList
                     ChessClubForm
                     Routes app.routes.ts pour players/categories/chessclubs
```

> * `TournamentDetail` est partiellement fait : affichage en lecture seule uniquement.
```

---

### Ordre d'implémentation suggéré

1. **TournamentDetail — actions** : c'est la valeur métier la plus importante (démarrer, jouer, saisir résultats)
2. **TournamentForm** : permettre la création de tournois depuis le front
3. **PlayerList + PlayerForm** : les tournois nécessitent des joueurs existants
4. **CategoryList + CategoryForm** : nécessaire pour la création de tournois avec catégories
5. **ChessClubList + ChessClubForm** : enrichit les profils joueurs
