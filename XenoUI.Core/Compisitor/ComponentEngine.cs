using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using XenoUI.Core.Components.Primary;
using XenoUI.Core.Doddb;
using XenoUI.Core.Pillars;

namespace XenoUI.Core.Compisitor;

/// <summary>
/// Represents the core engine responsible for managing UI components, their layout,
/// rendering, and lifecycle within the XenoUI framework.
/// This class acts as a central hub for interacting with and orchestrating the various
/// systems involved in the UI rendering pipeline, including caching, layout, and rendering mechanisms.
/// </summary>
public class ComponentEngine
{
    /// <summary>
    /// Represents the memory cache for UI components, storing relevant data and
    /// state information for efficient access and management of UI elements within the component engine.
    /// </summary>
    public readonly XenoUICacheMemory UiCacheMemory;

    /// <summary>
    /// Represents the layout system responsible for managing the layout and positioning of UI elements.
    /// </summary>
    public readonly LayoutSystem LayoutSystem;


    /// <summary>
    /// Represents the render system responsible for rendering UI elements based on the data stored in the UI 
    /// cache memory and the layout information provided by the layout system.
    /// </summary>
    public readonly RenderSystem RenderSystem;

    public ComponentEngine()
    {
        UiCacheMemory = new();
        LayoutSystem = new();
        RenderSystem = new();
    }

    /// <summary>
    /// Creates a button with the specified dimensions and background color,
    /// and registers it within the system for use in the UI layout.
    /// </summary>
    /// <param name="width">The width of the button in logical units.</param>
    /// <param name="height">The height of the button in logical units.</param>
    /// <param name="backgroundColor">The background color of the button, specified as a 32-bit unsigned integer in ARGB format.</param>
    /// <returns>The unique identifier of the created button layout entity.</returns>
    public int CreateButton(float width, float height, uint backgroundColor)
    {
        //Create a new StyleComponent for the button with the specified properties
        var style = new StyleComponent
        {
            Width = width,
            Height = height,
            BackgroundColor = backgroundColor
        };

        // Store the StyleComponent in the UiCacheMemory and get its ID
        var buttonLayoutId = UiCacheMemory.CreateEntity(style);

        // Register the button layout with the LayoutSystem using the obtained ID
        LayoutSystem.RegisterEntity(buttonLayoutId);

        // Return the ID of the created button layout for further reference
        return buttonLayoutId;
    }
    
    /// <summary>
    /// Adds a child entity to a parent entity within the layout system, establishing a hierarchical
    /// relationship between them. The child is appended as the last child of the specified parent.
    /// </summary>
    /// <param name="parentId">
    /// The unique identifier of the parent entity to which the child entity will be attached.
    /// </param>
    /// <param name="childId">
    /// The unique identifier of the child entity that will be added to the specified parent.
    /// </param>
    /// <returns>
    /// Returns the identifier of the parent entity after the child has been added.
    /// </returns>
    public int AddChild(int parentId, int childId)
    {
        LayoutSystem.RegisterChild(parentId, childId);
        return parentId; // return parent for
    }

    /// <summary>
    /// Updates the UI component engine by applying styles, calculating layouts, and harvesting layout results.
    /// This method ensures the UI components are properly configured and laid out based on the current screen dimensions.
    /// </summary>
    /// <param name="screenWidth">The width of the screen in logical units, used to calculate the layout.</param>
    /// <param name="screenHeight">The height of the screen in logical units, used to calculate the layout.</param>
    public void Update(float screenWidth, float screenHeight)
    {
        // Apply styles from the UI cache memory to the layout system
        LayoutSystem.ApplyStyles(UiCacheMemory);

        // Calculate the layout based on the current screen dimensions
        LayoutSystem.CalculateLayout(screenWidth, screenHeight);

        // Pull the layout results back into the UI cache memory for rendering or further processing
        LayoutSystem.HarvestResults(UiCacheMemory); //TODO: rename the HarvestResults method to something more intuitive like UpdateTransforms or ApplyLayoutResults
    }

    /// <summary>
    /// Updates the component engine by applying styles, calculating layout, harvesting results,
    /// and rendering the UI elements on the specified canvas based on the current screen dimensions.
    /// </summary>
    /// <param name="canvas">An SKCanvas object representing the GPU canvas where the UI elements are rendered.</param>
    /// <param name="screenWidth">The width of the screen in logical pixels used for layout calculation.</param>
    /// <param name="screenHeight">The height of the screen in logical pixels used for layout calculation.</param>
    public void Update(SKCanvas canvas, float screenWidth, float screenHeight)
    {
        // Apply styles from the UI cache memory to the layout system
        LayoutSystem.ApplyStyles(UiCacheMemory);

        // Calculate the layout based on the current screen dimensions
        LayoutSystem.CalculateLayout(screenWidth, screenHeight);

        // Pull the layout results back into the UI cache memory for rendering or further processing
        LayoutSystem.HarvestResults(UiCacheMemory);

        // Render the UI elements on the canvas using the render system and the updated UI cache memory
        RenderSystem.Draw(canvas, UiCacheMemory);
    }
}
