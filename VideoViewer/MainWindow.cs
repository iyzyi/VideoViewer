using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;


namespace VideoViewer
{
    public partial class MainWindow : Form
    {
        public string                   RootDir                 = "";
        public string[]                 VideoDirs;
        public int                      VideoNumber;
        public PictureBox[]             VideoCovers;
        public Label[]                  VideoTitles;
        public int                      VideoNumPerPage         = 20;
        public int                      CurrentPage             = 1;
        public int                      PageNumber;
        string                          CoverName               = "";


        public MainWindow()
        {
            InitializeComponent();

            VideoCovers = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12, pictureBox13, pictureBox14, pictureBox15, pictureBox16, pictureBox17, pictureBox18, pictureBox19, pictureBox20 };
            VideoTitles = new Label[] { label1, label2, label3, label4, label5, label6, label7, label8, label9, label10, label11, label12, label13, label14, label15, label16, label17, label18, label19, label20 };

            if (!Config.InitConfig())
            {
                System.Diagnostics.Process tt = System.Diagnostics.Process.GetProcessById(System.Diagnostics.Process.GetCurrentProcess().Id);
                tt.Kill();
            }
            Config.AddGroups2Combobox(this.comboBox1);

            InitView();
        }


        public void InitView()
        {
            if (GetGroup().Type == "pornhub")
            {
                CoverName = "cover.jpg";
            }
            else if (GetGroup().Type == "xvideos")
            {
                CoverName = "1.jpg";
            }

            // 获取RootDir中的合法（包含封面图片和*.mp4）的文件夹
            string[] AllDirs = Directory.GetDirectories(RootDir);
            List<string> VideoDirsTemp = new List<string>();
            foreach (string VideoDir in AllDirs)
            {
                if (CheckValidDir(VideoDir)){
                    VideoDirsTemp.Add(VideoDir);
                }
            }
            VideoDirs = VideoDirsTemp.ToArray();
            VideoNumber = VideoDirs.Length;
            PageNumber = (VideoNumber % VideoNumPerPage == 0) ? (VideoNumber / VideoNumPerPage) : (VideoNumber / VideoNumPerPage + 1);

            FlushPage();
        }


        // 显示视频封面、标题
        public void FlushPage()
        {
            int StartVideoId = (CurrentPage - 1) * VideoNumPerPage;
            int EndVideoId = StartVideoId + VideoNumPerPage;
            for (int VideoId = StartVideoId; VideoId < EndVideoId && VideoId < VideoNumber; VideoId++)
            {
                int IdOnCurrentPage = VideoId - StartVideoId;
                VideoCovers[IdOnCurrentPage].Load(VideoDirs[VideoId] + "\\" + CoverName);

                string[] words = VideoDirs[VideoId].Split('\\');
                string title = words[words.Length - 1];
                VideoTitles[IdOnCurrentPage].Text = title;
            }

            for (int VideoId = VideoNumber; VideoId < EndVideoId; VideoId++)
            {
                int IdOnCurrentPage = VideoId - StartVideoId;
                VideoCovers[IdOnCurrentPage].Image = null;
                VideoTitles[IdOnCurrentPage].Text = "";
            }

            labelPage.Text = $"页码：{CurrentPage}/{PageNumber}";
        }


        // 判断文件夹是否包含封面图片和*.mp4
        private bool CheckValidDir(string VideoDir)
        {
            bool valid = true;

            if (!File.Exists(VideoDir + "\\" + CoverName))
            {
                valid = false;
            }

            bool HaveMP4 = false;
            string[] Files = Directory.GetFiles(VideoDir);
            foreach (string file in Files)
            {
                if (Path.GetExtension(file) == ".mp4")
                {
                    HaveMP4 = true;
                }
            }
            if (!HaveMP4)
            {
                valid = false;
            }

            return valid;
        }


