namespace XenoUI.Core.Platforms;

/// <summary>
/// PlatformBridge provides a centralized mechanism for bridging platform-specific functionalities
/// with the core platform-agnostic application logic. This class acts as an adapter to manage
/// native UI components and their interactions across different platforms.
/// </summary>
public class PlatformBridge
{
    // These are hooked by XenoUI.Platforms.Android/Ios at startup
    public static Action<nint, float, float, float, float> MoveNativeView { get; set; } = delegate { };
    public static Action<nint> RemoveNativeView { get; set; } = delegate { };
    public static Func<uint, nint> CreateNativeView { get; set; } = _ => nint.Zero;
}