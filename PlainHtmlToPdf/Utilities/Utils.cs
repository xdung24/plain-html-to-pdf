using System.Drawing;
using PlainHtmlToPdf.Adapters.Entities;
using PdfSharp.Drawing;

namespace PlainHtmlToPdf.Utilities;

/// <summary>
/// Utilities for converting WinForms entities to HtmlRenderer core entities.
/// </summary>
internal class Utils
{
    /// <summary>
    /// Convert from WinForms point to core point.
    /// </summary>
    public static RPoint Convert(XPoint p)
    {
        return new RPoint(p.X, p.Y);
    }

    /// <summary>
    /// Convert from WinForms point to core point.
    /// </summary>
    public static XPoint[] Convert(RPoint[] points)
    {
        XPoint[] myPoints = new XPoint[points.Length];
        for (int i = 0; i < points.Length; i++)
            myPoints[i] = Convert(points[i]);
        return myPoints;
    }

    /// <summary>
    /// Convert from core point to WinForms point.
    /// </summary>
    public static XPoint Convert(RPoint p)
    {
        return new XPoint(p.X, p.Y);
    }

    /// <summary>
    /// Convert from WinForms size to core size.
    /// </summary>
    public static RSize Convert(XSize s)
    {
        return new RSize(s.Width, s.Height);
    }

    /// <summary>
    /// Convert from core size to WinForms size.
    /// </summary>
    public static XSize Convert(RSize s)
    {
        return new XSize(s.Width, s.Height);
    }

    /// <summary>
    /// Convert from WinForms rectangle to core rectangle.
    /// </summary>
    public static RRect Convert(XRect r)
    {
        return new RRect(r.X, r.Y, r.Width, r.Height);
    }

    /// <summary>
    /// Convert from core rectangle to WinForms rectangle.
    /// </summary>
    public static XRect Convert(RRect r)
    {
        return new XRect(r.X, r.Y, r.Width, r.Height);
    }

    /// <summary>
    /// Convert from core color to WinForms color.
    /// </summary>
    public static XColor Convert(RColor c)
    {
        return XColor.FromArgb(c.A, c.R, c.G, c.B);
    }

    /// <summary>
    /// Convert from  color to WinForms color.
    /// </summary>
    public static RColor Convert(Color c)
    {
        return RColor.FromArgb(c.A, c.R, c.G, c.B);
    }

}