using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyDirectory
{
    /**
     * This is the app's entry point, it holds the GUI
     * and 
     */
    class Program
    {
        static void Main(string[] args)
        {
            var message = FileUtilities.CopyDirectory(@"C:\Users\jpmon\Desktop\in",
                @"C:\Users\jpmon\Desktop\out", false);
            Console.WriteLine(message);
        }
    }
}
