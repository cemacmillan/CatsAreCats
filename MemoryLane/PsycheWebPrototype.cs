using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace MindMatters.Visualization
{
    /// <summary>
    /// Prototype implementation of a Mind Matters visualization system
    /// This creates a simple graph showing pawns, thoughts, and relationships
    /// </summary>
    public class Window_PsycheWeb : Window
    {
        private Pawn selectedPawn;
        private PsycheWebGraph graph;
        private Vector2 scrollPosition;
        private float zoomLevel = 1f;
        private Vector2 panOffset = Vector2.zero;
        private PsycheNode selectedNode;
        
        public override Vector2 InitialSize => new Vector2(800f, 600f);
        
        public Window_PsycheWeb(Pawn pawn = null)
        {
            selectedPawn = pawn ?? Find.CurrentMap?.mapPawns.FreeColonists.FirstOrDefault();
            graph = new PsycheWebGraph();
            layer = WindowLayer.GameUI;
            doCloseX = true;
            doCloseButton = true;
            closeOnClickedOutside = false;
            preventCameraMotion = false;
            
            if (selectedPawn != null)
            {
                BuildGraph();
            }
        }
        
        public override void DoWindowContents(Rect rect)
        {
            // Title
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0, 0, rect.width, 30), "Psyche Web - " + (selectedPawn?.Name?.ToStringShort ?? "No Pawn Selected"));
            Text.Font = GameFont.Small;
            
            // Controls
            DrawControls(new Rect(0, 30, rect.width, 30));
            
            // Main graph area
            Rect graphRect = new Rect(0, 60, rect.width, rect.height - 60);
            DrawGraph(graphRect);
            
            // Details panel
            if (selectedNode != null)
            {
                DrawDetailsPanel(new Rect(rect.width - 200, 60, 200, rect.height - 60));
            }
        }
        
        private void DrawControls(Rect rect)
        {
            // Pawn selector
            if (Widgets.ButtonText(new Rect(0, 0, 150, 25), "Select Pawn"))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (var pawn in Find.CurrentMap?.mapPawns.FreeColonists ?? new List<Pawn>())
                {
                    options.Add(new FloatMenuOption(pawn.Name.ToStringShort, () => SelectPawn(pawn)));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }
            
            // Zoom controls
            if (Widgets.ButtonText(new Rect(160, 0, 50, 25), "Zoom+"))
            {
                zoomLevel = Mathf.Min(zoomLevel * 1.2f, 3f);
            }
            if (Widgets.ButtonText(new Rect(220, 0, 50, 25), "Zoom-"))
            {
                zoomLevel = Mathf.Max(zoomLevel / 1.2f, 0.3f);
            }
            
            // Reset view
            if (Widgets.ButtonText(new Rect(280, 0, 80, 25), "Reset View"))
            {
                zoomLevel = 1f;
                panOffset = Vector2.zero;
            }
        }
        
        private void DrawGraph(Rect rect)
        {
            Widgets.BeginGroup(rect);
            
            // Apply zoom and pan
            GUI.matrix = Matrix4x4.TRS(new Vector3(panOffset.x, panOffset.y, 0), Quaternion.identity, Vector3.one * zoomLevel);
            
            // Draw edges first (behind nodes)
            foreach (var edge in graph.edges)
            {
                DrawEdge(edge, rect);
            }
            
            // Draw nodes
            foreach (var node in graph.nodes)
            {
                DrawNode(node, rect);
            }
            
            // Handle mouse interactions
            HandleMouseInput(rect);
            
            GUI.matrix = Matrix4x4.identity;
            Widgets.EndGroup();
        }
        
        private void DrawNode(PsycheNode node, Rect rect)
        {
            Vector2 screenPos = node.position;
            Rect nodeRect = new Rect(screenPos.x - 20, screenPos.y - 20, 40, 40);
            
            // Check if node is in view
            if (!rect.Overlaps(nodeRect))
                return;
            
            // Draw node background
            Color nodeColor = GetNodeColor(node);
            GUI.color = nodeColor;
            Widgets.DrawBoxSolid(nodeRect, nodeColor);
            GUI.color = Color.white;
            
            // Draw node border
            if (selectedNode == node)
            {
                GUI.color = Color.yellow;
                Widgets.DrawBox(nodeRect, 2);
                GUI.color = Color.white;
            }
            
            // Draw node icon/text
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(nodeRect, GetNodeLabel(node));
            Text.Anchor = TextAnchor.UpperLeft;
            
            // Draw tooltip on hover
            if (Mouse.IsOver(nodeRect))
            {
                TooltipHandler.TipRegion(nodeRect, GetNodeTooltip(node));
            }
        }
        
        private void DrawEdge(PsycheEdge edge, Rect rect)
        {
            Vector2 start = edge.from.position;
            Vector2 end = edge.to.position;
            
            // Simple line drawing
            Color edgeColor = GetEdgeColor(edge);
            GUI.color = edgeColor;
            
            // Draw line
            Vector2 direction = (end - start).normalized;
            float distance = Vector2.Distance(start, end);
            
            // Draw arrow
            Vector2 arrowPos = start + direction * (distance - 20);
            Vector2 arrowLeft = arrowPos + new Vector2(-direction.y, direction.x) * 5;
            Vector2 arrowRight = arrowPos + new Vector2(direction.y, -direction.x) * 5;
            
            // Simple line drawing (Unity doesn't have built-in line drawing)
            // This is a simplified version - in practice you'd use a more sophisticated line drawing method
            
            GUI.color = Color.white;
        }
        
        private void HandleMouseInput(Rect rect)
        {
            Event e = Event.current;
            
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                Vector2 mousePos = e.mousePosition;
                
                // Check for node clicks
                foreach (var node in graph.nodes)
                {
                    Rect nodeRect = new Rect(node.position.x - 20, node.position.y - 20, 40, 40);
                    if (nodeRect.Contains(mousePos))
                    {
                        selectedNode = node;
                        e.Use();
                        break;
                    }
                }
            }
            else if (e.type == EventType.MouseDrag && e.button == 0)
            {
                // Pan the view
                panOffset += e.delta;
                e.Use();
            }
            else if (e.type == EventType.ScrollWheel)
            {
                // Zoom
                float zoomDelta = e.delta.y * 0.1f;
                zoomLevel = Mathf.Clamp(zoomLevel - zoomDelta, 0.3f, 3f);
                e.Use();
            }
        }
        
        private void DrawDetailsPanel(Rect rect)
        {
            if (selectedNode == null) return;
            
            Widgets.DrawBoxSolid(rect, new Color(0, 0, 0, 0.8f));
            Widgets.DrawBox(rect, 1);
            
            Rect contentRect = rect.ContractedBy(10);
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(contentRect);
            
            listing.Label("Node Details");
            listing.Gap();
            
            listing.Label("Type: " + selectedNode.type.ToString());
            listing.Label("Label: " + selectedNode.label);
            
            if (selectedNode.data is Pawn pawn)
            {
                listing.Label("Mood: " + pawn.needs.mood.CurLevel.ToStringPercent());
                listing.Label("Age: " + pawn.ageTracker.AgeBiologicalYears);
            }
            else if (selectedNode.data is ThoughtDef thoughtDef)
            {
                listing.Label("Thought: " + thoughtDef.label);
                listing.Label("Mood Impact: " + thoughtDef.stages[0].baseMoodEffect);
            }
            
            listing.End();
        }
        
        private void SelectPawn(Pawn pawn)
        {
            selectedPawn = pawn;
            BuildGraph();
        }
        
        private void BuildGraph()
        {
            graph.Clear();
            
            if (selectedPawn == null) return;
            
            // Add pawn node
            var pawnNode = new PsycheNode
            {
                type = PsycheNodeType.Pawn,
                label = selectedPawn.Name.ToStringShort,
                position = new Vector2(400, 300),
                data = selectedPawn
            };
            graph.AddNode(pawnNode);
            
            // Add thought nodes
            int thoughtIndex = 0;
            foreach (var thought in selectedPawn.needs.mood.thoughts.memories)
            {
                var thoughtNode = new PsycheNode
                {
                    type = PsycheNodeType.Thought,
                    label = thought.def.label,
                    position = new Vector2(200 + thoughtIndex * 100, 200),
                    data = thought.def
                };
                graph.AddNode(thoughtNode);
                
                // Add edge from pawn to thought
                graph.AddEdge(pawnNode, thoughtNode, PsycheEdgeType.ActiveThought);
                
                thoughtIndex++;
            }
            
            // Add relationship nodes
            int relationshipIndex = 0;
            foreach (var otherPawn in Find.CurrentMap?.mapPawns.FreeColonists ?? new List<Pawn>())
            {
                if (otherPawn == selectedPawn) continue;
                
                var relationshipNode = new PsycheNode
                {
                    type = PsycheNodeType.Pawn,
                    label = otherPawn.Name.ToStringShort,
                    position = new Vector2(600 + relationshipIndex * 80, 300 + relationshipIndex * 50),
                    data = otherPawn
                };
                graph.AddNode(relationshipNode);
                
                // Add edge for social relationship
                graph.AddEdge(pawnNode, relationshipNode, PsycheEdgeType.SocialRelationship);
                
                relationshipIndex++;
            }
        }
        
        private Color GetNodeColor(PsycheNode node)
        {
            switch (node.type)
            {
                case PsycheNodeType.Pawn:
                    return Color.blue;
                case PsycheNodeType.Thought:
                    return Color.green;
                case PsycheNodeType.ThoughtComplex:
                    return Color.yellow;
                case PsycheNodeType.Mystery:
                    return Color.red;
                default:
                    return Color.gray;
            }
        }
        
        private string GetNodeLabel(PsycheNode node)
        {
            switch (node.type)
            {
                case PsycheNodeType.Pawn:
                    return "P";
                case PsycheNodeType.Thought:
                    return "T";
                case PsycheNodeType.ThoughtComplex:
                    return "C";
                case PsycheNodeType.Mystery:
                    return "?";
                default:
                    return "?";
            }
        }
        
        private string GetNodeTooltip(PsycheNode node)
        {
            return node.label + " (" + node.type.ToString() + ")";
        }
        
        private Color GetEdgeColor(PsycheEdge edge)
        {
            switch (edge.type)
            {
                case PsycheEdgeType.ActiveThought:
                    return Color.green;
                case PsycheEdgeType.SocialRelationship:
                    return Color.blue;
                case PsycheEdgeType.ThoughtConnection:
                    return Color.yellow;
                default:
                    return Color.gray;
            }
        }
    }
    
    // Supporting classes
    public class PsycheWebGraph
    {
        public List<PsycheNode> nodes = new List<PsycheNode>();
        public List<PsycheEdge> edges = new List<PsycheEdge>();
        
        public void AddNode(PsycheNode node)
        {
            nodes.Add(node);
        }
        
        public void AddEdge(PsycheNode from, PsycheNode to, PsycheEdgeType type)
        {
            edges.Add(new PsycheEdge { from = from, to = to, type = type });
        }
        
        public void Clear()
        {
            nodes.Clear();
            edges.Clear();
        }
    }
    
    public class PsycheNode
    {
        public PsycheNodeType type;
        public string label;
        public Vector2 position;
        public object data;
    }
    
    public class PsycheEdge
    {
        public PsycheNode from;
        public PsycheNode to;
        public PsycheEdgeType type;
    }
    
    public enum PsycheNodeType
    {
        Pawn,
        Thought,
        ThoughtComplex,
        Mystery
    }
    
    public enum PsycheEdgeType
    {
        ActiveThought,
        SocialRelationship,
        ThoughtConnection
    }
}
