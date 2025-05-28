namespace PlainHtmlToPdf.Tests;

public class TestBase
{
    protected TestBase()
    {
        // Any common setup can be done here if needed
    }

    protected static string ReadFileToStream(string fileName)
    {
        var sourceDir = Path.GetDirectoryName(typeof(TestBase).Assembly.Location);
        if (sourceDir == null)
        {
            throw new InvalidOperationException("Source directory cannot be determined.");
        }
        var projectRoot = Path.GetFullPath(Path.Combine(sourceDir, @"../../../../"));
        var filePath = Path.Combine(projectRoot, "PlainHtmlToPdf.Tests", "TestData", fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }
        return File.ReadAllText(filePath);
    }
}