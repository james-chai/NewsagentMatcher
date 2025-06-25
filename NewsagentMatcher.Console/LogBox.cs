using Serilog;
using System.Text;

namespace NewsagentMatcher.Console;

public static class LogBox
{
    private const int FixedWidth = 60;
    private const int TextPadding = 2; 

    public static void BoxedInformation(string title, string? content = null)
    {
        var border = new string('═', FixedWidth);
        var emptyLine = $"║{new string(' ', FixedWidth)}║";

        var message = new StringBuilder()
            .AppendLine($"╔{border}╗")
            .AppendLine(emptyLine);

        message.AppendLine(CreateCenteredLine(title));

        if (!string.IsNullOrEmpty(content))
        {
            message.AppendLine($"╠{new string('─', FixedWidth)}╣")
                   .AppendLine(emptyLine);

            foreach (var line in WrapText(content, FixedWidth - 2 * TextPadding))
            {
                message.AppendLine($"║  {line.PadRight(FixedWidth - 2)}║");
            }
        }

        message.AppendLine(emptyLine)
               .AppendLine($"╚{border}╝");

        Log.Information(message.ToString());
    }

    private static string CreateCenteredLine(string text)
    {
        if (text.Length >= FixedWidth - 2 * TextPadding)
            return $"║  {text.Substring(0, FixedWidth - 2 * TextPadding - 1)}… ║";

        var padding = (FixedWidth - text.Length) / 2;
        return $"║{text.PadLeft(padding + text.Length).PadRight(FixedWidth)}║";
    }

    private static IEnumerable<string> WrapText(string text, int maxWidth)
    {
        if (string.IsNullOrEmpty(text)) yield break;

        var words = text.Split(' ');
        var currentLine = new StringBuilder();

        foreach (var word in words)
        {
            if (currentLine.Length + word.Length + 1 > maxWidth)
            {
                yield return currentLine.ToString();
                currentLine.Clear();
            }
            currentLine.Append(currentLine.Length == 0 ? word : " " + word);
        }

        if (currentLine.Length > 0)
            yield return currentLine.ToString();
    }
}