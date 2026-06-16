import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { catchError, throwError } from "rxjs";

// HttpInterceptorFn = fonction qui intercepte toutes les requêtes HTTP
// req = la requête HTTP
// next = la prochaine étape (laisser passer la requête)
export const errorInterceptor: HttpInterceptorFn = (req, next) => {

    // pipe() = on applique des opérateurs sur l'Observable
    // catchError = on attrape les erreurs
    return next(req).pipe(
        catchError((error: HttpErrorResponse) => {

            // On choisit le message selon le code HTTP
            let errorMessage = '';

            // Message précis du backend s'il existe (disponible sur 400, 422, 500...)
            const backendMessage = error.error?.Message || error.error?.message || error.error?.title || null;

            switch(error.status) {
                case 400:
                    errorMessage = backendMessage || 'Données invalides !';
                    break;
                case 404:
                    errorMessage = backendMessage || 'Ressource introuvable !';
                    break;
                case 500:
                    errorMessage = backendMessage || 'Erreur serveur !';
                    break;
                default:
                    errorMessage = backendMessage || 'Une erreur est survenue !';
            }

            // On affiche l'erreur dans la console
            console.error(errorMessage);

            // throwError → on propage l'erreur au composant
            return throwError(() => new Error(errorMessage));
        })
    );
};