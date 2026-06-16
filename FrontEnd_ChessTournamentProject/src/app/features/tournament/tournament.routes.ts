import { Routes } from '@angular/router';
import { TournamentList } from './pages/tournament-list/tournament-list';
import { TournamentDetailPage } from './pages/tournament-detail/tournament-detail';
import { TournamentCreatePage } from './pages/tournament-create/tournament-create';
import { TournamentHistory } from './pages/tournament-history/tournament-history';

export const TOURNAMENT_ROUTES: Routes = [
    { path: '', component: TournamentList },
    { path: 'create', component: TournamentCreatePage },
    { path: 'history', component: TournamentHistory },
    { path: ':id', component: TournamentDetailPage }
];