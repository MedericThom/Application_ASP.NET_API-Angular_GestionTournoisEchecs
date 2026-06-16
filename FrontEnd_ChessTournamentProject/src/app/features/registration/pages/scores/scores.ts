import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RegistrationService } from '../../../../core/services/registration/registration.service';
import { Score } from '../../../../models/registration/score.interface';

@Component({
  selector: 'app-scores',
  standalone: true,
  imports: [],
  templateUrl: './scores.html',
  styleUrl: './scores.scss',
})
export class Scores implements OnInit {

  private registrationService = inject(RegistrationService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  // Signals pour stocker les données
  scores = signal<Score[]>([]);
  isLoading = signal<boolean>(false);
  error = signal<string | null>(null);

  // Les paramètres récupérés depuis l'URL
  tournamentId = signal<number>(0);
  round = signal<number>(0);

  ngOnInit(): void {
    // Je récupère tournamentId et round depuis l'URL
    // /registrations/scores/5/2 → tournamentId=5, round=2
    const tournamentId = Number(this.route.snapshot.params['tournamentId']);
    const round = Number(this.route.snapshot.params['round']);

    this.tournamentId.set(tournamentId);
    this.round.set(round);

    this.loadScores(tournamentId, round);
  }

  loadScores(tournamentId: number, round: number): void {
    this.isLoading.set(true);
    this.registrationService.getScores(tournamentId, round).subscribe({
      next: (data) => {
        this.scores.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.message);
        this.isLoading.set(false);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/tournaments', this.tournamentId()]);
  }
}