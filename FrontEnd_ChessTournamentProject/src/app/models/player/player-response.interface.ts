export interface PlayerResponse {
    player_Id: number;
    pseudo: string;
    email: string;
    birthDate: string;
    gender: string;
    elo: number;
    chessClub_Id?: number;
}