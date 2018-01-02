using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
namespace Catholic
{
    public partial class main : Form
    {
        private static string type;
        public main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if(folder.ShowDialog() == DialogResult.OK)
            {
                path.Text = folder.SelectedPath;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            type = "alt";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            type = "sop";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            type = "ten";
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            type = "bas";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(path.Text != null)
            {
                int s, n;
                if(radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked)
                {
                    if( int.TryParse(en.Text,out n) && int.TryParse(st.Text,out s))
                    {
                        if(s < n)
                        {
                            DownloadManager download = new DownloadManager(path.Text, type, s, n, progressBar1);
                            download.start();
                          
                        }
                        else
                        {
                            MessageBox.Show("시작 번호가 끝 번호보다 큽니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                       
                    }
                    else
                    {
                        MessageBox.Show("시작 번호와 끝 번호가 숫자가 아니거나 입력하지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("알토,소프라노,테너,베이스 중 한가지를 선택해주십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("저장 경로를 설정해 주십시오.(\"...\" 버튼)", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    public class DownloadManager
    {
        public static string SEPERATOR = "/";
        public static string FILE_SEPERATOR = @"\";
        public static string PARENT_URL = "https://www.mariasarang.net/files/music_catholic";
        private string path;
        private string t;
        private Timer async;
        private WebClient webClient;
        private Queue<string> queue;
        private ProgressBar progressBar;
  
        public DownloadManager(string path,string type,int from,int to,ProgressBar progressBar)
        {
         
            webClient = new WebClient();
            this.progressBar = progressBar;
            queue = new Queue<string>();
            this.path = path;
            if(path == null)
            {
                MessageBox.Show("파일 경로가 필요합니다","오류",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            t = type;
            if (type != null)
            {
                t = type.ToLower();
                for (int i = from; i < to; i++)
                {
                    queue.Enqueue(PARENT_URL + SEPERATOR + "part" + SEPERATOR + t + SEPERATOR + i.ToString("000") + t.ToCharArray()[0] + ".mid"+"|"+path+FILE_SEPERATOR + i.ToString("000") + t.ToCharArray()[0] + ".mid");
                }
            }
            async = new Timer();
        }
        public void start()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

         
                async.Tick += new EventHandler(download);
                progressBar.Maximum = queue.Count;
                async.Interval = 50;
                async.Start();
           
          
        }
        private void download(object o,EventArgs e)
        {
                if(queue.Count != 0)
            {
                string data = queue.Dequeue();
                webClient.DownloadFile(data.Split('|')[0], data.Split('|')[1]);
                progressBar.Increment(1);
            }
            else
            {
                async.Stop();
                MessageBox.Show("다운로드가 완료되었습니다.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                progressBar.Value = 0;
            }
      
        }
    }
    
}
