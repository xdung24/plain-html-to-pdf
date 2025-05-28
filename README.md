# PlainHtmlToPdf

PlainHtmlToPdf is a .NET library designed to generate PDFs from HTML content. It leverages the power of PdfSharpCore to create high-quality PDF documents with support for custom fonts, CSS styling, and more.

## Features

- Generate PDFs from HTML content.
- Support for custom font mappings.
- CSS DOM tree generation for styling.
- Integration with PdfSharpCore for PDF rendering.

## Usage

Generate PDF from HTML
```csharp
var pdfDocument = PdfGenerator.GeneratePdf(htmlContent, new PdfGenerateConfig
{
    PageSize = PdfSharpCore.PageSize.A4,
    Margin = 20
});

// Save the PDF to a file
pdfDocument.Save("output.pdf");
```

Generating Base64 PDF from HTML
```csharp
var result = string.Empty;

using (var stream = new MemoryStream())
{
    var pdf = PdfGenerator.GeneratePdf(htmlContent, new PdfGenerateConfig
    {
        PageSize = PdfSharpCore.PageSize.A4,
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

**PdfSharpCore:** https://github.com/ststeiger/PdfSharpCore


## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.