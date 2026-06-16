using DAL.Connection;
using DAL.Interfaces;
using DOMAIN.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories
{
	public class PlayerRepository : IPlayerRepository
	{
		#region Connection
		private readonly DbConnection _db;
		public PlayerRepository(DbConnection db)
		{
			_db = db;
		}
		#endregion

		public async Task CreatePlayerAsync(Player player)
		{
			//Ouvrir connexion et la refermer automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = "INSERT INTO Player " +
						   "(Pseudo, Email, Pwd, BirthDate, Gender, Elo, ChessClub_Id) " +
						   "VALUES (@Pseudo, @Email, @Pwd, @BirthDate, @Gender, @Elo, @ChessClub_Id)";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter paramètres
			cmd.Parameters.AddWithValue("@Pseudo", player.Pseudo);
			cmd.Parameters.AddWithValue("@Email",player.Email);
			cmd.Parameters.AddWithValue("@Pwd",player.Pwd);
			cmd.Parameters.AddWithValue("@BirthDate",player.BirthDate);
			cmd.Parameters.AddWithValue("@Gender",player.Gender);
			cmd.Parameters.AddWithValue("@Elo",player.Elo);
			cmd.Parameters.AddWithValue("@ChessClub_Id",player.ChessClub_Id ?? (object)DBNull.Value);

			//Exécuter la requête sql
			await cmd.ExecuteNonQueryAsync();
		}

		public async Task<Player> GetByPlayerIdAsync(int playerId)
		{
			//Ouvrir la connexion et la fermer automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = "SELECT * " +
				           "FROM Player " +
						   "WHERE Player_Id = @Player_Id";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter les paramètres
			cmd.Parameters.AddWithValue("@Player_Id", playerId);

			//Exécuter la requête sql
			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			//Lire avec if(reader.ReadAsync())
			if (await reader.ReadAsync())
			{
				//Construire l'objet
				Player player = new Player()
				{
					Player_Id = (int)reader["Player_Id"],
					Pseudo = (string)reader["Pseudo"],
					Email = (string)reader["Email"],
					Pwd = (string)reader["Pwd"],
					BirthDate = (DateTime)reader["BirthDate"],
					Gender = (string)reader["Gender"],
					Elo = (int)reader["Elo"],
					ChessClub_Id = reader["ChessClub_Id"] == DBNull.Value ? null : (int?)reader["ChessClub_Id"]
				};
				//Retourner objet ou null si pas trouvé
				return player;
			}

			return null;
		}

		public async Task<List<Player>> GetPlayersByTournamentIdAsync(int tournamentId)
		{
			//Ouvir la connexion et la fermer automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = @"SELECT p.*
                             FROM Player p
                             JOIN Registration r ON p.Player_Id = r.Player_Id
                             WHERE r.Tournament_Id = @Tournament_Id
                            ";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter paramètre variable
			cmd.Parameters.AddWithValue("@Tournament_Id",tournamentId);

			//Creer une liste vide
			List<Player> players = new List<Player>();

			//Executer la requête avec pointeur
			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			//Lire ligne par ligne avec while
			while (await reader.ReadAsync())
			{
				//Construire un objet pour chaque ligne
				Player player = new Player()
				{
					Player_Id = (int)reader["Player_Id"],
					Pseudo = (string)reader["Pseudo"],
					Email = (string)reader["Email"],
					//Pwd = (string)reader["Pwd"],
					BirthDate = (DateTime)reader["BirthDate"],
					Gender = (string)reader["Gender"],
					Elo = (int)reader["Elo"],
					ChessClub_Id = reader["ChessClub_Id"] == DBNull.Value ? null : (int?)reader["ChessClub_Id"]
				};

				//Ajouter chaque objet à la liste
				players.Add(player);
			};
			//Retourner la liste
			return players;
		}

		public async Task<Player> GetByEmailAsync(string email)
		{
			//Ouvrir la connexion et la fermer autommatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire requête sql
			string query = @"SELECT Player_Id,
                                    Email
                             FROM Player
                             WHERE Email = @Email";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter paramètre variable
			cmd.Parameters.AddWithValue("@Email",email);

			//Exécuter requête sql
			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			//Lire avec if (reader.ReadAsync())
			if (await reader.ReadAsync())
			{
				//Construire un objet Player
				Player player = new Player()
				{
					Player_Id = (int)reader["Player_Id"],
					Email = (string)reader["Email"]
				};

				//Retourner l'objet ou null si pas trouvé
				return player;
			}

			return null;
		}

		public async Task<Player> GetByPseudoAsync(string pseudo)
		{
			//Ouvrir la connexion et la fermer autommatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire requête sql
			string query = @"SELECT Player_Id,
                                    Pseudo
                             FROM Player
                             WHERE Pseudo = @Pseudo";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter paramètre variable
			cmd.Parameters.AddWithValue("@Pseudo", pseudo);

			//Exécuter requête sql
			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			//Lire avec if (reader.ReadAsync())
			if (await reader.ReadAsync())
			{
				//Construire un objet Player
				Player player = new Player()
				{
					Player_Id = (int)reader["Player_Id"],
					Pseudo = (string)reader["Pseudo"]
				};

				//Retourner l'objet ou null si pas trouvé
				return player;
			}

			return null;
		}

		public async Task<(int Trophies, int Victories, int Draws, int Defeats)> GetStatsByPlayerIdAsync(int playerId)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			string query = @"
                WITH PlayerScores AS (
                    SELECT
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
                    GROUP BY r.Player_Id, r.Tournament_Id
                ),
                TournamentWinners AS (
                    SELECT Tournament_Id, MAX(Score) AS MaxScore
                    FROM PlayerScores
                    GROUP BY Tournament_Id
                )
                SELECT
                    (SELECT COUNT(*)
                     FROM PlayerScores ps
                     JOIN TournamentWinners tw
                       ON ps.Tournament_Id = tw.Tournament_Id AND ps.Score = tw.MaxScore
                     WHERE ps.Player_Id = @Player_Id) AS Trophies,
                    (SELECT COUNT(*) FROM Match_
                     WHERE (Result = 1 AND WhitePlayer_Id = @Player_Id)
                        OR (Result = 2 AND BlackPlayer_Id = @Player_Id)) AS Victories,
                    (SELECT COUNT(*) FROM Match_
                     WHERE Result = 0
                       AND (WhitePlayer_Id = @Player_Id OR BlackPlayer_Id = @Player_Id)) AS Draws,
                    (SELECT COUNT(*) FROM Match_
                     WHERE (Result = 2 AND WhitePlayer_Id = @Player_Id)
                        OR (Result = 1 AND BlackPlayer_Id = @Player_Id)) AS Defeats";

			using SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@Player_Id", playerId);

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();
			await reader.ReadAsync();

			return (
				Trophies:  (int)reader["Trophies"],
				Victories: (int)reader["Victories"],
				Draws:     (int)reader["Draws"],
				Defeats:   (int)reader["Defeats"]
			);
		}

		public async Task DeletePlayerAsync(int playerId)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			using SqlTransaction transaction = (SqlTransaction)await conn.BeginTransactionAsync();
			try
			{
				// 1. Détacher le joueur des matchs (contrainte FK WhitePlayer_Id / BlackPlayer_Id)
				using (SqlCommand cmd = new SqlCommand(
					"UPDATE Match_ SET WhitePlayer_Id = NULL WHERE WhitePlayer_Id = @Player_Id;" +
					"UPDATE Match_ SET BlackPlayer_Id = NULL WHERE BlackPlayer_Id = @Player_Id", conn, transaction))
				{
					cmd.Parameters.AddWithValue("@Player_Id", playerId);
					await cmd.ExecuteNonQueryAsync();
				}

				// 2. Supprimer les inscriptions du joueur (contrainte FK Registration)
				using (SqlCommand cmd = new SqlCommand("DELETE FROM Registration WHERE Player_Id = @Player_Id", conn, transaction))
				{
					cmd.Parameters.AddWithValue("@Player_Id", playerId);
					await cmd.ExecuteNonQueryAsync();
				}

				// 3. Supprimer le joueur
				using (SqlCommand cmd = new SqlCommand("DELETE FROM Player WHERE Player_Id = @Player_Id", conn, transaction))
				{
					cmd.Parameters.AddWithValue("@Player_Id", playerId);
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

		public async Task<Player> UpdateClubAsync(int playerId, int? chessClubId)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			string query = "UPDATE Player SET ChessClub_Id = @ChessClub_Id WHERE Player_Id = @Player_Id";
			using SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@ChessClub_Id", chessClubId ?? (object)DBNull.Value);
			cmd.Parameters.AddWithValue("@Player_Id", playerId);

			int rows = await cmd.ExecuteNonQueryAsync();
			if (rows == 0) return null;

			return await GetByPlayerIdAsync(playerId);
		}

		public async Task<List<Player>> GetAllAsync()
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			string query = "SELECT * FROM Player";
			using SqlCommand cmd = new SqlCommand(query, conn);

			List<Player> players = new List<Player>();
			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			while (await reader.ReadAsync())
			{
				players.Add(new Player()
				{
					Player_Id = (int)reader["Player_Id"],
					Pseudo = (string)reader["Pseudo"],
					Email = (string)reader["Email"],
					Pwd = (string)reader["Pwd"],
					BirthDate = (DateTime)reader["BirthDate"],
					Gender = (string)reader["Gender"],
					Elo = (int)reader["Elo"],
					ChessClub_Id = reader["ChessClub_Id"] == DBNull.Value ? null : (int?)reader["ChessClub_Id"]
				});
			}
			return players;
		}
	}
}
