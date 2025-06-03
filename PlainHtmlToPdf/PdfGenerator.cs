using PlainHtmlToPdf.Adapters;
using PlainHtmlToPdf.Core;
using PlainHtmlToPdf.Core.Entities;
using PlainHtmlToPdf.Core.Utils;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace PlainHtmlToPdf;

public interface IPdfGenerator
{
    /// <summary>
    /// Adds a font mapping from <paramref name="fromFamily"/> to <paramref name="toFamily"/> iff the <paramref name="fromFamily"/> is not found.<br/>
    /// When the <paramref name="fromFamily"/> font is used in rendered html and is not found in existing 
    /// fonts (installed or added) it will be replaced by <paramref name="toFamily"/>.<br/>
    /// </summary>
    /// <remarks>
    /// This fonts mapping can be used as a fallback in case the requested font is not installed in the client system.
    /// </remarks>
    /// <param name="fromFamily">the font family to replace</param>
    /// <param name="toFamily">the font family to replace with</param>
    void AddFontFamilyMapping(string fromFamily, string toFamily);

    /// <summary>
    /// Create PDF document from given HTML.<br/>
    /// </summary>
    /// <param name="html">HTML source to create PDF from</param>
    /// <param name="pageSize">the page size to use for each page in the generated pdf </param>
    /// <param name="margin">the margin to use between the HTML and the edges of each page</param>
    /// <param name="cssData">optional: the style to use for html rendering (default - use W3 default style)</param>
    /// <param name="stylesheetLoad">optional: can be used to overwrite stylesheet resolution logic</param>
    /// <param name="imageLoad">optional: can be used to overwrite image resolution logic</param>
    /// <returns>the generated image of the html</returns>
    PdfDocument GeneratePdf(string html, PageSize pageSize, int margin = 20, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null);

    /// <summary>
    /// Create PDF document from given HTML.<br/>
    /// </summary>
    /// <param name="html">HTML source to create PDF from</param>
    /// <param name="config">the configuration to use for the PDF generation (page size/page orientation/margins/etc.)</param>
    /// <param name="cssData">optional: the style to use for html rendering (default - use W3 default style)</param>
    /// <param name="stylesheetLoad">optional: can be used to overwrite stylesheet resolution logic</param>
    /// <param name="imageLoad">optional: can be used to overwrite image resolution logic</param>
    /// <returns>the generated image of the html</returns>
    PdfDocument GeneratePdf(string html, PdfGenerateConfig config, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null);
}

/// <summary>
/// Provides methods to generate PDF documents from HTML content.<br/>
/// The generated PDF can be customized with different page sizes, margins, and styles.<br/>
/// </summary>
public class PdfGenerator : IPdfGenerator
{
#pragma warning disable CA1822 // Mark members as static
    public string DefaultFont
#pragma warning restore CA1822 // Mark members as static
    {
        get { return PdfGenerateConfig.DefaultFontFamily; }
        set { PdfGenerateConfig.DefaultFontFamily = value; }
    }

    public PdfGenerator()
    {
        PdfGenerateConfig.DefaultFontFamily = "Segoe UI";
    }

    public PdfGenerator(string defaultFont)
    {
        PdfGenerateConfig.DefaultFontFamily = defaultFont;
    }

    /// <summary>
    /// Adds a font mapping from <paramref name="fromFamily"/> to <paramref name="toFamily"/> iff the <paramref name="fromFamily"/> is not found.<br/>
    /// When the <paramref name="fromFamily"/> font is used in rendered html and is not found in existing 
    /// fonts (installed or added) it will be replaced by <paramref name="toFamily"/>.<br/>
    /// </summary>
    /// <remarks>
    /// This fonts mapping can be used as a fallback in case the requested font is not installed in the client system.
    /// </remarks>
    /// <param name="fromFamily">the font family to replace</param>
    /// <param name="toFamily">the font family to replace with</param>
    public void AddFontFamilyMapping(string fromFamily, string toFamily)
    {
        ArgChecker.AssertArgNotNullOrEmpty(fromFamily, "fromFamily");
        ArgChecker.AssertArgNotNullOrEmpty(toFamily, "toFamily");

        PdfSharpAdapter.Instance.AddFontFamilyMapping(fromFamily, toFamily);
    }

