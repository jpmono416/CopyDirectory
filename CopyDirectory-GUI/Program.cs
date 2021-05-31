using System;
using System.Diagnostics;

namespace CopyDirectory_GUI
{
    /**
     * This is the app's GUI. It builds and runs commands based on the user input.
     */
    public class Program
    {
        static void Main(string[] args)
        {
            // This path assumes that both final executable files will be in the same directory
            var exePath = "CopyDirectory.exe";
            Process.Start(exePath, @"copy C:\Users\jpmon\Desktop\in C:\Users\jpmon\Desktop\out");

            Console.ReadLine();

        }
    }
}
