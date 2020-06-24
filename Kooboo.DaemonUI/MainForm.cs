using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kooboo.DaemonUI
{
    public partial class MainForm : Form
    {
        Process _process;

        private bool Running
        {
            set
            {
                startBtn.Enabled = !value;
                stopBtn.Enabled = value;
                statusTxt.Text = value ? "运行中.." : "已停止";
            }
        }

        public MainForm()
        {
            InitializeComponent();
            Running = false;

        }

        private void Start(object sender, EventArgs e)
        {
            // Task
            StartApp();
        }

        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            AddMsg(e.Data);
        }

        private void Stop(object sender, EventArgs e)
        {
            if (_process != null && !_process.HasExited)
            {
                AddMsg("正在停止");
                _process.Kill();
                Running = false;
                AddMsg("停止成功");
            }

        }

        private void AddMsg(string msg)
        {
            Invoke(new Action(() =>
            {
                StringBuilder sb = new StringBuilder(textBox1.Text);
                textBox1.Text = sb.AppendLine(msg).ToString();
                textBox1.SelectionStart = this.textBox1.Text.Length;
                textBox1.ScrollToCaret();
            }));
        }

        private void StartApp()
        {
            _process = new Process();
            _process.StartInfo.FileName = "dotnet";
            _process.StartInfo.Arguments = "Kooboo.App.dll";
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.CreateNoWindow = true;
            _process.OutputDataReceived += _process_OutputDataReceived;
            _process.Start();
            _process.BeginOutputReadLine();
            Running = true;
        }
    }
}
