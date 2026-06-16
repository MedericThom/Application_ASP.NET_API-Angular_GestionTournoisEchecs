// J'importe Component pour le décorateur
// input  → nouvelle syntaxe pour recevoir des données du parent
// output → nouvelle syntaxe pour envoyer des données au parent
import { Component, input, output } from '@angular/core';
import { NgClass } from '@angular/common';
import { RouterLink } from '@angular/router';

import { TournamentResponse } from '../../../../models/tournament/tournament-response.interface';

@Component({
  selector: 'app-tournament-card',
  standalone: true,
  imports: [NgClass, RouterLink],
  templateUrl: './tournament-card.html',
  styleUrl: './tournament-card.scss',
})
export class TournamentCard {

  tournament = input.required<TournamentResponse>();

  tournamentSelected = output<number>();
  tournamentDelete = output<number>();

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('fr-BE');
  }

  onSelect(): void {
    this.tournamentSelected.emit(this.tournament().tournament_Id);
  }

  onDelete(): void {
    this.tournamentDelete.emit(this.tournament().tournament_Id);
  }
}
