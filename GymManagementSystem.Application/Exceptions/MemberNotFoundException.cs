namespace GymManagementSystem.Application.Exceptions
{
    public class MemberNotFoundException : Exception
    {
        public MemberNotFoundException(int id)
            : base($"Member with ID {id} was not found.")
        {
        }
    }
}