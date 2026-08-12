namespace RoboSAPiens.Recorder
{
    static class DictionaryExtensions
    {
        public static TValue? GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) where TKey : notnull
        {
            return dict.TryGetValue(key, out var value) ? value : default;
        }
    }
}