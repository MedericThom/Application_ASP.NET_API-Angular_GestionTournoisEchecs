import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { errorInterceptor } from './core/interceptors/error.interceptor';

/*
    APP.config.ts = Carte d'identité de l'application Angular
    => elle dit à Angular ce dont l'app a besoin pour fonctionner
    Détails:
    app.config.ts   Liste de courses !
    POUR faire fonctionner l'app
    j'ai besoin de :
    → un Router (pour naviguer)
    → HttpClient (pour appeler l'API)
    → ErrorInterceptor (pour gérer les erreurs)
*/
export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
  
  /*
    Configuration HttpClient avec l'interceptor d'erreurs
    provideHttpClient()     →  active HttpClient dans toute l'app
    withInterceptors()      →  active errorInterceptor
  */
  provideHttpClient(
    withFetch(),
    withInterceptors([errorInterceptor])
  )
]
};
