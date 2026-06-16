export interface TournamentCreate {
    nameTournament : string;
    place : string;
	minNbPlayer: number;
	maxNbPlayer : number;
    minElo? : number;
    maxElo? : number;
    womenOnly : boolean;
	registrationDeadline : string;
	categoryIds : number[];
    maxRounds: number;
}