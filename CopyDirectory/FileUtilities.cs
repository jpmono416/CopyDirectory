using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CopyDirectory
{
    public static class FileUtilities
    {
        public static void CopyFile(string fileSourcePath, string fileDestinationPath, bool overwrite)
        {
            // TODO Open new terminal with progress metrics
            
            File.Copy(fileSourcePath, fileDestinationPath, overwrite);
            
            // Close extra terminal
        }
        
        public static void MoveFile(string fileSourcePath, string fileDestinationPath, bool overwrite)
        {
            // TODO Open new terminal with progress metrics
            
            File.Copy(fileSourcePath, fileDestinationPath, overwrite);
            Directory.Delete(fileSourcePath);
            // Close extra terminal
        }

        /**
         * Copy a directory and all of its contents to a new given location.
         * <param name="dirSourcePath">The source (original) path of the directory</param>
         * <param name="dirDestinationPath"></param>
         * <param name="overwrite"></param>
         * <param name="move"></param>
         */
        public static string CopyDirectory(string dirSourcePath, string dirDestinationPath, bool overwrite = false)
        {
            // Check no data is null
            var paths = new[] {dirSourcePath, dirDestinationPath};
            if(!CheckNotNull(paths)) return GetMessage(ReturnMessages.NullData);
            
            // Check dir exists
            if (!DirectoryExists(dirSourcePath)) return GetMessage(ReturnMessages.DirDoesNotExist);

            // Get all files
            var filePaths = Directory.GetFiles(dirSourcePath);
            
            /*
             * Copy function only throws when data is not supplied or it's empty but
             * we already checked for null, so we don't need to catch ArgumentNullException
             */
            try
            {
                foreach (var file in filePaths)
                {
                    // Get file name from current path to build final path 
                    var name = Path.GetFileName(file);
                    var destinationFullAddress = Path.Combine(dirDestinationPath, name);
                
                    // Execute copy function
                    CopyFile(file, destinationFullAddress, overwrite);
                    
                    /* One issue with this approach is that if IOException occurs, the copied elements remain copied. 
                     * An approach to fx this is goes something as
                     * create temporary folder
                     * copy each file onto temp folder after checking it does not exist into the actual folder
                     * destroy temp folder if error occurs. In this way either all or nothing is copied.
                     * TODO Another approach is to avoid copying duplicated files by catching in Copy and skipping duplicated
                     */
                }

                return GetMessage(ReturnMessages.CopiedSuccessfully);
            }
            // No need to throw error and stop program execution, just return error message
            // IOException means 1 particular error in this context (internal code 80, file already exists)
            // ArgumentException and ArgumentNullException cannot be catched, as the code would not reach the try statement
            catch (IOException ioExcp) { return GetMessage(ReturnMessages.OverwriteError); } 
            catch (Exception e) { return GetMessage(ReturnMessages.FailedToCopy); }
        }

        public static string MoveDirectory(string dirSourcePath, string dirDestinationPath, bool overwrite = false)
        {
            // Copy contents over
            var output = CopyDirectory(dirSourcePath, dirDestinationPath, overwrite);
            
            // Remove contents after successful copy or inform user of otherwise
            if (output != GetMessage(ReturnMessages.CopiedSuccessfully)) return output;
            
            try { Directory.Delete(dirSourcePath, true); }
            catch (IOException e) { Console.WriteLine(e.Message); }

            // Return OK message
            return GetMessage(ReturnMessages.MovedSuccessfully);
        }

        /**
         * Return whether a given directory exists or not. Using function to improve
         * code readability and ease of usage.
         */
        private static bool DirectoryExists(string path) => Directory.Exists(path);

        /**
         * This function checks that a series of items are not null.
         * This is used for checking that no paths given by the user are null
         * but the function is designed in this way so that 
         */
        private static bool CheckNotNull(IEnumerable<string> args) => args.All(path => path != null); 
        
        /**
         * Get the message for the main terminal to be outputted to the user after the desired
         * operation is completed on the secondary terminal.
         */
        private static string GetMessage(ReturnMessages messageCode)
        {
            switch (messageCode)
            {
                default                                 : return "An unexpected error occurred. Please try again. If you keep getting this error, check the values inputted.";
                case ReturnMessages.CopiedSuccessfully  : return "Copied successfully.";
                case ReturnMessages.MovedSuccessfully   : return "Moved successfully.";
                case ReturnMessages.FailedToCopy        : return "An error occurred during the operation. Please ensure there is enough space and try again.";
                case ReturnMessages.DirDoesNotExist     : return "Directory does not exist. Please ensure path is correct.";
                case ReturnMessages.FileDoesNotExist    : return "File does not exist. Please ensure path is correct.";
                case ReturnMessages.NullData            : return "Please input all data required.";
                case ReturnMessages.OverwriteError      : return "Input/Output error. Copy was invoked with param 'overwrite' set to" +
                                                                 " false and at least one file already exists. Please set to true and try again.";
            }
        }
    }

    /**
     * Enum used to keycode the finite different types of return messages for the possible outcomes
     * regarding the copying process. This is later converted into user-friendly messages.
     */
    public enum ReturnMessages
    {
        DirDoesNotExist,
        FileDoesNotExist,
        FailedToCopy,
        CopiedSuccessfully,
        MovedSuccessfully,
        OverwriteError,
        NullData
    }
}