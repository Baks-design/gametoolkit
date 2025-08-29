using System;
using Object = UnityEngine.Object;

namespace GameToolkit.Runtime.Utils.Helpers
{
    public static class Checking
    {
        public static T AgainstNull<T>(T value, string paramName, string message = null)
            where T : class
        {
            if (value == null)
                throw new ArgumentNullException(paramName, message);
            return value;
        }

        public static T AgainstNullUnityObject<T>(
            T unityObject,
            string paramName,
            string message = null
        )
            where T : Object
        {
            if (!unityObject)
                throw new ArgumentNullException(paramName, message);
            return unityObject;
        }
    }
}
