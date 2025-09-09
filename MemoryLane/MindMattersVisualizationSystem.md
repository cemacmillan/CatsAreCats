# Mind Matters Visualization System Design

## Overview
A comprehensive visualization system for Mind Matters that displays the complex psychological networks and thought processes of pawns in an intuitive, interactive graph format.

## Core Concept
Create a "Psyche Web" that visualizes:
- **Nodes**: Pawns, thoughts, thought complexes, mystery boxes (unknown factors)
- **Edges**: Relationships, influences, thought connections
- **Visual States**: Color coding, size, animation to show intensity and relationships

## Technical Architecture

### 1. Base Window System
```csharp
public class Window_PsycheWeb : Window
{
    private PsycheWebGraph graph;
    private Pawn selectedPawn;
    private PsycheWebMode currentMode;
    
    public override Vector2 InitialSize => new Vector2(800f, 600f);
    public override void DoWindowContents(Rect rect) { /* Graph rendering */ }
}
```

### 2. Graph Data Structure
```csharp
public class PsycheWebGraph
{
    public List<PsycheNode> nodes = new List<PsycheNode>();
    public List<PsycheEdge> edges = new List<PsycheEdge>();
    
    public void AddNode(PsycheNode node) { /* Add node logic */ }
    public void AddEdge(PsycheNode from, PsycheNode to, PsycheEdgeType type) { /* Add edge logic */ }
    public void UpdateGraph(Pawn pawn) { /* Update based on pawn's psyche */ }
}

public class PsycheNode
{
    public PsycheNodeType type; // Pawn, Thought, ThoughtComplex, Mystery
    public string label;
    public Color color;
    public float size;
    public Vector2 position;
    public object data; // Pawn, ThoughtDef, etc.
}

public class PsycheEdge
{
    public PsycheNode from;
    public PsycheNode to;
    public PsycheEdgeType type; // Influence, Relationship, ThoughtConnection
    public float strength;
    public Color color;
}
```

### 3. Rendering System
```csharp
public class PsycheWebRenderer
{
    public void DrawGraph(Rect rect, PsycheWebGraph graph)
    {
        // Draw edges first (behind nodes)
        foreach (var edge in graph.edges)
        {
            DrawEdge(edge);
        }
        
        // Draw nodes on top
        foreach (var node in graph.nodes)
        {
            DrawNode(node);
        }
    }
    
    private void DrawNode(PsycheNode node)
    {
        // Custom node rendering based on type
        switch (node.type)
        {
            case PsycheNodeType.Pawn:
                DrawPawnNode(node);
                break;
            case PsycheNodeType.Thought:
                DrawThoughtNode(node);
                break;
            case PsycheNodeType.ThoughtComplex:
                DrawThoughtComplexNode(node);
                break;
            case PsycheNodeType.Mystery:
                DrawMysteryNode(node);
                break;
        }
    }
}
```

## Visualization Modes

### 1. Pawn-Centric View
- **Center**: Selected pawn
- **Inner Ring**: Active thoughts and thought complexes
- **Outer Ring**: Other pawns and their relationships
- **Connections**: Influence lines showing how thoughts affect relationships

### 2. Thought Network View
- **Nodes**: All active thoughts across all pawns
- **Clusters**: Grouped by thought complexes
- **Connections**: How thoughts influence each other
- **Colors**: Mood/impact coding

### 3. Social Network View
- **Nodes**: All pawns
- **Edges**: Social relationships
- **Thickness**: Relationship strength
- **Colors**: Relationship type (romance, friendship, rivalry, etc.)

### 4. Mystery Box View
- **Nodes**: Known factors (pawns, thoughts)
- **Mystery Boxes**: Unknown influences
- **Connections**: Hypothetical relationships
- **Purpose**: Help players understand what they don't know

## Node Types and Visual Design

### Pawn Nodes
```csharp
public class PawnNode : PsycheNode
{
    public Pawn pawn;
    public float moodLevel;
    public List<ThoughtDef> activeThoughts;
    
    public override void Draw(Rect rect)
    {
        // Draw pawn portrait
        // Color based on mood
        // Size based on importance/influence
        // Border indicating mental state
    }
}
```

### Thought Nodes
```csharp
public class ThoughtNode : PsycheNode
{
    public ThoughtDef thoughtDef;
    public float intensity;
    public ThoughtComplex complex;
    
    public override void Draw(Rect rect)
    {
        // Draw thought icon
        // Color based on mood impact
        // Size based on intensity
        // Pulsing animation for active thoughts
    }
}
```

