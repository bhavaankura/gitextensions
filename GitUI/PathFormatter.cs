﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace GitUI
{
    internal sealed class PathFormatter
    {
        [DllImport("shlwapi.dll")]
        private static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

        private readonly Graphics _graphics;
        private readonly Font _font;

        public PathFormatter(Graphics graphics, Font font)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            if (font == null)
                throw new ArgumentNullException("font");

            _graphics = graphics;
            _font = font;
        }

        private static string TruncatePath(string path, int length)
        {
            if (path.Length == length)
                return path;

            if (length <= 0)
                return string.Empty;

            var result = new StringBuilder(length);
            PathCompactPathEx(result, path, length, 0);
            return result.ToString();
        }

        public string FormatTextForDrawing(int width, string name, string oldName)
        {
            int step = 0;
            bool isNameBeingTruncated = true;
            int maxStep = oldName == null ? name.Length : Math.Max(name.Length, oldName.Length) * 2;
            string result = string.Empty;

            while (step <= maxStep)
            {
                result = FormatString(name, oldName, step, isNameBeingTruncated);

                if (_graphics.MeasureString(result, _font).Width <= width)
                    break;

                step++;
                isNameBeingTruncated = !isNameBeingTruncated;
            }

            return result;
        }

        private static string FormatString(string name, string oldName, int step, bool isNameTruncated)
        {
            if (oldName != null)
            {
                int numberOfTruncatedChars = step / 2;
                int nameTruncatedChars = isNameTruncated ? step - numberOfTruncatedChars : numberOfTruncatedChars;
                int oldNameTruncatedChars = step - nameTruncatedChars;

                return string.Concat(TruncatePath(name, name.Length - oldNameTruncatedChars), " (",
                                     TruncatePath(oldName, oldName.Length - oldNameTruncatedChars), ")");
            }

            return TruncatePath(name, name.Length - step);
        }
    }
}