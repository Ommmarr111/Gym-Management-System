namespace GymManagementSystem.Application.Exceptions
{
    public class DuplicateMemberEmailException : Exception
    {
        public DuplicateMemberEmailException(string email)
            : base($"Member with email '{email}' already exists.")
        {
        }
    }
}