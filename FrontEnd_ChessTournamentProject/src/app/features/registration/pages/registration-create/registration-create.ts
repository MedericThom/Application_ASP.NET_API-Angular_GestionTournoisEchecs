import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { RegistrationService } from '../../../../core/services/registration/registration.service';
import { PlayerService } from '../../../../core/services/player/player.service';
import { TournamentService } from '../../../../core/services/tournament/tournament.service';
import { PlayerResponse } from '../../../../models/player/player-response.interface';
import { TournamentResponse } from '../../../../models/tournament/tournament-response.interface';

@Component({
  selector: 'app-registration-create',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './registration-create.html',
  styleUrl: './registration-create.scss',
})
export class RegistrationCreate implements OnInit {

  private formBuilder = inject(FormBuilder);
  private registrationService = inject(RegistrationService);
  private playerService = inject(PlayerService);
  private tournamentService = inject(TournamentService);
  private route = inject(ActivatedRoute);

  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  players = signal<PlayerResponse[]>([]);
  tournaments = signal<TournamentResponse[]>([]);

  registrationForm: FormGroup = this.formBuilder.group({
    player_Id:     [null, [Validators.required]],
    tournament_Id: [null, [Validators.required]]
  });

  ngOnInit(): void {
    this.playerService.getAll().subscribe({
      next: (data) => this.players.set(data),
      error: (err) => this.errorMessage.set(err.message)
    });

    this.tournamentService.getAll().subscribe({
      next: (data) => {
        this.tournaments.set(data.filter(t => t.statusTournament !== 'Terminé'));
        const tournamentId = this.route.snapshot.queryParamMap.get('tournamentId');
        if (tournamentId) {
          this.registrationForm.patchValue({ tournament_Id: Number(tournamentId) });
        }
      },
      error: (err) => this.errorMessage.set(err.message)
    });
  }

  onSubmit(): void {
    this.registrationForm.markAllAsTouched();
    if (this.registrationForm.invalid) return;

    const selectedId = Number(this.registrationForm.value.tournament_Id);
    const selected = this.tournaments().find(t => t.tournament_Id === selectedId);
    if (selected && selected.playerCount >= selected.maxNbPlayer) {
      this.errorMessage.set('Tournoi complet !');
      setTimeout(() => this.errorMessage.set(null), 2000);
      return;
    }

    this.registrationService.registerPlayer(this.registrationForm.value).subscribe({
      next: () => {
        this.successMessage.set('Joueur inscrit avec succès !');
        setTimeout(() => this.successMessage.set(null), 2000);
        this.registrationForm.reset();
      },
      error: (err: Error) => {
        this.errorMessage.set(err.message);
        setTimeout(() => this.errorMessage.set(null), 2000);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    });
  }
}
