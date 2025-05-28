using PdfSharpCore;

namespace PlainHtmlToPdf.Tests;

public class PdfGenerator_ExternalResources_Tests : TestBase
{
    protected static PdfGenerator _pdfGenerator = new PdfGenerator();

    public PdfGenerator_ExternalResources_Tests()
    {
    }

    [Fact]
    public async Task GerneratePdfFromHtmlWithExternalResources()
    {
        // Arrange
        var html = ReadFileToStream("photo_slide.html");

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
        using var fileStream = new FileStream("photo_slide.pdf", FileMode.Create, FileAccess.Write);
        await resultStream.CopyToAsync(fileStream);
    }
}