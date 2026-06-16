export interface PlayerCreate {
    pseudo: string;
    email: string;
    birthDate: string;
    gender: string;
    elo: number;
    chessClub_Id?: number;
}
