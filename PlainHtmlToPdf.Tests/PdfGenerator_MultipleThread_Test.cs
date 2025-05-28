using PdfSharpCore;
using PdfSharpCore.Pdf;

namespace PlainHtmlToPdf.Tests;

public class PdfGenerator_MultipleThread_Test : TestBase
{
    [Fact]
    public async Task GeneratePdfFromHtmlInMultipleThreads()
    {
        // Arrange
        var template = ReadFileToStream("template.html");
        var tasks = new List<Task<PdfDocument>>();

        if (Directory.Exists("out"))
        {
            Directory.Delete("out", true);
        }
        int totalTasks = 100; // Total number of tasks to run

        // Act
        var _pdfGenerator = new PdfGenerator();
        for (int i = 0; i < totalTasks; i++)
        {
            int taskIndex = i + 1; // Capture the current index for the task
            tasks.Add(Task.Run(() =>
            {
                var img_src = $"https://github.com/yavuzceliker/sample-images/blob/main/images/image-{taskIndex}.jpg?raw=true";
                var html = template.Replace("{{img_src}}", img_src)
                                   .Replace("{{img_title}}", $"image {taskIndex}")
                                   .Replace("{{img_description}}", $"image {taskIndex} description");
                return _pdfGenerator.GeneratePdf(html,
                    new PdfGenerateConfig
                    {
                        PageSize = PageSize.A4,
                        PageOrientation = PageOrientation.Portrait,
                    });
            }));
        }

        var pdfDocuments = await Task.WhenAll(tasks);


        if (!Directory.Exists("out"))
        {
            Directory.CreateDirectory("out");
        }
        // Assert and save the generated PDFs to files
        Assert.True(pdfDocuments.Length == totalTasks, "The number of generated PDF documents should match the number of tasks.");
        for (int i = 0; i < totalTasks; i++)
        {
            Assert.NotNull(pdfDocuments[i]);
            Assert.True(pdfDocuments[i].PageCount > 0);
            var fileName = $"out\\output_{i + 1}.pdf";
            pdfDocuments[i].Save(fileName);
            Assert.True(File.Exists(fileName), $"The file {fileName} should exist after saving.");
        }
    }
}