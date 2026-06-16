import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TournamentCreate } from '../../../models/tournament/tournament-create.interface';
import { TournamentResponse } from '../../../models/tournament/tournament-response.interface';
import { TournamentDetail } from '../../../models/tournament/tournament-detail.interface';
import { WinnerResponse } from '../../../models/tournament/winner-response.interface';

@Injectable({
    providedIn: 'root'
})

export class TournamentService {
    //Injection 
    private http = inject(HttpClient);

    //Combine URL de l'API + /tournament
    private readonly apiUrl = `${environment.apiUrl}/tournament`;
    
    //Get /api/tournament
    //Retourne un Observable de liste de tournois
    //Observable = "je te préviendrai quand j'aurai la réponse"
    getAll(): Observable<TournamentResponse[]> {
        return this.http.get<TournamentResponse[]>(this.apiUrl);
    }

    // GET /api/tournament/{id}
    // Retourne un Observable du détail d'un tournoi
    // avec joueurs + matchs
    getTournamentById(id:number) : Observable<TournamentDetail> {
        return this.http.get<TournamentDetail>(`${this.apiUrl}/${id}`);
    }

    // POST /api/tournament
    // Envoie un TournamentCreate et retourne le tournoi créé
    createTournament(tournament: TournamentCreate) : Observable<TournamentResponse> {
        return this.http.post<TournamentResponse>(this.apiUrl, tournament);
    }

    // DELETE /api/tournament/{id}
    // Supprime un tournoi, retourne void (rien)
    deleteTournament(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`)
    }

    // PATCH /api/tournament/{id}/start
    // Démarre un tournoi, retourne void (rien)
    // {} = body vide obligatoire pour PATCH
    startTournament(id: number): Observable<void> {
        return this.http.patch<void>(`${this.apiUrl}/${id}/start`, {});
    }

    // PATCH /api/tournament/{id}/next-round
    // Passe à la ronde suivante, retourne void (rien)
    nextRound(id: number): Observable<void> {
        return this.http.patch<void>(`${this.apiUrl}/${id}/next-round`, {});
    }

    // PATCH /api/tournament/{id}/deadline
    // Met à jour la date de fin des inscriptions
    updateDeadline(id: number, registrationDeadline: string): Observable<void> {
        return this.http.patch<void>(`${this.apiUrl}/${id}/deadline`, { registrationDeadline });
    }

    // GET /api/tournament/{id}/winner
    // Retourne le joueur avec le score cumulé le plus élevé
    getWinner(id: number): Observable<WinnerResponse> {
        return this.http.get<WinnerResponse>(`${this.apiUrl}/${id}/winner`);
    }
}


// nomMethode(paramètres): TypeRetour {
//     return this.http.VERBE<TypeRetour>(url);
// }