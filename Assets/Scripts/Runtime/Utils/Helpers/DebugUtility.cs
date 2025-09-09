using UnityEngine;

namespace GameToolkit.Runtime.Utils.Helpers
{
    public static class DebugUtility
    {
        public static void HandleErrorIfNullGetComponent<TO, TS>(
            Component component,
            Component source,
            GameObject onObject
        )
        {
            if (component != null)
                return;

            Logging.LogError(
                $"Error: Component of type {typeof(TS)} on GameObject {source.gameObject.name} "
                    + $"expected to find a component of type {typeof(TO)} on GameObject {onObject.name}, "
                    + "but none were found."
            );
        }

        public static void HandleErrorIfNullFindObject<TO, TS>(Object obj, Component source)
        {
            if (obj != null)
                return;

            Logging.LogError(
                $"Error: Component of type {typeof(TS)} on GameObject {source.gameObject.name} "
                    + $"expected to find an object of type {typeof(TO)} in the scene, but none were found."
            );
        }

        public static void HandleErrorIfNoComponentFound<TO, TS>(
            int count,
            Component source,
            GameObject onObject
        )
        {
            if (count != 0)
                return;

            Logging.LogError(
                $"Error: Component of type {typeof(TS)} on GameObject {source.gameObject.name} "
                    + $"expected to find at least one component of type {typeof(TO)} on GameObject {onObject.name}, "
                    + "but none were found."
            );
        }

        public static void HandleWarningIfDuplicateObjects<TO, TS>(
            int count,
            Component source,
            GameObject onObject
        )
        {
            if (count <= 1)
                return;

            Logging.LogWarning(
                $"Warning: Component of type {typeof(TS)} on GameObject {source.gameObject.name} "
                    + $"expected to find only one component of type {typeof(TO)} on GameObject {onObject.name}, "
                    + "but several were found. First one found will be selected."
            );
        }
    }
}
