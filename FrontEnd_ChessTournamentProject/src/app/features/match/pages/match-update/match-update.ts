import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { MatchService } from '../../../../core/services/match/match.service';

@Component({
  selector: 'app-match-update',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './match-update.html',
  styleUrl: './match-update.scss',
})
export class MatchUpdate implements OnInit {

  private formBuilder = inject(FormBuilder);
  private matchService = inject(MatchService);
  private route = inject(ActivatedRoute);
  private location = inject(Location);

  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  matchId = signal<number>(0);

  matchForm: FormGroup = this.formBuilder.group({
    result: [null, [Validators.required, Validators.min(0), Validators.max(2)]]
  });

  ngOnInit(): void {
    const id = Number(this.route.snapshot.params['id']);
    this.matchId.set(id);
  }

  cancel(): void {
    this.location.back();
  }

  onSubmit(): void {
    if (this.matchForm.invalid) return;

    // J'envoie l'id du match + le résultat à l'API
    this.matchService.updateMatch(
      this.matchId(),
      this.matchForm.value
    ).subscribe({
      next: () => {
        this.location.back();
      },
      error: (err) => {
        this.errorMessage.set(err.message);
      }
    });
  }
}