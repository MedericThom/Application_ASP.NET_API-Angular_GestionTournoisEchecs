import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { PlayerService } from '../../../../core/services/player/player.service';
import { PlayerResponse } from '../../../../models/player/player-response.interface';

@Component({
  selector: 'app-player-list',
  standalone: true,
  imports: [],
  templateUrl: './player-list.html',
  styleUrl: './player-list.scss',
})
export class PlayerList implements OnInit {
  private playerService = inject(PlayerService);
  private router = inject(Router);

  players = signal<PlayerResponse[]>([]);
  isLoading = signal<boolean>(false);
  error = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading.set(true);
    this.playerService.getAll().subscribe({
      next: (players) => {
        this.players.set(players);
        this.isLoading.set(false);
      },
      error: (err: Error) => {
        this.error.set(err.message);
        this.isLoading.set(false);
      }
    });
  }

  deletePlayer(player: PlayerResponse): void {
    if (!confirm(`Supprimer le joueur "${player.pseudo}" ?`)) return;

    this.playerService.delete(player.player_Id).subscribe({
      next: () => {
        this.successMessage.set(`Joueur "${player.pseudo}" supprimé.`);
        this.load();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err: Error) => this.error.set(err.message)
    });
  }

  goBack(): void {
    this.router.navigate(['/players/create']);
  }
}
