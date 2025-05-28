using PdfSharpCore;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf.IO;

namespace PlainHtmlToPdf.Tests;

public class PdfGenerator_ExternalResources_Tests
{
    [Fact]
    public void GerneratePdfFromHtmlWithExternalResources()
    {
        // Arrange
        var html = ReadFileToStream("photo_slide.html");

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

        // Save the stream to file for manual inspection if needed
        resultStream.Position = 0;
        using var fileStream = new FileStream("photo_slide.pdf", FileMode.Create, FileAccess.Write);
        resultStream.CopyTo(fileStream);
    }


    private static string ReadFileToStream(string fileName)
    {
        var sourceDir = Path.GetDirectoryName(typeof(PdfGenerator_ExternalResources_Tests).Assembly.Location);
        var projectRoot = Path.GetFullPath(Path.Combine(sourceDir, @"../../../../"));
        var filePath = Path.Combine(projectRoot, "PlainHtmlToPdf.Tests", "TestData", fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }
        return File.ReadAllText(filePath);
    }
}