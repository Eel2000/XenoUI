namespace XenoUI.Core.Doddb.Data;

/// <summary>
/// Provides functionality for managing memory arenas specific to component types.
/// This class facilitates efficient storage and retrieval of components in a contiguous
/// memory structure by associating each component type with a dedicated arena.
/// </summary>
public static class Database
{
    // A dictionary of all the arenas. Stores Arenas keyed by the type of struct they hold
    private static readonly Dictionary<Type, object> _arenas = new();

    /// <summary>
    /// Registers a memory arena for the specified component type, allowing efficient storage and management
    /// of components of the given type. Each component type has its own dedicated arena, backed by a contiguous memory block.
    /// </summary>
    /// <typeparam name="T">
    /// The type of component to register the arena for. This must be an unmanaged type that implements <see cref="IComponent"/>.
    /// </typeparam>
    /// <param name="capacity">
    /// The initial capacity of the arena, which determines the number of components that can
    /// be stored in the arena without resizing.
    /// </param>
    public static void RegisterArena<T>(int capacity) where T : unmanaged, IComponent
    {
        _arenas[typeof(T)] = new Arena<T>(capacity);
    }

    /// <summary>
    /// Retrieves the memory arena for the specified component type, enabling access to the arena's
    /// functionality for storage, modification, and retrieval of components. This method assumes
    /// that an arena has already been registered for the given component type.
    /// </summary>
    /// <typeparam name="T">
    /// The type of component for which the memory arena is retrieved. This must be an unmanaged type
    /// that implements <see cref="IComponent"/>.
    /// </typeparam>
    /// <returns>
    /// The <see cref="Arena{T}"/> instance associated with the specified component type.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if no arena has been registered for the specified component type.
    /// </exception>
    public static Arena<T> GetArena<T>() where T : unmanaged, IComponent
    {
        return (Arena<T>)_arenas[typeof(T)];
    }


    /// <summary>
    /// Retrieves a reference to the component of the specified type associated with the given entity ID.
    /// The returned reference allows direct access to the component for read or write operations,
    /// providing efficient manipulation without additional memory allocations.
    /// </summary>
    /// <typeparam name="T">
    /// The type of component to retrieve. This must be an unmanaged type that implements <see cref="IComponent"/>.
    /// </typeparam>
    /// <param name="entityId">
    /// The unique identifier of the entity whose component is to be retrieved. This serves as the index in the component's storage arena.
    /// </param>
    /// <returns>
    /// A reference to the component of type <typeparamref name="T"/> associated with the specified entity ID.
    /// </returns>
    public static ref T Get<T>(int entityId) where T : unmanaged, IComponent
    {
        return ref GetArena<T>().Get(entityId);
    }
}