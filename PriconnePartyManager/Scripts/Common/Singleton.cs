namespace PriconnePartyManager.Scripts.Common
{
    public class Singleton<T> where T : new()
    {
        private static T s_Instance  = new T();

        public static T I => s_Instance;
    }
}