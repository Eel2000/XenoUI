using XenoUI.Core.Components.Primary;
using XenoUI.Core.Memory;

namespace XenoUI.Core.Doddb;

/// <summary>
/// Manages UI component memory in an efficient and structured manner.
/// This class combines styling and transform data for UI elements using
/// specialized memory management systems to ensure performance and scalability.
/// </summary>
public class XenoUICacheMemory : IDisposable
{
    /// <summary>
    /// Represents a memory-efficient slab allocator that manages a collection
    /// of <see cref="StyleComponent"/> structures in contiguous memory.
    /// Used within the <see cref="XenoUICacheMemory"/> class to store styling information
    /// for UI components.
    /// </summary>
    public readonly XenoSlab<StyleComponent> Styles;

    /// <summary>
    /// Represents a memory-efficient slab allocator that manages a collection
    /// of <see cref="TransformComponent"/> structures in contiguous memory.
    /// Used within the <see cref="XenoUICacheMemory"/> class to store transform data
    /// for UI components, such as position and dimensions in a 2D space.
    /// </summary>
    public readonly XenoSlab<TransformComponent> Transforms;


    public readonly XenoSlab<VisualComponent> Visuals;

    /// <summary>
    /// Gets the total number of entities currently managed within the <see cref="Styles"/> slab allocator.
    /// Provides a count of <see cref="StyleComponent"/> structures being stored in contiguous memory.
    /// </summary>
    public int EntityCount => Styles.Count;

    public XenoUICacheMemory(int initialCapacity = 1024)
    {
        Styles = new XenoSlab<StyleComponent>(initialCapacity);
        Visuals = new XenoSlab<VisualComponent>(initialCapacity);
        Transforms = new XenoSlab<TransformComponent>(initialCapacity);
    }


    /// <summary>
    /// Creates a new entity in the UI cache memory by adding a style component to the
    /// styles slab and a corresponding default transform component to the transforms slab.
    /// </summary>
    /// <param name="style">
    /// The <see cref="StyleComponent"/> that defines the styling properties for the new entity.
    /// </param>
    /// <returns>
    /// The zero-based index of the newly created entity within the styles slab.
    /// </returns>
    public int CreateEntity(StyleComponent style)
    {
        int id = Styles.Push(style);

        //keep the slabs aligned
        Transforms.Push(default);
        
        return id;
    }

    public int CreateEntity(StyleComponent style, VisualComponent visual)
    {
        //Create a new entity in the UI cache memory by adding a style component to the
        int id = Styles.Push(style);

        //keep the slabs aligned
        Transforms.Push(default);

        // Add a corresponding visual component to the visuals slab for the new entity.
        Visuals.Push(visual);

        return id;
    }

    public void Dispose()
    {
        Styles.Dispose();
        Transforms.Dispose();
        Visuals.Dispose();
    }
}