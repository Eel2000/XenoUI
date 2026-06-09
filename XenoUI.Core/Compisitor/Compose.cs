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
    private static int CreateBase(XenoEngine engine, LayoutDirection type)
    {
        var style = new StyleComponent { LayoutDirection = type };
        var visual = new VisualComponent{ Color = 0xFF000000, CornerRadius = 0 };

        return engine.CreateEntity(style, visual);
    }

    /// <summary>
    /// Creates a box UI element in the XenoUI framework with the specified dimensions and background color.
    /// </summary>
    /// <param name="engine">The ComponentEngine instance responsible for managing the entity's lifecycle and caching.</param>
    /// <param name="width">The width of the box in logical units.</param>
    /// <param name="height">The height of the box in logical units.</param>
    /// <param name="backgroundColor">The background color of the box, represented as a 32-bit unsigned integer.</param>
    /// <returns>An integer identifier for the created box entity within the engine's cache.</returns>
    public static int Box(XenoEngine engine, float width, float height, uint backgroundColor)
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
        
        return engine.CreateEntity(style, visual);
    }

    /// <summary>
    /// Creates a vertical layout column with a specified gap and automatically wires up nested child entities.
    /// </summary>
    /// <param name="engine">The XenoEngine instance used to create and manage the column layout.</param>
    /// <param name="gap">The spacing between elements within the column layout.</param>
    /// <param name="children">The child entity IDs to be contained inside this column layout container.</param>
    /// <returns>An integer identifier for the created column layout within the engine's cache.</returns>
    public static int Column(XenoEngine engine, float gap, params int[] children)
    {
        var stackId = CreateBase(engine, LayoutDirection.Vertical);

        // Using the 'ref' keyword assignment to prevent mutating an isolated struct copy
        ref var style = ref engine.XenoUICacheMemory.Styles.Get(stackId);
        style.Gap = gap;

        // Automatically map children relationships down into the database and layout trees
        foreach (var childId in children)
        {
            engine.AddChild(stackId, childId);
        }

        return stackId;
    }

    /// <summary>
    /// Creates a horizontal row container with a specified gap and automatically wires up nested child entities.
    /// </summary>
    /// <param name="engine">The XenoEngine instance used to manage and cache the created row container.</param>
    /// <param name="gap">The spacing applied between child elements within the row container.</param>
    /// <param name="children">The child entity IDs to be contained inside this row layout container.</param>
    /// <returns>An integer identifier representing the created row container in the engine's cache.</returns>
    public static int Row(XenoEngine engine, float gap, params int[] children)
    {
        var stackId = CreateBase(engine, LayoutDirection.Horizontal);

        // using the 'ref' keyword assignment to prevent mutating an isolated struct copy
        ref var style = ref engine.XenoUICacheMemory.Styles.Get(stackId);
        style.Gap = gap;

        // Automatically map children relationships down into the database and layout trees
        foreach (var childId in children)
        {
            engine.AddChild(stackId, childId);
        }

        return stackId;
    }
}