using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using WDE.Common.Avalonia.Utils;
using WDE.Common.Managers;
using WDE.SmartScriptEditor.Avalonia.Extensions;
using WDE.SmartScriptEditor.Models;

namespace WDE.SmartScriptEditor.Avalonia.Editor.UserControls;

public class VirtualizedSmartScriptPanelRenderOverlay : Control
{
    private readonly VirtualizedSmartScriptPanel parent;

    public VirtualizedSmartScriptPanelRenderOverlay(VirtualizedSmartScriptPanel parent)
    {
        this.parent = parent;
        IsHitTestVisible = false;
    }
    
    public override void Render(DrawingContext context)
    {
        parent.RenderOverlay(context);
    }
}

public partial class VirtualizedSmartScriptPanel
{
    private static FormattedText? vvvvText;
    private static SmartScriptPanelLayout.FormattedTextNumberCache NumberCache = new();
    public void RenderOverlay(DrawingContext context)
    {
        base.Render(context);
        if (script == null)
            return;

        SizingContext sizing = new SizingContext(Bounds.Width, Padding, EventPaddingLeft, VisibleRect);
        
        if (AnythingSelected())
        {
            if (draggingActions)
            {
                double x = sizing.eventRect.Right;
                double y = overIndexAction.y - overIndexAction.height / 2 - 1;
                context.DrawArrow(new Pen(Brushes.Gray, 3), new Point(x, y), new Point(x + 200, y), 10);
            }
            else if (draggingConditions)
            {
                double x = sizing.conditionRect.X;
                double y = overIndexCondition.y - overIndexCondition.height / 2 - 1;
                context.DrawArrow(new Pen(Brushes.Gray, 3), new Point(x, y), new Point(sizing.conditionRect.Right, y), 10);         
            }
            else if (draggingEvents)
            {
                double x = OverIndexEvent.inGroup ?  sizing.groupedEventRect.X : sizing.eventRect.X;
                double right = OverIndexEvent.inGroup ? sizing.groupedEventRect.Right : sizing.eventRect.Right;
                double y = OverIndexEvent.y;
                context.DrawArrow(new Pen(Brushes.Gray, 3), new Point(x, y), new Point(right, y), 10);
            }
            else if (draggingGroups)
            {
                double x = sizing.groupRect.X;
                double right = sizing.groupRect.Right;
                double y = overIndexGroup.y;
                context.DrawArrow(new Pen(Brushes.Gray, 3), new Point(x, y), new Point(right, y), 10);
            }
        }
        
        if (vvvvText == null)
        {
            vvvvText = new FormattedText();
            vvvvText.FontSize = 7;
            vvvvText.Text = "vvvv";
            vvvvText.Typeface = Typeface.Default;
        }
        
        var visibleRect = VisibleRect;

        bool inGroup = false;
        bool groupIsExpanded = false;
        foreach (var e in script.Events)
        {
            if (e.IsBeginGroup)
            {
                inGroup = true;
                groupIsExpanded = new SmartGroup(e).IsExpanded;
            }
            else if (e.IsEndGroup)
            {
                inGroup = false;
            }
            else
            {
                if (inGroup && !groupIsExpanded)
                    continue;
             
                if (e.Actions.Count == 0)
                {
                    if (!visibleRect.Intersects(e.EventPosition.ToRect()))
                        continue;
                
                    double yPos = e.Position.Y;

                    DrawProblems(context, e.VirtualLineId, new Point(0, yPos));
                }
                else
                {
                    foreach (var a in e.Actions)
                    {
                        if (a.Id == SmartConstants.ActionComment && HideComments)
                            continue;

                        if (!visibleRect.Intersects(a.Position.ToRect()))
                            continue;
                    
                        double yPos = a.Position.Y;

                        float x = a.IsInInlineActionList ? 10 : 0;
                        if (a.DestinationEventId is { } eventId)
                        {
                            var ft = NumberCache.Get(eventId);
                            context.DrawText(Brushes.DarkGray, new Point(PaddingLeft + x, yPos + 5), ft);
                        }
                        DrawProblems(context, a.VirtualLineId, new Point(x, yPos));
                    }
                }
            }
        }
    }
    
    private void DrawProblems(DrawingContext dc, int index, Point pos)
    {
        if (Problems != null && Problems.TryGetValue(index, out var severity))
        { 
            dc.DrawText(severity is DiagnosticSeverity.Error or DiagnosticSeverity.Critical ? Brushes.Red : Brushes.Orange, new Point(PaddingLeft + pos.X, pos.Y + 5 + 10), vvvvText);   
        }
    }

}