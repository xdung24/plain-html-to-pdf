using PdfSharpCore;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf.IO;

namespace PlainHtmlToPdf.Tests;

public class PdfGenerator_CustomFonts_Tests : TestBase
{
    private static readonly bool _useSystemFonts = false; // Set to true to use system fonts
    private static bool _fontResolverInitialized = false;
    private static PdfGenerator _pdfGenerator = new PdfGenerator();

    // Ensure the custom font resolver is set only once for all test cases
    static PdfGenerator_CustomFonts_Tests()
    {
        // If useSystemFonts is true, we will not use the custom font resolver
        // and will rely on the default fonts installed in the system.
        if (_useSystemFonts)
        {
            return;
        }
        // CustomFontResolver will use custom fonts instead of looking the default fonts installed in the system
        if (!_fontResolverInitialized)
        {
            GlobalFontSettings.FontResolver = new CustomFontResolver();
            _fontResolverInitialized = true;
        }
        // Fallback to Times New Roman for Arial font, because Arial is not available in the default font resolver
        _pdfGenerator.AddFontFamilyMapping("Arial", "Times New Roman");
        // Fallback to Consolas for Courier New font, because Courier New is not available in the default font resolver
        _pdfGenerator.AddFontFamilyMapping("Courier New", "Consolas");
        // Fallback to Noto Sans for Liberation Sans font, because Liberation Sans is not available in the default font resolver
        _pdfGenerator.AddFontFamilyMapping("Liberation Sans", "Noto Sans");
        // Fallback to Noto Serif font for Liberation Serif, because Liberation Serif is not available in the default font resolver
        _pdfGenerator.AddFontFamilyMapping("Liberation Serif", "Noto Serif");
    }

    [Theory]
    [InlineData("eform_fpt", "Times New Roman")]
    [InlineData("eform_fpt", "Segoe UI")]
    [InlineData("eform_fpt", "Consolas")]
    [InlineData("eform_fpt", "Noto Sans")]
    [InlineData("eform_fpt", "Noto Serif")]
    [InlineData("eform_fpt", "Arial")]
    [InlineData("eform_fpt", "Courier New")]
    [InlineData("eform_fpt", "Liberation Sans")]
    [InlineData("eform_fpt", "Liberation Serif")]
    public async Task GerneratePdfFromHtml(string testName, string fontName)
    {
        // Set the default font for PDF generation
        _pdfGenerator.DefaultFont = fontName;

        // Arrange
        var html = ReadFileToStream($"{testName}.html");

        // Act
        var pdfDocument = _pdfGenerator.GeneratePdf(html,
            new PdfGenerateConfig
            {
                PageSize = PageSize.A4,
                PageOrientation = PageOrientation.Portrait,
                MarginTop = 40,
                MarginBottom = 40,
                MarginLeft = 25,
                MarginRight = 25
            });
        var resultStream = new MemoryStream();
        pdfDocument.Save(resultStream, false);
        resultStream.Position = 0;
        pdfDocument.Close();
        pdfDocument.Dispose();

        // Assert
        Assert.NotNull(resultStream);
        Assert.True(resultStream.Length > 0);

        // Verify PDF has 2 pages
        resultStream.Position = 0;
        var pdf = PdfReader.Open(resultStream, PdfDocumentOpenMode.Import);
        Assert.True(pdf.PageCount >= 2, "The generated PDF should have at least 2 pages.");

        // Rollback the stream position to the beginning for further processing
        resultStream.Position = 0;

        // Save the stream to file for manual inspection if needed
        using var fileStream = new FileStream($"{testName}_{fontName.Replace(' ', '_')}.pdf", FileMode.Create, FileAccess.Write);
        await resultStream.CopyToAsync(fileStream);
    }
}