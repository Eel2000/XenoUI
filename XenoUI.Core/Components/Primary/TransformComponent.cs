namespace XenoUI.Core.Components.Primary;

/// <summary>
/// Represents a 2D transform component used for defining the position
/// and dimensions of an object in a coordinate space.
/// </summary>
public struct TransformComponent
{
    /// <summary>
    /// Represents the horizontal position or coordinate of the transform component.
    /// This value typically corresponds to the X-axis position in a 2D coordinate system.
    /// </summary>
    public float X;

    /// <summary>
    /// Represents the vertical position or coordinate of the transform component.
    /// This value typically corresponds to the Y-axis position in a 2D coordinate system.
    /// </summary>
    public float Y;

    /// <summary>
    /// Specifies the horizontal dimension or extent of the transform component.
    /// This value typically defines the width of the component in a 2D space.
    /// </summary>
    public float Width;

    /// <summary>
    /// Specifies the vertical dimension or extent of the transform component.
    /// This value typically defines the height of the component in a 2D space.
    /// </summary>
    public float Height;
}