using System;

namespace laundry.project.Presentation
{
    public enum ConsoleZone { Left, Right }

    internal static class Uitility
    {
        private static readonly object _lockLeft = new();
        private static readonly object _lockRight = new();

        private static int _topLeft = 0;
        private static int _topRight = 0;

        private static readonly int Margin = 2;

        private static int HalfWidth => Console.WindowWidth / 2;
        private static int LeftStart => Margin;
        private static int RightStart => HalfWidth + Margin;

        public static void ClearZone(ConsoleZone zone)
        {
            int startColumn = zone == ConsoleZone.Left ? 0 : HalfWidth;
            int width = HalfWidth;

            for (int row = 0; row < Console.WindowHeight; row++)
            {
                Console.SetCursorPosition(startColumn, row);
                Console.Write(new string(' ', width));
            }

            if (zone == ConsoleZone.Left)
                _topLeft = 0;
            else
                _topRight = 0;
        }

        public static void WriteLeft(this string text, ConsoleColor color)
        {
            lock (_lockLeft)
            {
                Write(text, ConsoleZone.Left, ref _topLeft, color, newLine: false);
            }
        }

        public static void WriteLineLeft(this string text, ConsoleColor color)
        {
            lock (_lockLeft)
            {
                Write(text, ConsoleZone.Left, ref _topLeft, color, newLine: true);
            }
        }

        public static void WriteRight(this string text, ConsoleColor color)
        {
            lock (_lockRight)
            {
                Write(text, ConsoleZone.Right, ref _topRight, color, newLine: false);
            }
        }

        public static void WriteLineRight(this string text, ConsoleColor color)
        {
            lock (_lockRight)
            {
                Write(text, ConsoleZone.Right, ref _topRight, color, newLine: true);
            }
        }

        public static string ReadLineLeft(ConsoleColor color)
        {
            lock (_lockLeft)
            {
                int cursorLeft = LeftStart + 0;
                Console.SetCursorPosition(cursorLeft, _topLeft);
                Console.ForegroundColor = color;
                string? input = Console.ReadLine();
                _topLeft++;
                return input ?? string.Empty;
            }
        }

        private static void Write(string text, ConsoleZone zone, ref int top, ConsoleColor color, bool newLine)
        {
            int left = zone == ConsoleZone.Left ? LeftStart : RightStart;

            // Clamp the top to avoid overflow
            if (top >= Console.WindowHeight - 1)
            {
                ClearZone(zone);
                top = 0;
            }

            // Clear current line fully
            Console.SetCursorPosition(left, top);
            Console.Write(new string(' ', HalfWidth - Margin * 2));

            // Reset cursor and write fresh
            Console.SetCursorPosition(left, top);
            Console.ForegroundColor = color;
            Console.Write(text);

            if (newLine) top++;
        }

    }
}
