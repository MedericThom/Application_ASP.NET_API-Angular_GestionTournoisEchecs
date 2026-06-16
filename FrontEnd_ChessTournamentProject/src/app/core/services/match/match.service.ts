import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../../../environments/environment";
import { Observable } from "rxjs";
import { MatchUpdate } from "../../../models/match/match-update.interface";
import { MatchResponse } from "../../../models/match/match-response.interface";

@Injectable({providedIn:'root'})
export class MatchService {
    private http = inject(HttpClient)

    private readonly apiUrl = `${environment.apiUrl}/match`;

    getAll(): Observable<MatchResponse[]> {
        return this.http.get<MatchResponse[]>(this.apiUrl);
    }

    getByTournament(tournamentId: number): Observable<MatchResponse[]> {
        return this.http.get<MatchResponse[]>(`${this.apiUrl}/tournament/${tournamentId}`);
    }

    updateMatch(match_Id: number, match: MatchUpdate) : Observable<void> {
        return this.http.patch<void>(`${this.apiUrl}/${match_Id}`, match);
    }
}