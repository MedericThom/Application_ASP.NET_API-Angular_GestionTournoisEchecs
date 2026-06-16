export interface TournamentResponse {
    	tournament_Id: number;
		nameTournament: string; 
		place: string;
		minNbPlayer: number; 
		maxNbPlayer: number;
		minElo?: number;
		maxElo?: number; 
		statusTournament: string; 
		currentRound: number;
		womenOnly: boolean;
		registrationDeadline: string; 
		creationDate: string;
        updateDate: string;
        playerCount: number;
        maxRounds: number;
}