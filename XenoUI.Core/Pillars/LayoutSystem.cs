using Facebook.Yoga;
using XenoUI.Core.Components.Enums;
using XenoUI.Core.Doddb;

namespace XenoUI.Core.Pillars;

/// <summary>
/// Represents a layout management system designed to handle the hierarchical arrangement
/// and visual layout calculations for UI entities within a user interface framework.
/// Provides functionality for entity registration, style application, layout calculation,
/// and result harvesting.
/// </summary>
public class LayoutSystem
{
    /// <summary>
    /// Represents a collection of layout nodes managed by the layout system,
    /// explicitly mapped by their unique Entity ID.
    /// </summary>
    private readonly Dictionary<int, Node> _nodes = new();

    /// <summary>
    /// Serves as the primary node in the layout system hierarchy.
    /// This node acts as the root for all layout calculations and
    /// provides the foundational context for organizing and rendering
    /// child nodes within the system.
    /// </summary>
    private readonly Node _rootNode;

    public LayoutSystem()
    {
        _rootNode = new Node();
    }

    /// <summary>
    /// Registers an entity in the layout system by creating a new node for it.
    /// Hierarchy is established later via RegisterChild due to bottom-up execution.
    /// </summary>
    /// <param name="entityId">
    /// The unique identifier for the entity to be registered.
    /// </param>
    /// <param name="parentEntityId">
    /// The unique identifier for the parent entity to which the new entity will be attached.
    /// </param>
    public void RegisterEntity(int entityId, int? parentEntityId = null)
    {
        var newNode = new Node();
        _nodes[entityId] = newNode;

        if (parentEntityId.HasValue)
        {
            RegisterChild(parentEntityId.Value, entityId);
        }
    }

    /// <summary>
    /// Registers a child entity to a parent entity within the layout system, establishing
    /// a hierarchical relationship between the two entities. The child will be appended
    /// as the last child of the specified parent.
    /// </summary>
    /// <param name="parentId">
    /// The unique identifier of the parent entity to which the child entity will be attached.
    /// </param>
    /// <param name="childId">
    /// The unique identifier of the child entity that will be registered and attached to the specified parent entity.
    /// </param>
    public void RegisterChild(int parentId, int childId)
    {
        if (_nodes.TryGetValue(parentId, out var parentNode) &&
            _nodes.TryGetValue(childId, out var childNode))
        {
            // If the child was previously attached to the root node during 
            // initialization, we must disconnect it before nesting it.
            // (Safe protection for bottom-up compilation engines)
            _rootNode.RemoveChild(childNode);

            parentNode.InsertChild(childNode, parentNode.GetChildCount());
        }
    }

    /// <summary>
    /// Applies style properties to layout nodes based on the styling data from the provided UI cache memory.
    /// Each UI entity's style is extracted and mapped to its corresponding layout node, updating attributes such as width and height.
    /// </summary>
    /// <param name="uiCacheMemory">
    /// The UI cache memory containing the style components for all UI entities.
    /// It provides access to style data and ensures alignment between the UI system and the layout system.
    /// </param>
    public void ApplyStyles(XenoUICacheMemory uiCacheMemory)
    {
        for (var index = 0; index < uiCacheMemory.EntityCount; index++)
        {
            // Ensure the node exists before modifying it
            if (!_nodes.TryGetValue(index, out var node)) continue;

            // Get the style component for the entity
            ref var style = ref uiCacheMemory.Styles.Get(index);

            // Map our slab values to Yoga properties
            YGNodeStyleAPI.YGNodeStyleSetWidth(node, style.Width);
            YGNodeStyleAPI.YGNodeStyleSetHeight(node, style.Height);

            // Layout direction
            var layoutType = style.LayoutDirection == LayoutDirection.Horizontal ?
                YGFlexDirection.Row :
                YGFlexDirection.Column;
            YGNodeStyleAPI.YGNodeStyleSetFlexDirection(node, layoutType);

            // Gap between children
            var calculatedGutter = layoutType == YGFlexDirection.Row ? YGGutter.Row : YGGutter.Column;
            YGNodeStyleAPI.YGNodeStyleSetGap(node, calculatedGutter, style.Gap);

            // Set paddings
            YGNodeStyleAPI.YGNodeStyleSetPadding(node, YGEdge.Left, style.PaddingLeft);
            YGNodeStyleAPI.YGNodeStyleSetPadding(node, YGEdge.Top, style.PaddingTop);
            YGNodeStyleAPI.YGNodeStyleSetPadding(node, YGEdge.Right, style.PaddingRight);
            YGNodeStyleAPI.YGNodeStyleSetPadding(node, YGEdge.Bottom, style.PaddingBottom);

            // Set margins
            YGNodeStyleAPI.YGNodeStyleSetMargin(node, YGEdge.Left, style.MarginLeft);
            YGNodeStyleAPI.YGNodeStyleSetMargin(node, YGEdge.Top, style.MarginTop);
            YGNodeStyleAPI.YGNodeStyleSetMargin(node, YGEdge.Right, style.MarginRight);
            YGNodeStyleAPI.YGNodeStyleSetMargin(node, YGEdge.Bottom, style.MarginBottom);

            // Allow nodes to grow if configured (Crucial for Scaffold/Fill layout behaviors)
            YGNodeStyleAPI.YGNodeStyleSetFlexGrow(node, 1.0f);// Default to allowing all nodes to grow, but this can be adjusted based on specific style properties if needed.

            // Center alignment rule
            YGNodeStyleAPI.YGNodeStyleSetAlignItems(node, YGAlign.Center);
        }
    }

