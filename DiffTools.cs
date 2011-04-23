#region License
/* 
ClipboardDiff Visual Studio Extension
Copyright (C) 2011 Einar Egilsson
http://tech.einaregilsson.com/2011/xx/xx/clipboard-diff/

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
