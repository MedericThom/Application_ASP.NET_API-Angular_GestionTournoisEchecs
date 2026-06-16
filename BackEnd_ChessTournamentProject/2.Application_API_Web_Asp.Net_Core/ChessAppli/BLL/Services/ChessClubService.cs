using BLL.Interfaces;
using DAL.Interfaces;
using DOMAIN.Entities;

namespace BLL.Services
{
	public class ChessClubService : IChessClubService
	{
		#region Connexion à la DAL
		private readonly IChessClubRepository _chessClubRepository;

		public ChessClubService(IChessClubRepository chessClubRepository) //INJECTION DE DEPENDANCE => Le constructeur reçoit un IChessClubRepository en paramètre. C'est ce qu'on appelle l'injection de dépendances : on ne crée pas le Repository nous mêmes, on le reçoit de l'extérieur !
		{
			_chessClubRepository = chessClubRepository;
		}

		#region Comments
		//BLL(ChessClubService)
		//      │
		//      ├── _chessClubRepository  => Téléphone pour call la DAL (= lien vers la DAL)
		//      │
		//      └── Constructeur          =>  reçoit la DAL via injection
		#endregion

		#endregion

		public async Task CreateChessClubAsync(ChessClub chessclub)
		{
			if (string.IsNullOrEmpty(chessclub.NameChessClub))
			{
				throw new ArgumentException("Le nom du club ne peut être vide !");
			}

			await _chessClubRepository.CreateChessClubAsync(chessclub);
		}
		public async Task<List<ChessClub>> GetAllAsync()
		{
			return await _chessClubRepository.GetAllAsync();
		}

		public async Task DeleteChessClubAsync(int chessClubId)
		{
			ChessClub club = await _chessClubRepository.GetByIdAsync(chessClubId);
			if (club is null)
				throw new KeyNotFoundException($"Club {chessClubId} introuvable.");

			await _chessClubRepository.DeleteChessClubAsync(chessClubId);
		}

		#region Comments
		//		BLL
		//  throw new ArgumentException("Le nom est vide !")
		//        ↓
		//GlobalExceptionHandler
		//  1. Choisit 400 Bad Request
		//  2.Construit le JSON
		//  3.Envoie au client
		//        ↓
		//Client reçoit
		//		{
		//    "statusCode": 400,
		//    "message": "Le nom est vide !"
		//}

		//GlobalExceptionHandler  →  traducteur entre les exceptions C#
		//et les réponses HTTP JSON !
		#endregion
	}
}
