using DAL.Connection;
using DAL.Interfaces;
using DOMAIN.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories
{
	public class CategoryRepository : ICategoryRepository
	{
		#region Conection
		private readonly DbConnection _db;
		public CategoryRepository(DbConnection db)
		{
			_db = db;
		}
		#endregion
		public async Task CreateCategoryAsync(Category category)
		{
			//Ouvir la connexion et la refermer automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = @"INSERT INTO Category
                             (NameCategory, MinAge, MaxAge)
                             VALUES
                             (@NameCategory, @MinAge, @MaxAge)
                            ";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter les paramètres variables
			cmd.Parameters.AddWithValue("@NameCategory", category.NameCategory);
			cmd.Parameters.AddWithValue("@MinAge", category.MinAge);
			cmd.Parameters.AddWithValue("@MaxAge", category.MaxAge);

			//Exécuter requête sql
			await cmd.ExecuteNonQueryAsync();
		}
		public async Task<List<Category>> GetCategoriesByTournamentIdAsync(int tournamentId)
		{
			//Ouvrir la connexion et la fermer automatiquement
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			//Ecrire la requête sql
			string query = @"SELECT c.*
                             FROM Category c
                             JOIN TournamentCategory tc ON c.Category_Id = tc.Category_Id
                             WHERE tc.Tournament_Id = @Tournament_Id
                            ";
			using SqlCommand cmd = new SqlCommand(query, conn);

			//Ajouter les paramètres variables
			cmd.Parameters.AddWithValue("@Tournament_Id",tournamentId);

		    //Créer une liste vide
			List<Category> categories = new List<Category>();

			//Exécuter la commande
			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			//Lire ligne par ligne avec while(await reader.ReadAsync())
			while (await reader.ReadAsync())
			{
				//Construit un objet pour chaque ligne
				Category category = new Category()
				{
					Category_Id = (int)reader["Category_Id"],
					NameCategory = (string)reader["NameCategory"],
					MinAge = (int)reader["MinAge"],
					MaxAge = (int)reader["MaxAge"]
	            };

				//Ajouter à la liste vide
				categories.Add(category);
			}
			//Retourner la liste
			return categories;
		}
		public async Task<List<Category>> GetAllAsync()
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			string query = "SELECT * FROM Category";
			using SqlCommand cmd = new SqlCommand(query, conn);

			List<Category> categories = new List<Category>();
			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			while (await reader.ReadAsync())
			{
				categories.Add(new Category()
				{
					Category_Id = (int)reader["Category_Id"],
					NameCategory = (string)reader["NameCategory"],
					MinAge = (int)reader["MinAge"],
					MaxAge = (int)reader["MaxAge"]
				});
			}
			return categories;
		}

		public async Task<bool> IsUsedByTournamentsAsync(int categoryId)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			string query = @"SELECT COUNT(1)
                             FROM TournamentCategory tc
                             JOIN Tournament t ON t.Tournament_Id = tc.Tournament_Id
                             WHERE tc.Category_Id = @Category_Id
                               AND t.StatusTournament != @Status";
			using SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@Category_Id", categoryId);
			cmd.Parameters.AddWithValue("@Status", "Terminé");

			object? result = await cmd.ExecuteScalarAsync();
			return Convert.ToInt32(result) > 0;
		}

		public async Task DeleteCategoryAsync(int categoryId)
		{
			using SqlConnection conn = _db.GetConnection();
			await conn.OpenAsync();

			string query = "DELETE FROM Category WHERE Category_Id = @Category_Id";
			using SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@Category_Id", categoryId);

			await cmd.ExecuteNonQueryAsync();
		}
	}
}
