import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CategoryService } from '../../../../core/services/category/category.service';

@Component({
  selector: 'app-category-create',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './category-create.html',
  styleUrl: './category-create.scss',
})
export class CategoryCreate {

  private formBuilder = inject(FormBuilder);
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  categoryForm: FormGroup = this.formBuilder.group({
    nameCategory: ['', [Validators.required]],
    minAge: [0, [Validators.required, Validators.min(0), Validators.max(110)]],
    maxAge: [99, [Validators.required, Validators.min(0), Validators.max(110)]]
  });

  cancel(): void {
    this.router.navigate(['/tournaments']);
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) return;
    this.categoryService.createCategory(this.categoryForm.value).subscribe({
      next: () => {
        this.successMessage.set('Catégorie créée avec succès !');
        setTimeout(() => this.router.navigate(['/tournaments']), 2000);
      },
      error: (err) => {
        this.errorMessage.set(err.message);
      }
    });
  }
}