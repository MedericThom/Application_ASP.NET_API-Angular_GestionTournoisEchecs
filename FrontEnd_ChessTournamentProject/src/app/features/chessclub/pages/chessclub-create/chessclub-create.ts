import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ChessClubService } from '../../../../core/services/chessclub/chessclub.service';

@Component({
  selector: 'app-chessclub-create',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './chessclub-create.html',
  styleUrl: './chessclub-create.scss',
})
export class ChessclubCreate {

  // inject(FormBuilder) → Angular me donne automatiquement
  // un FormBuilder pour créer des formulaires réactifs
  private formBuilder = inject(FormBuilder);

  // inject(ChessClubService) → Angular me donne automatiquement
  // le service qui fait POST /api/chessclub
  private chessClubService = inject(ChessClubService);

  // inject(Router) → Angular me donne automatiquement
  // le Router pour naviguer entre les pages
  private router = inject(Router);

  // signal<string | null>(null)
  // = variable réactive qui peut être un string ou null
  // null = pas de message au départ
  // string = message affiché quand la création réussit
  successMessage = signal<string | null>(null);

  // signal<string | null>(null)
  // null = pas d'erreur au départ
  // string = message d'erreur si l'API répond avec une erreur
  errorMessage = signal<string | null>(null);

  // FormGroup = le formulaire avec ses champs et validations
  // this.formBuilder.group() = je crée le formulaire
  // nameChessClub = le champ du formulaire
  // ['', [Validators.required]] = valeur initiale vide + validation obligatoire
  chessClubForm: FormGroup = this.formBuilder.group({
    nameChessClub: ['', [Validators.required]]
  });

  // Méthode appelée quand l'utilisateur clique sur "Annuler"
  // → navigue vers /tournaments
  cancel(): void {
    this.router.navigate(['/tournaments']);
  }

  // Méthode appelée quand l'utilisateur soumet le formulaire
  onSubmit(): void {

    // Si le formulaire est invalide → on arrête immédiatement
    // ex: nameChessClub vide → invalid → on ne fait rien !
    if (this.chessClubForm.invalid) return;

    // J'envoie les données du formulaire à l'API
    // this.chessClubForm.value = { nameChessClub: "Club de Bruxelles" }
    this.chessClubService.createChessClub(this.chessClubForm.value).subscribe({

      // next = l'API a répondu avec succès !
      next: () => {
        // J'affiche un message de succès
        this.successMessage.set('Club créé avec succès !');

        // Après 2 secondes → je navigue vers /tournaments
        // setTimeout = attendre X millisecondes avant d'exécuter
        // 2000 = 2000 millisecondes = 2 secondes
        setTimeout(() => this.router.navigate(['/tournaments']), 2000);
      },

      // error = l'API a répondu avec une erreur !
      // err = l'erreur reçue
      error: (err) => {
        // J'affiche le message d'erreur
        this.errorMessage.set(err.message);
      }
    });
  }
}