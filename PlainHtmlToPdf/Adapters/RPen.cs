using PlainHtmlToPdf.Adapters.Entities;

namespace PlainHtmlToPdf.Adapters;

/// <summary>
/// Adapter for platform specific pen objects - used to draw graphics (lines, rectangles and paths) 
/// </summary>
public abstract class RPen
{
    /// <summary>
    /// Gets or sets the width of this Pen, in units of the Graphics object used for drawing.
    /// </summary>
    public abstract double Width { get; set; }

    /// <summary>
    /// Gets or sets the style used for dashed lines drawn with this Pen.
    /// </summary>
    public abstract RDashStyle DashStyle { set; }
}