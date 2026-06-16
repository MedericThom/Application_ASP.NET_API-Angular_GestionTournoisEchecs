using DOMAIN.Entities;

namespace BLL.Interfaces
{
	public interface IRegistrationService
	{
		public Task RegisterPlayerAsync(Registration registration);
		public Task<List<Registration>> GetScoresAsync(int tournamentId, int round);
		public Task UnregisterPlayerAsync(int playerId, int tournamentId); //ELLE NE RETOURNE RIEN => ACTION !!!
	}
}
