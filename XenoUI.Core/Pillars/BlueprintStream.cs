namespace XenoUI.Core.Pillars;

/// <summary>
/// A compiler-safe, non-generic alternative to Span<T> for ref structs.
/// Wraps unmanaged memory and allows fast, zero-copy iteration.
/// </summary>
public unsafe readonly ref struct BlueprintStream
{
    private readonly Blueprint* _ptr;
    public int Length {get;}

    public BlueprintStream(Blueprint* ptr, int length)
    {
        _ptr = ptr;
        Length = length;
    }
    
    public ref Blueprint this[int index]
    {
        get
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException($"Index {index} is out of range for BlueprintStream of length {Length}.");
            return ref _ptr[index];
        }
    }
}