import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs';
import { Injectable, inject } from "@angular/core";
import { environment } from '../../../../environments/environment';
import { PlayerResponse } from "../../../models/player/player-response.interface";
import { PlayerCreate } from "../../../models/player/player-create.interface";
import { PlayerStats } from "../../../models/player/player-stats.interface";

@Injectable({
    providedIn: 'root'
})

export class PlayerService {
    private http = inject(HttpClient);
    private readonly apiUrl = `${environment.apiUrl}/player`;

    getAll(): Observable<PlayerResponse[]> {
        return this.http.get<PlayerResponse[]>(this.apiUrl)
    }

    create(player: PlayerCreate) : Observable<PlayerResponse> {
        return this.http.post<PlayerResponse>(this.apiUrl, player);
    }

    // Met à jour le club d'un joueur → PATCH /api/player/{id}/club
    updateClub(playerId: number, chessClub_Id: number | null): Observable<PlayerResponse> {
        return this.http.patch<PlayerResponse>(`${this.apiUrl}/${playerId}/club`, { chessClub_Id });
    }

    // Récupère un joueur par son id → GET /api/player/{id}
    getById(id: number): Observable<PlayerResponse> {
        return this.http.get<PlayerResponse>(`${this.apiUrl}/${id}`);
    }

    // Récupère les statistiques d'un joueur → GET /api/player/{id}/stats
    getStats(id: number): Observable<PlayerStats> {
        return this.http.get<PlayerStats>(`${this.apiUrl}/${id}/stats`);
    }

    // Supprime un joueur → DELETE /api/player/{id}
    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}