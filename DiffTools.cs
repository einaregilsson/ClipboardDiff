#region License
/* 
ClipboardDiff Visual Studio Extension
Copyright (C) 2011-2014 Einar Egilsson
http://einaregilsson.com/clipboarddiff-visual-studio-extension/

This program is licensed under the MIT license, see the file LICENSE
for details.
*/

#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace EinarEgilsson.ClipboardDiff
{
    internal class DiffTool
    {
        public string Path { get; set; }
        public string Arguments { get; set; }
    }

    internal static class DiffTools
    {

        internal static List<DiffTool> GetCandidates()
        {
            var result = new List<DiffTool>();
            //Ok, vsDiffMerge is our default choice here, since it's integrated into VS itself...
            var processFullPath = Process.GetCurrentProcess().MainModule.FileName;
            var vsFolder = Path.GetDirectoryName(processFullPath);
            const string vsDiffMergeArgs = "$FILE1$ $FILE2$ /t";

            //Older VS versions
            result.Add(new DiffTool
            {
                Path = Path.Combine(vsFolder, "vsDiffMerge.exe"),
                Arguments = vsDiffMergeArgs
            });

            //VS 2017 versions
            result.Add(new DiffTool
            {
                Path = Path.Combine(vsFolder, @"CommonExtensions\Microsoft\TeamFoundation\Team Explorer\vsDiffMerge.exe"),
                Arguments = vsDiffMergeArgs
            });

            const string defaultArgs = "$FILE1$ $FILE2$";

            //These will probably never be used anymore, except in really old VS version, but we'll leave them in here just in case...
            foreach (var path in new[] { @"Perforce\p4merge.exe", @"tortoisesvn\bin\TortoiseMerge.exe", @"WinMerge\WinMergeU.exe" })
            {
                result.Add(new DiffTool
                {
                    //Hardcode the program files because running in 32-bit VS will always return the x86 folder, and
                    //we might have a program in the "real" program files...
                    Path = Path.Combine(@"C:\Program Files", path),
                    Arguments = defaultArgs
                });
                result.Add(new DiffTool
                {
                    Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), path),
                    Arguments = defaultArgs
                });
            }
            return result;
        }
    }
}
