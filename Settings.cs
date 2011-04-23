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
using System.IO;
using System.Windows.Forms;

namespace EinarEgilsson.ClipboardDiff
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        public Settings(string program, string arguments) : this()
        {
            txtProgram.Text = program;
            txtArguments.Text = arguments;
        }

        public string Program
        {
            get { return txtProgram.Text; }
        }

        public string Arguments
        {
            get { return txtArguments.Text; }
        }

        private void OnFindProgramClick(object sender, EventArgs e)
        {
            var filePicker = new OpenFileDialog();
            filePicker.Title = "Choose diff program to use...";
            filePicker.CheckFileExists = true;
            filePicker.Filter = "Executable files|*.exe";
            filePicker.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            if (filePicker.ShowDialog(this) == DialogResult.OK)
            {
                txtProgram.Text = filePicker.FileName;
            }
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void OnOKClick(object sender, EventArgs e)
        {
            if (!File.Exists(txtProgram.Text))
            {
                MessageBox.Show("The program you have picked doesn't exist!", "File doesn't exist", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
            if (!txtArguments.Text.Contains("$FILE1$") || !txtArguments.Text.Contains("$FILE2$"))
            {
                MessageBox.Show("The arguments must contain both $FILE1$ and $FILE2$ for the diff to work!", "Arguments are invalid", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
