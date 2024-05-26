using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace VideoViewer
{

    class VideoGroup
    {
        public string   Label;
        public string   Type;
        public string   DirPath;
        public bool     DirExists;

        public VideoGroup(string Label, string Type, string DirPath)
        {
            this.Label = Label;
            this.Type = Type;
            this.DirPath = DirPath;
            this.DirExists = Directory.Exists(DirPath);
            
            if (this.DirExists)
            {
                Console.WriteLine($"读取配置：\n组名: {this.Label}\n类型: {this.Type}\n路径: {this.DirPath}\n");
            }
        }
    }


    class Config
    {
        public static VideoGroup[] VideoGroups;

        public static bool InitConfig()
        {
            if (!(File.Exists("config.ini")))
            { 
                MessageBox.Show("未找到config.ini", "错误");
                return false;
            }
            else
            {
                List<VideoGroup> VideoGroupsTemp = new List<VideoGroup>();

                string config = System.IO.File.ReadAllText("config.ini");

                string pattern = @"\[Label\]\s*([^\s]+?)\r?\n\[Type\]\s*([^\s]+?)\r?\n\[DirPath\]\s*([^\s].+?)((\r?\n)|$)";
                MatchCollection matches = Regex.Matches(config, pattern);

                int GroupNumber = matches.Count;                

                for (int i = 0; i < GroupNumber; i++)
                {
                    GroupCollection groups = matches[i].Groups;
                    VideoGroup VideoGroupTemp = new VideoGroup(groups[1].ToString(), groups[2].ToString(), groups[3].ToString());
                    if (VideoGroupTemp.DirExists)
                    {
                        VideoGroupsTemp.Add(VideoGroupTemp);
                    }
                }

                VideoGroups = VideoGroupsTemp.ToArray();

                if (VideoGroups.Length != 0)
                {
                    return true;
                }
                else {
                    MessageBox.Show("config.ini中未发现可用视频文件夹", "错误");
                    return false;
                }   
            }
        }

        public static void AddGroups2Combobox(ComboBox comboBox)
        {
            foreach (VideoGroup Group in VideoGroups)
            {
                comboBox.Items.Add(Group.Label);
            }
            comboBox.SelectedIndex = 0;
        }
    }
}
