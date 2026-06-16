using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DAL.Connection
{
	public class DbConnection
	{
		private readonly string _connectionString;

		public DbConnection(IConfiguration configuration)
		{
			_connectionString = configuration["CHESS_CONNECTION_STRING"]
				?? throw new Exception("Connection string non trouvée");
		}

		public SqlConnection GetConnection()
		{
			return new SqlConnection(_connectionString);
		}

	}
}
