namespace GymManagementSystem.Application.Exceptions
{
    public class GymNotFoundException : Exception
    {
        public GymNotFoundException(int id)
            : base($"Gym with ID {id} was not found.")
        {
        }
    }
}