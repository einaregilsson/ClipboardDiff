using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
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
    public sealed class ClipboardDiffPackage : Package, IOleCommandTarget
    {
        public static readonly Guid CommandSetId = new Guid("6f04d587-0360-458b-8501-02b2bc7bb002");
        public const int ShowSettingsWindowCommandId = 0x100;
        public const int ClipboardDiffCommandId = 0x120;

        private const string RegistryRoot = "ClipboardDiff";
        private const string RegistryProgram = "DiffProgram";
        private const string RegistryArguments = "Arguments";

        public string Program { get; set; }
        public string Arguments { get; set; }
        private DTE2 _app;

        private void SaveSettings()
        {
            
            var key = UserRegistryRoot.OpenSubKey(RegistryRoot, true);
            if (key == null)
            {
                UserRegistryRoot.CreateSubKey(RegistryRoot);
                key = UserRegistryRoot.OpenSubKey(RegistryRoot, true);
            }
            Debug.Assert(key != null);
            key.SetValue(RegistryProgram, Program);
            key.SetValue(RegistryArguments, Arguments);
        }



        private void LoadSettings()
        {
            var subKey = UserRegistryRoot.OpenSubKey(RegistryRoot);
            if (subKey != null)
            {
                Program = (string)subKey.GetValue(RegistryProgram);
                Arguments = (string)subKey.GetValue(RegistryArguments);
            }

            //Didn't exist, try default paths
            if (string.IsNullOrEmpty(Program))
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
                mcs.AddCommand(new MenuCommand(ShowSettingsWindow, new CommandID(CommandSetId, ShowSettingsWindowCommandId)));
                mcs.AddCommand(new MenuCommand(DiffWithClipboard, new CommandID(CommandSetId, ClipboardDiffCommandId)));
            }
            _app = (DTE2)GetGlobalService(typeof(DTE));
            LoadSettings();
        }

        private bool CanDiff()
        {
            return Clipboard.ContainsText()
                && _app.ActiveDocument != null
                && _app.ActiveDocument.Selection is TextSelection
                && ((TextSelection)_app.ActiveDocument.Selection).Text.Length > 0;
        }

        private void DiffWithClipboard(object sender, EventArgs e)
        {
            if (!CanDiff())
            {
                return;
            }

            if (string.IsNullOrEmpty(Program))
            {
                MessageBox.Show(
                    "You have not used Clipboard Diff before. You must first choose which diff tool you want to use.",
                    "Diff tool missing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowSettingsWindow(this, e);
                return;
            }

            string clipboardText = Clipboard.GetText();

            string folder = Path.Combine(Path.GetTempPath(), "ClipboardDiff");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string extension = Path.GetExtension(_app.ActiveDocument.Name);
            string clipboardFile = Path.Combine(folder, "clipboard_" + DateTime.Now.Ticks + extension);
            string selectionFile = Path.Combine(folder, "selection_" + DateTime.Now.Ticks + extension);

            string selectionText = ((TextSelection)_app.ActiveDocument.Selection).Text;

            File.WriteAllText(clipboardFile, clipboardText);
            File.WriteAllText(selectionFile, selectionText);

            string args = Arguments.Replace("$FILE1$", "\"" + clipboardFile + "\"").Replace("$FILE2$",
                                                                                "\"" + selectionFile + "\"");
            args = args.Replace("\"\"", "\"");
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
                    catch (Exception)
                    {
                        //Do nothing! We just try
                    }
                }
            }
        }


        private void ShowSettingsWindow(object sender, EventArgs e)
        {
            var dlg = new Settings(Program ?? "", Arguments ?? "$FILE1$ $FILE2$");
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Program = dlg.Program;
                Arguments = dlg.Arguments;
                SaveSettings();
            }
        }
    }
}
