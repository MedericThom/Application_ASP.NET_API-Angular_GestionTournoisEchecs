import { Routes } from '@angular/router';
import { RegistrationCreate } from './pages/registration-create/registration-create';
import { Scores } from './pages/scores/scores';

export const REGISTRATION_ROUTES: Routes = [
    { path: '', redirectTo: 'create', pathMatch: 'full' },
    { path: 'create', component: RegistrationCreate },
    { path: 'scores/:tournamentId/:round', component: Scores }
];