        // 上一页
        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage -= 1;
                FlushPage();
            }
        }

        private void buttonPreviousPage_Click(object sender, EventArgs e)
        {
            PreviousPage();
        }


        // 下一页
        private void NextPage()
        {
            if (CurrentPage * VideoNumPerPage < VideoNumber)
            {
                CurrentPage += 1;
                FlushPage();
            }
        }

        private void buttonNextPage_Click(object sender, EventArgs e)
        {
            NextPage();
        }


        // 方向左键、方向右键 翻页，方向上键、方向下键 改变选项卡的选项
        // 需要将主窗口的KeyPreview设为True, 不然按键消息直接传递给各控件
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                PreviousPage();
            }
            if (e.KeyCode == Keys.Right)
            {
                NextPage();
            }
            if (e.KeyCode == Keys.Up)
            {
                if (comboBox1.SelectedIndex > 0)
                {
                    comboBox1.SelectedIndex -= 1;
                }
            }
            if (e.KeyCode == Keys.Down)
            {
                if (comboBox1.SelectedIndex < comboBox1.Items.Count - 1)
                {
                    comboBox1.SelectedIndex += 1;
                }
            }
        }


        // 本来是上下左右键都会改变选项卡的选项，这里禁用上下左右键，然后在MainWindow_KeyDown实现“改变选项卡的选项”这一功能
        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
            }
        }


        // Form中添加了按钮后，方向键的KeyDown消息就不响应了。原因：方向键是作为系统键来处理的，
        // 默认方向键的作用是移动焦点，所以按方向键的效果是焦点不断地在按钮之间转换。也就是说，
        // 按方向键产生的消息被系统处理掉了，那么我们自己定义的消息响应函数就不会执行了。解决方
        // 案：覆盖默认的系统键处理方式，如果是方向键就直接返回。
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
            {
                return false;
            }
            return base.ProcessDialogKey(keyData);
        }


        // 打开视频
        private void OpenVideo(int num)
        {
            int IdOnCurrentPage = num - 1;              // // 在当前页面上的相对id，从0到VideoNumPerPage-1

            if (VideoCovers[IdOnCurrentPage].Image == null)
            {
                return;
            }

            int StartVideoId = (CurrentPage - 1) * VideoNumPerPage;         
            string VideoDir = VideoDirs[StartVideoId + IdOnCurrentPage];

            string VideoPath = "";
            string[] Files = Directory.GetFiles(VideoDir);
            foreach (string FilePath in Files)
            {
                if (Path.GetExtension(FilePath) == ".mp4")
                {
                    if (!(GetGroup().Type == "xvideos" && Path.GetFileName(FilePath) == "预览.mp4"))
                    {
                        VideoPath = FilePath;
                        break;
                    }
                }
            }
            if (VideoPath != "")
            {
                Console.WriteLine("【打开视频】 " + VideoPath);
                System.Diagnostics.Process.Start(VideoPath);
            }
            else
            {
                MessageBox.Show("此文件夹不存在mp4视频文件", "错误");
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RootDir = GetGroup().DirPath;
            CurrentPage = 1;
            InitView();
            int GroupIndex = comboBox1.SelectedIndex+1;
            int GroupNumber = comboBox1.Items.Count;
            labelGroup.Text = $"组号：{GroupIndex}/{GroupNumber}";
        }


        private VideoGroup GetGroup()
        {
            int SelectGroupIndex = comboBox1.SelectedIndex;
            return Config.VideoGroups[SelectGroupIndex];
        }


        #region ClickPictureBox
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenVideo(1);
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenVideo(2);
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            OpenVideo(3);
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            OpenVideo(4);
        }
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            OpenVideo(5);
        }
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            OpenVideo(6);
        }
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            OpenVideo(7);
        }
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            OpenVideo(8);
        }
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            OpenVideo(9);
        }
        private void pictureBox10_Click(object sender, EventArgs e)
        {
            OpenVideo(10);
        }
        private void pictureBox11_Click(object sender, EventArgs e)
        {
            OpenVideo(11);
        }
        private void pictureBox12_Click(object sender, EventArgs e)
        {
            OpenVideo(12);
        }
        private void pictureBox13_Click(object sender, EventArgs e)
        {
            OpenVideo(13);
        }
        private void pictureBox14_Click(object sender, EventArgs e)
        {
            OpenVideo(14);
        }
        private void pictureBox15_Click(object sender, EventArgs e)
        {
            OpenVideo(15);
        }
        private void pictureBox16_Click(object sender, EventArgs e)
        {
            OpenVideo(16);
        }
        private void pictureBox17_Click(object sender, EventArgs e)
        {
            OpenVideo(17);
        }
        private void pictureBox18_Click(object sender, EventArgs e)
        {
            OpenVideo(18);
        }
        private void pictureBox19_Click(object sender, EventArgs e)
        {
            OpenVideo(19);
        }
        private void pictureBox20_Click(object sender, EventArgs e)
        {
            OpenVideo(20);
        }
        #endregion


        #region ClickLabel
        private void label1_Click(object sender, EventArgs e)
        {
            OpenVideo(1);
        }
        private void label2_Click(object sender, EventArgs e)
        {
            OpenVideo(2);
        }
        private void label3_Click(object sender, EventArgs e)
        {
            OpenVideo(3);
        }
        private void label4_Click(object sender, EventArgs e)
        {
            OpenVideo(4);
        }
        private void label5_Click(object sender, EventArgs e)
        {
            OpenVideo(5);
        }
        private void label6_Click(object sender, EventArgs e)
        {
            OpenVideo(6);
        }
        private void label7_Click(object sender, EventArgs e)
        {
            OpenVideo(7);
        }
        private void label8_Click(object sender, EventArgs e)
        {
            OpenVideo(8);
        }
        private void label9_Click(object sender, EventArgs e)
        {
            OpenVideo(9);
        }
        private void label10_Click(object sender, EventArgs e)
        {
            OpenVideo(10);
        }
        private void label11_Click(object sender, EventArgs e)
        {
            OpenVideo(11);
        }
        private void label12_Click(object sender, EventArgs e)
        {
            OpenVideo(12);
        }
        private void label13_Click(object sender, EventArgs e)
        {
            OpenVideo(13);
        }
        private void label14_Click(object sender, EventArgs e)
        {
            OpenVideo(14);
        }
        private void label15_Click(object sender, EventArgs e)
        {
            OpenVideo(15);
        }
        private void label16_Click(object sender, EventArgs e)
        {
            OpenVideo(16);
        }
        private void label17_Click(object sender, EventArgs e)
        {
            OpenVideo(17);
        }
        private void label18_Click(object sender, EventArgs e)
        {
            OpenVideo(18);
        }
        private void label19_Click(object sender, EventArgs e)
        {
            OpenVideo(19);
        }
        private void label20_Click(object sender, EventArgs e)
        {
            OpenVideo(20);
        }
        #endregion

        private void buttonOpenConfig_Click(object sender, EventArgs e)
        {
            if (!(File.Exists("config.ini")))
            {
                MessageBox.Show("未找到config.ini", "错误");
            }
            else
            {
                System.Diagnostics.Process.Start("config.ini");
            }
        }

        private void buttonFlushConfig_Click(object sender, EventArgs e)
        {
            this.comboBox1.Items.Clear();

            if (!Config.InitConfig())
            {
                System.Diagnostics.Process tt = System.Diagnostics.Process.GetProcessById(System.Diagnostics.Process.GetCurrentProcess().Id);
                tt.Kill();
            }
            Config.AddGroups2Combobox(this.comboBox1);

            InitView();
        }

        private void buttonOpenMediaDir_Click(object sender, EventArgs e)
        {
            string FilePath = GetGroup().DirPath;
            System.Diagnostics.Process.Start(FilePath);
        }
    }
}