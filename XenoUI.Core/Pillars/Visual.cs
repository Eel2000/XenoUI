using XenoUI.Core.Platforms;

namespace XenoUI.Core.Pillars;

public abstract class Visual : IDisposable
{
    public uint Id { get; internal set; }
    
    // Persistent Bounds
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }

    // THE NATIVE LINK: Pointer to the actual Android.View or UIView.
    // We use nint (native int) to be cross-platform and zero-allocation.
    public nint NativeHandle { get; protected set; }

    // Hierarchy management
    public Visual? Parent { get; internal set; }
    public List<Visual> Children { get; } = new();

    public void SyncNativeTransform()
    {
        if(NativeHandle == nint.Zero) return;
        
        // Call the bridge to move the physical OS view. This is where the actual platform-specific magic happens.
        PlatformBridge.MoveNativeView(NativeHandle, X, Y, Width, Height);
    }
    
    /// <summary>
    /// Applies specialized data from the Blueprint (e.g. Text string, Button color).
    /// Note: 'ref Blueprint' is legal as a parameter in a class method.
    /// </summary>
    public abstract void OnApplyBlueprint(ref Blueprint bp);
    
    public void Dispose()
    {
        if (NativeHandle != nint.Zero)
        {
            PlatformBridge.RemoveNativeView(NativeHandle);
            NativeHandle = nint.Zero;
        }
        
        foreach (var child in Children) child.Dispose();
        Children.Clear();
    }
}