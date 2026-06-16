import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { PlayerService } from '../../../../core/services/player/player.service';
import { ChessClubService } from '../../../../core/services/chessclub/chessclub.service';
import { ChessClubResponse } from '../../../../models/chessclub/chessclub-response.interface';

@Component({
  selector: 'app-player-create',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './player-create.html',
  styleUrl: './player-create.scss',
})
export class PlayerCreate implements OnInit {

  // FormBuilder → outil Angular pour créer des formulaires réactifs
  private fb = inject(FormBuilder);

  // PlayerService → POST /api/player
  private playerService = inject(PlayerService);
  private chessClubService = inject(ChessClubService);
  private router = inject(Router);

  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  clubs = signal<ChessClubResponse[]>([]);

  // FormGroup → le formulaire avec ses champs et validations
  playerForm: FormGroup = this.fb.group({
    // [valeur initiale, validations]
    pseudo:    ['', [Validators.required, Validators.maxLength(50)]],
    email:     ['', [Validators.required, Validators.email]],
    password:  ['', [Validators.required]],
    birthDate: ['', [Validators.required]],
    gender:    ['', [Validators.required]],
    elo:          [1200, [Validators.min(0), Validators.max(3000)]],
    chessClub_Id: [null]
  });

  ngOnInit(): void {
    this.chessClubService.getAllChessClub().subscribe({
      next: (data) => this.clubs.set(data),
      error: (err) => this.errorMessage.set(err.message)
    });
  }

  cancel(): void {
    this.router.navigate(['/tournaments']);
  }

  onSubmit(): void {
    // Si le formulaire est invalide → on arrête
    if (this.playerForm.invalid) return;

    // J'envoie les données à l'API
    this.playerService.create(this.playerForm.value).subscribe({
      next: () => {
        this.successMessage.set('Joueur créé avec succès !');
        // Retour vers la liste après 2 secondes
        setTimeout(() => this.router.navigate(['/tournaments']), 2000);
      },
      error: (err) => {
        this.errorMessage.set(err.message);
      }
    });
  }
}