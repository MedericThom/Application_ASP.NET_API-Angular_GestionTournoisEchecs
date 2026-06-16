import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TournamentService } from '../../../../core/services/tournament/tournament.service';
import { RegistrationService } from '../../../../core/services/registration/registration.service';
import { ChessClubService } from '../../../../core/services/chessclub/chessclub.service';
import { MatchService } from '../../../../core/services/match/match.service';
import { TournamentDetail as TournamentDetailInterface } from '../../../../models/tournament/tournament-detail.interface';
import { WinnerResponse } from '../../../../models/tournament/winner-response.interface';
import { ChessClubResponse } from '../../../../models/chessclub/chessclub-response.interface';
import { MatchResponse } from '../../../../models/match/match-response.interface';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-tournament-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tournament-detail.html',
  styleUrl: './tournament-detail.scss',
})
export class TournamentDetailPage implements OnInit {

  // ActivatedRoute → lit l'id dans l'URL (/tournaments/5)
  private route = inject(ActivatedRoute);

  // Router → pour naviguer
  private router = inject(Router);

  // TournamentService → GET /api/tournament/{id}
  private tournamentService = inject(TournamentService);
  private registrationService = inject(RegistrationService);
  private chessClubService = inject(ChessClubService);
  private matchService = inject(MatchService);

  // Signal → stocke le détail du tournoi
  // null au départ car pas encore chargé
  tournament = signal<TournamentDetailInterface | null>(null);
  clubs = signal<ChessClubResponse[]>([]);

  isLoading = signal<boolean>(false);
  error = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  showDeadlineForm = signal<boolean>(false);
  winner = signal<WinnerResponse | null>(null);
  selectedPlayerId = signal<number | null>(null);
  playerMatches = signal<MatchResponse[]>([]);
  loadingPlayerResults = signal<boolean>(false);

  get minDeadlineDate(): string {
    return new Date().toISOString().split('T')[0];
  }

  currentRoundMatches() {
    const t = this.tournament();
    if (!t) return [];
    return t.matches.filter(m => m.roundNumber === t.currentRound);
  }

  getPlayerPseudo(playerId: number): string {
    const player = this.tournament()!.players.find(p => p.player_Id === playerId);
    return player ? player.pseudo : `Joueur #${playerId}`;
  }

  getResultLabel(match: { result?: number; whitePlayer_Id: number; blackPlayer_Id: number }): string {
    if (match.result === null || match.result === undefined) return 'En cours';
    if (match.result === 0) return 'Match nul';
    if (match.result === 1) return `${this.getPlayerPseudo(match.whitePlayer_Id)} a gagné`;
    if (match.result === 2) return `${this.getPlayerPseudo(match.blackPlayer_Id)} a gagné`;
    return 'En cours';
  }

  getClubName(chessClub_Id?: number): string {
    if (!chessClub_Id) return '—';
    const club = this.clubs().find(c => c.chessClub_Id === chessClub_Id);
    return club ? club.nameChessClub : '—';
  }

  ngOnInit(): void {
    // Je récupère l'id depuis l'URL
    // /tournaments/5 → id = 5
    const id = Number(this.route.snapshot.params['id']);
    this.loadTournament(id);
    this.chessClubService.getAllChessClub().subscribe({
      next: (clubs) => this.clubs.set(clubs),
      error: () => {}
    });
  }

  loadTournament(id: number): void {
    this.isLoading.set(true);
    this.tournamentService.getTournamentById(id).subscribe({
      next: (data) => {
        this.tournament.set(data);
        this.isLoading.set(false);
        if (data.statusTournament === 'Terminé') {
          this.loadWinner(id);
        }
      },
      error: (err : Error) => {
        this.error.set(err.message);
        this.isLoading.set(false);
      }
    });
  }

  loadWinner(id: number): void {
    this.tournamentService.getWinner(id).subscribe({
      next: (data) => this.winner.set(data),
      error: () => this.winner.set(null)
    });
  }

  // Démarrer le tournoi → PATCH /api/tournament/{id}/start
  startTournament(): void {
    this.tournamentService.startTournament(this.tournament()!.tournament_Id).subscribe({
      next: () => {
        this.successMessage.set('Tournoi démarré !');
        // Recharge le tournoi pour voir les changements
        this.loadTournament(this.tournament()!.tournament_Id);
      },
      error: (err) => this.error.set(err.message)
    });
  }

