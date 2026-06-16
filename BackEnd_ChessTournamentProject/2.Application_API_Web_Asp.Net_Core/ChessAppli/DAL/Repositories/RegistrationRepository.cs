using DAL.Connection;
using DAL.Interfaces;
using DOMAIN.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories
{
	public class RegistrationRepository : IRegistrationRepository
	{
		#region Connection
		private readonly DbConnection _db;
		public RegistrationRepository(DbConnection db)
		{
			_db = db;
		}
		#endregion

		public async Task CreateRegistrationAsync(Registration registration)
		{
			//Ouvrir la connexion
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = @"INSERT INTO Registration
                             (Player_Id, Tournament_Id, Wins, Losses, Draws, Score, MatchesPlayed, RegistrationDate)
                             VALUES
                             (@Player_Id, @Tournament_Id, @Wins, @Losses, @Draws, @Score, @MatchesPlayed, @RegistrationDate)
                             ";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter paramètres variables
			cmd.Parameters.AddWithValue("@Player_Id", registration.Player_Id);
			cmd.Parameters.AddWithValue("@Tournament_Id", registration.Tournament_Id);
			cmd.Parameters.AddWithValue("@Wins", registration.Wins);
			cmd.Parameters.AddWithValue("@Losses", registration.Losses);
			cmd.Parameters.AddWithValue("@Draws", registration.Draws);
			cmd.Parameters.AddWithValue("@Score", registration.Score);
			cmd.Parameters.AddWithValue("@MatchesPlayed", registration.MatchesPlayed);
			cmd.Parameters.AddWithValue("@RegistrationDate", registration.RegistrationDate);

			//Executer requete sql
			await cmd.ExecuteNonQueryAsync();
		}

		public async Task<bool> IsPlayerRegisteredAsync(int playerId, int tournamentId)
		{
			//Ouvrir la connexion et la refermer automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = @"SELECT *
                             FROM Registration
                             WHERE Player_Id = @Player_Id
                             AND Tournament_Id = @Tournament_Id";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter paramètre variable
			cmd.Parameters.AddWithValue("@Player_Id",playerId);
			cmd.Parameters.AddWithValue("@Tournament_Id", tournamentId);

			//Exécuter la reqûête sql
			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			//Créer un if (await reader.ReadAsync())
			if (await reader.ReadAsync())
			{
				return true;
			}

			return false;
		}

		public async Task<int> GetRegisteredPlayersCountAsync(int tournamentId)
		{
			//Ouvrir la connexion et la refermer automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = @"SELECT COUNT(*)
                             FROM Registration
                             WHERE Tournament_Id = @Tournament_Id";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter paramètre variable
			cmd.Parameters.AddWithValue("@Tournament_Id", tournamentId);

			//Exécuter la requête sql
			int count = (int)await cmd.ExecuteScalarAsync(); // => objet à caster !

			return count; // Le executeScalarAsync retourne directment la valeur ! (NO DataReader)
		}

		public async Task<List<Registration>> GetScoresByTournamentAsync(int tournamentId, int round)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			// Calcule les stats à la volée depuis Match_ pour les rondes 1..@Round.
			// result = 0 : nul (+0.5 chacun)
			// result = 1 : blancs gagnent (WhitePlayer_Id +1)
			// result = 2 : noirs gagnent  (BlackPlayer_Id +1)
			string query = @"
                SELECT
                    r.Player_Id,
                    r.Tournament_Id,
                    SUM(CASE
                            WHEN m.WhitePlayer_Id = r.Player_Id AND m.Result = 1 THEN 1
                            WHEN m.BlackPlayer_Id = r.Player_Id AND m.Result = 2 THEN 1
                            ELSE 0 END) AS Wins,
                    SUM(CASE
                            WHEN m.WhitePlayer_Id = r.Player_Id AND m.Result = 2 THEN 1
                            WHEN m.BlackPlayer_Id = r.Player_Id AND m.Result = 1 THEN 1
                            ELSE 0 END) AS Losses,
                    SUM(CASE
                            WHEN (m.WhitePlayer_Id = r.Player_Id OR m.BlackPlayer_Id = r.Player_Id)
                             AND m.Result = 0 THEN 1
                            ELSE 0 END) AS Draws,
                    SUM(CASE
                            WHEN m.WhitePlayer_Id = r.Player_Id AND m.Result = 1 THEN 1.0
                            WHEN m.BlackPlayer_Id = r.Player_Id AND m.Result = 2 THEN 1.0
                            WHEN (m.WhitePlayer_Id = r.Player_Id OR m.BlackPlayer_Id = r.Player_Id)
                             AND m.Result = 0 THEN 0.5
                            ELSE 0 END) AS Score,
                    SUM(CASE
                            WHEN (m.WhitePlayer_Id = r.Player_Id OR m.BlackPlayer_Id = r.Player_Id)
                             AND m.Result IS NOT NULL THEN 1
                            ELSE 0 END) AS MatchesPlayed
                FROM Registration r
                LEFT JOIN Match_ m
                       ON m.Tournament_Id = r.Tournament_Id
                      AND (m.WhitePlayer_Id = r.Player_Id OR m.BlackPlayer_Id = r.Player_Id)
                      AND m.RoundNumber <= @Round
                WHERE r.Tournament_Id = @Tournament_Id
                GROUP BY r.Player_Id, r.Tournament_Id
                ORDER BY Score DESC";

			using SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@Tournament_Id", tournamentId);
			cmd.Parameters.AddWithValue("@Round", round);

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			List<Registration> statPlayers = new List<Registration>();

			while (await reader.ReadAsync())
			{
				statPlayers.Add(new Registration()
				{
					Player_Id    = (int)reader["Player_Id"],
					Tournament_Id = (int)reader["Tournament_Id"],
					Wins         = (int)reader["Wins"],
					Losses       = (int)reader["Losses"],
					Draws        = (int)reader["Draws"],
					Score        = (decimal)reader["Score"],
					MatchesPlayed = (int)reader["MatchesPlayed"]
				});
			}

			return statPlayers;
		}

		public async Task<Registration?> GetWinnerByTournamentAsync(int tournamentId)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			// TOP 1 cumulatif sur toutes les rondes, trié par score desc
			string query = @"
                SELECT TOP 1
                    r.Player_Id,
                    r.Tournament_Id,
                    ISNULL(SUM(CASE
                        WHEN m.WhitePlayer_Id = r.Player_Id AND m.Result = 1 THEN 1.0
                        WHEN m.BlackPlayer_Id = r.Player_Id AND m.Result = 2 THEN 1.0
                        WHEN (m.WhitePlayer_Id = r.Player_Id OR m.BlackPlayer_Id = r.Player_Id)
                         AND m.Result = 0 THEN 0.5
                        ELSE 0 END), 0) AS Score
                FROM Registration r
                LEFT JOIN Match_ m
                       ON m.Tournament_Id = r.Tournament_Id
                      AND (m.WhitePlayer_Id = r.Player_Id OR m.BlackPlayer_Id = r.Player_Id)
                      AND m.Result IS NOT NULL
                WHERE r.Tournament_Id = @Tournament_Id
                GROUP BY r.Player_Id, r.Tournament_Id
                ORDER BY Score DESC";

			using SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@Tournament_Id", tournamentId);

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			if (await reader.ReadAsync())
			{
				return new Registration
				{
					Player_Id     = (int)reader["Player_Id"],
					Tournament_Id = (int)reader["Tournament_Id"],
					Score         = (decimal)reader["Score"]
				};
			}
			return null;
		}

		public async Task DeleteRegistrationAsync(int playerId, int tournamentId) //QuestionASePoser:"Pour supprimer une inscription, de quoi ai-je besoin?" réponse :"Je dois savoir QUEL joueur ET DANS QUEL tournoi ! ET ce dont j'ai besoin c'est quoi ? => int playerId ET int tournamentId"
		{
			//Ouvrir la connexion et la fermer automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = @"DELETE FROM Registration
                            WHERE Player_id = @Player_Id
                            AND   Tournament_Id = @Tournament_Id";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter paramètre variable
			cmd.Parameters.AddWithValue("@Player_id",playerId);
			cmd.Parameters.AddWithValue("@Tournament_Id", tournamentId);

			//Exécuter requête sql
			await cmd.ExecuteNonQueryAsync();
		}
	}
}
