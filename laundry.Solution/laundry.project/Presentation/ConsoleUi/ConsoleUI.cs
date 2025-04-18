using laundry.project.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laundry.project.Presentation.ConsoleUi
{
    class ConsoleUI
    {
        public static void ShowLoading(string message)
        {
            message.WriteLineLeft(ConsoleColor.Yellow);
            message.WriteLineRight(ConsoleColor.Blue);
        }

        public static void ShowSuccess(string message)
        {
            message.WriteLineLeft(ConsoleColor.Green);
            message.WriteLineRight(ConsoleColor.Green);
        }

        public static void ShowError(string message)
        {
            message.WriteLineLeft(ConsoleColor.Red);
            Thread.Sleep(2000);
            Console.Clear();
        }
        public static void SetConsoleSizeToMax()
        {
            try
            {
                int maxWidth = Console.LargestWindowWidth;
                int maxHeight = Console.LargestWindowHeight;
                Console.SetWindowSize(maxWidth, maxHeight);
                Console.SetBufferSize(maxWidth, maxHeight);
                Console.Title = "Laundry Management System";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting console size: {ex.Message}");
            }
        }
        public static void DrawSplitScreenSeparator()
        {
            int halfWidth = Console.WindowWidth / 2;
            ConsoleColor originalColor = Console.ForegroundColor;

            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(halfWidth, i);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("│");
            }

            Console.ForegroundColor = originalColor;
        }

        public static void ShowWelcomeScreen()
        {
            Console.Clear();
            "*** LAUNDRY MANAGEMENT SYSTEM  ***".WriteLineLeft(ConsoleColor.Cyan);
            "----------------------------------".WriteLineLeft(ConsoleColor.Cyan);
            "Welcome to the Laundry Management System".WriteLineLeft(ConsoleColor.White);
            "".WriteLineLeft(ConsoleColor.White);
        }
        public static void WriteHeaderLeft(string title, ConsoleColor color)
        {
            int headerWidth = Console.WindowWidth / 2 - 6;
            string border = new string('═', headerWidth);

            $"╔{border}╗".WriteLineLeft(color);
            $"║ {title.PadRight(headerWidth - 2)} ║".WriteLineLeft(color);
            $"╚{border}╝".WriteLineLeft(color);
            " ".WriteLineLeft(color); // Empty line after header
        }
        public static ConsoleColor GetStateColor(MachineState state)
        {
            return state switch
            {
                MachineState.A => ConsoleColor.Gray,    // OFF
                MachineState.D => ConsoleColor.Cyan,    // ON
                MachineState.C => ConsoleColor.Green,   // CYCLE (changed from Red to better distinguish)
                _ => ConsoleColor.DarkGray              // UNKNOWN
            };
        }

        public static void WriteHeaderRight(string title, ConsoleColor color)
        {
            int headerWidth = Console.WindowWidth / 2 - 6;
            string border = new string('═', headerWidth);

            $"╔{border}╗".WriteLineRight(color);
            $"║ {title.PadRight(headerWidth - 2)} ║".WriteLineRight(color);
            $"╚{border}╝".WriteLineRight(color);
            " ".WriteLineRight(color); // Empty line after header
        }
        public static void WriteSectionTitleLeft(string title, ConsoleColor color)
        {
            " ".WriteLineLeft(ConsoleColor.White); // Empty line before section
            title.WriteLineLeft(color);
            string border = new string('─', Console.WindowWidth / 2 - 6);
            border.WriteLineLeft(color);
        }
        public static void WriteSectionTitleRight(string title, ConsoleColor color)
        {
            " ".WriteLineRight(ConsoleColor.White); // Empty line before section
            title.WriteLineRight(color);
            string border = new string('─', Console.WindowWidth / 2 - 6);
            border.WriteLineRight(color);
        }
    }
}