  // Passer à la ronde suivante → PATCH /api/tournament/{id}/next-round
  nextRound(): void {
    this.tournamentService.nextRound(this.tournament()!.tournament_Id).subscribe({
      next: () => {
        this.successMessage.set('Ronde suivante !');
        this.loadTournament(this.tournament()!.tournament_Id);
      },
      error: (err: Error) => {
        const msg = err.message.toLowerCase();
        if (msg.includes('unique') || msg.includes('duplicate')) {
          this.error.set('Impossible de générer la ronde suivante : des appariements en double ont été détectés. Contactez l\'administrateur.');
        } else {
          this.error.set(err.message);
        }
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    });
  }

  // Voir les scores → /registrations/scores/{tournamentId}/{round}
  viewScores(): void {
    this.router.navigate([
      '/registrations/scores',
      this.tournament()!.tournament_Id,
      this.tournament()!.currentRound
    ]);
  }

  // Mettre à jour la date limite → PATCH /api/tournament/{id}/deadline
  updateDeadline(value: string): void {
    if (!value) return;
    this.tournamentService.updateDeadline(this.tournament()!.tournament_Id, value).subscribe({
      next: () => {
        this.successMessage.set('Date de fin des inscriptions mise à jour !');
        this.showDeadlineForm.set(false);
        this.loadTournament(this.tournament()!.tournament_Id);
      },
      error: (err: Error) => {
        this.error.set(err.message);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    });
  }

  // Désinscrire un joueur → DELETE /api/registration/{playerId}/{tournamentId}
  unregisterPlayer(playerId: number): void {
    if (confirm('Êtes-vous sûr de vouloir désinscrire ce joueur ?')) {
      this.registrationService.unregisterPlayer(playerId, this.tournament()!.tournament_Id).subscribe({
        next: () => {
          this.successMessage.set('Joueur désinscrit avec succès !');
          this.loadTournament(this.tournament()!.tournament_Id);
        },
        error: (err: Error) => {
          this.error.set(err.message);
          window.scrollTo({ top: 0, behavior: 'smooth' });
        }
      });
    }
  }

  // Modifier un match → /matches/update/{matchId}
  updateMatch(matchId: number): void {
    this.router.navigate(['/matches/update', matchId]);
  }

  // Supprimer le tournoi → DELETE /api/tournament/{id}
  deleteTournament(): void {
    if (confirm('Êtes-vous sûr de vouloir supprimer ce tournoi ?')) {
      this.tournamentService.deleteTournament(this.tournament()!.tournament_Id).subscribe({
        next: () => {
          this.router.navigate(['/tournaments']);
        },
        error: (err: Error) => {
          const msg = err.message.toLowerCase();
          if (msg.includes('conflict') || msg.includes('reference') || msg.includes('fk_') || msg.includes('foreign key')) {
            this.error.set('Impossible de supprimer ce tournoi : il contient des données liées (catégories, inscriptions ou matchs). Supprimez-les d\'abord.');
          } else {
            this.error.set(err.message);
          }
          window.scrollTo({ top: 0, behavior: 'smooth' });
        }
      });
    }
  }

  togglePlayerResults(playerId: number): void {
    if (this.selectedPlayerId() === playerId) {
      this.selectedPlayerId.set(null);
      this.playerMatches.set([]);
      return;
    }
    this.selectedPlayerId.set(playerId);
    this.loadingPlayerResults.set(true);
    this.matchService.getByTournament(this.tournament()!.tournament_Id).subscribe({
      next: (matches) => {
        this.playerMatches.set(
          matches
            .filter(m => (m.whitePlayer_Id === playerId || m.blackPlayer_Id === playerId) && m.result !== null && m.result !== undefined)
            .sort((a, b) => a.roundNumber - b.roundNumber)
        );
        this.loadingPlayerResults.set(false);
      },
      error: () => this.loadingPlayerResults.set(false)
    });
  }

  getOpponentPseudo(match: MatchResponse, playerId: number): string {
    const opponentId = match.whitePlayer_Id === playerId ? match.blackPlayer_Id : match.whitePlayer_Id;
    return this.getPlayerPseudo(opponentId);
  }

  getPlayerMatchResult(match: MatchResponse, playerId: number): string {
    if (match.result === 0) return 'Match nul';
    if (match.result === 1 && match.whitePlayer_Id === playerId) return '✅ Victoire';
    if (match.result === 2 && match.blackPlayer_Id === playerId) return '✅ Victoire';
    return '❌ Défaite';
  }

  // Retour vers la liste des tournois
  goBack(): void {
    this.router.navigate(['/tournaments']);
  }
}
