import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    path: 'tournaments/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'registrations/scores/:tournamentId/:round',
    renderMode: RenderMode.Server
  },
  {
    path: 'matches/update/:id',
    renderMode: RenderMode.Server
  },
  {
    path: '**',
    renderMode: RenderMode.Prerender
  }
];
