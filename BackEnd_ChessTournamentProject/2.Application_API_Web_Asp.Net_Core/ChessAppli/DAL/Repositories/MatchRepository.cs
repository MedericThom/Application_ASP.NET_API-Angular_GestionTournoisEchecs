using DAL.Connection;
using DAL.Interfaces;
using DOMAIN.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories
{
	public class MatchRepository : IMatchRepository
	{
		#region Connection
		private readonly DbConnection _db;

		public MatchRepository(DbConnection db)
		{
			_db = db;
		}
		#endregion

		public async Task CreateMatchesAsync(List<Match_> matches)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			string query = @"INSERT INTO Match_
                             (RoundNumber, Result, Tournament_Id, WhitePlayer_Id, BlackPlayer_Id)
                             VALUES
                             (@RoundNumber, @Result, @Tournament_Id, @WhitePlayer_Id, @BlackPlayer_Id)";

			using SqlTransaction transaction = (SqlTransaction)await conn.BeginTransactionAsync();
			try
			{
				foreach (Match_ match in matches)
				{
					using SqlCommand cmd = new SqlCommand(query, conn, transaction);
					cmd.Parameters.AddWithValue("@RoundNumber", match.RoundNumber);
					cmd.Parameters.AddWithValue("@Result", match.Result ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Tournament_Id", match.Tournament_Id);
					cmd.Parameters.AddWithValue("@WhitePlayer_Id", match.WhitePlayer_Id);
					cmd.Parameters.AddWithValue("@BlackPlayer_Id", match.BlackPlayer_Id);
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

		public async Task<Match_?> GetMatchByIdAsync(int matchId)
		{
			//Ouvrir la connexion et la fermer automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = @"SELECT *
                             FROM Match_
                             WHERE Match_Id = @Match_Id";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter paramètres variables
			cmd.Parameters.AddWithValue("@Match_Id",matchId);

			//Exécuter la requête sql
			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			//Utiliser un if avec (await reader.ReadAsync())
			if (await reader.ReadAsync())
			{
				//Créer objet
				Match_ match = new Match_()
				{
					Match_Id = (int)reader["Match_Id"],
					RoundNumber = (int)reader["RoundNumber"],
					Result = reader["Result"] == DBNull.Value ? null : (int?)reader["Result"],
					Tournament_Id = (int)reader["Tournament_Id"],
					WhitePlayer_Id = (int)reader["WhitePlayer_Id"],
					BlackPlayer_Id = (int)reader["BlackPlayer_Id"]
				};
				return match;
			}
			return null;
		}

		public async Task<List<Match_>> GetMatchesByTournamentAndRoundAsync(int tournamentId, int round)
		{
			//Ouvrir la connexion et la refermer automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = @"SELECT m.*
                             FROM Match_ m
                             WHERE tournament_Id = @tournament_Id
                             AND   roundNumber = @Round";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter paramètres variables
			cmd.Parameters.AddWithValue("@tournament_Id", tournamentId);
			cmd.Parameters.AddWithValue("@Round", round);

			//Créer liste vide
			List<Match_> matches = new List<Match_>();

			//Exécuter la commande sql
			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			//Lire ligne par ligne avec while (await reader.ReadAsync())
			while (await reader.ReadAsync())
			{
				//Construire un objet (match) pour chaque ligne
				Match_ match = new Match_
				{
					Match_Id = (int)reader["Match_Id"],
					RoundNumber = (int)reader["RoundNumber"],
					Result = reader["Result"] == DBNull.Value ? null : (int?)reader["Result"],
					Tournament_Id = (int)reader["Tournament_Id"],
					WhitePlayer_Id = (int)reader["WhitePlayer_Id"],
					BlackPlayer_Id = (int)reader["BlackPlayer_Id"]
				};
				//Ajouter chaque objet (match) à la liste
				matches.Add(match);
			};
			//Retrouner la liste (de match)
			return matches;
		}

		public async Task<List<Match_>> GetAllMatchesByTournamentAsync(int tournamentId)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			string query = @"SELECT * FROM Match_ WHERE Tournament_Id = @Tournament_Id";
			using SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@Tournament_Id", tournamentId);

			List<Match_> matches = new List<Match_>();
			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			while (await reader.ReadAsync())
			{
				matches.Add(new Match_
				{
					Match_Id       = (int)reader["Match_Id"],
					RoundNumber    = (int)reader["RoundNumber"],
					Result         = reader["Result"] == DBNull.Value ? null : (int?)reader["Result"],
					Tournament_Id  = (int)reader["Tournament_Id"],
					WhitePlayer_Id = (int)reader["WhitePlayer_Id"],
					BlackPlayer_Id = (int)reader["BlackPlayer_Id"]
				});
			}
			return matches;
		}

		public async Task UpdateMatchAsync(Match_ match)
		{
			//Ouvrir la connexion et la refermer automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = @"UPDATE Match_
                             SET    Result = @Result
                             WHERE  Match_Id = @Match_Id";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajoute les paramètres variables
			cmd.Parameters.AddWithValue("@Result",match.Result ?? (object)DBNull.Value);
			cmd.Parameters.AddWithValue("@Match_Id",match.Match_Id);

			//Exécuter la commande
			await cmd.ExecuteNonQueryAsync();
		}
	}
}
