namespace RoboSAPiens.Recorder
{
    static class IEnumberableExtensions
    {
        public static void Deconstruct<T>(this IEnumerable<T> list, out T  head, out IEnumerable<T> tail)
        {
            head = list.First();
            tail = [.. list.Skip(1)];
        }

        public static void Deconstruct<T>(this IEnumerable<T> list, out T  head, out T second, out IEnumerable<T> tail)
        {
            head = list.First();
            second = list.Skip(1).First();
            tail = [.. list.Skip(2)];
        }
    }
}