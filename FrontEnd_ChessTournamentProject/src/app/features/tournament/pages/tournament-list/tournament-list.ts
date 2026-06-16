// J'importe les outils dont j'ai besoin depuis Angular
// Component → décorateur pour définir un composant
// inject    → pour injecter des services
// OnInit    → interface qui force l'implémentation de ngOnInit()
// signal    → pour créer des variables réactives
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
// J'importe mon service qui fait les appels HTTP vers /api/tournament
import { TournamentService } from '../../../../core/services/tournament/tournament.service';

// J'importe l'interface qui définit la forme d'un tournoi
// = mon TournamentResponseDTO du back end
import { TournamentResponse } from '../../../../models/tournament/tournament-response.interface';
import { TournamentCard } from '../../components/tournament-card/tournament-card';

// @Component = décorateur qui dit à Angular "cette classe est un composant"
@Component({
  // selector = le nom de la balise HTML pour utiliser ce composant
  // ex: <app-tournament-list></app-tournament-list>
  selector: 'app-tournament-list',

  // standalone = pas besoin de module Angular
  // c'est la nouvelle façon de faire Angular 17+ ✅
  standalone: true,

  // imports = les autres composants/directives utilisés dans le template
  imports: [TournamentCard, RouterLink],

  // templateUrl = le fichier HTML de ce composant
  templateUrl: './tournament-list.html',

  // styleUrl = le fichier SCSS de ce composant
  styleUrl: './tournament-list.scss',
})

// implements OnInit = je m'engage à implémenter ngOnInit()
export class TournamentList implements OnInit {

  // inject(TournamentService) = Angular me donne automatiquement
  // une instance du TournamentService
  // private = accessible uniquement dans cette classe
  private tournamentService = inject(TournamentService);
  private router = inject(Router);

  // signal<TournamentResponse[]>([])
  // = je crée une variable réactive de type liste de TournamentResponse
  // ([]) = valeur initiale = liste vide
  // quand je ferai .set(data) → Angular mettra à jour l'affichage automatiquement !
  tournaments = signal<TournamentResponse[]>([]);

  // signal<boolean>(false)
  // = variable réactive booléenne
  // false = pas en chargement au départ
  // true quand l'API est en train de répondre
  isLoading = signal<boolean>(false);

  // signal<string | null>(null)
  // = variable réactive qui peut être un string ou null
  // null = pas d'erreur au départ
  // string = message d'erreur si l'API répond avec une erreur
  error = signal<string | null>(null);

  // ngOnInit() = méthode appelée automatiquement par Angular
  // juste après la création du composant
  // c'est ici qu'on fait les appels API au chargement !
  ngOnInit(): void {
    // Je charge les tournois dès que le composant est créé
    this.loadTournaments();
  }

  loadTournaments(): void {
    // Je passe isLoading à true → l'affichage montrera un loader
    this.isLoading.set(true);

    // J'appelle mon service qui fait GET /api/tournament
    // .subscribe() = je m'abonne à l'Observable
    // "préviens moi quand l'API répond !"
    this.tournamentService.getAll().subscribe({

      // next = l'API a répondu avec succès !
      // data = la liste des tournois reçue
      next: (data) => {
        // Je stocke les tournois dans mon signal
        // → Angular met à jour l'affichage automatiquement !
        this.tournaments.set(data.filter(t => t.statusTournament !== 'Terminé'));

        // Je passe isLoading à false → le loader disparaît
        this.isLoading.set(false);
      },

      // error = l'API a répondu avec une erreur !
      // err = l'erreur reçue
      error: (err) => {
        // Je stocke le message d'erreur dans mon signal
        // → Angular affiche le message d'erreur automatiquement
        this.error.set(err.message);

        // Je passe isLoading à false → le loader disparaît
        this.isLoading.set(false);
      }
    });
  }

    onTournamentSelected(id: number): void {
    this.router.navigate(['/tournaments', id]);
  }
}
