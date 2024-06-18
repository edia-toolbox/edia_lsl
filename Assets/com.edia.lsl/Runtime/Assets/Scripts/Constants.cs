/// <summary>
/// Constants used throughout the EDIA LSL package.
/// </summary>

namespace Edia.Lsl {

    /// <summary>
    /// A pose describes the combination of position (always x, y, z coordinates) and rotation (can be in Euler Angles [6D] or Quaternions [7D]) of an object.
    /// </summary>
    public enum PoseFormat { PosEul6D, PosQuat7D }

    /// <summary>
    /// Tracking space defines the reference system of an objects location and rotation (e.g., world space or local/relative to another object).
    /// </summary>
    public enum TrackingSpace { WorldSpace, LocalSpace }
    
}
