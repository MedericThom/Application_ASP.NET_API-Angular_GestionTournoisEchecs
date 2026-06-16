import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { environment } from "../../../../environments/environment";
import { RegistrationCreate } from "../../../models/registration/registration-create.interface";
import { Observable } from "rxjs";
import { RegistrationResponse } from "../../../models/registration/registration-response.interface";
import { Score } from "../../../models/registration/score.interface";

/*
 * SERVICE = est le seul endroit qui parle à l'API. Les composants ne parlent jamais directement à l'API !"
 * SERVICE => Appelle l'API && Service Donne les données (que l'API lui a données) au component
 * Le component se chargera d'afficher les données
 * AngularprovidedIn: 'root'
 * "Ce service est fourni dans toute l'application"
 * "Disponible partout, un seul exemplaire"
 * Injectable est un décorateur (@) = INSTRUCTION pour angular
 * Cette classe est injectable DONC Angular peut la donner à n'importe qui qui en a besoin
*/
@Injectable({providedIn:'root'})
export class RegistrationService{
    /* Comments
     * inject(HttpClient) → Angular me donne automatiquement
     * un HttpClient pour faire mes appels HTTP
    */
    private http = inject(HttpClient);
    /* Comments
     * Je construis l'URL de base de mon API
     * + "/registration" = "http://localhost:5083/api/registration"
    */
    private readonly apiUrl = `${environment.apiUrl}/registration`;

    /* Comments
     * POST /api/registration
     * Je reçois → un RegistrationCreate (playerId + tournamentId)
     * Je retourne → un Observable de RegistrationResponse
     * Observable = Angular me préviendra quand l'API répondra
    */
    registerPlayer(registration: RegistrationCreate): Observable<RegistrationResponse>{
        /* Comments
         * this.http.post = j'envoie des données (POST)
         * <RegistrationResponse> = ce que je reçois en retour
         * this.apiUrl = l'adresse de l'API
         * registration = ce que j'envoie dans le body
        */
        return this.http.post<RegistrationResponse>(this.apiUrl, registration);
    }

    /* Comments
     * DELETE /api/registration/{playerId}/{tournamentId}
     * Je reçois → playerId + tournamentId dans l'URL
     * Je retourne → void (rien, juste confirmation)
    */
    unregisterPlayer(player_Id: number, tournament_Id: number) : Observable<void>{
        /* Comments
         * Je construis l'URL avec les deux paramètres
         * ex: /api/registration/5/3
        */
        return this.http.delete<void>(`${this.apiUrl}/${player_Id}/${tournament_Id}`);
    }

    /* Comments
     * GET /api/registration/{tournamentId}/{round}
     * Je reçois → tournamentId + round dans l'URL
     * Je retourne → liste de scores
    */
    getScores(tournament_Id: number, round: number) : Observable<Score[]>{
        /* Comments
         * Je construis l'URL avec les deux paramètres
         * ex: /api/registration/3/2
        */
        return this.http.get<Score[]>(`${this.apiUrl}/${tournament_Id}/${round}`);
    }
}