    /// <summary>
    /// Create PDF document from given HTML.<br/>
    /// </summary>
    /// <param name="html">HTML source to create PDF from</param>
    /// <param name="pageSize">the page size to use for each page in the generated pdf </param>
    /// <param name="margin">the margin to use between the HTML and the edges of each page</param>
    /// <param name="cssData">optional: the style to use for html rendering (default - use W3 default style)</param>
    /// <param name="stylesheetLoad">optional: can be used to overwrite stylesheet resolution logic</param>
    /// <param name="imageLoad">optional: can be used to overwrite image resolution logic</param>
    /// <returns>the generated image of the html</returns>
    public PdfDocument GeneratePdf(string html, PageSize pageSize, int margin = 20, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
    {
        var config = new PdfGenerateConfig();
        config.PageSize = pageSize;
        config.SetMargins(margin);
        return GeneratePdf(html, config, cssData, stylesheetLoad, imageLoad);
    }

    /// <summary>
    /// Create PDF document from given HTML.<br/>
    /// </summary>
    /// <param name="html">HTML source to create PDF from</param>
    /// <param name="config">the configuration to use for the PDF generation (page size/page orientation/margins/etc.)</param>
    /// <param name="cssData">optional: the style to use for html rendering (default - use W3 default style)</param>
    /// <param name="stylesheetLoad">optional: can be used to overwrite stylesheet resolution logic</param>
    /// <param name="imageLoad">optional: can be used to overwrite image resolution logic</param>
    /// <returns>the generated image of the html</returns>
    public PdfDocument GeneratePdf(string html, PdfGenerateConfig config, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
    {
        // create PDF document to render the HTML into
        var document = new PdfDocument();

        // add rendered PDF pages to document
        addPdfPages(document, html, config, cssData, stylesheetLoad, imageLoad);

        return document;
    }

    #region Private/Protected methods
    /// <summary>
    /// Create PDF pages from given HTML and appends them to the provided PDF document.<br/>
    /// </summary>
    /// <param name="document">PDF document to append pages to</param>
    /// <param name="html">HTML source to create PDF from</param>
    /// <param name="pageSize">the page size to use for each page in the generated pdf </param>
    /// <param name="margin">the margin to use between the HTML and the edges of each page</param>
    /// <param name="cssData">optional: the style to use for html rendering (default - use W3 default style)</param>
    /// <param name="stylesheetLoad">optional: can be used to overwrite stylesheet resolution logic</param>
    /// <param name="imageLoad">optional: can be used to overwrite image resolution logic</param>
    /// <returns>the generated image of the html</returns>
    private static void addPdfPages(PdfDocument document, string html, PageSize pageSize, int margin = 20, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
    {
        var config = new PdfGenerateConfig();
        config.PageSize = pageSize;
        config.SetMargins(margin);
        addPdfPages(document, html, config, cssData, stylesheetLoad, imageLoad);
    }

    /// <summary>
    /// Create PDF pages from given HTML and appends them to the provided PDF document.<br/>
    /// </summary>
    /// <param name="document">PDF document to append pages to</param>
    /// <param name="html">HTML source to create PDF from</param>
    /// <param name="config">the configuration to use for the PDF generation (page size/page orientation/margins/etc.)</param>
    /// <param name="cssData">optional: the style to use for html rendering (default - use W3 default style)</param>
    /// <param name="stylesheetLoad">optional: can be used to overwrite stylesheet resolution logic</param>
    /// <param name="imageLoad">optional: can be used to overwrite image resolution logic</param>
    /// <returns>the generated image of the html</returns>
    private static void addPdfPages(PdfDocument document, string html, PdfGenerateConfig config, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
    {
        XSize orgPageSize;
        // get the size of each page to layout the HTML in
        if (config.PageSize != PageSize.Undefined)
            orgPageSize = PageSizeConverter.ToSize(config.PageSize);
        else
            orgPageSize = config.ManualPageSize;

        if (config.PageOrientation == PageOrientation.Landscape)
        {
            // invert pagesize for landscape
            orgPageSize = new XSize(orgPageSize.Height, orgPageSize.Width);
        }

        var pageSize = new XSize(orgPageSize.Width - config.MarginLeft - config.MarginRight, orgPageSize.Height - config.MarginTop - config.MarginBottom);

        if (!string.IsNullOrEmpty(html))
        {
            using (var container = new HtmlContainer())
            {
                if (stylesheetLoad != null)
                    container.StylesheetLoad += stylesheetLoad;
                if (imageLoad != null)
                    container.ImageLoad += imageLoad;

                container.Location = new XPoint(config.MarginLeft, config.MarginTop);
                container.MaxSize = new XSize(pageSize.Width, 0);
                container.SetHtml(html, cssData);
                container.PageSize = pageSize;
                container.MarginBottom = config.MarginBottom;
                container.MarginLeft = config.MarginLeft;
                container.MarginRight = config.MarginRight;
                container.MarginTop = config.MarginTop;

                // layout the HTML with the page width restriction to know how many pages are required
                using (var measure = XGraphics.CreateMeasureContext(pageSize, XGraphicsUnit.Point, XPageDirection.Downwards))
                {
                    container.PerformLayout(measure);
                }

                // while there is un-rendered HTML, create another PDF page and render with proper offset for the next page
                double scrollOffset = 0;
                while (scrollOffset > -container.ActualSize.Height)
                {
                    var page = document.AddPage();
                    page.Height = XUnit.FromPoint(orgPageSize.Height);
                    page.Width = XUnit.FromPoint(orgPageSize.Width);

                    using (var g = XGraphics.FromPdfPage(page))
                    {
                        //g.IntersectClip(new XRect(config.MarginLeft, config.MarginTop, pageSize.Width, pageSize.Height));
                        g.IntersectClip(new XRect(0, 0, page.Width.Point, page.Height.Point));

                        container.ScrollOffset = new XPoint(0, scrollOffset);
                        container.PerformPaint(g);
                    }
                    scrollOffset -= pageSize.Height;
                }

                // add web links and anchors
                handleLinks(document, container, orgPageSize, pageSize);
            }
        }
    }

    /// <summary>
    /// Handle HTML links by create PDF Documents link either to external URL or to another page in the document.
    /// </summary>
    private static void handleLinks(PdfDocument document, HtmlContainer container, XSize orgPageSize, XSize pageSize)
    {
        foreach (var link in container.GetLinks())
        {
            int i = (int)(link.Rectangle.Top / pageSize.Height);
            for (; i < document.Pages.Count && pageSize.Height * i < link.Rectangle.Bottom; i++)
            {
                var offset = pageSize.Height * i;

                // fucking position is from the bottom of the page
                var xRect = new XRect(link.Rectangle.Left, orgPageSize.Height - (link.Rectangle.Height + link.Rectangle.Top - offset), link.Rectangle.Width, link.Rectangle.Height);

                if (link.IsAnchor)
                {
                    // create link to another page in the document
                    var anchorRect = container.GetElementRectangle(link.AnchorId);
                    if (anchorRect.HasValue)
                    {
                        // document links to the same page as the link is not allowed
                        int anchorPageIdx = (int)(anchorRect.Value.Top / pageSize.Height);
                        if (i != anchorPageIdx)
                            document.Pages[i].AddDocumentLink(new PdfRectangle(xRect), anchorPageIdx);
                    }
                }
                else
                {
                    // create link to URL
                    document.Pages[i].AddWebLink(new PdfRectangle(xRect), link.Href);
                }
            }
        }
    }

    #endregion
}