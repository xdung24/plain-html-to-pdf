# PlainHtmlToPdf

PlainHtmlToPdf is a .NET library designed to generate PDFs from HTML content. It leverages the power of PdfSharp to create high-quality PDF documents with support for custom fonts, CSS styling, and more.

## Features

- Generate PDFs from HTML content.
- Support for custom font mappings.
- CSS DOM tree generation for styling.
- Integration with PdfSharp for PDF rendering.

## Usage

Generate PDF from HTML
```csharp
var pdfGenerator = new PdfGenerator();

var pdfDocument = pdfGenerator.GeneratePdf(htmlContent, new PdfGenerateConfig
{
    PageSize = PdfSharp.PageSize.A4,
    Margin = 20
});

// Save the PDF to a file
pdfDocument.Save("output.pdf");
```

Generating Base64 PDF from HTML
```csharp
var pdfGenerator = new PdfGenerator("Segoe UI");

var result = string.Empty;
using (var stream = new MemoryStream())
{
    var pdf = pdfGenerator.GeneratePdf(htmlContent, new PdfGenerateConfig
    {
        PageSize = PdfSharp.PageSize.A4,
        Margin = 20
    });

    pdf.Save(stream);

    result = Convert.ToBase64String(stream.ToArray());
}
```

## Known Issues
- The library does not support all HTML and CSS features. Complex layouts and advanced CSS may not render as expected.
- Some HTML elements may not be fully supported, leading to unexpected rendering in the PDF. (e.g., JavaScript, SVGs, css display: flex; ...)


## Documentation

**HtmlRenderer:** https://archive.codeplex.com/?p=htmlrenderer

**PdfSharp:** https://github.com/ststeiger/PdfSharp


## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.