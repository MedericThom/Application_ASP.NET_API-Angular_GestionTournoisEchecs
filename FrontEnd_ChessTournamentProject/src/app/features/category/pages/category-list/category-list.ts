import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CategoryService } from '../../../../core/services/category/category.service';
import { CategoryResponse } from '../../../../models/category/category-response.interface';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './category-list.html',
  styleUrl: './category-list.scss',
})
export class CategoryList implements OnInit {

  private categoryService = inject(CategoryService);

  categories = signal<CategoryResponse[]>([]);
  isLoading = signal<boolean>(false);
  error = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading.set(true);
    this.categoryService.getAllCategories().subscribe({
      next: (data) => {
        this.categories.set(data);
        this.isLoading.set(false);
      },
      error: (err: Error) => {
        this.error.set(err.message);
        this.isLoading.set(false);
      }
    });
  }

  deleteCategory(id: number): void {
    if (confirm('Êtes-vous sûr de vouloir supprimer cette catégorie ?')) {
      this.categoryService.deleteCategory(id).subscribe({
        next: () => {
          this.successMessage.set('Catégorie supprimée avec succès !');
          this.loadCategories();
        },
        error: (err: Error) => {
          const msg = err.message.toLowerCase();
          if (msg.includes('conflict') || msg.includes('reference') || msg.includes('foreign key') || msg.includes('fk_')) {
            this.error.set('Impossible de supprimer cette catégorie : elle est utilisée par un ou plusieurs tournois.');
          } else {
            this.error.set(err.message);
          }
          window.scrollTo({ top: 0, behavior: 'smooth' });
        }
      });
    }
  }
}
