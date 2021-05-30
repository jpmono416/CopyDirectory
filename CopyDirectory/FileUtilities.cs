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
         * This function moves the source directory (including the directory)
         * onto the destination location. Resulting in destination/sourceDir/*
         * The Move function implements the same behaviour.
         * 
         * <param name="dirSourcePath">The source (original) path of the directory</param>
         * <param name="dirDestinationPath"></param>
         * <param name="overwrite"></param>
         * <param name="mergeDirectories">Whether to blend the source directory into the destination in case it is preexisting, or cancel the operation.</param>
         */
        public static string CopyDirectory(string dirSourcePath, string dirDestinationPath, bool overwrite = false, bool mergeDirectories = false)
        {
            // Check no data is null
            var paths = new[] {dirSourcePath, dirDestinationPath};
            if(!CheckNotNull(paths)) return GetMessage(ReturnMessages.NullData);
            
            var finalDirPath = Path.Combine(dirDestinationPath, new DirectoryInfo(dirSourcePath).Name);
            
            if (Directory.Exists(finalDirPath) && !mergeDirectories)
                return GetMessage(ReturnMessages.InvalidMoveLocation);
            
            // Check dir exists
            if (!Directory.Exists(dirSourcePath)) return GetMessage(ReturnMessages.DirDoesNotExist);
            if (!Directory.Exists(finalDirPath)) Directory.CreateDirectory(finalDirPath);
            
            try
            {
                // Copy all files
                foreach (var file in Directory.GetFiles(dirSourcePath))
                {
                    // Get file name from current path to build final path
                    var finalFilePath = Path.Combine(finalDirPath, Path.GetFileName(file));
                
                    // Execute copy function
                    CopyFile(file, finalFilePath, overwrite);
                    
                    /*
                     * One issue with this approach is that if IOException occurs, the copied elements remain copied. 
                     * An approach to fx this is goes something as
                     *      create temporary folder
                     *      copy each file onto temp folder after checking it does not exist into the actual folder
                     *      destroy temp folder if error occurs. In this way either all or nothing is copied.
                     * Another approach is to avoid copying duplicated files by try/catching in Copy funct and skip duplicates
                     */
                }
                
                //Copy all the directories by invoking itself with each nested dir's path
                foreach (var dir in Directory.GetDirectories(dirSourcePath, "*", SearchOption.TopDirectoryOnly))
                    CopyDirectory(dir, finalDirPath, overwrite, mergeDirectories);
            }
            
            // IOException can be only 1 particular error in this context (internal code 80, file already exists)
            // ArgumentException and ArgumentNullException cannot be catched, as the code would not reach the try statement
            catch (IOException) { return GetMessage(ReturnMessages.OverwriteError); } 
            catch (Exception) { return GetMessage(ReturnMessages.FailedToCopy); }
            
            return GetMessage(ReturnMessages.CopiedSuccessfully);
        }

        /**
         * This function moves the source directory (including the directory)
         * onto the destination location. Resulting in destination/sourceDir/*
         * The Move function implements the same behaviour.
         * <param name="mergeDirectories">Whether to blend the source directory into the destination in case it is preexisting, or cancel the operation.</param>
         */
        public static string MoveDirectory(string dirSourcePath, string dirDestinationPath, bool overwrite = false, bool mergeDirectories = false)
        {
            // Check no data is null - this is executed twice as Copy does it too,
            // however, it is an important-enough check to have at the beginning of both these functions.
            // Possible improvement for this solution if I worked more time on it -> Avoid duplicating these 2 lines
            var paths = new[] {dirSourcePath, dirDestinationPath};
            if(!CheckNotNull(paths)) return GetMessage(ReturnMessages.NullData);

            // Stop if there's an error in the copy process
            var output = CopyDirectory(dirSourcePath, dirDestinationPath, overwrite);
            if (output != GetMessage(ReturnMessages.CopiedSuccessfully)) return output;
            
            // Delete function uses the recycling bin
            try { Directory.Delete(dirSourcePath, true); }
            catch (IOException e) { Console.WriteLine(e.Message); }
            
            return GetMessage(ReturnMessages.MovedSuccessfully);
        }

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
                case ReturnMessages.InvalidMoveLocation : return "Merge directories option is set to false and there is already a directory at the destination location.";
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
        InvalidMoveLocation,
        OverwriteError,
        NullData
    }
}