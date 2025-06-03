using PdfSharp.Fonts;
namespace PlainHtmlToPdf.Tests;

public class CustomFontResolver : IFontResolver
{
    public string DefaultFontName => "Times New Roman";

    public byte[]? GetFont(string faceName)
    {
        // Segoe UI
        if (faceName.Equals("Segoe UI", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/segoeui.ttf");
        if (faceName.Equals("Segoe UI#Regular", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/segoeui.ttf");
        if (faceName.Equals("Segoe UI#Bold", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/segoeuib.ttf");
        if (faceName.Equals("Segoe UI#Italic", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/segoeuii.ttf");
        if (faceName.Equals("Segoe UI#BoldItalic", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/segoeuib.ttf");
        // Times New Roman
        if (faceName.Equals("Times New Roman", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/times.ttf");
        if (faceName.Equals("Times New Roman#Regular", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/times.ttf");
        if (faceName.Equals("Times New Roman#Bold", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/timesbd.ttf");
        if (faceName.Equals("Times New Roman#Italic", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/timesi.ttf");
        if (faceName.Equals("Times New Roman#BoldItalic", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/timesbi.ttf");
        // Consolas
        if (faceName.Equals("Consolas", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/consola.ttf");
        if (faceName.Equals("Consolas#Regular", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/consola.ttf");
        if (faceName.Equals("Consolas#Bold", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/consolab.ttf");
        if (faceName.Equals("Consolas#Italic", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/consolai.ttf");
        if (faceName.Equals("Consolas#BoldItalic", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/consolaz.ttf");
        // Noto Sans
        if (faceName.Equals("Noto Sans", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/NotoSans-Regular.ttf");
        if (faceName.Equals("Noto Sans#Regular", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/NotoSans-Regular.ttf");
        if (faceName.Equals("Noto Sans#Bold", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/NotoSans-Bold.ttf");
        if (faceName.Equals("Noto Sans#Italic", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/NotoSans-Italic.ttf");
        if (faceName.Equals("Noto Sans#BoldItalic", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/NotoSans-BoldItalic.ttf");
        // Noto Serif
        if (faceName.Equals("Noto Serif", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/NotoSerif-Regular.ttf");
        if (faceName.Equals("Noto Serif#Regular", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/NotoSerif-Regular.ttf");
        if (faceName.Equals("Noto Serif#Bold", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/NotoSerif-Bold.ttf");
        if (faceName.Equals("Noto Serif#Italic", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/NotoSerif-Italic.ttf");
        if (faceName.Equals("Noto Serif#BoldItalic", StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes("Fonts/NotoSerif-BoldItalic.ttf");

        // fallback
        return null;
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        // Segoe UI
        if (familyName.Equals("Segoe UI", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold && isItalic)
                return new FontResolverInfo("Segoe UI#BoldItalic");
            if (isBold)
                return new FontResolverInfo("Segoe UI#Bold");
            if (isItalic)
                return new FontResolverInfo("Segoe UI#Italic");
            return new FontResolverInfo("Segoe UI#Regular");
        }
        // Times New Roman
        if (familyName.Equals("Times New Roman", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold && isItalic)
                return new FontResolverInfo("Times New Roman#BoldItalic");
            if (isBold)
                return new FontResolverInfo("Times New Roman#Bold");
            if (isItalic)
                return new FontResolverInfo("Times New Roman#Italic");
            return new FontResolverInfo("Times New Roman#Regular");
        }
        // Consolas
        if (familyName.Equals("Consolas", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold && isItalic)
                return new FontResolverInfo("Consolas#BoldItalic");
            if (isBold)
                return new FontResolverInfo("Consolas#Bold");
            if (isItalic)
                return new FontResolverInfo("Consolas#Italic");
            return new FontResolverInfo("Consolas#Regular");
        }
        // Noto Sans
        if (familyName.Equals("Noto Sans", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold && isItalic)
                return new FontResolverInfo("Noto Sans#BoldItalic");
            if (isBold)
                return new FontResolverInfo("Noto Sans#Bold");
            if (isItalic)
                return new FontResolverInfo("Noto Sans#Italic");
            return new FontResolverInfo("Noto Sans#Regular");
        }
        // Noto Serif
        if (familyName.Equals("Noto Serif", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold && isItalic)
                return new FontResolverInfo("Noto Serif#BoldItalic");
            if (isBold)
                return new FontResolverInfo("Noto Serif#Bold");
            if (isItalic)
                return new FontResolverInfo("Noto Serif#Italic");
            return new FontResolverInfo("Noto Serif#Regular");
        }
        // fallback to default
        return PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);
    }
}