### Thought Complex Nodes
```csharp
public class ThoughtComplexNode : PsycheNode
{
    public ThoughtComplex complex;
    public List<ThoughtDef> constituentThoughts;
    
    public override void Draw(Rect rect)
    {
        // Draw complex shape (hexagon, circle, etc.)
        // Color based on overall impact
        // Size based on complexity
        // Inner structure showing constituent thoughts
    }
}
```

### Mystery Box Nodes
```csharp
public class MysteryBoxNode : PsycheNode
{
    public string mysteryType; // "Unknown Influence", "Hidden Factor", etc.
    public float uncertaintyLevel;
    
    public override void Draw(Rect rect)
    {
        // Draw question mark or mystery symbol
        // Pulsing animation
        // Color based on uncertainty level
        // Tooltip with hints about what it might be
    }
}
```

## Edge Types and Visual Design

### Influence Edges
- **Direction**: Arrow showing influence direction
- **Thickness**: Strength of influence
- **Color**: Type of influence (positive/negative)
- **Animation**: Pulsing or flowing effect

### Relationship Edges
- **Style**: Solid line for strong, dashed for weak
- **Color**: Relationship type
- **Thickness**: Relationship strength
- **Animation**: Subtle movement

### Thought Connection Edges
- **Style**: Curved lines
- **Color**: Connection type
- **Thickness**: Connection strength
- **Animation**: Thought flow visualization

## Interactive Features

### 1. Node Interaction
- **Click**: Select node, show details
- **Hover**: Show tooltip with information
- **Drag**: Move nodes (in edit mode)
- **Right-click**: Context menu with actions

### 2. View Controls
- **Zoom**: Mouse wheel or zoom controls
- **Pan**: Click and drag background
- **Filter**: Show/hide different node types
- **Layout**: Auto-arrange, force-directed, hierarchical

### 3. Information Panels
- **Details Panel**: Shows detailed information about selected node
- **Legend**: Explains colors, symbols, and meanings
- **Timeline**: Shows how the network changes over time

## Integration with Mind Matters

### 1. Data Sources
```csharp
public class MindMattersDataProvider
{
    public PsycheWebGraph BuildGraph(Pawn pawn)
    {
        var graph = new PsycheWebGraph();
        
        // Add pawn node
        graph.AddNode(new PawnNode(pawn));
        
        // Add active thoughts
        foreach (var thought in pawn.needs.mood.thoughts.memories)
        {
            graph.AddNode(new ThoughtNode(thought));
            graph.AddEdge(pawn, thought, PsycheEdgeType.ActiveThought);
        }
        
        // Add thought complexes
        foreach (var complex in GetThoughtComplexes(pawn))
        {
            graph.AddNode(new ThoughtComplexNode(complex));
            // Add connections to constituent thoughts
        }
        
        // Add social relationships
        foreach (var otherPawn in GetRelatedPawns(pawn))
        {
            graph.AddNode(new PawnNode(otherPawn));
            graph.AddEdge(pawn, otherPawn, PsycheEdgeType.SocialRelationship);
        }
        
        return graph;
    }
}
```

### 2. Real-time Updates
- **Tick-based updates**: Update graph every few ticks
- **Event-driven updates**: Update when thoughts change
- **Smooth transitions**: Animate changes over time

### 3. Performance Optimization
- **Level of detail**: Show fewer nodes when zoomed out
- **Culling**: Don't render off-screen nodes
- **Caching**: Cache graph data between updates

## Implementation Phases

### Phase 1: Basic Window and Node System
- Create basic window structure
- Implement simple node rendering
- Add basic interaction (click, hover)

### Phase 2: Graph Layout and Rendering
- Implement force-directed layout
- Add edge rendering
- Implement zoom and pan

### Phase 3: Mind Matters Integration
- Connect to Mind Matters data
- Add thought and relationship nodes
- Implement real-time updates

### Phase 4: Advanced Features
- Add mystery box nodes
- Implement multiple view modes
- Add animation and transitions

### Phase 5: Polish and Optimization
- Improve performance
- Add more visual effects
- Implement advanced interactions

## Technical Considerations

### 1. Unity GUI System
- Use `GUI.BeginGroup` and `GUI.EndGroup` for clipping
- Implement custom mouse handling for interactions
- Use `Event.current` for input handling

### 2. Performance
- Limit number of visible nodes
- Use object pooling for nodes and edges
- Implement efficient collision detection

### 3. Mod Compatibility
- Use Harmony patches for integration
- Avoid conflicts with other UI mods
- Provide configuration options

## Conclusion

This visualization system would provide players with an intuitive way to understand the complex psychological networks in Mind Matters. By showing the relationships between pawns, thoughts, and unknown factors, players can better understand and manage their colony's mental health.

The system is designed to be modular and extensible, allowing for future enhancements and integration with other mods. The visual design emphasizes clarity and usability while providing rich information about the psychological state of the colony.
