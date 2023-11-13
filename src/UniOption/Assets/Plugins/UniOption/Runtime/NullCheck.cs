#nullable enable
using System;

namespace UniOption {
    internal static class NullCheck {
        internal static bool IsNull<T>(T? nullable) where T : class {
            if (nullable is not UnityEngine.Component unityObject) return nullable == null;

            try {
                return unityObject.gameObject == null;
            }
            catch {
                return true;
            }
        }

        internal static bool IsNull<T>(T? nullable) where T : struct    => nullable is null;
        internal static bool IsNotNull<T>(T? nullable) where T : class  => !IsNull(nullable);
        internal static bool IsNotNull<T>(T? nullable) where T : struct => !IsNull(nullable);

        internal static T NullReturn<T>(T? nullable) where T : class => IsNull(nullable)
                                                                                 ? throw new ArgumentNullException(nameof(nullable))
                                                                                 : nullable!;

        internal static T NullReturn<T>(T? nullable) where T : struct => IsNull(nullable)
                                                                                  ? default
                                                                                  : nullable!.Value;
    }
}