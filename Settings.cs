using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EinarEgilsson.ClipboardDiff
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
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
            this.DialogResult = DialogResult.Cancel;
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
            Close();
        }
    }
}
