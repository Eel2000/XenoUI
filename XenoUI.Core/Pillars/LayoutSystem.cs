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
    /// Represents a collection of layout nodes managed by the layout system.
    /// This list is used to store and organize nodes that participate in the
    /// layout calculations, enabling structured management and rendering of the UI elements.
    /// </summary>
    private readonly List<Node> _nodes = new();

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
    /// Registers an entity in the layout system by creating a new node for it and
    /// attaching it to the specified parent node or to the root node if no parent is specified.
    /// </summary>
    /// <param name="entityId">
    /// The unique identifier for the entity to be registered.
    /// </param>
    /// <param name="parentEntityId">
    /// The unique identifier for the parent entity to which the new entity will be attached.
    /// If null, the entity will be attached to the root node.
    /// </param>
    public void RegisterEntity(int entityId, int? parentEntityId = null)
    {
        // Creates a new node for the entity and adds it to the list of nodes in the layout system.
        var newNode = new Node();
        _nodes.Add(newNode);
        
        if(parentEntityId.HasValue)
        {
            //if a parent is specified, the node is added to the parent's child list'
            var parentNode = _nodes[parentEntityId.Value];
            parentNode.InsertChild(newNode,parentNode.GetChildCount());
        }
        else
        {
            //if no parent is specified, the node is added to the root
            _rootNode.InsertChild(newNode,_rootNode.GetChildCount());
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
        var parentNode = _nodes[parentId];
        var childNode = _nodes[childId];
        parentNode.InsertChild(childNode,parentNode.GetChildCount());
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
            //get the style component for the entity and the corresponding node in the layout system
            var node = _nodes[index];
            
            //get the style component for the entity
            ref var style = ref uiCacheMemory.Styles.Get(index);
            
            //map our slab values to Yoga properties
            YGNodeStyleAPI.YGNodeStyleSetWidth(node, style.Width);
            YGNodeStyleAPI.YGNodeStyleSetHeight(node, style.Height);
            
            //layout direction
            var layoutType = style.LayoutDirection == LayoutDirection.Horizontal ?
                YGFlexDirection.Row : 
                YGFlexDirection.Column;
            YGNodeStyleAPI.YGNodeStyleSetFlexDirection(node, layoutType);
            
            //Gap between children, we need to determine if it's a row or column layout to set the correct gap type
            // Yoga's gutter = gap between children, and it has different types for row and column layouts
            var calculatedGutter = layoutType == YGFlexDirection.Row ? YGGutter.Row : YGGutter.Column;
            YGNodeStyleAPI.YGNodeStyleSetGap(node,calculatedGutter, style.Gap);
            
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
            
            // center everything if the flag is set
            // NOTE: IMPORTANT! in the first time center everything but in the future we need to make it more flexible
            // and allow users to specify how they want to align their children , we will add properties for alignment in the style component and map them to Yoga properties
            YGNodeStyleAPI.YGNodeStyleSetAlignItems(node,YGAlign.Center);
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
    public void CalculateLayout(float screenWidth, float screenHeight)
    {
        // Calculate the layout for the root node, which will recursively calculate the layout for all child nodes.
        YGNodeAPI.YGNodeCalculateLayout(_rootNode, screenWidth, screenHeight, YGDirection.LTR);
    }

    /// <summary>
    /// Updates the layout transformations for all registered entities by extracting the computed
    /// layout properties (position and size) and applying them to the corresponding transform components
    /// in the given UI cache memory.
    /// </summary>
    /// <param name="uiCacheMemory">
    /// The UI cache memory containing the transform components to be updated. Each transform
    /// component will be modified to reflect the calculated layout properties (X, Y, Width, Height)
    /// based on the layout system's computed results for each entity.
    /// </param>
    public void HarvestResults(XenoUICacheMemory uiCacheMemory)
    {
        for (var index = 0; index < uiCacheMemory.EntityCount; index++)
        {
            var node = _nodes[index];
            
            ref var transform = ref uiCacheMemory.Transforms.Get(index);
             
            var width = YGNodeStyleAPI.YGNodeStyleGetHeight(node);
            var height = YGNodeStyleAPI.YGNodeStyleGetWidth(node);

            transform.Width = width.Value;
            transform.Height = height.Value;

            transform.X = YGNodeLayoutAPI.YGNodeLayoutGetLeft(node);
            transform.Y = YGNodeLayoutAPI.YGNodeLayoutGetTop(node);
        }
    }
}