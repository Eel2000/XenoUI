using XenoUI.Core.Platforms;

namespace XenoUI.Core.Pillars;

internal static class VisualFactory
{
    public static Visual Create(uint typeId)
    {
        Visual visual = typeId switch
        {
            BuiltInTypes.Button => new ButtonVisual(),
            BuiltInTypes.Text   => new TextVisual(),
            _                   => new ContainerVisual()
        };

        // Create the OS View immediately
        visual.NativeHandle = PlatformBridge.CreateNativeView(1);
        
        return visual;
    }
}