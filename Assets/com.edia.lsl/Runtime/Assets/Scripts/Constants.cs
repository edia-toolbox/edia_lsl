/// <summary>
/// Constants used throughout the EDIA LSL package.
/// </summary>

namespace Edia.Lsl {

    /// <summary>
    /// Rotation can be in EulerAngles Angles or a Quaternion.
    /// </summary>
    public enum RotationFormat { EulerAngles, Quaternion }

    /// <summary>
    /// Tracking space defines the reference system of an objects location and rotation (e.g., world space or local/relative to another object).
    /// </summary>
    public enum TrackingSpace { WorldSpace, LocalSpace }
    
}
