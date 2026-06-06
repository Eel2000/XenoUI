using Android.Content;
using SkiaSharp;
using SkiaSharp.Views.Android;
using XenoUI.Core.Compisitor;

namespace XenoUI.Android.Xeno
{
    public class XenoView : SKGLSurfaceView
    {
        private readonly ComponentEngine _componentEngine;

        public XenoView(Context context) : base(context)
        {
            // Initialize the ComponentEngine instance to manage UI components and rendering logic for the XenoView.
            _componentEngine = new();

            // let test first
            _componentEngine.CreateButton(400, 400, 0xFFCC7A00); // Create a button with specified width, height, and background color.
        }

        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            // Obtain the SKCanvas from the SKPaintGLSurfaceEventArgs to perform drawing operations on the surface of the XenoView.
            var canvas = e.Surface.Canvas;


            // Clear the GPU buffer
            canvas.Clear(SKColors.Black);

            // Get Screen info
            var screenWidth = e.Info.Width;
            var screenHeight = e.Info.Height;

            // Update the ComponentEngine with the current screen dimensions to calculate layout and prepare for rendering.
            _componentEngine.Update(canvas, screenWidth, screenHeight);

            // 4. THE MAGIC: This tells Android to refresh as fast as the screen allows
            // On a modern phone, this will trigger 120 times per second.
            Invalidate();
        }
    }
}
