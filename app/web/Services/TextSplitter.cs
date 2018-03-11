using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace LangBot.Web.Services
{
    public class TextSplitter
    {
        private static readonly Regex _whitespace = new Regex(@"\s+", RegexOptions.Compiled);

        public IList<string> SplitText(string text, int count)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            text = _whitespace.Replace(text, " ").Trim();

            if (count == 1)
                return new[] { text };

            if (text.Contains(";"))
                return text.Split(';', count).TrimAll().PadToLength(count, "");

            var words = _whitespace.Split(text);
            if (words.Length <= count) return words.PadToLength(count, "");

            TextLineStack best = null;
            long bestCost = long.MaxValue;
            foreach (var stack in AllSplits(words, count))
            {
                var cost = stack.Cost;
                if (cost < bestCost)
                {
                    best = stack;
                    bestCost = cost;
                }
            }

            return best.GetLines().PadToLength(count, "");
        }

        private IEnumerable<TextLineStack> AllSplits(ArraySegment<string> words, int groupCount)
        {
            if (groupCount == 1)
            {
                yield return new TextLineStack(new TextLine(words));
                yield break;
            }

            for (int take = 1; take <= words.Count - groupCount + 1; take++)
            {
                var line = new TextLine(words.Slice(0, take));
                foreach (var tail in AllSplits(words.Slice(take), groupCount - 1))
                    yield return new TextLineStack(line, tail);
            }
        }

        private class TextLine
        {
            private readonly ArraySegment<string> _segment;

            public int Length { get; }

            public override string ToString() => String.Join(" ", _segment);

            public TextLine(ArraySegment<string> segment)
            {
                _segment = segment;
                Length = segment.Sum(x => x.Length) + segment.Count - 1;
            }

            public long Cost(int longestLine)
            {
                long deficit = longestLine - Length;
                return deficit * deficit;
            }
        }

        private class TextLineStack
        {
            private readonly ImmutableStack<TextLine> _stack;

            public int LongestLine { get; }

            public long Cost { get => _stack.Sum(x => x.Cost(LongestLine)); }
            public IList<string> GetLines() => _stack.Select(x => x.ToString()).ToList();

            public TextLineStack(TextLine line)
            {
                _stack = ImmutableStack.Create(line);
                LongestLine = line.Length;
            }

            public TextLineStack(TextLine line, TextLineStack stack)
            {
                _stack = stack._stack.Push(line);
                LongestLine = Math.Max(line.Length, stack.LongestLine);
            }
        }
    }
}
