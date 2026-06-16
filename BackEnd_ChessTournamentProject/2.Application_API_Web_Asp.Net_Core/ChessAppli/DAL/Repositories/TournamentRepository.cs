using DAL.Connection;
using DAL.Interfaces;
using DOMAIN.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories
{
	public class TournamentRepository : ITournamentRepository
	{
		#region Connection
		private readonly DbConnection _db;

		public TournamentRepository(DbConnection db)
		{
			_db = db;
		}
		#endregion

		public async Task CreateTournamentAsync(Tournament tournament)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			string query = "INSERT INTO Tournament " +
				"(NameTournament, Place, MinNbPlayer, MaxNbPlayer, MinElo, MaxElo, StatusTournament, CurrentRound, WomenOnly, MaxRounds, RegistrationDeadline, CreationDate, UpdateDate) " +
				"OUTPUT INSERTED.Tournament_Id " +
				"VALUES (@NameTournament, @Place, @MinNbPlayer, @MaxNbPlayer, @MinElo, @MaxElo, @StatusTournament, @CurrentRound, @WomenOnly, @MaxRounds, @RegistrationDeadline, @CreationDate, @UpdateDate)";
			using SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@NameTournament", tournament.NameTournament);
			cmd.Parameters.AddWithValue("@Place", tournament.Place);
			cmd.Parameters.AddWithValue("@MinNbPlayer", tournament.MinNbPlayer);
			cmd.Parameters.AddWithValue("@MaxNbPlayer", tournament.MaxNbPlayer);
			cmd.Parameters.AddWithValue("@MinElo", tournament.MinElo ?? (object)DBNull.Value);
			cmd.Parameters.AddWithValue("@MaxElo", tournament.MaxElo ?? (object)DBNull.Value);
			cmd.Parameters.AddWithValue("@StatusTournament", tournament.StatusTournament);
			cmd.Parameters.AddWithValue("@CurrentRound", tournament.CurrentRound);
			cmd.Parameters.AddWithValue("@WomenOnly", tournament.WomenOnly);
			cmd.Parameters.AddWithValue("@MaxRounds", tournament.MaxRounds);
			cmd.Parameters.AddWithValue("@RegistrationDeadline", tournament.RegistrationDeadline);
			cmd.Parameters.AddWithValue("@CreationDate", tournament.CreationDate);
			cmd.Parameters.AddWithValue("@UpdateDate", tournament.UpdateDate);

			tournament.Tournament_Id = (int)await cmd.ExecuteScalarAsync();
		}

		public async Task AddCategoriesToTournamentAsync(int tournamentId, List<int> categoryIds)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			foreach (int categoryId in categoryIds)
			{
				string query = @"INSERT INTO TournamentCategory
                         (Tournament_Id, Category_Id)
                         VALUES (@Tournament_Id, @Category_Id)";

				using SqlCommand cmd = new SqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@Tournament_Id", tournamentId);
				cmd.Parameters.AddWithValue("@Category_Id", categoryId);
				await cmd.ExecuteNonQueryAsync();
			}
		}

		public async Task<List<Tournament>> GetLastTenTournamentInProgressAsync()
		{
			//Ouvrir connexion et la fermer automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = @"SELECT TOP 10 t.*,
                                (SELECT COUNT(*) FROM Registration r WHERE r.Tournament_Id = t.Tournament_Id) AS PlayerCount
                             FROM Tournament t
                             WHERE t.StatusTournament != 'Clôturé'
                             ORDER BY t.UpdateDate DESC";
			using SqlCommand cmd = new SqlCommand(query, conn);

            //Créer liste vide pour stocker les résultats
			List<Tournament> tournaments = new List<Tournament>();

			//Executer la requête sql avec ExecuteReaderAsync
			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			//Lire les résultats ligne par ligne avec reader.ReadAsync()
			while (await reader.ReadAsync())
			{
				//Construire un objet pour chaque ligne
				Tournament tournament = new Tournament()
				{
					Tournament_Id = (int)reader["Tournament_Id"],
					NameTournament = (string)reader["NameTournament"],
					Place = (string)reader["Place"],
					MinNbPlayer = (int)reader["MinNbPlayer"],
					MaxNbPlayer = (int)reader["MaxNbPlayer"],
					MinElo = reader["MinElo"] == DBNull.Value ? null : (int?)reader["MinElo"],
					MaxElo = reader["MaxElo"] == DBNull.Value ? null : (int?)reader["MaxElo"],
					StatusTournament = (string)reader["StatusTournament"],
					CurrentRound = (int)reader["CurrentRound"],
					WomenOnly = (bool)reader["WomenOnly"],
					MaxRounds = (int)reader["MaxRounds"],
					RegistrationDeadline = (DateTime)reader["RegistrationDeadline"],
					CreationDate = (DateTime)reader["CreationDate"],
					UpdateDate = (DateTime)reader["UpdateDate"],
					PlayerCount = (int)reader["PlayerCount"]
				};

				//Ajouter chaque objet à la liste
				tournaments.Add(tournament);
			};

			//Retourner la liste
			return tournaments;
		}

		#region Comments
		// SqlDataReader = curseur qui lit les résultats ligne par ligne
		// ExecuteReaderAsync = envoie le SELECT à SQL Server et retourne les résultats
		// using = ferme et libère le reader automatiquement
		// EN 1 ligne =>>> Lit les résultats ligne par ligne depuis SQL Server

		//cmd = Abréviation de SqlCommand. C'est le messager entre ton code C# et SQL Server !

		//├── ExecuteNonQueryAsync  →  INSERT, UPDATE, DELETE (pas de retour)
		//├── ExecuteReaderAsync    →  SELECT (retourne un SqlDataReader)
		//├── ExecuteScalarAsync    →  SELECT COUNT(*) (retourne une seule valeur)
		//├── reader.ReadAsync()    →  avance d'une ligne, retourne true/false
		//└── DBNull.Value          →  NULL en ADO.NET

		//Je veux insérer / modifier / supprimer  =>  ExecuteNonQueryAsync
		//Je veux lire plusieurs lignes           =>  ExecuteReaderAsync
		//Je veux lire une seule valeur           =>  ExecuteScalarAsync
		#endregion

		public async Task<Tournament?> GetByIdAsync(int tournamentId)
		{
			//Ouvir la connexion et on la ferme automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = "SELECT * " +
				           "FROM Tournament " +
						   "WHERE Tournament_Id = @Tournament_Id";

		   //Ajouter le paramètre
		   using SqlCommand cmd = new SqlCommand(query, conn);
		   cmd.Parameters.AddWithValue("@Tournament_Id", tournamentId);

		   //Executer avec ExecuteReaderAsync
		   using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			// 2. LIRE avec if(reader.ReadAsync())
			if (await reader.ReadAsync())
			{
				// 3. CONSTRUIRE l'objet
				Tournament tournament = new Tournament()
				{
					Tournament_Id = (int)reader["Tournament_Id"],
					NameTournament = (string)reader["NameTournament"],
					Place = (string)reader["Place"],
					MinNbPlayer = (int)reader["MinNbPlayer"],
					MaxNbPlayer = (int)reader["MaxNbPlayer"],
					MinElo = reader["MinElo"] == DBNull.Value ? null : (int?)reader["MinElo"],
					MaxElo = reader["MaxElo"] == DBNull.Value ? null : (int?)reader["MaxElo"],
					StatusTournament = (string)reader["StatusTournament"],
					CurrentRound = (int)reader["CurrentRound"],
					WomenOnly = (bool)reader["WomenOnly"],
					MaxRounds = (int)reader["MaxRounds"],
					RegistrationDeadline = (DateTime)reader["RegistrationDeadline"],
					CreationDate = (DateTime)reader["CreationDate"],
					UpdateDate = (DateTime)reader["UpdateDate"]
				};
				// 4. RETOURNER l'objet ou null si pas trouvé
				return tournament;
			}
			 return null;
		}

		public async Task UpdateTournamentAsync(Tournament tournament)
		{
			//Ouvrir la connexion et la fermer automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = @"UPDATE Tournament
                             SET StatusTournament = @StatusTournament,
						         CurrentRound = @CurrentRound,
						         UpdateDate = @UpdateDate
						     WHERE Tournament_Id = @Tournament_Id";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter paramètres
			cmd.Parameters.AddWithValue("@StatusTournament", tournament.StatusTournament);
			cmd.Parameters.AddWithValue("@CurrentRound",tournament.CurrentRound);
			cmd.Parameters.AddWithValue("@UpdateDate",tournament.UpdateDate);
			cmd.Parameters.AddWithValue("@Tournament_Id",tournament.Tournament_Id);

			//Executer la requête sql
			await cmd.ExecuteNonQueryAsync();
		}

		public async Task UpdateDeadlineAsync(int tournamentId, DateTime newDeadline)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			string query = @"UPDATE Tournament
                             SET RegistrationDeadline = @RegistrationDeadline,
                                 UpdateDate = @UpdateDate
                             WHERE Tournament_Id = @Tournament_Id";
			using SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@RegistrationDeadline", newDeadline);
			cmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
			cmd.Parameters.AddWithValue("@Tournament_Id", tournamentId);

			await cmd.ExecuteNonQueryAsync();
		}

		public async Task DeleteTournamentAsync(int tournamentId)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			using SqlTransaction transaction = conn.BeginTransaction();
			try
			{
				// 1. Supprimer les matchs du tournoi
				using (SqlCommand cmd = new SqlCommand("DELETE FROM Match_ WHERE Tournament_Id = @Id", conn, transaction))
				{
					cmd.Parameters.AddWithValue("@Id", tournamentId);
					await cmd.ExecuteNonQueryAsync();
				}

				// 2. Supprimer les inscriptions du tournoi
				using (SqlCommand cmd = new SqlCommand("DELETE FROM Registration WHERE Tournament_Id = @Id", conn, transaction))
				{
					cmd.Parameters.AddWithValue("@Id", tournamentId);
					await cmd.ExecuteNonQueryAsync();
				}

				// 3. Supprimer les catégories liées au tournoi
				using (SqlCommand cmd = new SqlCommand("DELETE FROM TournamentCategory WHERE Tournament_Id = @Id", conn, transaction))
				{
					cmd.Parameters.AddWithValue("@Id", tournamentId);
					await cmd.ExecuteNonQueryAsync();
				}

				// 4. Supprimer le tournoi
				using (SqlCommand cmd = new SqlCommand("DELETE FROM Tournament WHERE Tournament_Id = @Id", conn, transaction))
				{
					cmd.Parameters.AddWithValue("@Id", tournamentId);
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
	}
}

