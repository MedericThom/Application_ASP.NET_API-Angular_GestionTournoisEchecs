import { Routes } from '@angular/router';
import { MatchList } from './pages/match-list/match-list';
import { MatchUpdate } from './pages/match-update/match-update';

export const MATCH_ROUTES: Routes = [
    { path: '', component: MatchList },
    { path: 'update/:id', component: MatchUpdate }
];