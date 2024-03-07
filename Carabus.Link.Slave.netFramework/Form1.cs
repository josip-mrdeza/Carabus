using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carabus.Link.Slave.netFramework
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ok = openFileDialog1.ShowDialog();
            if (ok == DialogResult.OK)
            {
                var files = openFileDialog1.FileNames;
                foreach (var file in files)
                {
                    Send(file);
                }
            }
        }
        HttpClient client = new HttpClient();
        private void Send(string file)
        {
            FileInfo info = new FileInfo(file);
            var stream = info.OpenRead();
            var sc = new StreamContent(stream);
            sc.Headers.Add("Content-Type", "application/stream");
            client.PostAsync($"http://{textBox1.Text}/{info.Name}", sc).GetAwaiter().GetResult();
            stream.Dispose();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            
        }
    }
}