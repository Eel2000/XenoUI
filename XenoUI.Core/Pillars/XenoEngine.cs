using XenoUI.Core.Doddb;

namespace XenoUI.Core.Pillars;

public class XenoEngine
{
    public readonly XenoUICacheMemory XenoUICacheMemory;
    public readonly LayoutSystem LayoutSystem;
    
    public XenoEngine()
    {
        XenoUICacheMemory = new XenoUICacheMemory();
        LayoutSystem = new LayoutSystem();
    }



    public void Dispose()
    {
        XenoUICacheMemory.Dispose();
    }
}