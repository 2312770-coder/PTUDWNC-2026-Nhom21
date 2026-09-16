using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CulinaryBlog.Domain.ValueObjects;

// SRS mục 6.2 - Value Object Slug.
// Sinh URL-friendly identifier từ chuỗi tiếng Việt có dấu:
// "Món Tráng Miệng" -> "mon-trang-mieng". Dùng cho Recipe.Slug và Category.Slug.
public sealed record Slug
{
    public string Value { get; }

    private Slug(string value) => Value = value;

    public static Slug Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Không thể sinh slug từ chuỗi rỗng.", nameof(input));

        var normalized = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);

        var sb = new StringBuilder();
        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }

        var slug = sb.ToString()
            .Replace("đ", "d")
            .Normalize(NormalizationForm.FormC);

        slug = Regex.Replace(slug, "[^a-z0-9\\s-]", "");
        slug = Regex.Replace(slug, "[\\s-]+", "-").Trim(char.Parse("-"));

        return new Slug(slug);
    }

    public override string ToString() => Value;
    public static implicit operator string(Slug slug) => slug.Value;
}
