using System.Text.RegularExpressions;

namespace CulinaryBlog.Domain.ValueObjects;

// SRS mục 6.2 - Value Object EmailAddress. Đảm bảo email luôn hợp lệ và
// được chuẩn hóa (lowercase, trim) ngay từ tầng Domain.
public sealed record EmailAddress
{
    private static readonly Regex Pattern =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Email không được để trống.", nameof(input));

        var normalized = input.Trim().ToLowerInvariant();
        if (!Pattern.IsMatch(normalized))
            throw new ArgumentException($"Email \"{input}\" không đúng định dạng.", nameof(input));

        return new EmailAddress(normalized);
    }

    public override string ToString() => Value;
    public static implicit operator string(EmailAddress email) => email.Value;
}
