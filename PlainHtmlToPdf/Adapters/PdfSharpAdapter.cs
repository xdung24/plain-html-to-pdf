﻿using System.Drawing;
using System.Text;
using PlainHtmlToPdf.Adapters.Entities;
using PlainHtmlToPdf.Utilities;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace PlainHtmlToPdf.Adapters;

/// <summary>
/// Adapter for PdfSharp library platform.
/// </summary>
internal sealed class PdfSharpAdapter : RAdapter
{
    #region Fields and Consts

    /// <summary>
    /// Singleton instance of global adapter.
    /// </summary>
    private static readonly PdfSharpAdapter _instance = new PdfSharpAdapter();

    #endregion

    /// <summary>
    /// Init color resolve.
    /// </summary>
    private PdfSharpAdapter()
    {
#if !NETSTANDARD2_1
        // Register code page provider to support encoding 1252 (WinAnsiEncoding)
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        AddFontFamilyMapping("monospace", "Courier New");
        AddFontFamilyMapping("Helvetica", "Arial");
        AddFontFamilyMapping("Times New Roman", "Times New Roman");
    }

    /// <summary>
    /// Singleton instance of global adapter.
    /// </summary>
    public static PdfSharpAdapter Instance
    {
        get { return _instance; }
    }

    protected override RColor GetColorInt(string colorName)
    {
        try
        {
            var color = Color.FromKnownColor((KnownColor)Enum.Parse(typeof(KnownColor), colorName, true));
            return Utils.Convert(color);
        }
        catch
        {
            return RColor.Empty;
        }
    }

    protected override RPen CreatePen(RColor color)
    {
        return new PenAdapter(new XPen(Utils.Convert(color)));
    }

    protected override RBrush CreateSolidBrush(RColor color)
    {
        XBrush solidBrush;
        if (color == RColor.White)
            solidBrush = XBrushes.White;
        else if (color == RColor.Black)
            solidBrush = XBrushes.Black;
        else if (color.A < 1)
            solidBrush = XBrushes.Transparent;
        else
            solidBrush = new XSolidBrush(Utils.Convert(color));

        return new BrushAdapter(solidBrush);
    }

    protected override RBrush CreateLinearGradientBrush(RRect rect, RColor color1, RColor color2, double angle)
    {
        XLinearGradientMode mode;
        if (angle < 45)
            mode = XLinearGradientMode.ForwardDiagonal;
        else if (angle < 90)
            mode = XLinearGradientMode.Vertical;
        else if (angle < 135)
            mode = XLinearGradientMode.BackwardDiagonal;
        else
            mode = XLinearGradientMode.Horizontal;
        return new BrushAdapter(new XLinearGradientBrush(Utils.Convert(rect), Utils.Convert(color1), Utils.Convert(color2), mode));
    }

    protected override RImage ConvertImageInt(object image)
    {
        return image != null ? new ImageAdapter((XImage)image) : null;
    }

    protected override RImage ImageFromStreamInt(Stream memoryStream)
    {
        return new ImageAdapter(XImage.FromStream(memoryStream));
    }

    protected override RFont CreateFontInt(string family, double size, RFontStyle style)
    {
        var fontStyle = (XFontStyleEx)((int)style);
        var fontOption = new XPdfFontOptions(PdfFontEncoding.Unicode);
        var xFont = new XFont(family, size, fontStyle, fontOption);
        return new FontAdapter(xFont);
    }

    protected override RFont CreateFontInt(RFontFamily family, double size, RFontStyle style)
    {
        var fontStyle = (XFontStyleEx)((int)style);
        var fontOption = new XPdfFontOptions(PdfFontEncoding.Unicode);
        var xFont = new XFont(((FontFamilyAdapter)family).FontFamily.Name, size, fontStyle, fontOption);
        return new FontAdapter(xFont);
    }
}