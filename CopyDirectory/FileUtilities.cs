using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CopyDirectory
{
    public static class FileUtilities
    {
        /**
         * This function moves one particular file at a time
         * When invoked individually e.g. not copying a whole directory, the name
         * and extension needs to be included in the destination path.
         * Same applies for MoveFile function
         * <param name="fileSourcePath">Path including name and extension of new file location</param>
         * <param name="fileDestinationPath">Path including name and extension of new file location</param>
         * <param name="overwrite">Overwrite file if it already exists on destination</param>
         */
        public static string CopyFile(string fileSourcePath, string fileDestinationPath, bool overwrite = false)
        {
            // Print has 1 indent for better visualization when copying dirs
            Console.WriteLine("    -Copying file {0} to {1}", fileSourcePath, fileDestinationPath);
            try 
            {
                File.Copy(fileSourcePath, fileDestinationPath, overwrite);    
            }
            catch(ArgumentException) {return GetMessage(ReturnMessages.NullData); }
            catch (IOException) { return GetMessage(ReturnMessages.OverwriteError); } 
            catch (Exception) { return GetMessage(ReturnMessages.FailedToCopy); }
            
            return GetMessage(ReturnMessages.CopiedSuccessfully);
        }
        
        /**
         * This is the move function. It is designed for individual use only,
         * since the MoveDirectory function implements the CopyFile pathway and manually deletes later.
         * <param name="fileSourcePath">Path including name and extension of new file location</param>
         * <param name="fileDestinationPath">Path including name and extension of new file location</param>
         * <param name="overwrite">Overwrite file if it already exists on destination</param>
         */
        public static string MoveFile(string fileSourcePath, string fileDestinationPath, bool overwrite = false)
        {
            // Print has 1 indent for better visualization when moving dirs
            Console.WriteLine("    -Moving file {0} to {1}", fileSourcePath, fileDestinationPath);
            try
            {
                File.Copy(fileSourcePath, fileDestinationPath, overwrite);
                Directory.Delete(fileSourcePath);
            }
            catch(ArgumentException) {return GetMessage(ReturnMessages.NullData); }
            catch (IOException) { return GetMessage(ReturnMessages.OverwriteError); } 
            catch (Exception) { return GetMessage(ReturnMessages.FailedToCopy); }
            
            return GetMessage(ReturnMessages.MovedSuccessfully);
        }

        /**
         * This function makes a copy of the source directory
         * onto the destination location. Resulting in destination/sourceDir/*
         * The MoveDirectory function implements the same behaviour.
         * <param name="dirSourcePath">Path including name and extension of new file location</param>
         * <param name="dirDestinationPath">Path of new directory</param>
         * <param name="overwrite">Overwrite file if it already exists on destination</param>
         * <param name="mergeDirectories">Whether to blend the source directory into the destination in case it is preexisting, or cancel the operation.</param>
         */
        public static string CopyDirectory(string dirSourcePath, string dirDestinationPath, bool overwrite = false, bool mergeDirectories = false)
        {
            try
            {
                // Check no data is null
                var paths = new[] {dirSourcePath, dirDestinationPath};
                if(!CheckNotNull(paths)) return GetMessage(ReturnMessages.NullData);
            
                var finalDirPath = Path.Combine(dirDestinationPath, new DirectoryInfo(dirSourcePath).Name);
            
                if (Directory.Exists(finalDirPath) && !mergeDirectories)
                    return GetMessage(ReturnMessages.InvalidMoveLocation);
            
                // Check dir exists and log process start
                if (!Directory.Exists(dirSourcePath)) return GetMessage(ReturnMessages.DirDoesNotExist);
                if (!Directory.Exists(finalDirPath)) Directory.CreateDirectory(finalDirPath);
                Console.WriteLine("+Copying directory {0} to {1}", dirSourcePath, finalDirPath);
            
                // Copy all files
                foreach (var file in Directory.GetFiles(dirSourcePath))
                {
                    // Get file name from current path to build final path
                    var finalFilePath = Path.Combine(finalDirPath, Path.GetFileName(file));

                    // Execute copy function
                    var result = CopyFile(file, finalFilePath, overwrite);
                    if (result != GetMessage(ReturnMessages.CopiedSuccessfully)) return result;
                }
                
                // Copy all the directories by invoking itself with each nested dir's path
                foreach (var dir in Directory.GetDirectories(dirSourcePath, "*", SearchOption.TopDirectoryOnly))
                {
                    var result = CopyDirectory(dir, finalDirPath, overwrite, mergeDirectories);
                    if (result != GetMessage(ReturnMessages.CopiedSuccessfully)) return result;
                }
            
                return GetMessage(ReturnMessages.CopiedSuccessfully);
            }
            catch (Exception)
            {
                return GetMessage(ReturnMessages.FailedToCopy);
            }
            
        }
        
        /**
         * This function makes use of the CopyDirectory function
         * and safely deletes the source repository afterwards. 
         * <param name="dirSourcePath">Path including name and extension of new file location</param>
         * <param name="dirDestinationPath">Path of new directory</param>
         * <param name="overwrite">Overwrite file if it already exists on destination</param>
         * <param name="mergeDirectories">Whether to blend the source directory into the destination in case it is preexisting, or cancel the operation.</param>
         */
        public static string MoveDirectory(string dirSourcePath, string dirDestinationPath, bool overwrite = false, bool mergeDirectories = false)
        {
            /*
             * Check no data is null - this is executed twice as Copy does it too,
             * however, it is an important-enough check to have at the beginning of both of these functions.
             *
             * Possible improvement for this solution if I worked more time on it -> Avoid duplicating these 2 lines
             */
            var paths = new[] {dirSourcePath, dirDestinationPath};
            if(!CheckNotNull(paths)) return GetMessage(ReturnMessages.NullData);

            // Stop if there's an error in the copy process
            var output = CopyDirectory(dirSourcePath, dirDestinationPath, overwrite, mergeDirectories);
            if (output != GetMessage(ReturnMessages.CopiedSuccessfully)) return output;
            
            // Delete function uses the recycling bin
            try { Directory.Delete(dirSourcePath, true); }
            catch (IOException e) { Console.WriteLine(e.Message); }
            
            return GetMessage(ReturnMessages.MovedSuccessfully);
        }
        
        
        private static bool CheckNotNull(IEnumerable<string> paths) => paths.All(path => path != null); 
        
        /**
         * Gets the final message to be outputted to the user.
         * It acts as a bridge between the enum and the main functions.
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