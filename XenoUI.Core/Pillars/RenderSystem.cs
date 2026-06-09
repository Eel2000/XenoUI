using SkiaSharp;
using XenoUI.Core.Doddb;

namespace XenoUI.Core.Pillars;

/// <summary>
/// The <c>RenderSystem</c> class is responsible for rendering user interface (UI) elements on a canvas.
/// It processes visual and transform data from a UI cache memory to draw elements efficiently using SkiaSharp.
/// </summary>
/// <remarks>
/// This rendering system operates at the core of the UI framework, processing cached data to output the visual
/// representation of the UI. It supports features such as drawing rounded or rectangular shapes with specified
/// dimensions and colors.
/// </remarks>
public class RenderSystem
{
    /// <summary>
    /// A paint cache organized by color hex key. This eliminates allocations inside the render loop
    /// while ensuring distinct UI elements don't corrupt each other's styling states.
    /// </summary>
    private readonly Dictionary<uint, SKPaint> _paintCache = new();

    /// <summary>
    /// Renders UI elements onto the provided canvas based on the cached data in the specified memory slab.
    /// </summary>
    /// <param name="canvas">The SKCanvas instance where the UI elements will be drawn.</param>
    /// <param name="cacheMemory">The XenoUICacheMemory instance containing precomputed visual, style, and transform data for rendering.</param>
    /// <param name="density">The screen density factor, used to convert layout units to physical pixels.</param>
    public void Render(SKCanvas canvas, XenoUICacheMemory cacheMemory, float density)
    {
        for (int elementIndex = 0; elementIndex < cacheMemory.EntityCount; elementIndex++)
        {
            // Retrieve the style, visual, and transform components for the current UI element from the cache memory.
            ref var transform = ref cacheMemory.Transforms.Get(elementIndex);
            ref var visual = ref cacheMemory.Visuals.Get(elementIndex);

            // Fetch a distinct, safely isolated paint instance for this specific color
            var paint = GetPaintForColor(visual.Color);


            // MULTIPLY BY DENSITY TO CONVERT DP BACK TO PHYSICAL PIXELS FOR THE CANVAS
            var rect = new SKRect(
                transform.X * density,
                transform.Y * density,
                (transform.X + transform.Width) * density,
                (transform.Y + transform.Height) * density
            );

            float physicalRadius = visual.CornerRadius * density;

            if (physicalRadius > 0)
                canvas.DrawRoundRect(rect, physicalRadius, physicalRadius, paint);
            else
                canvas.DrawRect(rect, paint);
        }
    }

    /// <summary>
    /// Retrieves a cached SKPaint instance for the given color, creating it if it doesn't exist.
    /// </summary>
    private SKPaint GetPaintForColor(uint colorHex)
    {
        if (!_paintCache.TryGetValue(colorHex, out var paint))
        {
            paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = new SKColor(colorHex)
            };
            _paintCache[colorHex] = paint;
        }
        return paint;
    }

    /// <summary>
    /// Completely disposes all cached paints to prevent native C++ Skia memory leaks on mobile devices.
    /// </summary>
    public void ClearCache()
    {
        foreach (var paint in _paintCache.Values)
        {
            paint.Dispose();
        }
        _paintCache.Clear();
    }
}