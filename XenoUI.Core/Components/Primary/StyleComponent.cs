using XenoUI.Core.Components.Enums;

namespace XenoUI.Core.Components.Primary;

/// <summary>
/// Defines a struct that encapsulates styling properties for a UI component,
/// including dimensions and background color.
/// </summary>
public struct StyleComponent
{
    /// <summary>
    /// Represents the width of the component in floating-point units.
    /// </summary>
    public float Width;

    /// <summary>
    /// Represents the height of the component in floating-point units.
    /// </summary>
    public float Height;

    /// <summary>
    /// Represents the background color of the component as an unsigned integer value.
    /// </summary>
    public uint BackgroundColor;

    /// <summary>
    /// Represents the left margin of the component in floating-point units.
    /// </summary>
    public float MarginLeft;

    /// <summary>
    /// Represents the top margin of the component in floating-point units.
    /// </summary>
    public float MarginTop;

    /// <summary>
    /// Represents the right margin of the component in floating-point units.
    /// </summary>
    public float MarginRight;

    /// <summary>
    /// Represents the bottom margin of the component in floating-point units.
    /// </summary>
    public float MarginBottom;

    /// <summary>
    /// Represents the left padding of the component in floating-point units.
    /// </summary>
    public float PaddingLeft;

    /// <summary>
    /// Represents the top padding of the component in floating-point units.
    /// </summary>
    public float PaddingTop;

    /// <summary>
    /// Represents the right padding of the component in floating-point units.
    /// </summary>
    public float PaddingRight;

    /// <summary>
    /// Represents the bottom padding of the component in floating-point units.
    /// </summary>
    public float PaddingBottom;

    /// <summary>
    /// Represents the spacing between child elements within the component in floating-point units.
    /// </summary>
    public float Gap;

    /// <summary>
    /// Specifies the direction in which the layout for a UI component is arranged.
    /// </summary>
    public LayoutDirection LayoutDirection;
}