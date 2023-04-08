﻿using Gum.InnerThoughts;

namespace Gum
{
    internal static partial class Tokens
    {
        public const char SpeakerToken = ':';
        public const char SpeakerPortraitToken = '.';
    }

    public partial class Parser
    {
        private bool ParseLine(ReadOnlySpan<char> line, int _, int columnIndex, bool isNested)
        {
            CheckAndCreateLinearBlock(joinLevel: 0, isNested);

            // This is probably just a line! So let's just read as it is.
            AddLineToBlock(line);
            return true;
        }

        private void AddLineToBlock(ReadOnlySpan<char> line)
        {
            (string speaker, string? portrait) = ReadSpeakerAndLine(line, out int end);
            if (end != -1)
            {
                line = line.Slice(end + 1);
            }

            line = line.TrimStart().TrimEnd();

            Block.AddLine(speaker, portrait, line);
        }

        /// <summary>
        /// Read an optional string for the speaker and portrait in a line.
        /// Valid examples:
        ///     {speaker}.{portrait}: {line}
        ///     {speaker}: {line}
        ///     {line}
        /// </summary>
        private (string Speaker, string? Portrait) ReadSpeakerAndLine(ReadOnlySpan<char> line, out int end)
        {
            string speaker = Line.OWNER;
            string? portrait = null;

            end = -1;

            if (line.IsEmpty)
            {
                return (speaker, portrait);
            }

            ReadOnlySpan<char> speakerText = GetNextWord(line, out end);
            if (end == -1)
            {
                return (speaker, portrait);
            }

            // First, check if there is a namespace specified.
            end = speakerText.IndexOf(Tokens.SpeakerToken);
            if (end == -1)
            {
                return (speaker, portrait);
            }

            speakerText = speakerText.Slice(0, end);

            int portraitEndIndex = speakerText.IndexOf(Tokens.SpeakerPortraitToken);
            if (portraitEndIndex == -1)
            {
                return (speakerText.ToString(), portrait);
            }

            speaker = speakerText.Slice(0, portraitEndIndex).ToString();
            portrait = speakerText.Slice(portraitEndIndex + 1).ToString();

            return (speaker, portrait);
        }
    }
}
