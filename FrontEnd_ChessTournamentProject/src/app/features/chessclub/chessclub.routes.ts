import { Routes } from '@angular/router';
import { ChessclubCreate } from './pages/chessclub-create/chessclub-create';
import { ChessclubList } from './pages/chessclub-list/chessclub-list';

export const CHESSCLUB_ROUTES: Routes = [
    { path: '', redirectTo: 'create', pathMatch: 'full' },
    { path: 'create', component: ChessclubCreate },
    { path: 'list', component: ChessclubList }
];