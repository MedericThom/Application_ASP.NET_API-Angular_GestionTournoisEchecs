using DAL.Connection;
using DAL.Interfaces;
using DOMAIN.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories
{
	public class ChessClubRepository : IChessClubRepository
	{
		#region Connection
		private readonly DbConnection _db;

		public ChessClubRepository(DbConnection db)
		{
			_db = db;
		}
		#endregion

		public async Task CreateChessClubAsync(ChessClub chessclub)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			string query = "INSERT INTO ChessClub (NameChessClub) VALUES (@NameChessClub)";

			using SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@NameChessClub", chessclub.NameChessClub);

			await cmd.ExecuteNonQueryAsync();
		}
		public async Task<ChessClub> GetByIdAsync(int chessClubId)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			string query = "SELECT * FROM ChessClub WHERE ChessClub_Id = @ChessClub_Id";
			using SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@ChessClub_Id", chessClubId);

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();
			if (await reader.ReadAsync())
			{
				return new ChessClub()
				{
					ChessClub_Id  = (int)reader["ChessClub_Id"],
					NameChessClub = (string)reader["NameChessClub"]
				};
			}
			return null;
		}

		public async Task DeleteChessClubAsync(int chessClubId)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			using SqlTransaction transaction = (SqlTransaction)await conn.BeginTransactionAsync();
			try
			{
				// 1. Détacher les joueurs du club (évite la violation FK)
				string unlinkQuery = "UPDATE Player SET ChessClub_Id = NULL WHERE ChessClub_Id = @ChessClub_Id";
				using (SqlCommand cmd = new SqlCommand(unlinkQuery, conn, transaction))
				{
					cmd.Parameters.AddWithValue("@ChessClub_Id", chessClubId);
					await cmd.ExecuteNonQueryAsync();
				}

				// 2. Supprimer le club
				string deleteQuery = "DELETE FROM ChessClub WHERE ChessClub_Id = @ChessClub_Id";
				using (SqlCommand cmd = new SqlCommand(deleteQuery, conn, transaction))
				{
					cmd.Parameters.AddWithValue("@ChessClub_Id", chessClubId);
					await cmd.ExecuteNonQueryAsync();
				}

				await transaction.CommitAsync();
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		public async Task<List<ChessClub>> GetAllAsync()
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			string query = "SELECT * FROM ChessClub";
			using SqlCommand cmd = new SqlCommand(query, conn);

			List<ChessClub> chessClubs = new List<ChessClub>();
			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			while (await reader.ReadAsync())
			{
				chessClubs.Add(new ChessClub()
				{
					ChessClub_Id = (int)reader["ChessClub_Id"],
					NameChessClub = (string)reader["NameChessClub"]
				});
			}
			return chessClubs;
		}
	}
}

#region Comments
//ChessClubRepository
//        │
//        ├── _db                 →  stocke la référence à DbConnection
//        │
//        ├── Constructeur        →  reçoit et stocke DbConnection
//        │
//        └── CreateChessClubAsync()  →  appelle _db.GetConnection()
//                                        puis INSERT en base de données

//Les 5 questions dans l'ordre
//1. "De quoi ai-je besoin pour me connecter à la DB ?"
//2. "Quelle requête SQL dois-je écrire ?"
//3. "Quels paramètres SQL dois-je passer ?"
//4. "Que fait la requête ? (INSERT, SELECT, UPDATE, DELETE)"
//5. "Est ce que je retourne quelque chose ?"

//1. De quoi ai - je besoin pour me connecter ?
//	// Une SqlConnection avec ma connection string
//using SqlConnection connection = new SqlConnection(_connectionString);

//2. Quelle requête SQL ?
//	-- C'est un INSERT car je crée un club
//INSERT INTO ChessClub (NameChessClub) VALUES (@NameChessClub)

//3. Quels paramètres SQL ?
//	// J'ai besoin du nom du club
//@NameChessClub = chessclub.NameChessClub

//	4. Que fait la requête ?
//	INSERT → j'utilise ExecuteNonQueryAsync()

//	5. Est ce que je retourne quelque chose ?
//	Task → non, je retourne rien !

//	Les étapes ADO.NET dans l'ordre
//	1. Ouvrir la connexion           →  await conn.OpenAsync()
//	2. Ecrire la requête SQL
//	3. Ajouter les paramètres
//	4. Exécuter la requête           →  await cmd.ExecuteNonQueryAsync()
//	5. Fermer la connexion
#endregion
