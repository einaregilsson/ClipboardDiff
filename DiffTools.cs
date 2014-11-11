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
using System.IO;
using System.Linq;

namespace EinarEgilsson.ClipboardDiff
{
    internal static class DiffTools
    {
        static DiffTools()
        {
            foreach (var key in Paths.Keys.ToArray())
            {
                string args = Paths[key];
                Paths.Remove(key);
                Paths.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), key), args);
                string x86Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), key);
                if (!Paths.ContainsKey(x86Path))
                {
                    Paths.Add(x86Path, args);
                }
            }
        }

        internal static Dictionary<string, string> Paths = new Dictionary<string, string>
        {
            //TODO: Add as many diff tools as possible...
            {@"Perforce\p4merge.exe", "$FILE1$ $FILE2$"},
            {@"tortoisesvn\bin\TortoiseMerge.exe", "$FILE1$ $FILE2$"},
            {@"WinMerge\WinMergeU.exe", "$FILE1$ $FILE2$"},
        };
    }
}
