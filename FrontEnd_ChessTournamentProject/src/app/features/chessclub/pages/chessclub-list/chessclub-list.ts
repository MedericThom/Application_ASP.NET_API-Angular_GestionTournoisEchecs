import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { forkJoin, Observable } from 'rxjs';
import { ChessClubService } from '../../../../core/services/chessclub/chessclub.service';
import { PlayerService } from '../../../../core/services/player/player.service';
import { ChessClubResponse } from '../../../../models/chessclub/chessclub-response.interface';
import { PlayerResponse } from '../../../../models/player/player-response.interface';
import { PlayerStats } from '../../../../models/player/player-stats.interface';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chessclub-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './chessclub-list.html',
  styleUrl: './chessclub-list.scss',
})
export class ChessclubList implements OnInit {

  private chessClubService = inject(ChessClubService);
  private playerService = inject(PlayerService);
  private router = inject(Router);

  clubs = signal<ChessClubResponse[]>([]);
  players = signal<PlayerResponse[]>([]);
  isLoading = signal<boolean>(false);
  error = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  // Id du joueur dont le select de club est ouvert
  activePlayerId = signal<number | null>(null);
  // Affiche/cache le select de suppression de club
  showDeleteSelect = signal<boolean>(false);
  // Stats par joueur (victories, defeats, draws)
  playerStats = signal<Map<number, PlayerStats>>(new Map());

  ngOnInit(): void {
    this.isLoading.set(true);
    this.error.set(null);

    // Charge les clubs et les joueurs en parallèle
    forkJoin({
      clubs: this.chessClubService.getAllChessClub(),
      players: this.playerService.getAll()
    }).subscribe({
      next: ({ clubs, players }) => {
        this.clubs.set(clubs);
        this.players.set(players);

        if (players.length === 0) {
          this.isLoading.set(false);
          return;
        }

        // Charge les stats de chaque joueur en parallèle
        const statsRequests: Record<string, Observable<PlayerStats>> = {};
        players.forEach(p => {
          statsRequests[String(p.player_Id)] = this.playerService.getStats(p.player_Id);
        });

        forkJoin(statsRequests).subscribe({
          next: (results) => {
            const map = new Map<number, PlayerStats>();
            Object.entries(results).forEach(([id, stats]) => {
              map.set(Number(id), stats);
            });
            this.playerStats.set(map);
            this.isLoading.set(false);
          },
          error: () => { this.isLoading.set(false); }
        });
      },
      error: (err: Error) => {
        this.error.set(err.message);
        this.isLoading.set(false);
      }
    });
  }

  // Retourne les stats d'un joueur ou null si non chargées
  getPlayerStats(playerId: number): PlayerStats | null {
    return this.playerStats().get(playerId) ?? null;
  }

  // Retourne les joueurs appartenant à un club donné
  getPlayersForClub(clubId: number): PlayerResponse[] {
    return this.players().filter(p => p.chessClub_Id === clubId);
  }

  // Retourne les joueurs sans club
  getPlayersWithoutClub(): PlayerResponse[] {
    return this.players().filter(p => !p.chessClub_Id);
  }

  // Affiche/cache le select de club pour un joueur
  toggleClubSelect(playerId: number): void {
    this.activePlayerId.set(this.activePlayerId() === playerId ? null : playerId);
  }

  // Retire un joueur de son club
  leaveClub(playerId: number): void {
    this.playerService.updateClub(playerId, null).subscribe({
      next: () => {
        this.successMessage.set('Joueur retiré du club.');
        this.ngOnInit();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err: Error) => this.error.set(err.message)
    });
  }

  // Assigne un club au joueur (null = retirer du club)
  assignClub(playerId: number, event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    const clubId = value === '' ? null : Number(value);

    this.playerService.updateClub(playerId, clubId).subscribe({
      next: () => {
        this.successMessage.set('Club mis à jour !');
        this.activePlayerId.set(null);
        // Recharge pour refléter le changement
        this.ngOnInit();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err: Error) => {
        this.error.set(err.message);
      }
    });
  }

  // Supprime le club sélectionné dans le dropdown
  onDeleteClubSelect(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    if (!value) return;

    const clubId = Number(value);
    const club = this.clubs().find(c => c.chessClub_Id === clubId);
    if (!club) return;

    if (!confirm(`Supprimer le club "${club.nameChessClub}" ? Les joueurs seront déplacés dans "Sans club".`)) {
      (event.target as HTMLSelectElement).value = '';
      return;
    }

    this.chessClubService.deleteClub(clubId).subscribe({
      next: () => {
        this.successMessage.set(`Club "${club.nameChessClub}" supprimé.`);
        this.showDeleteSelect.set(false);
        this.ngOnInit();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err: Error) => {
        this.error.set(err.message);
        (event.target as HTMLSelectElement).value = '';
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/chessclubs/create']);
  }
}
