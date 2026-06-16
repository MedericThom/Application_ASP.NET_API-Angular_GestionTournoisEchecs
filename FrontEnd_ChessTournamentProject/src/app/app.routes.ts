import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        //Redirection par défaut vers /tournaments
        //Si l'utilisateur arrive sur l'URL vide, redirige le vers /tournaments. Mais UNIQUEMENT si l'URL est exactement vide !
        path: '',
        redirectTo: 'tournaments',
        pathMatch: 'full'
    },
    {
        //Lazy loading => charge tournament only quand nécessaire
        path: 'tournaments',
        loadChildren: () => import('./features/tournament/tournament.routes')
                            .then(m => m.TOURNAMENT_ROUTES) //import() ouvre le livre/ m est le livre ouvert/ m.TOURNAMENT_ROUTES est le chapitre visé

                            /* Comments
                             loadChildren
                                → "charge les routes enfants de manière lazy"
                                → pas au démarrage, mais quand on visite /tournaments
                             () =>
                                → c'est une fonction fléchée
                                → elle s'exécute UNIQUEMENT quand on visite /tournaments

                             import('./features/tournament/tournament.routes')
                                → import dynamique
                                → charge le fichier tournament.routes.ts
                                → uniquement quand nécessaire !

                             .then(m => m.TOURNAMENT_ROUTES)
                                → import() retourne une Promise
                                → .then() = quand le fichier est chargé
                                → m = le module chargé
                                → m.TOURNAMENT_ROUTES = on prend les routes du module(donc le module c'est un fichier, le fichier c'est tournament.routes)
                            */
    },
    {
        path: 'players',
        loadChildren: () => import('./features/player/player.routes')
            .then(m => m.PLAYER_ROUTES)
    },
    {
        path: 'categories',
        loadChildren: () => import('./features/category/category.routes')
            .then(m => m.CATEGORY_ROUTES)
    },
    {
        path: 'chessclubs',
        loadChildren: () => import('./features/chessclub/chessclub.routes')
            .then(m => m.CHESSCLUB_ROUTES)
    },
    {
        path: 'registrations',
        loadChildren: () => import('./features/registration/registration.routes')
            .then(m => m.REGISTRATION_ROUTES)
    },
    {
        path: 'matches',
        loadChildren: () => import('./features/match/match.routes')
            .then(m => m.MATCH_ROUTES)
    }
];
