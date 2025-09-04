namespace GameToolkit.Runtime.Utils.Extensions
{
    public static class IntExtensions
    {
        public static bool HasFlag(this int a, int b) => (a & b) == b;

        public static int AddFlag(this int a, int b) => a |= b;

        public static int RemoveFlag(this int a, int b) => a &= ~b;
    }
}
