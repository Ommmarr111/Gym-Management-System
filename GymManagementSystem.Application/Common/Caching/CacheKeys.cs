namespace GymManagementSystem.Application.Common.Caching
{
    public static class CacheKeys
    {
        public static class Plans
        {
            public const string All = "plans";
            public static string ById(int id) => $"plan:{id}";
            public static string ByGymId(int gymId) => $"plans:gym:{gymId}";
        }
    }
}