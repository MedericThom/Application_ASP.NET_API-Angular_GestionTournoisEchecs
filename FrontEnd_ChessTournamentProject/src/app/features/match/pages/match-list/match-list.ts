import { Component, inject, OnInit, signal } from '@angular/core';
import { forkJoin, Observable } from 'rxjs';
import { TournamentService } from '../../../../core/services/tournament/tournament.service';
import { MatchService } from '../../../../core/services/match/match.service';
import { MatchResponse } from '../../../../models/match/match-response.interface';
import { TournamentDetail } from '../../../../models/tournament/tournament-detail.interface';
import { PlayerResponse } from '../../../../models/player/player-response.interface';

interface RoundGroup {
  roundNumber: number;
  matches: MatchResponse[];
}

interface TournamentGroup {
  tournament: TournamentDetail;
  upcomingMatches: MatchResponse[];
  resultRounds: RoundGroup[];
  showResults: boolean;
  loadingResults: boolean;
}

@Component({
  selector: 'app-match-list',
  standalone: true,
  imports: [],
  templateUrl: './match-list.html',
  styleUrl: './match-list.scss',
})
export class MatchList implements OnInit {

  private tournamentService = inject(TournamentService);
  private matchService = inject(MatchService);

  isLoading = signal<boolean>(false);
  error = signal<string | null>(null);
  tournamentGroups = signal<TournamentGroup[]>([]);

  ngOnInit(): void {
    this.isLoading.set(true);
    this.tournamentService.getAll().subscribe({
      next: (tournaments) => {
        const active = tournaments.filter(t => t.statusTournament === 'En cours');
        if (active.length === 0) {
          this.isLoading.set(false);
          return;
        }
        const requests: Record<string, Observable<TournamentDetail>> = {};
        active.forEach(t => {
          requests[String(t.tournament_Id)] = this.tournamentService.getTournamentById(t.tournament_Id);
        });
        forkJoin(requests).subscribe({
          next: (details) => {
            const groups: TournamentGroup[] = Object.values(details).map(detail => ({
              tournament: detail,
              upcomingMatches: detail.matches.filter(m => m.result === null || m.result === undefined),
              resultRounds: [],
              showResults: false,
              loadingResults: false
            }));
            this.tournamentGroups.set(groups);
            this.isLoading.set(false);
          },
          error: (err) => {
            this.error.set(err.message);
            this.isLoading.set(false);
          }
        });
      },
      error: (err) => {
        this.error.set(err.message);
        this.isLoading.set(false);
      }
    });
  }

  toggleResults(group: TournamentGroup): void {
    if (group.showResults) {
      group.showResults = false;
      this.tournamentGroups.set([...this.tournamentGroups()]);
      return;
    }
    group.loadingResults = true;
    this.tournamentGroups.set([...this.tournamentGroups()]);

    this.matchService.getByTournament(group.tournament.tournament_Id).subscribe({
      next: (matches) => {
        const played = matches.filter(m => m.result !== null && m.result !== undefined);
        const roundNumbers = [...new Set(played.map(m => m.roundNumber))].sort((a, b) => a - b);
        group.resultRounds = roundNumbers.map(r => ({
          roundNumber: r,
          matches: played.filter(m => m.roundNumber === r)
        }));
        group.showResults = true;
        group.loadingResults = false;
        this.tournamentGroups.set([...this.tournamentGroups()]);
      },
      error: () => {
        group.loadingResults = false;
        this.tournamentGroups.set([...this.tournamentGroups()]);
      }
    });
  }

  getPlayerPseudo(players: PlayerResponse[], id: number): string {
    return players.find(p => p.player_Id === id)?.pseudo ?? `Joueur #${id}`;
  }
}
