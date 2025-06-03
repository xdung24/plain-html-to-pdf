using PdfSharp;

namespace PlainHtmlToPdf.Tests;

public class PdfGenerator_BasicPage_Tests : TestBase
{
    private static PdfGenerator _pdfGenerator = new PdfGenerator();
    public PdfGenerator_BasicPage_Tests()
    {
        // Initialize the PDF generator
        _pdfGenerator.DefaultFont = "Arial"; // Set a default font
    }

    [Fact]
    public async Task GerneratePdfFromBasicHtml()
    {
        // Arrange
        var html = ReadFileToStream("basic_page.html");

        // Act
        var pdfDocument = _pdfGenerator.GeneratePdf(html,
            new PdfGenerateConfig
            {
                PageSize = PageSize.A4,
                PageOrientation = PageOrientation.Portrait,
            });
        var resultStream = new MemoryStream();
        pdfDocument.Save(resultStream, false);
        resultStream.Position = 0;
        pdfDocument.Close();
        pdfDocument.Dispose();

        // Assert
        Assert.NotNull(resultStream);
        Assert.True(resultStream.Length > 0);

        // Save the stream to file for manual inspection if needed
        resultStream.Position = 0;
        using var fileStream = new FileStream("basic_page.pdf", FileMode.Create, FileAccess.Write);
        await resultStream.CopyToAsync(fileStream);
    }
}