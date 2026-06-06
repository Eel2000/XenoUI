using Facebook.Yoga;
using XenoUI.Core.Doddb;

namespace XenoUI.Core.Pillars;

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
            
            // We can add more later (Padding, Margin, FlexDirection)
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