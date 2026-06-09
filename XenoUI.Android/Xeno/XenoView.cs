using Android.Content;
using SkiaSharp;
using SkiaSharp.Views.Android;
using XenoUI.Core.Compisitor;
using XenoUI.Core.Pillars;

namespace XenoUI.Android.Xeno
{
    public class XenoView : SKGLSurfaceView
    {
        private readonly XenoEngine _componentEngine;
        private int _rootPageEntityId;

        public XenoView(Context context) : base(context)
        {
            // Initialize the ComponentEngine instance to manage UI components and rendering logic for the XenoView.
            _componentEngine = new();

            // 1. EXTRACT THE HARDWARE DENSITY FACTOR
            // This returns values like 2.0, 3.0, 4.0 depending on screen resolution
            float density = context.Resources.DisplayMetrics.Density;

            // 2. STASH IT IN THE ENGINE IMMEDIATELY
            _componentEngine.SetDensity(density);

            // let test first
            // _componentEngine.CreateButton(100, 400, 0xFFCC7A00); // Create a button with specified width, height, and background color.

            // Inside a Screen component file
            int rootPageId = Compose.Column(_componentEngine, gap: 20f,
                Compose.Box(_componentEngine, width: 200f, height: 50f, backgroundColor: 0xFFFF0000), // Red header box
                Compose.Row(_componentEngine, gap: 10f,
                    Compose.Box(_componentEngine, width: 90f, height: 90f, backgroundColor: 0xFF00FF00), // Green box left
                    Compose.Box(_componentEngine, width: 90f, height: 90f, backgroundColor: 0xFF0000FF)  // Blue box right
                )
            );

            _rootPageEntityId = rootPageId; // Store the root page entity ID for rendering.
        }

        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            // Obtain the SKCanvas from the SKPaintGLSurfaceEventArgs to perform drawing operations on the surface of the XenoView.
            var canvas = e.Surface.Canvas;


            // Clear the GPU buffer
            canvas.Clear(SKColors.White);

            // Get Screen info
            var screenWidth = e.Info.Width;
            var screenHeight = e.Info.Height;

            // Update the ComponentEngine with the current screen dimensions to calculate layout and prepare for rendering.
            _componentEngine.Refresh(canvas, screenWidth, screenHeight, _rootPageEntityId);

            // 4. THE MAGIC: This tells Android to refresh as fast as the screen allows
            // On a modern phone, this will trigger 120 times per second.
            Invalidate();
        }
    }
}
