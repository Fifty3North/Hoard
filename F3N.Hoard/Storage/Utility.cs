using System.Text;

namespace F3N.Hoard.Storage
{
    public static class Utility
    {
        // Key format <Type:Id>

        public static string MakeKey<T>(string id)
        {
            return MakeKey(typeof(T).Name, id);
        }

        public static string MakeKey(string type, string id)
        {
            StringBuilder keyBuilder = new StringBuilder();
            keyBuilder.Append(type).Append(':').Append(id);

            return keyBuilder.ToString();
        }
    }
}
