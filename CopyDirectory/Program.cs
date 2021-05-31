using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyDirectory
{
    /**
     * This is the app's entry point. 
     * Arguments are to be provided in the order below for the operations to be performed correctly. Optionals can be in any order after 2.
     * 0. action keyword [ copy | move  ] depending on operation to be executed, only 1 of them.
     * 1. file/directory's path to be copied/moved
     * 2. directory's path where it will be newly stored
     * 3. (optional) flag '-f' without quotes, for moving individual files. Default mode works with whole directories and subdirectories.
     * 4. (optional) flag '-m' for allowing directory merge if destination dir already exists. False by default
     * 5. (optional) flag '-o' for overwriting files with same name. False by default
     */
    class Program
    {
        static void Main(string[] args)
        {
            
            if (args.Length == 0)
            {
                Console.WriteLine("Please supply arguments.");
                Console.ReadLine();
                return;
            } 
            
            
            /*
             * Possible future improvement idea
             * Implement a parameter parser to use unix-like commands (similar to the -f flag)
             * In this way, the order would not matter and if the solution was to expand in the future,
             * increasing the amount of parameters, it would prove more easily scalable.
             */
            var action = args[0];
            var onlyOneFile = args.Contains("-f");
            var mergeDirs = args.Contains("-m");
            var overwriteFiles = args.Contains("-o");
            var finalOutput = "";

            switch (action)
            {
                case "copy" :
                    finalOutput = onlyOneFile ? FileUtilities.CopyFile(args[1], args[2], overwriteFiles) 
                        : FileUtilities.CopyDirectory(args[1], args[2], overwriteFiles, mergeDirs);
                    break;
                
                case "move" :
                    finalOutput = onlyOneFile ? FileUtilities.MoveFile(args[1], args[2], overwriteFiles) 
                        : FileUtilities.MoveDirectory(args[1], args[2], overwriteFiles, mergeDirs);
                    break;
                default :
                    finalOutput = "Please check arguments order or input correct values when opening the app";
                    break;
            }
           
            Console.WriteLine(finalOutput);
            Console.ReadLine();
        }
    }
}
