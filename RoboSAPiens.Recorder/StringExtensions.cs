namespace RoboSAPiens.Recorder
{
    static class StringExtensions
    {
        public static string Capitalize(this string s)
        {
            return char.ToUpper(s[0]) + s[1..];
        }

        public static string? NullIfEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) ? null : s;
        }

        public static string toFileName(this string str)
        {
            return 
                Path.GetInvalidFileNameChars()
                .Aggregate(
                    str.Replace(" ", "_"), 
                    (_, c) => _.Replace(c.ToString(), "")
                )
                .TrimEnd('.');
        }
    }
}