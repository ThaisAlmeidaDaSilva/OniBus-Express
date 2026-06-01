using System.Text.RegularExpressions;

namespace OnibusExpress.Application.Validation;

public static partial class CpfValidator
{
    public static bool IsValid(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) || !CpfFormatRegex().IsMatch(cpf))
        {
            return false;
        }

        var digits = OnlyDigits(cpf);

        if (digits.Distinct().Count() == 1)
        {
            return false;
        }

        return CalculateDigit(digits, 9) == digits[9] - '0' &&
            CalculateDigit(digits, 10) == digits[10] - '0';
    }

    public static string Format(string cpf)
    {
        var digits = OnlyDigits(cpf);
        return $"{digits[..3]}.{digits[3..6]}.{digits[6..9]}-{digits[9..]}";
    }

    private static string OnlyDigits(string value)
    {
        return new string(value.Where(char.IsDigit).ToArray());
    }

    private static int CalculateDigit(string digits, int length)
    {
        var sum = 0;

        for (var index = 0; index < length; index++)
        {
            sum += (digits[index] - '0') * (length + 1 - index);
        }

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }

    [GeneratedRegex(@"^(\d{11}|\d{3}\.\d{3}\.\d{3}-\d{2})$")]
    private static partial Regex CpfFormatRegex();
}
