import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../../../environments/environment";
import { ChessClubCreate } from "../../../models/chessclub/chessclub-create.interface";
import { Observable } from "rxjs";
import { ChessClubResponse } from "../../../models/chessclub/chessclub-response.interface";

@Injectable({providedIn:'root'})
export class ChessClubService {
    private http = inject(HttpClient);

    private readonly apiUrl = `${environment.apiUrl}/chessclub`

    createChessClub (chessclub: ChessClubCreate): Observable<ChessClubResponse>{
        return this.http.post<ChessClubResponse>(this.apiUrl, chessclub);
    }

    getAllChessClub(): Observable<ChessClubResponse[]>{
        return this.http.get<ChessClubResponse[]>(this.apiUrl);
    }

    // Supprime un club → DELETE /api/chessclub/{id}
    deleteClub(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}