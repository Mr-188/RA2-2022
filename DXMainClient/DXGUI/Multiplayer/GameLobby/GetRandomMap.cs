using ClientCore;
using ClientGUI;
using DTAClient.Domain.Multiplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rampastring.Tools;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DTAClient.DXGUI.Multiplayer.GameLobby
{
    class GetRandomMap : XNAWindow
    {
        private const int OPTIONHEIGHT = 85;

        private XNALabel lblTitle;

        private XNALabel lblClimate; //气候
        private XNAClientDropDown ddClimate;

        private XNALabel lblPeople; //人数
        private XNAClientDropDown ddPeople;

        private XNAClientCheckBox cbDamage;//建筑物损伤

        private XNALabel lblSize;
        private XNAClientDropDown ddSize;
        private XNAClientButton btnGenerate;
        private XNAClientButton btnCancel;
        private XNAClientButton btnSave;
        private XNAButton btnpreview;

        private XNALabel lblStatus;

        private Thread thread1;

        private Thread thread;

        private  bool Stop = false;

        public static bool TF = false;
        private bool isSave;

        private string[] People;

        private string Damage = string.Empty;

        public MapLoader MapLoader;

        public GetRandomMap(WindowManager windowManager, MapLoader mapLoader) : base(windowManager)
        {
            MapLoader = mapLoader;
        }
        public override void Initialize()
        {
            base.Initialize();
            Name = "GetRandomMap";
            CenterOnParent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            ClientRectangle = new Rectangle(200, 100, 800, 500);

            lblTitle = new XNALabel(WindowManager);
            lblTitle.ClientRectangle = new Rectangle(360, 20, 0, 0);
            lblTitle.CenterOnParentHorizontally();
            lblTitle.Text = "生成随机地图";

            lblStatus = new XNALabel(WindowManager);
            lblStatus.ClientRectangle = new Rectangle(360, 420, 0, 0);

            btnGenerate = new XNAClientButton(WindowManager);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.ClientRectangle = new Rectangle(350, 460, 100, 20);
            btnGenerate.Text = "生成";
            btnGenerate.IdleTexture = AssetLoader.LoadTexture("92pxbtn.png");
            btnGenerate.HoverTexture = AssetLoader.LoadTexture("92pxbtn_c.png");
            btnGenerate.LeftClick += btnGenerat_LeftClick;


            btnCancel = new XNAClientButton(WindowManager);
            btnCancel.Name = "btnCancel";
            btnCancel.ClientRectangle = new Rectangle(40, 460, 100, 20);
            btnCancel.Text = "取消";
            btnCancel.IdleTexture = AssetLoader.LoadTexture("92pxbtn.png");
            btnCancel.HoverTexture = AssetLoader.LoadTexture("92pxbtn_c.png");
            btnCancel.LeftClick += btnCancel_LeftClick;

            btnSave = new XNAClientButton(WindowManager);
            btnSave.Name = "btnSave";
            btnSave.ClientRectangle = new Rectangle(660, 460, 100, 20);
            btnSave.Text = "保存";
            btnSave.IdleTexture = AssetLoader.LoadTexture("92pxbtn.png");
            btnSave.HoverTexture = AssetLoader.LoadTexture("92pxbtn_c.png");
            btnSave.Enabled = false;
            btnSave.LeftClick += btnSave_LeftClick;

            lblClimate = new XNALabel(WindowManager);
            lblClimate.ClientRectangle = new Rectangle(40, OPTIONHEIGHT, 0, 0);
            lblClimate.Text = "气候类型";

            ddClimate = new XNAClientDropDown(WindowManager);
            ddClimate.ClientRectangle = new Rectangle(lblClimate.X + 70, OPTIONHEIGHT, 80, 20);
            XNADropDownItem Desert = new XNADropDownItem();
            Desert.Text = "沙漠";
            Desert.Tag = "DESERT";
            XNADropDownItem Newurban = new XNADropDownItem();
            Newurban.Text = "城市";
            Newurban.Tag = "NEWURBAN";
            XNADropDownItem Temperate = new XNADropDownItem();
            Temperate.Text = "温带";
            Temperate.Tag = "TEMPERATE";
            XNADropDownItem Temperate_Islands = new XNADropDownItem();
            Temperate_Islands.Text = "温带群岛";
            Temperate_Islands.Tag = "TEMPERATE_Islands";

            btnpreview = new XNAButton(WindowManager);
            btnpreview.ClientRectangle = new Rectangle(100, 150, 600, 250);


            ddClimate.AddItem("随机");
            ddClimate.AddItem(Temperate);
            ddClimate.AddItem(Temperate_Islands);
            ddClimate.AddItem(Newurban);
            ddClimate.AddItem(Desert);
            ddClimate.SelectedIndex = 0;

            lblPeople = new XNALabel(WindowManager);
            lblPeople.ClientRectangle = new Rectangle(ddClimate.X + 100, OPTIONHEIGHT, 80, 0);
            lblPeople.Text = "人数";

            ddPeople = new XNAClientDropDown(WindowManager);
            ddPeople.ClientRectangle = new Rectangle(lblPeople.X + 40, OPTIONHEIGHT, 80, 20);
            ddPeople.AddItem("随机");


            for (int i = 2; i <= 8; i++)
            {
                ddPeople.AddItem(i.ToString());
            }
            ddPeople.SelectedIndex = 0;

            lblSize = new XNALabel(WindowManager);
            lblSize.ClientRectangle = new Rectangle(ddPeople.X+100, OPTIONHEIGHT, 0, 0);
            lblSize.Text = "大小";

            ddSize = new XNAClientDropDown(WindowManager);
            ddSize.ClientRectangle = new Rectangle(lblSize.X + 40, OPTIONHEIGHT, 80, 20);
            ddSize.AddItem("小");
            ddSize.AddItem("中等");
            ddSize.AddItem("大");
            ddSize.AddItem("很大");
            ddSize.SelectedIndex = 1;

            
            cbDamage = new XNAClientCheckBox(WindowManager);
            cbDamage.ClientRectangle = new Rectangle(ddSize.X + 150, OPTIONHEIGHT, 0, 0);
            cbDamage.Text = "随机建筑损伤";


            //thread.Abort()
            AddChild(lblTitle);
            AddChild(lblStatus);
            AddChild(btnpreview);

            AddChild(lblClimate);
            AddChild(ddClimate);
            
            AddChild(lblPeople);
            AddChild(ddPeople);

            AddChild(lblSize);
            AddChild(ddSize);

            AddChild(cbDamage);
            AddChild(btnGenerate);
            AddChild(btnCancel);
            AddChild(btnSave);
        }

        public bool GetIsSave()
        {
            return isSave;
        }

        private void btnCancel_LeftClick(object sender, EventArgs e)
        {
            isSave = false;
            Disable();
        }

        private void btnSave_LeftClick(object sender, EventArgs e)
        {
            isSave = true;
            Disable();
        }

        private void btnGenerat_LeftClick(object sender, EventArgs e)
        {
            btnGenerate.Enabled = false;
            btnSave.Enabled = false;
            thread1 = new Thread(new ThreadStart(StartText));
            thread = new Thread(new ThreadStart(RunCmd));
            thread1.Start();
            thread.Start();
        }

        public void RunCmd()
        {
            string strCmdText;
            Random r = new Random();
            string Generate = (string)ddClimate.SelectedItem.Tag;
            if (ddClimate.SelectedItem.Text == "随机")
            {
                Generate = (string)ddClimate.Items[r.Next(1, 5)].Tag;
            }

            int sizex = 35*(ddSize.SelectedIndex+1) + r.Next(30,50);
            int sizey= 35 * (ddSize.SelectedIndex+1) +r.Next(30,50);

            People = GetPeople(ddPeople.SelectedItem.Text);

            if (cbDamage.Checked)
            {
                Damage = "-d";
            }
            strCmdText = "/c cd /d \"" + ProgramConstants.GamePath + "RandomMapGenerator_RA2\" &&" +
                string.Format(" RandomMapGenerator.exe -w {10} -h {11} --nwp {0} --sep {1} --nep {2} --swp {3} --sp {4} --wp {5} --ep {6} --np {7} {8} --type {9} -g standard &&", People[0], People[1], People[2], People[3], People[4], People[5], People[6], People[7], Damage, Generate,sizex,sizey) +
                string.Format(" cd Map Renderer &&" + " CNCMaps.Renderer.exe -i \"{0}Maps/Custom/随机地图.map\" -o 随机地图 -m \"{1}\" -z +(1000,0) --thumb-png --bkp ", ProgramConstants.GamePath, ProgramConstants.GamePath);
          
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = strCmdText;
            process.StartInfo.UseShellExecute = false;   //是否使用操作系统shell启动 
            process.StartInfo.CreateNoWindow = true;   //是否在新窗口中启动该进程的值 (不显示程序窗口)
            process.Start();
            process.WaitForExit();  //等待程序执行完退出进程
            process.Close();
            Stop = true;

        }
        public void StartText()
        {
            string[] TextList = { "正在驱散平民", "正在挖掘矿石", "正在列装基地建设车", "正在检查弹药", "正在为动员兵发放波波沙", "正在让幻影坦克熟悉环境","正在安抚警犬","正在捕捉海豚", "正在跟后勤讨价还价", "正在给运输机加注燃料", "正在让潜艇下沉", "正在给建筑刷漆" };
            Random r = new Random();

            while (!Stop)
            {
                lblStatus.Text = TextList[r.Next(TextList.Length)];
                Thread.Sleep(500);
            }
            
          File.Delete("Maps/Custom/随机地图.png");
            FileInfo fi = new FileInfo("Maps/Custom/thumb_随机地图.png");
            fi.MoveTo("Maps/Custom/随机地图.png");
            try
            {
                btnpreview.IdleTexture = AssetLoader.LoadTextureUncached("Maps/Custom/随机地图.png");
            }
            catch
            {
                thread = new Thread(new ThreadStart(RunCmd));
                thread.Start();
            }
            lblStatus.Text = "已完成";
            TF = true;
            btnGenerate.Enabled = true;
            btnSave.Enabled = true;
            Stop = false;

            //MapLoader.LoadRandomMaps();

            //MapLoader.GameModes.RemoveAll(g => g.Maps.Count < 1);
           
            //MapLoader.GameModeMaps = new GameModeMapCollection(MapLoader.GameModes);
            
        }


        private string[] GetPeople(string Peoples)
        {
            int[] p =  { 0, 0, 0, 0, 0, 0, 0, 0 };
            int Current;
            Random r = new Random();
            if (Peoples == "随机")
                Current = r.Next(2,8);
            else
                Current = int.Parse(Peoples);
            
            while(Current>0)
            {

                p[r.Next(8)]++;

                Current--;
            }
            return string.Join(",", p).Split(',');
        }


        public MapLoader GetMapLoader()
        {
            return MapLoader;
        }
    }
}
