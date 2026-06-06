using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using XenoUI.Core.Components.Primary;
using XenoUI.Core.Doddb;
using XenoUI.Core.Pillars;

namespace XenoUI.Core.Compisitor;


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

    public void Update(float screenWidth, float screenHeight)
    {
        // Apply styles from the UI cache memory to the layout system
        LayoutSystem.ApplyStyles(UiCacheMemory);

        // Calculate the layout based on the current screen dimensions
        LayoutSystem.CalculateLayout(screenWidth, screenHeight);

        // Pull the layout results back into the UI cache memory for rendering or further processing
        LayoutSystem.HarvestResults(UiCacheMemory); //TODO: rename the HarvestResults method to something more intuitive like UpdateTransforms or ApplyLayoutResults
    }

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
