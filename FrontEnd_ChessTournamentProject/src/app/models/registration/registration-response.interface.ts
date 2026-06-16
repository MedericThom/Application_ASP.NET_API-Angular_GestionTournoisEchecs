export interface RegistrationResponse {
    registration_Id: number;
    player_Id: number;
    tournament_Id: number;
    wins: number;
    losses: number;
    draws: number;
    score: number;
    matchesPlayed: number;
    registrationDate: string;
}