#region Comments
// ═══════════════════════════════════════════
// Les étapes ADO.NET ASYNC dans l'ordre
// ═══════════════════════════════════════════

// ───────────────────────────────────────────
// TOUJOURS OBLIGATOIRE
// ───────────────────────────────────────────
// A. OUVRIR la connexion            →  await conn.OpenAsync()
// B. ECRIRE la requête SQL
// C. AJOUTER les paramètres SQL (si valeur variable dans le WHERE)

// ───────────────────────────────────────────
// CAS 1 : INSERT, UPDATE, DELETE
// ───────────────────────────────────────────
// 1. EXECUTER avec ExecuteNonQueryAsync
// 2. pas de retour (Task)

// ───────────────────────────────────────────
// CAS 2 : SELECT plusieurs résultats (List)
// ───────────────────────────────────────────
// 1. CREER une liste vide
// 2. EXECUTER avec ExecuteReaderAsync
// 3. LIRE ligne par ligne avec while(await reader.ReadAsync())
// 4. CONSTRUIRE un objet pour chaque ligne
// 5. AJOUTER chaque objet à la liste
// 6. RETOURNER la liste

// ───────────────────────────────────────────
// CAS 3 : SELECT un seul résultat (objet)
// ───────────────────────────────────────────
// 1. EXECUTER avec ExecuteReaderAsync
// 2. LIRE avec if(await reader.ReadAsync())
// 3. CONSTRUIRE l'objet
// 4. RETOURNER l'objet ou null si pas trouvé

// ───────────────────────────────────────────
// CAS 4 : SELECT une seule valeur (COUNT, SUM...)
// ───────────────────────────────────────────
// 1. EXECUTER avec ExecuteScalarAsync
// 2. RETOURNER la valeur

// ───────────────────────────────────────────
// TOUJOURS OBLIGATOIRE
// ───────────────────────────────────────────
// Z. FERMER la connexion (automatique avec using)
// ═══════════════════════════════════════════
#endregion
