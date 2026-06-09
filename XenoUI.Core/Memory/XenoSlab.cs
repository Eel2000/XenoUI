using System.Runtime.InteropServices;

namespace XenoUI.Core.Memory;

/// <summary>
/// A memory-efficient, unmanaged slab allocator designed for handling large numbers of value types in contiguous memory.
/// Provides direct access and dynamic resizing capabilities.
/// </summary>
/// <typeparam name="T">
/// The unmanaged value type that the slab will store. Must be a struct.
/// </typeparam>
public unsafe class XenoSlab<T> : IDisposable where T : unmanaged
{
    private T* _buffer;
    private int _capacity;
    private int _count;

    public XenoSlab(int initialCapacity = 1014)
    {
        _capacity = initialCapacity;
        
        // Why NativeMemory? It's the fastest way to get raw RAM in .NET.
        // It's like malloc in C++.
        _buffer = (T*)NativeMemory.Alloc((nuint)(sizeof(T) * _capacity));
    }

    /// <summary>
    /// Gets the total number of elements currently stored in the slab.
    /// </summary>
    /// <value>
    /// An integer representing the count of elements in the slab.
    /// </value>
    /// <remarks>
    /// The value of <c>Count</c> reflects the current number of elements stored,
    /// which is updated as elements are added or removed. It does not represent
    /// the total capacity of the slab.
    /// </remarks>
    public int Count => _count;

    /// <summary>
    /// Retrieves a reference to the element at the specified index within the buffer.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the element to retrieve. Must be within the bounds of the allocated buffer.
    /// </param>
    /// <remarks>
    /// This 'ref' is critical. It allows us to modify the struct directly in the slab
    /// without copying it to the stack first. This is a key performance optimization, especially for large structs.
    /// </remarks>
    /// <returns>
    /// A reference to the element of type <typeparamref name="T"/> at the specified index.
    /// </returns>
    public ref T Get(int index) => ref _buffer[index];


    /// <summary>
    /// Adds a new element to the buffer and returns its position within the buffer.
    /// </summary>
    /// <param name="value">
    /// The value of type <typeparamref name="T"/> to be added to the buffer.
    /// </param>
    /// <returns>
    /// The zero-based index at which the element was added within the buffer.
    /// </returns>
    public int Push(T value)
    {
        if (_count >= _capacity) Resize();

        _buffer[_count] = value;
        return _count++;
    }

    /// <summary>
    /// Expands the capacity of the buffer to accommodate additional elements.
    /// </summary>
    /// <remarks>
    /// This method doubles the current capacity of the buffer and allocates a new memory block large enough to hold the expanded capacity.
    /// The existing elements are copied from the old buffer to the new buffer. The old buffer is then freed to release its allocated memory.
    /// </remarks>
    private void Resize()
    {
        int newCapacity = _capacity * 2;
        T* newBuf = (T*)NativeMemory.Alloc((nuint)(sizeof(T) * newCapacity));
        
        //copy the old data to this new buffer
        NativeMemory.Copy(_buffer, newBuf, (nuint)(_capacity * sizeof(T)));
        
        // free the old buffer
        NativeMemory.Free(_buffer);
        
        // update the buffer and capacity
        _buffer = newBuf;
        _capacity = newCapacity;
    }

    public void Dispose()
    {
        // Free the allocated memory when the slab is disposed. This is crucial to prevent memory leaks, as we're manually managing memory here.
        if (_buffer != null) NativeMemory.Free(_buffer);
    }
}