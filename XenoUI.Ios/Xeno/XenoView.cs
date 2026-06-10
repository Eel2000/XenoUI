using SkiaSharp.Views.iOS;
using XenoUI.Core.Compisitor;
using XenoUI.Core.Pillars;

namespace XenoUI.Ios.Xeno;

public class XenoView : UIViewController
{
    private SKCanvasView _canvasView;
    private XenoEngine _componentEngine;
    private int _rootPageEntityId;

    public override void LoadView()
    {
        base.LoadView();
        
        // instantiate skia native IOS view
        _canvasView = new(View.Frame)
        {
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
        };
        
        View = _canvasView;
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        
        _componentEngine = new();
        
        var scalFactor = (float)UIScreen.MainScreen.Scale;
        
        _componentEngine.SetDensity(scalFactor);
        
        int rootPageId = Compose.Column(_componentEngine, gap: 20f,
            Compose.Box(_componentEngine, width: 200f, height: 50f, backgroundColor: 0xFFFF0000), // Red header box
            Compose.Row(_componentEngine, gap: 10f,
                Compose.Box(_componentEngine, width: 90f, height: 90f, backgroundColor: 0xFF00FF00), // Green box left
                Compose.Box(_componentEngine, width: 90f, height: 90f, backgroundColor: 0xFF0000FF)  // Blue box right
            )
        );
        
        _rootPageEntityId = rootPageId;

        // 5. Bind the Skia paint surface event straight to our rendering loop handoff
        _canvasView.PaintSurface += OnPaintSurface;
    }
    
    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        // 6. Extract the backing surface physical pixel dimensions
        float physicalWidth = e.Info.Width;
        float physicalHeight = e.Info.Height;

        // 7. Fire the Engine Heartbeat! 
        // Skia canvas, physical sizes, and the compiled page ID are pushed downstream
        _componentEngine.Refresh(e.Surface.Canvas, physicalWidth, physicalHeight, _rootPageEntityId);
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Unbind the canvas event to prevent iOS memory footprint leaks
            _canvasView.PaintSurface -= OnPaintSurface;
            // _componentEngine.Clear(); // to be added
        }
        base.Dispose(disposing);
    }
}