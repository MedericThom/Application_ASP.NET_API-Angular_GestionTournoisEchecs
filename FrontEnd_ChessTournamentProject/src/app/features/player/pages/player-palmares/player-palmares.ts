import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { forkJoin } from 'rxjs';
import { PlayerService } from '../../../../core/services/player/player.service';
import { PlayerResponse } from '../../../../models/player/player-response.interface';
import { PlayerStats } from '../../../../models/player/player-stats.interface';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-player-palmares',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './player-palmares.html',
  styleUrl: './player-palmares.scss',
})
export class PlayerPalmares implements OnInit {

  private route = inject(ActivatedRoute);
  private location = inject(Location);
  private playerService = inject(PlayerService);

  player = signal<PlayerResponse | null>(null);
  stats = signal<PlayerStats | null>(null);
  isLoading = signal<boolean>(false);
  error = signal<string | null>(null);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.params['id']);
    this.isLoading.set(true);

    // Charge la liste complète des joueurs et les stats en parallèle
    forkJoin({
      players: this.playerService.getAll(),
      stats: this.playerService.getStats(id)
    }).subscribe({
      next: ({ players, stats }) => {
        const found = players.find(p => p.player_Id === id) ?? null;
        this.player.set(found);
        this.stats.set(stats);
        this.isLoading.set(false);
      },
      error: (err: Error) => {
        this.error.set(err.message);
        this.isLoading.set(false);
      }
    });
  }

  // Calcule l'âge à partir de la date de naissance
  getAge(birthDate: string): number {
    const today = new Date();
    const birth = new Date(birthDate);
    let age = today.getFullYear() - birth.getFullYear();
    const m = today.getMonth() - birth.getMonth();
    if (m < 0 || (m === 0 && today.getDate() < birth.getDate())) age--;
    return age;
  }

  goBack(): void {
    this.location.back();
  }
}
