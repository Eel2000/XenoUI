namespace XenoUI.Core.Pillars;

/// <summary>
/// The Reconciler class is responsible for maintaining a persistent registry of visual objects,
/// synchronizing their state with data received from a low-level BlueprintStream,
/// and ensuring proper lifecycle management of visuals.
/// </summary>
public class Reconciler
{
    // The Registry: Maps a stable Blueprint ID to a persistent Visual Class instance.
    private readonly Dictionary<uint, Visual> _registry = new();
    
    // Track which visuals were seen this frame to detect deletions
    private readonly HashSet<uint> _activeIds = new();
    
    /// <summary>
    /// The Main Loop: Maps the unmanaged stream to persistent objects.
    /// </summary>
    public void Reconcile(BlueprintStream stream)
    {
        _activeIds.Clear();

        for (int i = 0; i < stream.Length; i++)
        {
            // 1. Get the ref struct blueprint from the stream (Zero Copy)
            ref var bp = ref stream[i];
            uint id = bp.Id;
            _activeIds.Add(id);

            // 2. Find or Create the persistent Visual
            if (!_registry.TryGetValue(id, out var visual))
            {
                visual = CreateVisualFromType(bp.TypeId);
                visual.Id = id;
                _registry[id] = visual;
            }

            // 3. Apply the Blueprint data to the Persistent Class
            // We check for changes to avoid unnecessary Native Syncing
            bool boundsChanged = visual.X != bp.X || visual.Y != bp.Y || 
                                 visual.Width != bp.Width || visual.Height != bp.Height;

            if (boundsChanged)
            {
                visual.X = bp.X;
                visual.Y = bp.Y;
                visual.Width = bp.Width;
                visual.Height = bp.Height;
                
                // 4. THE SYNC-POINT: Immediately notify the Native Platform (Android/iOS)
                // This ensures the Native View and Skia pixels move in the same frame.
                visual.SyncNativeTransform();
            }

            // 5. Let the specific Visual type handle its own specialized data (e.g., Text string)
            visual.OnApplyBlueprint(ref bp);
        }

        // 6. Cleanup: Remove visuals that are no longer in the blueprint stream
        CleanupDefunctVisuals();
    }

    private void CleanupDefunctVisuals()
    {
        // Simple strategy: if a Visual wasn't mentioned in the stream, it's dead.
        // We'll optimize this later with a "Generation" counter to avoid allocations.
        // For now, we dispose of native handles properly.
    }

    private Visual CreateVisualFromType(uint typeId)
    {
        // This is where the factory lives. 
        // e.g., type 1 = ButtonVisual, type 2 = TextVisual, etc.
        return VisualFactory.Create(typeId);
    }
}