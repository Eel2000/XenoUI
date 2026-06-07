using SkiaSharp;
using XenoUI.Core.Doddb;

namespace XenoUI.Core.Pillars
{
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
        // Create a shared SKPaint instance to be used for rendering UI elements.
        // This instance is configured with anti-aliasing enabled and set to fill style,
        // which allows for smooth and visually appealing rendering of shapes and backgrounds in the UI.
        // IMPORTANT: This shared SKPaint instance should be used for all rendering operations to ensure consistency and performance across the UI rendering process.
        private readonly SKPaint _sharePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        /// <summary>
        /// Renders UI elements onto the provided canvas based on the cached data in the specified memory slab.
        /// </summary>
        /// <param name="canvas">The SKCanvas instance where the UI elements will be drawn.</param>
        /// <param name="cacheMemory">The XenoUICacheMemory instance containing precomputed visual, style, and transform data for rendering.</param>
        public void Draw(SKCanvas canvas, XenoUICacheMemory cacheMemory)
        {
            for (int elementIndex = 0; elementIndex < cacheMemory.EntityCount; elementIndex++)
            {
                // Retrieve the style, visual, and transform components for the current UI element from the cache memory.

                // get the transform component for the current element index from the cache memory's transform slab.
                ref var transform = ref cacheMemory.Transforms.Get(elementIndex);

                // get the visual component for the current element index from the cache memory's Visuals slab.
                ref var visual = ref cacheMemory.Visuals.Get(elementIndex);

                // Set the color of the shared SKPaint instance to the color specified in the visual component.
                _sharePaint.Color = new SKColor(visual.Color);

                // Create a new SKRect using the position and size specified in the transform component.
                var rect = new SKRect(
                    transform.X,
                    transform.Y,
                    transform.X + transform.Width,
                    transform.Y + transform.Height
                );


                // Draw a filled rectangle on the canvas using the defined SKRect and the shared SKPaint instance.
                if(visual.CornerRadius > 0)
                {
                    // If the visual component has a corner radius greater than 0, draw a rounded rectangle using the shared SKPaint instance.
                    canvas.DrawRoundRect(rect, visual.CornerRadius, visual.CornerRadius, _sharePaint);
                }
                else
                {
                    // If there is no corner radius, draw a regular filled rectangle using the shared SKPaint instance.
                    canvas.DrawRect(rect, _sharePaint);
                }
            }
        }
    }
}
