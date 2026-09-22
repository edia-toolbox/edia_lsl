using UnityEngine;

namespace LSL4Unity.Utils
{
    /// <summary>
    /// Compatibility shim for Unity's InstanceID -> EntityId migration.
    ///
    /// Unity 6.3 (6000.3) introduced <c>Object.GetEntityId()</c> and deprecated
    /// <c>Object.GetInstanceID()</c>. The deprecation warns in 6.4 and becomes a hard
    /// compile error in 6.5 (6000.5). Because this package ships to end users on
    /// unknown editor versions, every call site goes through this shim instead of
    /// calling either API directly.
    ///
    /// See: https://docs.unity3d.com/6000.5/Documentation/Manual/instanceid-to-entityid-migration.html
    /// </summary>
    public static class ObjectIdCompat
    {
        /// <summary>
        /// Returns the object's runtime identity as a string, using whichever API the
        /// running editor/player supports.
        ///
        /// A string is used deliberately: <see cref="Hash128"/> has no
        /// <c>Append(EntityId)</c> overload, and <c>EntityId</c> is an opaque 64-bit
        /// struct with no conversion to <c>int</c> - casting it would silently truncate.
        /// Formatting to text sidesteps both problems and keeps the value lossless.
        ///
        /// Note this is a *session-local* identifier, same as the InstanceID it replaces.
        /// It is suitable for telling two objects apart within one run, not for
        /// persisting a reference across runs.
        /// </summary>
        public static string GetObjectIdString(this Object obj)
        {
            if (obj == null)
                return "0";

#if UNITY_6000_3_OR_NEWER
            return obj.GetEntityId().ToString();
#else
            return obj.GetInstanceID().ToString();
#endif
        }
    }
}
