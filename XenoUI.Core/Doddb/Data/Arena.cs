using System.Runtime.CompilerServices;

namespace XenoUI.Core.Doddb.Data;

/// <summary>
/// The Memory Arena (Arena.cs)
///This is the vault 
/// </summary>
/// <typeparam name="T"></typeparam>
public class Arena<T> where T: unmanaged, IComponent
{
    // A Single, Contiguous block of memory. Highly cache-friendly.
    private T[] _buffer;
    private int _count;
    
    public Arena(int capacity)
    {
        _buffer = new T[capacity];
        _count = 0;
    }

    /// <summary>
    /// Adds a component to the arena, storing it in a contiguous memory block.
    /// If the buffer capacity is exceeded, it is automatically resized to accommodate the new component.
    /// </summary>
    /// <param name="component">The component of type <typeparamref name="T"/> to be added to the arena.</param>
    /// <returns>The index in the arena where the component was added(correspond to the id).</returns>
    public int Add(in T component)
    {
        if (_count >= _buffer.Length)
        {
            // Double the length of the buffer when capacity is reached
            Array.Resize(ref _buffer, _buffer.Length * 2);
        }
        
        int id = _count;
        _buffer[id] = component;
        _count++;
        return id;
    }

    /// <summary>
    /// Retrieves a reference to the component stored at the specified index in the arena's buffer.
    /// This method allows direct access to the component for read or write operations.
    /// The Magic Method: Returns a DIRECT POINTER to the struct in memory. - Modifying this mutates the database instantly with zero allocations.
    /// </summary>
    /// <param name="entityId">The index of the component in the arena's buffer.</param>
    /// <returns>A reference to the component of type <typeparamref name="T"/> at the specified index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(int entityId) => ref _buffer[entityId];
}