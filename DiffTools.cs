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
                Paths.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)), args);
                Paths.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)), args);
            }
        }

        internal static Dictionary<string, string> Paths = new Dictionary<string, string>
        {
            {@"Perforce\p4merge.exe", "$FILE1$ $FILE2$"},
            {@"WinMerge\WinMergeU.exe", "$FILE1$ $FILE2$"},
            {@"tortoisesvn\bin\TortoiseIDiff.exe", "$FILE1$ $FILE2$"},
        };
    }
}
