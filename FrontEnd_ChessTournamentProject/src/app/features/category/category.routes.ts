import { Routes } from '@angular/router';
import { CategoryCreate } from './pages/category-create/category-create';
import { CategoryList } from './pages/category-list/category-list';

export const CATEGORY_ROUTES: Routes = [
    { path: '', redirectTo: 'create', pathMatch: 'full' },
    { path: 'create', component: CategoryCreate },
    { path: 'list', component: CategoryList }
];