import { PlayerResponse } from '../player/player-response.interface';
import { MatchResponse } from '../match/match-response.interface';

export interface TournamentDetail {
    	tournament_Id: number;
		nameTournament: string;
		place: string;
		minNbPlayer: number;
		maxNbPlayer: number; 
		minElo?: number;
		maxElo?: number;
		statusTournament: string;
		currentRound: number;
		maxRounds: number;
		womenOnly: boolean;
		registrationDeadline: string;
		creationDate: string; 
		updateDate: string;
        players: PlayerResponse[];
		matches: MatchResponse[];
}