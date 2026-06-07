using XenoUI.Core.Components.Enums;
using XenoUI.Core.Components.Primary;
using XenoUI.Core.Pillars;

namespace XenoUI.Core.Compisitor;

/// <summary>
/// Provides a set of static methods for composing UI elements or components within the XenoUI framework.
/// </summary>
public static class Compose
{
    /// <summary>
    /// Creates a base entity in the XenoUI framework using the specified engine and layout direction.
    /// </summary>
    /// <param name="engine">The ComponentEngine instance used to manage and cache the created entity.</param>
    /// <param name="type">The layout direction to be applied to the created entity.</param>
    /// <returns>An integer identifier for the created entity within the engine's cache.</returns>
    private static int CreateBase(ComponentEngine engine, LayoutDirection type)
    {
        var style = new StyleComponent { LayoutDirection = type };
        var visual = new VisualComponent{ Color = 0xFF000000, CornerRadius = 0 };
        return engine.UiCacheMemory.CreateEntity(style, visual);
    }

    /// <summary>
    /// Creates a box UI element in the XenoUI framework with the specified dimensions and background color.
    /// </summary>
    /// <param name="engine">The ComponentEngine instance responsible for managing the entity's lifecycle and caching.</param>
    /// <param name="width">The width of the box in logical units.</param>
    /// <param name="height">The height of the box in logical units.</param>
    /// <param name="backgroundColor">The background color of the box, represented as a 32-bit unsigned integer.</param>
    /// <returns>An integer identifier for the created box entity within the engine's cache.</returns>
    public static int Box(ComponentEngine engine, float width, float height, uint backgroundColor)
    {
        var style = new StyleComponent
        {
            Width = width,
            Height = height
        };

        var visual = new VisualComponent
        {
            Color = backgroundColor
        };
        
        return engine.UiCacheMemory.CreateEntity(style, visual);
    }

    /// <summary>
    /// Creates a vertical layout column with the specified gap between elements in the XenoUI framework.
    /// </summary>
    /// <param name="engine">The ComponentEngine instance used to create and manage the column layout.</param>
    /// <param name="gap">The spacing between elements within the column layout.</param>
    /// <returns>An integer identifier for the created column layout within the engine's cache.</returns>
    public static int Column(ComponentEngine engine, float gap)
    {
        var stackId = CreateBase(engine, LayoutDirection.Vertical);
        engine.UiCacheMemory.Styles.Get(stackId).Gap = gap;
        return stackId;
    }

    /// <summary>
    /// Creates a horizontal row container within the XenoUI framework using the specified gap setting.
    /// </summary>
    /// <param name="engine">The ComponentEngine instance used to manage and cache the created row container.</param>
    /// <param name="gap">The spacing applied between child elements within the row container.</param>
    /// <returns>An integer identifier representing the created row container in the engine's cache.</returns>
    public static int Row(ComponentEngine engine, float gap)
    {
        var stackId = CreateBase(engine, LayoutDirection.Horizontal);
        engine.UiCacheMemory.Styles.Get(stackId).Gap = gap;
        return stackId;
    }
}