    /// <summary>
    /// Attaches the final top-level layout element of the active page directly to the Root node.
    /// </summary>
    public void SetRootPageEntity(int rootPageEntityId)
    {
        if (_nodes.TryGetValue(rootPageEntityId, out var rootPageNode))
        {
            // Clear any old page roots left in the root node
            _rootNode.Reset();
            _rootNode.InsertChild(rootPageNode, 0);
        }
    }

    /// <summary>
    /// Calculates the layout of the root node and its child nodes based on the specified screen dimensions.
    /// The layout calculations determine the positioning and sizing of UI elements within the layout system.
    /// </summary>
    /// <param name="screenWidth">
    /// The width of the screen or container, used as the basis for layout calculations.
    /// </param>
    /// <param name="screenHeight">
    /// The height of the screen or container, used as the basis for layout calculations.
    /// </param>
    /// <param name="density">
    /// The screen density factor, used to convert layout units to physical pixels.
    /// </param>
    public void CalculateLayout(float screenWidth, float screenHeight, float density)
    {
        // Convert incoming physical window dimensions to DP values for Yoga's calculation tree
        float dpWidth = screenWidth / density;
        float dpHeight = screenHeight / density;

        // Force the root container to match the exact physical glass size of the phone
        YGNodeStyleAPI.YGNodeStyleSetWidth(_rootNode, dpWidth);
        YGNodeStyleAPI.YGNodeStyleSetHeight(_rootNode, dpHeight);

        // Calculate the layout properties recursively down the entire tree
        YGNodeAPI.YGNodeCalculateLayout(_rootNode, dpWidth, dpHeight, YGDirection.LTR);
    }

    /// <summary>
    /// Updates the layout transformations for all registered entities by extracting the computed
    /// layout properties (position and size) and applying them to the corresponding transform components
    /// in the given UI cache memory.
    /// </summary>
    /// <param name="uiCacheMemory">
    /// The UI cache memory containing the transform components to be updated.
    /// </param>
    public void HarvestResults(XenoUICacheMemory uiCacheMemory)
    {
        for (var index = 0; index < uiCacheMemory.EntityCount; index++)
        {
            if (!_nodes.TryGetValue(index, out var node)) continue;

            ref var transform = ref uiCacheMemory.Transforms.Get(index);

            // FIXED: Harvest the actual calculated LAYOUT metrics instead of the input STYLES
            var w = YGNodeStyleAPI.YGNodeStyleGetWidth(node);
            var h = YGNodeStyleAPI.YGNodeStyleGetHeight(node);

            var layoutX = YGNodeLayoutAPI.YGNodeLayoutGetLeft(node);
            var layoutY = YGNodeLayoutAPI.YGNodeLayoutGetTop(node);

            transform.Width = w.Value;
            transform.Height = h.Value;
            transform.X = layoutX;
            transform.Y = layoutY;
        }
    }

    /// <summary>
    /// Completely flushes the layout nodes tree. Called during page transitions 
    /// to prevent memory footprint leaks.
    /// </summary>
    public void Clear()
    {
        _nodes.Clear();
        _rootNode.Reset();
    }
}