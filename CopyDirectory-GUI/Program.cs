using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace CopyDirectory_GUI
{
    /**
     * This is the app's GUI. It builds and runs commands based on the user input.
     */
    public static class Program
    {
        static void Main(string[] args)
        {
            // Path to the CopyDirectory.exe file
            // Possible improvement: Allow all path inputs to be recognised as such by terminal to allow for autocomplete
            Console.WriteLine("Please enter the source path to the CopyDirectory app");
            var exePath = Console.ReadLine();
            while(exePath == null || exePath.Equals(""))
                exePath = Console.ReadLine();
            
            const string space = " ";
            var flags = "";
            
            var possibleActions = new []{"copy", "move"} ;
            var repeat = true;
            while (repeat)
            {
                // Action
                var action ="";
                Console.WriteLine("\nWhich operation would you like to carry out?");
                while(!ValidateOptions(action,possibleActions))
                    action = Console.ReadLine();
                flags = string.Concat(flags, $"{space}{action}");

                // Source path
                Console.WriteLine("\nPlease enter the source path of the file/directory to be worked with.");
                var sourcePath = Console.ReadLine();
                flags = string.Concat(flags, $"{space}{sourcePath ?? "*"}"); // * is a token for null value 
                
                // Destination path -- possible improvement: detect it's single file and auto generate name.ext if destination doesn't include it
                Console.WriteLine("\nPlease enter the path of the destination to place the file/directory. This includes name and extension for single files");
                var destinationPath = Console.ReadLine();
                flags = string.Concat(flags, $"{space}{destinationPath ?? "*"}"); // * is a token for null value 
                
                // Working mode
                Console.WriteLine("\nAre you affecting a single file? [Y]es, [N]o");
                var singleFile = ValidateYesNo();
                
                // Overwrite
                Console.WriteLine("\nWould you like to overwrite any files with the same name? [Y]es, [N]o");
                var overwrite = ValidateYesNo();
                
                // Merge directories
                Console.WriteLine("\nIf the destination directory already exists, would you like to merge it with the source directory? [Y]es, [N]o");
                var merge = ValidateYesNo();
                
                // Add optional parameters
                if (singleFile) flags = string.Concat(flags, " -f");
                if (merge) flags = string.Concat(flags, " -m");
                if (overwrite) flags = string.Concat(flags, " -o");

                // Launch new terminal with process 
                var psi = new ProcessStartInfo(exePath, flags)
                {
                    /*
                     * Arguments = $"start {exePath}{space}{flags}",
                     * This is to open a new cmd process and run exePath in parallel but
                     * it doesn't seem to work fine, debugging is needed. Hence why the display will be
                     * in the same terminal as being executed and will be blocked during copy.
                     * With some extra time it can be finished.
                     */
                    UseShellExecute = false,
                };
                try { Process.Start(psi); }
                catch (Exception)
                {
                    Console.WriteLine("Please check parameters and try again");
                    
                    continue;
                }
                
                // Allow for message to be displayed after operations complete since output is shared.
                System.Threading.Thread.Sleep(2000); 
                // Repeat?
                Console.WriteLine("Would you like to continue? [Y]es, [N]o");
                repeat = ValidateYesNo();
            }
        }

        // Small function that validates yes/no answers from the user
        private static bool ValidateYesNo()
        {
            var input = Console.ReadLine() ?? "";
            while(true)
            {
                switch (input.ToLower())
                {
                    default:
                        Console.WriteLine("Please enter yes, no, or either of the initials (y/n).");
                        input = Console.ReadLine() ?? "";
                        continue;
                    
                    case "yes":
                    case "y": return true;
                    case "no":
                    case "n": return false;
                }
            }
        }

        // Small function that validates user input lays within a preset collection of choices
        private static bool ValidateOptions(string input, string[] options)
        {
            if (input != null && options.Contains(input)) return true;
            
            Console.Write("Please enter one of: ");
            foreach (var op in options) Console.Write(", " + op);
            Console.WriteLine("\r");
            return false;
        }
    }
}
