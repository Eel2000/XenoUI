using SkiaSharp;
using XenoUI.Core.Components.Primary;
using XenoUI.Core.Doddb;

namespace XenoUI.Core.Pillars;

public class XenoEngine
{
    public readonly XenoUICacheMemory XenoUICacheMemory;
    public readonly LayoutSystem LayoutSystem;
    public readonly RenderSystem RenderSystem;

    private int _nextEntityId = 0;

    public float ScreenDensity { get; private set; } = 1.0f;

    public XenoEngine()
    {
        XenoUICacheMemory = new XenoUICacheMemory();
        LayoutSystem = new LayoutSystem();
        RenderSystem = new RenderSystem();
    }

    /// <summary>
    /// Generates a unique entity ID for a new UI element.
    /// </summary>
    /// <param name="density"></param>
    public void SetDensity(float density)
    {
        ScreenDensity = density;
    }

    /// <summary>
    /// Creates a button with the specified dimensions and background color, and registers it within the system for use in the UI layout.
    /// </summary>
    /// <param name="style"></param>
    /// <param name="visual"></param>
    /// <returns></returns>
    public int CreateEntity(StyleComponent style, VisualComponent visual)
    {
        // we use the styles id as the main index for all it corresponding neighboring components,
        // so we push the style component first to get its id, then we push the visual component using the same id.
        var id = XenoUICacheMemory.Styles.Push(style);
        XenoUICacheMemory.Visuals.Push(visual);

        LayoutSystem.RegisterEntity(id);

        return id;
    }

    /// <summary>
    /// Establishes a parent-child relationship in the layout tree.
    /// </summary>
    public void AddChild(int parentId, int childId)
    {
        // CRITICAL SYNC STEP: Tell Yoga how these entities nest together
        LayoutSystem.RegisterChild(parentId, childId);
    }

    /// <summary>
    /// The heart-beat of the engine. Coordinates layout calculations 
    /// and visual rendering onto the physical glass.
    /// </summary>
    public void Refresh(SKCanvas canvas, float width, float height, int rootPageEntityId)
    {
        // 1. Connect the active screen's top-level container to Yoga's base window anchor
        LayoutSystem.SetRootPageEntity(rootPageEntityId);

        // 2. Push all developer margins, paddings, widths, and heights into Yoga
        LayoutSystem.ApplyStyles(XenoUICacheMemory);

        // 3. Let Yoga run its multi-threaded/recursive layout calculations
        LayoutSystem.CalculateLayout(width, height, ScreenDensity);

        // 4. Extract the mathematical coordinates back out into our Transforms slab
        LayoutSystem.HarvestResults(XenoUICacheMemory);

        // 5. Hand the final calculated transforms over to Skia to blit pixels to the screen
        RenderSystem.Render(canvas, XenoUICacheMemory, ScreenDensity);
    }

    public void Dispose()
    {
        XenoUICacheMemory.Dispose();
        RenderSystem.ClearCache();
        LayoutSystem.Clear();
    }
}