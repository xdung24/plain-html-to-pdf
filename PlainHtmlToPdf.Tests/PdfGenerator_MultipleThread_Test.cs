using PdfSharpCore;
using PdfSharpCore.Pdf;

namespace PlainHtmlToPdf.Tests;

public class PdfGenerator_MultipleThread_Test : TestBase
{
    [Fact]
    public async Task GeneratePdfFromHtmlInMultipleThreads()
    {
        // Arrange
        var html = ReadFileToStream("basic_page.html");
        var tasks = new List<Task<PdfDocument>>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            int taskIndex = i; // Capture the current index for the task
            tasks.Add(Task.Run(() =>
            {
                var _pdfGenerator = new PdfGenerator();
                return _pdfGenerator.GeneratePdf(html,
                    new PdfGenerateConfig
                    {
                        PageSize = PageSize.A4,
                        PageOrientation = PageOrientation.Portrait,
                    });
            }));
        }

        var pdfDocuments = await Task.WhenAll(tasks);

        // Assert
        foreach (var pdfDocument in pdfDocuments)
        {
            Assert.NotNull(pdfDocument);
            Assert.True(pdfDocument.PageCount > 0);
        }
    }
}