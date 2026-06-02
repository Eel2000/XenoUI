using System.Runtime.InteropServices;

namespace XenoUI.Core.Pillars;

/// <summary>
/// Represents a low-level memory buffer designed for efficient blueprint rendering and storage
/// within the XenoUI Core framework. This class is optimized for unsafe operations and provides
/// mechanisms to handle blueprint data that is crucial in the UI rendering pipeline.
/// </summary>
/// <remarks>
/// The BlueprintBuffer class is intended to operate in scenarios requiring manual memory management
/// and raw access to blueprint data.
/// Use this class with caution as it performs unsafe operations to achieve high performance.
/// Proper handling is critical to avoid memory leaks or corruption.
/// </remarks>
/// <threadsafety>
/// BlueprintBuffer instances are not thread-safe. Synchronization mechanisms must be implemented
/// externally if multiple threads access the same instance.
/// </threadsafety>
public unsafe class BlueprintBuffer : IDisposable
{
    private readonly Blueprint* _ptr;
    private readonly int _capacity;
    private int _index;

    public BlueprintBuffer(int capacity = 10_000)
    {
        _capacity = capacity;
        
        // Allocate unmanaged memory once for the lifetime of the app
        _ptr =(Blueprint*)NativeMemory.Alloc((nuint)(capacity * sizeof(Blueprint)));
    }

    /// <summary>
    /// Adds a blueprint to the internal buffer.
    /// </summary>
    /// <param name="blueprint">
    /// The blueprint to be added to the buffer. This parameter represents a single UI blueprint
    /// that contains layout and theme-related information.
    /// </param>
    /// <exception cref="StackOverflowException">
    /// Thrown when the buffer's capacity is exceeded. To prevent this, increase the buffer capacity
    /// during the initialization of the <c>BlueprintBuffer</c>.
    /// </exception>
    public void Push(Blueprint blueprint)
    {
        if (_index >= _capacity)
            throw new StackOverflowException("XenoUI: Blueprint Buffer Overflow. Increase capacity in Sdk.props."); 
        
        _ptr[_index++] = blueprint;
    }

    /// <summary>
    /// Gets a <see cref="BlueprintStream"/> that represents a slice of the current data stored
    /// in the <see cref="BlueprintBuffer"/>, starting from the beginning of the buffer up to the
    /// current index.
    /// </summary>
    /// <remarks>
    /// This property provides a view into the active section of the buffer without performing
    /// additional allocations or copies. It enables efficient access to the buffered blueprint
    /// data and supports zero-copy iteration through the underlying unmanaged memory.
    /// The resulting <see cref="BlueprintStream"/> is a stack-allocated, ref struct construct,
    /// which can only be used within the current execution context.
    /// </remarks>
    /// <threadsafety>
    /// The returned <see cref="BlueprintStream"/> maintains a reference to the current buffer data and
    /// is not thread-safe. Ensure external synchronization if it is accessed within multiple threads.
    /// </threadsafety>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if the <see cref="BlueprintBuffer"/> is disposed before accessing the slice.
    /// </exception>
    public BlueprintStream Slice
        => new BlueprintStream(_ptr, _index);

    /// <summary>
    /// Resets the internal buffer by clearing all stored blueprints and resetting the index to zero.
    /// </summary>
    /// <remarks>
    /// This method does not deallocate memory or reduce the buffer's capacity.
    /// The buffer remains allocated and can be reused to store new blueprints.
    /// Use this method to efficiently clear the buffer without incurring the overhead of
    /// memory allocation or deallocation.
    /// </remarks>
    public void Clear() => _index = 0;
    
    public void Dispose()
    {
        NativeMemory.Free(_ptr);
        GC.SuppressFinalize(this);
    }
    
    ~BlueprintBuffer() => Dispose();
}