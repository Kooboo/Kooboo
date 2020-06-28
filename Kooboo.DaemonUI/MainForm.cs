using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kooboo.DaemonUI
{
    public partial class MainForm : Form
    {
        Process _process;
        bool _running;

        private bool Running
        {
            get
            {
                return _running;
            }
            set
            {
                _running = value;
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
            try
            {
                StartApp();
                Running = true;

                Task.Factory.StartNew(() =>
                {
                    while (Running)
                    {
                        Thread.Sleep(1000);
                        if (_process == null) continue;
                        if (_process.HasExited)
                        {
                            StartApp();
                        }
                    }
                });
            }
            catch (Exception)
            {
                AddMsg("启动失败");
                ReleaseProcess();
            }

        }

        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            AddMsg(e.Data);
        }

        private void Stop(object sender, EventArgs e)
        {
            AddMsg("正在停止");
            ReleaseProcess();
            Running = false;
            AddMsg("停止成功");
        }

        void ReleaseProcess()
        {
            var last = _process;
            _process = null;

            try
            {
                if (last != null && !last.HasExited)
                {
                    last.Kill();
                    last.Close();
                }
            }
            catch (Exception)
            {
            }

        }

        private void AddMsg(string msg)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    StringBuilder sb = new StringBuilder(textBox1.Text);
                    textBox1.Text = sb.AppendLine(msg).ToString();
                    textBox1.SelectionStart = this.textBox1.Text.Length;
                    textBox1.ScrollToCaret();
                }));
            }
            catch (Exception)
            {

            }

        }

        private void StartApp()
        {
            ReleaseProcess();
            AddMsg("正在启动");
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
            Thread.Sleep(300);
            if (_process.HasExited) throw new ServerException();
            AddMsg("启动成功");
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                Activate();
                ShowInTaskbar = true;
                notifyIcon1.Visible = false;
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowInTaskbar = false;
                notifyIcon1.Visible = true;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            WindowState = FormWindowState.Minimized;
        }

        private void Show(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            BringToFront();
        }

        private void Close(object sender, EventArgs e)
        {
            ReleaseProcess();
            Dispose();
            Close();
        }
    }
}
