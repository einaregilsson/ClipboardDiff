using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Process = System.Diagnostics.Process;

namespace EinarEgilsson.ClipboardDiff
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid("b02989c2-1a8e-4f11-81a4-957f1d18db10")]
    public sealed class ClipboardDiffPackage : Package
    {
        public static readonly Guid CommandSetId = new Guid("6f04d587-0360-458b-8501-02b2bc7bb002");
        public const int ShowSettingsWindowCommandId = 0x100;
        public const int ClipboardDiffCommandId = 0x120;

        public string Program { get; set; }
        public string Arguments { get; set; }
        private DTE _app;

        private string SettingsFile
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ClipboardDiff.config"); }
        }
        
        private void SaveSettings()
        {
            File.WriteAllLines(SettingsFile, new string[]{Program, Arguments});
        }

        private void LoadSettings()
        {
            if (File.Exists(SettingsFile))
            {
                var lines = File.ReadAllLines(SettingsFile);
                Program = lines[0];
                Arguments = lines[1];
            }
            else
            {
                foreach (var pair in DiffTools.Paths)
                {
                    if (File.Exists(pair.Key))
                    {
                        Program = pair.Key;
                        Arguments = pair.Value;
                        return;
                    }
                }
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs != null)
            {
                mcs.AddCommand( new MenuCommand(ShowSettingsWindow, new CommandID(CommandSetId, ShowSettingsWindowCommandId)));
                mcs.AddCommand(new MenuCommand(DiffWithClipboard, new CommandID(CommandSetId, ClipboardDiffCommandId)));
            }
            LoadSettings();
        }

        private void DiffWithClipboard(object sender, EventArgs e)
        {
            string clipboardText = Clipboard.GetText();

            string folder = Path.Combine(Path.GetTempPath(), "ClipboardDiff");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);    
            }
            string extension = Path.GetExtension(_app.ActiveDocument.Path);
            string clipboardFile = Path.Combine(folder, "clipboard_" + DateTime.Now.Ticks +"."+extension);
            string selectionFile = Path.Combine(folder, "selection_" + DateTime.Now.Ticks + "."+extension);
            TextSelection ts = _app.ActiveDocument.Selection as TextSelection;
            if (ts == null)
            {
                return;
            }
            string selectionText = ts.Text;
            File.WriteAllText(clipboardFile, clipboardText);
            File.WriteAllText(selectionFile, selectionText);
            string args = Arguments.Replace("$FILE1$", "\"" + clipboardFile + "\"").Replace("$FILE2$",
                                                                                            "\"" + selectionFile + "\"");
            Process.Start(Program, args);
            foreach (string filename in Directory.GetFiles(folder))
            {
                //Clean older files that are still hanging around
                if (filename != selectionFile && filename != clipboardFile)
                {
                    try
                    {
                        File.Delete(filename);    
                    }
                    catch(Exception)
                    {
                        //Do nothing! We just try
                    }
                }
            }
        }

        private void ShowSettingsWindow(object sender, EventArgs e)
        {
            var dlg = new Settings();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Program = dlg.Program;
                Arguments = dlg.Arguments;
                SaveSettings();
            }
        }
    }
}
