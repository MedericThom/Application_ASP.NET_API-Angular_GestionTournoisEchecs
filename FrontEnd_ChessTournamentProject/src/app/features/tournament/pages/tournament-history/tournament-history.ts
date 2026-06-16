import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { TournamentService } from '../../../../core/services/tournament/tournament.service';
import { TournamentResponse } from '../../../../models/tournament/tournament-response.interface';
import { TournamentCard } from '../../components/tournament-card/tournament-card';

@Component({
  selector: 'app-tournament-history',
  standalone: true,
  imports: [TournamentCard, RouterLink],
  templateUrl: './tournament-history.html',
  styleUrl: './tournament-history.scss',
})
export class TournamentHistory implements OnInit {

  private tournamentService = inject(TournamentService);
  private router = inject(Router);

  tournaments = signal<TournamentResponse[]>([]);
  isLoading = signal<boolean>(false);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.isLoading.set(true);
    this.tournamentService.getAll().subscribe({
      next: (data) => {
        this.tournaments.set(data.filter(t => t.statusTournament === 'Terminé'));
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.message);
        this.isLoading.set(false);
      }
    });
  }

  onTournamentSelected(id: number): void {
    this.router.navigate(['/tournaments', id]);
  }

  onDeleteTournament(id: number): void {
    if (!confirm('Supprimer ce tournoi ?')) return;
    this.tournamentService.deleteTournament(id).subscribe({
      next: () => {
        this.tournaments.set(this.tournaments().filter(t => t.tournament_Id !== id));
      },
      error: (err) => this.error.set(err.message)
    });
  }
}
