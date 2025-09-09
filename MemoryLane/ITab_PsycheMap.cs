using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace MindMatters.Visualization
{
    /// <summary>
    /// ITab that shows a "Psyche Map" - a visual representation of what's bugging a pawn
    /// Uses different shapes to represent different types of psychological influences
    /// </summary>
    public class ITab_PsycheMap : ITab
    {
        private Vector2 scrollPosition;
        private float zoomLevel = 1f;
        private Vector2 panOffset = Vector2.zero;
        
        public ITab_PsycheMap()
        {
            labelKey = "PsycheMap";
            size = new Vector2(600f, 400f);
        }
        
        protected override void FillTab()
        {
            if (SelPawn == null) return;
            
            Rect rect = new Rect(0, 0, size.x, size.y);
            
            // Title
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0, 0, rect.width, 30), "Psyche Map - " + SelPawn.Name.ToStringShort);
            Text.Font = GameFont.Small;
            
            // Controls
            DrawControls(new Rect(0, 30, rect.width, 25));
            
            // Main psyche map area
            Rect mapRect = new Rect(0, 55, rect.width, rect.height - 55);
            DrawPsycheMap(mapRect);
        }
        
        private void DrawControls(Rect rect)
        {
            // Zoom controls
            if (Widgets.ButtonText(new Rect(0, 0, 50, 20), "Zoom+"))
            {
                zoomLevel = Mathf.Min(zoomLevel * 1.2f, 2f);
            }
            if (Widgets.ButtonText(new Rect(55, 0, 50, 20), "Zoom-"))
            {
                zoomLevel = Mathf.Max(zoomLevel / 1.2f, 0.5f);
            }
            
            // Reset view
            if (Widgets.ButtonText(new Rect(110, 0, 60, 20), "Reset"))
            {
                zoomLevel = 1f;
                panOffset = Vector2.zero;
            }
            
            // Legend
            DrawLegend(new Rect(180, 0, rect.width - 180, 20));
        }
        
        private void DrawLegend(Rect rect)
        {
            // Legend items
            string legend = "🔴 Fixed Ideas  🟡 Attractions  🔵 Fresh Memories  ⚫ Dead/Events  ⚪ Current Thoughts";
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect, legend);
            Text.Anchor = TextAnchor.UpperLeft;
        }
        
        private void DrawPsycheMap(Rect rect)
        {
            Widgets.BeginGroup(rect);
            
            // Apply zoom and pan
            GUI.matrix = Matrix4x4.TRS(new Vector3(panOffset.x, panOffset.y, 0), Quaternion.identity, Vector3.one * zoomLevel);
            
            // Get psyche data
            var psycheObjects = GetPsycheObjects();
            
            // Draw connections first (behind objects)
            DrawConnections(psycheObjects, rect);
            
            // Draw psyche objects
            foreach (var obj in psycheObjects)
            {
                DrawPsycheObject(obj, rect);
            }
            
            // Handle mouse input
            HandleMouseInput(rect);
            
            GUI.matrix = Matrix4x4.identity;
            Widgets.EndGroup();
        }
        
        private List<PsycheObject> GetPsycheObjects()
        {
            var objects = new List<PsycheObject>();
            
            if (SelPawn == null) return objects;
            
            // Center pawn
            objects.Add(new PsycheObject
            {
                type = PsycheObjectType.Pawn,
                label = SelPawn.Name.ToStringShort,
                position = new Vector2(300, 200),
                intensity = 1f,
                data = SelPawn
            });
            
            // Get thoughts with high valency (beyond -25 or +25)
            var highValencyThoughts = SelPawn.needs.mood.thoughts.memories
                .Where(t => Math.Abs(t.MoodOffset()) >= 25f)
                .ToList();
            
            int thoughtIndex = 0;
            foreach (var thought in highValencyThoughts)
            {
                float angle = (thoughtIndex * 2f * Mathf.PI) / highValencyThoughts.Count;
                float distance = 80f + (Math.Abs(thought.MoodOffset()) - 25f) * 2f; // Distance based on intensity
                
                objects.Add(new PsycheObject
                {
                    type = PsycheObjectType.Thought,
                    label = thought.def.label,
                    position = new Vector2(300 + Mathf.Cos(angle) * distance, 200 + Mathf.Sin(angle) * distance),
                    intensity = Math.Abs(thought.MoodOffset()) / 100f,
                    moodImpact = thought.MoodOffset(),
                    data = thought
                });
                
                thoughtIndex++;
            }
            
            // Get fixed ideas (hallucinations, obsessions, etc.)
            var fixedIdeas = GetFixedIdeas();
            int fixedIndex = 0;
            foreach (var idea in fixedIdeas)
            {
                float angle = (fixedIndex * 2f * Mathf.PI) / Math.Max(fixedIdeas.Count, 1);
                float distance = 120f;
                
                objects.Add(new PsycheObject
                {
                    type = PsycheObjectType.FixedIdea,
                    label = idea.label,
                    position = new Vector2(300 + Mathf.Cos(angle) * distance, 200 + Mathf.Sin(angle) * distance),
                    intensity = idea.intensity,
                    data = idea
                });
                
                fixedIndex++;
            }
            
            // Get attractions (things/people that attract this pawn)
            var attractions = GetAttractions();
            int attractionIndex = 0;
            foreach (var attraction in attractions)
            {
                float angle = (attractionIndex * 2f * Mathf.PI) / Math.Max(attractions.Count, 1);
                float distance = 160f;
                
                objects.Add(new PsycheObject
                {
                    type = PsycheObjectType.Attraction,
                    label = attraction.label,
                    position = new Vector2(300 + Mathf.Cos(angle) * distance, 200 + Mathf.Sin(angle) * distance),
                    intensity = attraction.intensity,
                    data = attraction
                });
                
                attractionIndex++;
            }
            
            // Get fresh memories (recent events)
            var freshMemories = GetFreshMemories();
            int memoryIndex = 0;
            foreach (var memory in freshMemories)
            {
                float angle = (memoryIndex * 2f * Mathf.PI) / Math.Max(freshMemories.Count, 1);
                float distance = 200f;
                
                objects.Add(new PsycheObject
                {
                    type = PsycheObjectType.FreshMemory,
                    label = memory.label,
                    position = new Vector2(300 + Mathf.Cos(angle) * distance, 200 + Mathf.Sin(angle) * distance),
                    intensity = memory.intensity,
                    data = memory
                });
                
                memoryIndex++;
            }
            
            // Get dead/events with high gravity
            var deadEvents = GetDeadEvents();
            int deadIndex = 0;
            foreach (var dead in deadEvents)
            {
                float angle = (deadIndex * 2f * Mathf.PI) / Math.Max(deadEvents.Count, 1);
                float distance = 240f;
                
                objects.Add(new PsycheObject
                {
                    type = PsycheObjectType.DeadEvent,
                    label = dead.label,
                    position = new Vector2(300 + Mathf.Cos(angle) * distance, 200 + Mathf.Sin(angle) * distance),
                    intensity = dead.intensity,
                    data = dead
                });
                
                deadIndex++;
            }
            
            return objects;
        }
        
        private List<FixedIdea> GetFixedIdeas()
        {
            var ideas = new List<FixedIdea>();
            
            // Check for hallucinations
            if (SelPawn.health.hediffSet.HasHediff(HediffDefOf.PsychicSoothe))
            {
                ideas.Add(new FixedIdea { label = "Hallucination", intensity = 0.8f });
            }
            
            // Check for obsessions
            if (SelPawn.story.traits.HasTrait(TraitDefOf.Psychopath))
            {
                ideas.Add(new FixedIdea { label = "Psychopathic Thoughts", intensity = 0.6f });
            }
            
            // Check for other mental conditions
            foreach (var hediff in SelPawn.health.hediffSet.hediffs)
            {
                if (hediff.def.mentalState != null)
                {
                    ideas.Add(new FixedIdea { label = hediff.def.label, intensity = 0.7f });
                }
            }
            
            return ideas;
        }
        
        private List<Attraction> GetAttractions()
        {
            var attractions = new List<Attraction>();
            
            // Check for romantic interests
            foreach (var otherPawn in Find.CurrentMap?.mapPawns.FreeColonists ?? new List<Pawn>())
            {
                if (otherPawn != SelPawn)
                {
                    var relation = SelPawn.relations.GetDirectRelation(PawnRelationDefOf.Lover, otherPawn);
                    if (relation != null)
                    {
                        attractions.Add(new Attraction { label = otherPawn.Name.ToStringShort, intensity = 0.9f });
                    }
                }
            }
            
            // Check for favorite things
            if (SelPawn.foodRestriction != null)
            {
                attractions.Add(new Attraction { label = "Favorite Food", intensity = 0.5f });
            }
            
            return attractions;
        }
        
        private List<FreshMemory> GetFreshMemories()
        {
            var memories = new List<FreshMemory>();
            
            // Recent deaths
            var recentDeaths = SelPawn.needs.mood.thoughts.memories
                .Where(m => m.def == ThoughtDefOf.ObservedLayingCorpse && m.age < 60000) // Last day
                .ToList();
            
            if (recentDeaths.Any())
            {
                memories.Add(new FreshMemory { label = "Recent Death", intensity = 0.8f });
            }
            
            // Recent fights
            var recentFights = SelPawn.needs.mood.thoughts.memories
                .Where(m => m.def == ThoughtDefOf.WitnessedDeathAlly && m.age < 30000) // Last 12 hours
                .ToList();
            
            if (recentFights.Any())
            {
                memories.Add(new FreshMemory { label = "Recent Violence", intensity = 0.7f });
            }
            
            return memories;
        }
        
        private List<DeadEvent> GetDeadEvents()
        {
            var deadEvents = new List<DeadEvent>();
            
            // Dead colonists
            var deadColonists = Find.CurrentMap?.mapPawns.PawnsInFaction(Faction.OfPlayer)
                .Where(p => p.Dead)
                .ToList() ?? new List<Pawn>();
            
            foreach (var dead in deadColonists)
            {
                if (SelPawn.relations.DirectRelationExists(PawnRelationDefOf.Spouse, dead) ||
                    SelPawn.relations.DirectRelationExists(PawnRelationDefOf.Child, dead) ||
                    SelPawn.relations.DirectRelationExists(PawnRelationDefOf.Parent, dead))
                {
                    deadEvents.Add(new DeadEvent { label = dead.Name.ToStringShort, intensity = 0.9f });
                }
            }
            
            return deadEvents;
        }
        
        private void DrawPsycheObject(PsycheObject obj, Rect rect)
        {
            Vector2 screenPos = obj.position;
            float size = 20f + obj.intensity * 30f;
            Rect objRect = new Rect(screenPos.x - size/2, screenPos.y - size/2, size, size);
            
            // Check if object is in view
            if (!rect.Overlaps(objRect))
                return;
            
            // Draw object based on type
            switch (obj.type)
            {
                case PsycheObjectType.Pawn:
                    DrawPawnObject(objRect, obj);
                    break;
                case PsycheObjectType.Thought:
                    DrawThoughtObject(objRect, obj);
                    break;
                case PsycheObjectType.FixedIdea:
                    DrawFixedIdeaObject(objRect, obj);
                    break;
                case PsycheObjectType.Attraction:
                    DrawAttractionObject(objRect, obj);
                    break;
                case PsycheObjectType.FreshMemory:
                    DrawFreshMemoryObject(objRect, obj);
                    break;
                case PsycheObjectType.DeadEvent:
                    DrawDeadEventObject(objRect, obj);
                    break;
            }
            
            // Draw tooltip on hover
            if (Mouse.IsOver(objRect))
            {
                TooltipHandler.TipRegion(objRect, GetObjectTooltip(obj));
            }
        }
        
        private void DrawPawnObject(Rect rect, PsycheObject obj)
        {
            // Draw pawn as a circle
            GUI.color = Color.blue;
            Widgets.DrawBoxSolid(rect, Color.blue);
            GUI.color = Color.white;
            
            // Draw pawn name
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, "P");
            Text.Anchor = TextAnchor.UpperLeft;
        }
        
        private void DrawThoughtObject(Rect rect, PsycheObject obj)
        {
            // Draw thought as a square
            Color thoughtColor = obj.moodImpact > 0 ? Color.green : Color.red;
            GUI.color = thoughtColor;
            Widgets.DrawBoxSolid(rect, thoughtColor);
            GUI.color = Color.white;
            
            // Draw thought symbol
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, "T");
            Text.Anchor = TextAnchor.UpperLeft;
        }
        
        private void DrawFixedIdeaObject(Rect rect, PsycheObject obj)
        {
            // Draw fixed idea as a diamond
            GUI.color = Color.red;
            Widgets.DrawBoxSolid(rect, Color.red);
            GUI.color = Color.white;
            
            // Draw diamond symbol
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, "♦");
            Text.Anchor = TextAnchor.UpperLeft;
        }
        
        private void DrawAttractionObject(Rect rect, PsycheObject obj)
        {
            // Draw attraction as a triangle
            GUI.color = Color.yellow;
            Widgets.DrawBoxSolid(rect, Color.yellow);
            GUI.color = Color.white;
            
            // Draw triangle symbol
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, "▲");
            Text.Anchor = TextAnchor.UpperLeft;
        }
        
        private void DrawFreshMemoryObject(Rect rect, PsycheObject obj)
        {
            // Draw fresh memory as a circle
            GUI.color = Color.cyan;
            Widgets.DrawBoxSolid(rect, Color.cyan);
            GUI.color = Color.white;
            
            // Draw memory symbol
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, "●");
            Text.Anchor = TextAnchor.UpperLeft;
        }
        
        private void DrawDeadEventObject(Rect rect, PsycheObject obj)
        {
            // Draw dead/event as a square
            GUI.color = Color.black;
            Widgets.DrawBoxSolid(rect, Color.black);
            GUI.color = Color.white;
            
            // Draw dead symbol
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, "■");
            Text.Anchor = TextAnchor.UpperLeft;
        }
        
        private void DrawConnections(List<PsycheObject> objects, Rect rect)
        {
            // Draw connections from pawn to other objects
            var pawn = objects.FirstOrDefault(o => o.type == PsycheObjectType.Pawn);
            if (pawn == null) return;
            
            foreach (var obj in objects)
            {
                if (obj == pawn) continue;
                
                // Draw line from pawn to object
                Vector2 start = pawn.position;
                Vector2 end = obj.position;
                
                // Simple line drawing (in practice, you'd use a more sophisticated method)
                // This is a placeholder for the actual line drawing
            }
        }
        
        private void HandleMouseInput(Rect rect)
        {
            Event e = Event.current;
            
            if (e.type == EventType.MouseDrag && e.button == 0)
            {
                // Pan the view
                panOffset += e.delta;
                e.Use();
            }
            else if (e.type == EventType.ScrollWheel)
            {
                // Zoom
                float zoomDelta = e.delta.y * 0.1f;
                zoomLevel = Mathf.Clamp(zoomLevel - zoomDelta, 0.5f, 2f);
                e.Use();
            }
        }
        
        private string GetObjectTooltip(PsycheObject obj)
        {
            string tooltip = obj.label;
            if (obj.intensity > 0)
            {
                tooltip += $"\nIntensity: {obj.intensity:P0}";
            }
            if (obj.moodImpact != 0)
            {
                tooltip += $"\nMood Impact: {obj.moodImpact:+0;-0}";
            }
            return tooltip;
        }
    }
    
    // Supporting classes
    public class PsycheObject
    {
        public PsycheObjectType type;
        public string label;
        public Vector2 position;
        public float intensity;
        public float moodImpact;
        public object data;
    }
    
    public enum PsycheObjectType
    {
        Pawn,
        Thought,
        FixedIdea,
        Attraction,
        FreshMemory,
        DeadEvent
    }
    
    public class FixedIdea
    {
        public string label;
        public float intensity;
    }
    
    public class Attraction
    {
        public string label;
        public float intensity;
    }
    
    public class FreshMemory
    {
        public string label;
        public float intensity;
    }
    
    public class DeadEvent
    {
        public string label;
        public float intensity;
    }
}
