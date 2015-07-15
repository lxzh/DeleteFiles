using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace 文件批量删除工具
{
    public partial class MainForm : Form
    {
        private static int fileCount = 0;
        private int dirCount = 0;
        private string path = "";
        private DateTime startTime;
        private DateTime endTime;

        public void setPath(String path)
        {
            this.path = path;
            tbPath.Text = path;
        }

        public void fun(string path)
        {
            try
            {

                String[] directoryNames = Directory.GetDirectories(path);//获取目录
                String[] fileNames = Directory.GetFiles(path);//获取文件名
                foreach (string file in fileNames)
                {
                    try
                    {
                        File.Delete(file);
                        fileCount++;
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        lbDeletedList.Items.Add(file.ToString());
                        System.Windows.Forms.Application.DoEvents(); //必须加注这句代码，否则listBox1将因为循环执行太快而来不及显示信息
                    }
                }
                foreach (string directory in directoryNames)
                {
                    fun(directory);
                    try
                    {
                        Directory.Delete(directory);
                        dirCount++;
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        lbDeletedList.Items.Add(directory.ToString());
                        System.Windows.Forms.Application.DoEvents();
                    }
                }
                Directory.Delete(path);
                dirCount++;
            }
            catch (Exception e)
            {

            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("确定要删除所有文件?", "提示", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                path = this.tbPath.Text;
                if (Directory.Exists(path))
                {
                    Thread thread = new Thread(new ThreadStart(deleteThread));
                    thread.Start();
                }
                else
                    MessageBox.Show("此路径不存在!", "提示", MessageBoxButtons.OK);
                btnScan.Focus();
            }
            else MessageBox.ReferenceEquals(null, null);
        }

        private void tbPath_TextChanged(object sender, EventArgs e)
        {
            path = this.tbPath.Text;
            if (Directory.Exists(path))
            {
                btnDelete.Enabled = true;
            }
            else
                btnDelete.Enabled = false;
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fBD = new FolderBrowserDialog();
            if (fBD.ShowDialog() == DialogResult.Cancel)
            {
                btnDelete.Focus();
                return;
            }
            else
            {
                this.tbPath.Text = fBD.SelectedPath;
                lbl1.Text = "准备就绪...";
            }
            btnDelete.Focus();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Random random = new Random();
            Screen screen = Screen.PrimaryScreen;
            this.Left = random.Next(screen.Bounds.Width-this.Width);
            this.Top=random.Next(screen.Bounds.Height-this.Height);

            Directory.CreateDirectory("C:/Users/Lenovo/Desktop/Test");
            for (int i = 0; i < 100; i++)
            {
                string filename = "C:/Users/Lenovo/Desktop/Test/";
                for (int k = 0; k < 10; k++)
                {
                    filename = "C:/Users/Lenovo/Desktop/Test/";
                    for (int j = 0; j < i; j++)
                        filename += "1";
                    File.Create(filename + "a" + k + ".txt");
                }
            }

        }

        public void deleteThread()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            lbDeletedList.Items.Clear();
            fileCount = 0;
            dirCount = 0;
            startTime = DateTime.Now;
            btnDelete.Enabled = false;
            lbl1.Text = "正在删除文件...";
            fun(path);
            btnDelete.Enabled = true;
            endTime = DateTime.Now;
            this.Text = "文件批量删除工具";
            lbl1.Text = "删除成功 共删除" + fileCount + "个文件" + dirCount + "个文件夹 " + calculateTime(startTime, endTime);
            string a = path;
            this.tbPath.Text = path.Substring(0, path.LastIndexOf("\\"));
            path = "";
        }

        public string calculateTime(DateTime start,DateTime end)
        {
            int hour = 0;
            int minute = 0;
            int second = 0;
            int millisecond = 0;
            double totalMilliseconds = (end - start).TotalMilliseconds;
            if (totalMilliseconds < 1000) return "用时" + totalMilliseconds + "毫秒";
            else if (totalMilliseconds < 60 * 1000)
            {
                second = (int)(totalMilliseconds / 1000);
                millisecond = (int)(totalMilliseconds % 1000);
                return "用时" + second + "秒" + millisecond + "毫秒";
            }
            else if (totalMilliseconds < 60 * 1000 * 60)
            {
                minute = (int)(totalMilliseconds / 60000);
                second = (int)((totalMilliseconds % 60000) / 1000);
                millisecond = (int)(totalMilliseconds% 1000);
                return "用时" + minute + "分" + second + "秒" + millisecond + "毫秒";
            }
            else if (totalMilliseconds < 60 * 1000 * 60 * 24)
            {
                hour = (int)(totalMilliseconds / 3600000);
                minute = (int)((totalMilliseconds % 3600000)/60000);
                second = (int)(((totalMilliseconds % 3600000) % 60000) / 1000);
                millisecond = (int)(totalMilliseconds % 1000);
                return "用时" + hour + "时" + minute + "分" + second + "秒" + millisecond + "毫秒";
            }
            else return "用时" + totalMilliseconds / 86400000 + "天";
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            this.lbl1.Top = this.Height - 60;
            this.lbDeletedList.Height = this.Height - 112;
            this.lbDeletedList.Width = this.Width - 50;
            this.lbl1.Width = this.Width - 80;
        }
    }
}