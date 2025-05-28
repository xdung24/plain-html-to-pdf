using PdfSharpCore;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf.IO;

namespace PlainHtmlToPdf.Tests;

public class PdfGenerator_Tests
{
    private static readonly bool _useSystemFonts = true; // Set to true to use system fonts
    private static bool _fontResolverInitialized = false;

    // Ensure the custom font resolver is set only once for all test cases
    static PdfGenerator_Tests()
    {
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
    }

    [Theory]
    [InlineData("eform_fpt", "Times New Roman")]
    [InlineData("eform_fpt", "Segoe UI")]
    [InlineData("eform_fpt", "Consolas")]
    [InlineData("eform_fpt", "Noto Sans")]
    [InlineData("eform_fpt", "Noto Serif")]
    [InlineData("eform_fpt", "Liberation Sans")]
    [InlineData("eform_fpt", "Liberation Serif")]
    [InlineData("eform_fpt", "Liberation Mono")]
    public void GerneratePdfFromHtml(string testName, string fontName)
    {
        // Add font family mappings for the specified font
        PdfGenerator.AddFontFamilyMapping("Times New Roman", "Times New Roman");
        PdfGenerator.AddFontFamilyMapping("Segoe UI", "Segoe UI");
        PdfGenerator.AddFontFamilyMapping("Consolas", "Consolas");
        PdfGenerator.AddFontFamilyMapping("Noto Sans", "Noto Sans");
        PdfGenerator.AddFontFamilyMapping("Noto Serif", "Noto Serif");
        PdfGenerator.AddFontFamilyMapping("Liberation Sans", "Liberation Sans");
        PdfGenerator.AddFontFamilyMapping("Liberation Serif", "Liberation Serif");
        PdfGenerator.AddFontFamilyMapping("Liberation Mono", "Liberation Mono");

        // Set the default font for PDF generation
        PdfGenerator.DefaultFont = fontName;

        // Arrange
        var html = ReadFileToStream($"{testName}.html");

        // Act
        var pdfDocument = PdfGenerator.GeneratePdf(html,
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
        resultStream.CopyTo(fileStream);
    }


    private static string ReadFileToStream(string fileName)
    {
        var sourceDir = Path.GetDirectoryName(typeof(PdfGenerator_Tests).Assembly.Location);
        var projectRoot = Path.GetFullPath(Path.Combine(sourceDir, @"../../../../"));
        var filePath = Path.Combine(projectRoot, "PlainHtmlToPdf.Tests", "TestData", fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }
        return File.ReadAllText(filePath);
    }
}