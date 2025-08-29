﻿namespace GameToolkit.Runtime.Utils.Extensions
{
    public static class IntExtensions
    {
        /// <summary>
        ///
        /// /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool HasFlag(this int a, int b) => (a & b) == b;

        /// <summary>
        ///
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int AddFlag(this int a, int b) => a |= b;

        /// <summary>
        ///
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int RemoveFlag(this int a, int b) => a &= ~b;
    }
}
