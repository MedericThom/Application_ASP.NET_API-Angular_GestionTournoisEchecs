import { Component, inject, signal, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TournamentService } from '../../../../core/services/tournament/tournament.service';
import { CategoryService } from '../../../../core/services/category/category.service';
import { CategoryResponse } from '../../../../models/category/category-response.interface';

function minDateTodayValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const selected = new Date(control.value);
    const min = new Date();
    min.setHours(0, 0, 0, 0);
    return selected >= min ? null : { minDate: true };
  };
}

@Component({
  selector: 'app-tournament-create',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './tournament-create.html',
  styleUrl: './tournament-create.scss',
})
export class TournamentCreatePage implements OnInit {

  private fb = inject(FormBuilder);
  private tournamentService = inject(TournamentService);
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  validationError = signal<string | null>(null);

  get minDate(): string {
    return new Date().toISOString().split('T')[0];
  }

  get minDateLabel(): string {
    return new Date().toLocaleDateString('fr-BE');
  }

  // Liste des catégories disponibles chargées depuis l'API
  categories = signal<CategoryResponse[]>([]);

  tournamentForm: FormGroup = this.fb.group({
    nameTournament:       ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
    place:                ['', [Validators.required]],
    minNbPlayer:          [2, [Validators.required, Validators.min(2), Validators.max(100)]],
    maxNbPlayer:          [100, [Validators.required, Validators.min(2), Validators.max(100)]],
    minElo:               [null, [Validators.min(0), Validators.max(3000)]],
    maxElo:               [null, [Validators.min(0), Validators.max(3000)]],
    womenOnly:            [false],
    registrationDeadline: ['', [Validators.required, minDateTodayValidator()]],
    categoryIds:          [[], [Validators.required]],
    maxRounds:            [3, [Validators.required, Validators.min(1), Validators.max(20)]]
  });

  ngOnInit(): void {
    // Je charge les catégories au chargement du formulaire
    this.categoryService.getAllCategories().subscribe({
      next: (data: CategoryResponse[]) => this.categories.set(data),
      error: (err: Error) => this.errorMessage.set(err.message)
    });
  }

  // Méthode pour gérer la sélection des catégories
  onCategoryChange(categoryId: number, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    const currentIds: number[] = this.tournamentForm.get('categoryIds')?.value || [];

    if (checked) {
      // Ajouter l'id si coché
      this.tournamentForm.patchValue({ categoryIds: [...currentIds, categoryId] });
    } else {
      // Retirer l'id si décoché
      this.tournamentForm.patchValue({ categoryIds: currentIds.filter(id => id !== categoryId) });
    }
  }

  cancel(): void {
    this.router.navigate(['/tournaments']);
  }

  onSubmit(): void {
    this.tournamentForm.markAllAsTouched();
    this.validationError.set(null);

    if (this.tournamentForm.invalid) {
      const errors: string[] = [];
      const f = this.tournamentForm;

      if (f.get('nameTournament')?.invalid) errors.push('Le nom du tournoi est invalide (3-50 caractères)');
      if (f.get('place')?.invalid)          errors.push('Le lieu est obligatoire');
      if (f.get('minElo')?.invalid)         errors.push('ELO minimum doit être entre 0 et 3000');
      if (f.get('maxElo')?.invalid)         errors.push('ELO maximum doit être entre 0 et 3000');
      if (f.get('registrationDeadline')?.errors?.['required']) errors.push('La date de fin des inscriptions est obligatoire');
      if (f.get('registrationDeadline')?.errors?.['minDate'])  errors.push(`La date doit être à partir du ${this.minDateLabel}`);
      if (f.get('categoryIds')?.value?.length === 0)           errors.push('Sélectionnez au moins une catégorie');
      if (f.get('maxRounds')?.invalid)                         errors.push('Le nombre de rondes maximum doit être entre 1 et 20');

      this.validationError.set(errors.join(' • '));
      window.scrollTo({ top: 0, behavior: 'smooth' });
      return;
    }

    this.tournamentService.createTournament(this.tournamentForm.value).subscribe({
      next: () => {
        this.successMessage.set('Tournoi créé avec succès !');
        setTimeout(() => this.router.navigate(['/tournaments']), 2000);
      },
      error: (err: Error) => {
        this.errorMessage.set(err.message);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    });
  }
}
