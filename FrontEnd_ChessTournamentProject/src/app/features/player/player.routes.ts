import { Routes } from '@angular/router';
import { PlayerCreate } from './pages/player-create/player-create';
import { PlayerPalmares } from './pages/player-palmares/player-palmares';
import { PlayerList } from './pages/player-list/player-list';

export const PLAYER_ROUTES: Routes = [
    {
        // Si /players → redirige vers /players/create
        path: '',
        redirectTo: 'create',
        pathMatch: 'full'
    },
    {
        path: 'create',
        component: PlayerCreate
    },
    {
        path: 'list',
        component: PlayerList
    },
    {
        path: ':id/palmares',
        component: PlayerPalmares
    }
];