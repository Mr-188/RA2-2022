using ClientCore;
using ClientCore.CnCNet5;
using ClientCore.Settings;
using ClientGUI;
using Microsoft.Xna.Framework;
using Rampastring.Tools;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DTAConfig.OptionPanels
{
    class LocalSkinPanel : XNAOptionsPanel
    {
        public LocalSkinPanel(WindowManager windowManager, UserINISettings iniSettings)
            : base(windowManager, iniSettings)
        {
            this.iniSettings = iniSettings;
        }
        public StringSetting setting;
        private XNAListBox NameBox;
        private XNAClientDropDown DdSkin;
        private XNAClientDropDown DdScreen;
        private UserINISettings iniSettings;
        private List<string> SkinName;
        private List<string[]> AllSkin;
        private XNAButton btnImage;
        private XNALabel lblselect;
        private XNALabel lblScreen;
        private XNAClientButton btnDefault;
        private List<XNAClientDropDown> DropDown = new List<XNAClientDropDown>();
        private XNALabel lblAllText;

        private string selectName;

        private string Text;
        private string Folder;
        private string[] Options;
        private int Select;
        private string[] Image;
        private string[] Types;
        public override void Initialize()
        {
            base.Initialize();
            btnImage = new XNAButton(WindowManager);
            btnImage.ClientRectangle = new Rectangle(250, 80, 400, 250);

            SkinName = iniSettings.GetSkinName("所有");
            selectName = iniSettings.GetAIISkin()[0][5];

            DdScreen = new XNAClientDropDown(WindowManager);
            DdScreen.ClientRectangle = new Rectangle(80, 15, 100, 10);
            

            Name = "LocalSkinPanel";
            
            Types = IniSettings.GetTypes();

            lblAllText = new XNALabel(WindowManager);
            lblAllText.ClientRectangle = new Rectangle(420, 50, 0, 0);

            NameBox = new XNAListBox(WindowManager);

            NameBox.ClientRectangle = new Rectangle(30, 50, 100, 300);
            NameBox.FontIndex = 1;
            NameBox.LineHeight = 20;
            NameBox.SelectedIndexChanged += NameBox_SelectedChanged;

            DdScreen.AddItem("所有");
            for (int i = 0; i < Types.Length; i++)
                DdScreen.AddItem(Types[i]);

            
            for (int i = 0; i < SkinName.Count; i++)
            {
                string[] SelectSkin = GetSkinByName(SkinName[i]);
                Options = SelectSkin[2].Split(',');
                Folder = SelectSkin[1];
                Image = SelectSkin[4].Split(',');
                DdSkin = new XNAClientDropDown(WindowManager);
                DdSkin.Tag = SelectSkin[5];
                DdSkin.ClientRectangle = new Rectangle(250, 50, 100, 15);
                DdSkin.SelectedIndex = iniSettings.GetSkinBy(SelectSkin[5], "Select");
                DdSkin.Disable();
                
                DropDown.Add(DdSkin);
                DdSkin.SelectedIndexChanged += DdSkin_SelectedChanged;
                AddChild(DdSkin);
                for (int j = 0; j < Options.Length; j++)
                    DdSkin.AddItem(Options[j]);

                DdSkin.Name = SelectSkin[5];
            }

            
            DdScreen.SelectedIndexChanged += DdScreen_SelectedChanged;
            DdScreen.SelectedIndex = 0;

            NameBox.SelectedIndex = 0;
            lblScreen = new XNALabel(WindowManager);
            lblScreen.Text = "筛选:";
            lblScreen.ClientRectangle = new Rectangle(30, 15, 0, 0);


           lblselect = new XNALabel(WindowManager);
            lblselect.Text = "当前选择：";
            lblselect.ClientRectangle = new Rectangle(150, 50, 0, 0);

            btnDefault = new XNAClientButton(WindowManager);
            btnDefault.Name = nameof(btnDefault);
            btnDefault.IdleTexture = AssetLoader.LoadTexture("92pxbtn.png");
            btnDefault.HoverTexture = AssetLoader.LoadTexture("92pxbtn_c.png");
            btnDefault.Text = "全部恢复默认";
            btnDefault.ClientRectangle = new Rectangle(600, 30, 100, 30);
            btnDefault.LeftClick += btnDefaultLeftClick;

            

            base.Initialize();

            AddChild(NameBox);
            AddChild(lblselect);
            AddChild(lblScreen);
            AddChild(DdScreen);
            AddChild(btnDefault);
            AddChild(btnImage);
            AddChild(lblAllText);
        }

        public override void Load()
        {
            base.Load();
            
            foreach (XNAClientDropDown dd in DropDown)
            {
             
                dd.SelectedIndex = int.Parse(iniSettings.GetAIISkin().Find(s => s[5] == dd.Name)[3]);

            }
            
        }

       public  override bool Save() {

            File.WriteAllText("SkinRulesmd.ini", ";皮肤Rules"+ Environment.NewLine);
            File.WriteAllText("SkinArtmd.ini", ";皮肤Art"+ Environment.NewLine);
            List<string[]> Rules = new List<string[]>();
            List<string[]> Art = new List<string[]>();
           
            foreach (XNAClientDropDown dd in DropDown)
            {
                //  List<string> file = new List<string>();
                
                string[] file = GetSkin((string)dd.Tag);
                List<string> del = new List<string>();
                int oldselect = int.Parse(file[3]);
                if (file != null)
                {
                    string[] AllFile = Directory.GetFiles(file[1]+ file[10].Split(',')[oldselect]);
                    if (file[6] == "")
                        for (int i = 0; i < AllFile.Length; i++)
                            del.Add(Path.GetFileName(AllFile[i]));
                    else {
                        string[] delete= file[6].Split('|')[oldselect].Split(',');
                        for (int i = 0; i < delete.Length; i++)
                            del.Add(delete[i]);
                    }
                    DelFile(del.ToArray());
                    if (dd.SelectedIndex != 0)
                    {
                        if (file[7] != ""&& file[7].Split('|')[dd.SelectedIndex] != "")
                        {
                            Rules.Add(File.ReadAllLines(file[1] + file[10].Split(',')[dd.SelectedIndex] + "/" + file[7].Split('|')[dd.SelectedIndex], Encoding.UTF8));
                        }
                        if (file[8] != "" && file[8].Split('|')[dd.SelectedIndex] != "")
                        {
                            Art.Add(File.ReadAllLines(file[1] + file[10].Split(',')[dd.SelectedIndex] + "/" + file[8].Split('|')[dd.SelectedIndex], Encoding.UTF8));
                        }
                    }

                    if (dd.SelectedIndex != 0)
                    {
                        CopyDirectory(file[1]+file[10].Split(',')[dd.SelectedIndex], "./");
                    }
                   
                }
                
                iniSettings.SetSkinIndex(dd.Name, dd.SelectedIndex);
            }

          

            for (int i = 0; i < Rules.Count; i++)
            {
                File.AppendAllLines("SkinRulesmd.ini", Rules[i]);
            }
            for (int i = 0; i < Art.Count; i++)
            {
                File.AppendAllLines("SkinArtmd.ini", Art[i]);
            }
            return false;
        }


         private void btnDefaultLeftClick(object sender, EventArgs e)
        {
            XNAMessageBox messageBox = new XNAMessageBox(WindowManager, "恢复默认", "你确定要将所有皮肤效果恢复默认吗？", XNAMessageBoxButtons.YesNo);
            messageBox.Show();
            messageBox.YesClickedAction += Default_YesClicked;
        }

        private void Default_YesClicked(XNAMessageBox messageBox)
        {
           
            foreach (XNAClientDropDown dd in DropDown)
            {
                dd.SelectedIndex = 0;
            }
            XNAMessageBox Box = new XNAMessageBox(WindowManager, "恢复默认", "操作成功，如果后悔可以点击取消。", XNAMessageBoxButtons.OK);
            Box.Show();
        }
private void DelFile(string[] deleteFile)
{
    //  string resultDirectory = Environment.CurrentDirectory;//目录

    if (deleteFile != null)
    {
        for (int i = 0; i < deleteFile.Length; i++)
        {
                       try { 
                    if (deleteFile[i] != "")
                    {
                          
                        File.Delete(deleteFile[i]);
                    }
           }
           catch
            {
               continue;
           }
        }
    }
}

private void CopyDirectory(string sourceDirPath, string saveDirPath)
        {
            if (sourceDirPath != null)
            {
                string[] files = Directory.GetFiles(sourceDirPath);
                foreach (string file in files)
                {
                    string pFilePath = saveDirPath + "\\" + Path.GetFileName(file);
                    File.Copy(file, pFilePath, true); 
                }
            }
        }

        public void DdScreen_SelectedChanged(object sender, EventArgs e)
        {
     
            NameBox.Clear();
            List<string> SkinName = new List<string>();
            SkinName = iniSettings.GetSkinName(DdScreen.SelectedItem.Text);

            for (int i = 0; i < SkinName.Count; i++)
            {
                XNAListBoxItem item = new XNAListBoxItem(SkinName[i]);
                item.Tag = GetSkinByName(SkinName[i])[5];
                NameBox.AddItem(item);
            }
            NameBox.SelectedIndex = 0;
        }

        public void NameBox_SelectedChanged(object sender, EventArgs e)
        {
          //  List<string> file = new List<string>();
            Tag = (string)NameBox.SelectedItem.Tag;
  
            string[] SelectSkin = GetSkin((string)Tag);
            Folder = SelectSkin[1];
            Image = SelectSkin[4].Split(',');
            //设置描述语句
            if (SelectSkin[9] != "")
                lblAllText.Text = SelectSkin[9];
            else
                lblAllText.Text = SelectSkin[0];

            //隐藏上一个选项，显示下一个选项
            DropDown.Find(p => p.Name == selectName).Disable();
            DropDown.Find(p => p.Name == (string)NameBox.SelectedItem.Tag).Enable();

            //标记当前选项
            selectName = (string)NameBox.SelectedItem.Tag;
         //   file.Add(SelectSkin[6]);
           // file.Add(SelectSkin[7]);
           // file.Add(SelectSkin[8]);
            DdSkin_SelectedChanged(DropDown.Find(p => p.Name == (string)NameBox.SelectedItem.Tag),e);
      //      if (Folder.Length> DropDown.Find(p => p.Name == (string)NameBox.SelectedItem.Tag).SelectedIndex)
        //     file.Add(Folder);
       //     DropDown.Find(p => p.Name == (string)NameBox.SelectedItem.Tag).Tag = file;

        }

        public void DdSkin_SelectedChanged(object sender, EventArgs e)
        {
            btnImage.IdleTexture = AssetLoader.LoadTexture(Folder + Image[DropDown.Find(p => p.Name == (string)NameBox.SelectedItem.Tag).SelectedIndex]);
        }

        public string[] GetSkin(string Name)
        {
            AllSkin = iniSettings.GetAIISkin();

            for(int i = 0; i < AllSkin.Count; i++)
            {
                if (AllSkin[i][5] == Name)
                    return AllSkin[i];
            }
            return null;
        }
        public string[] GetSkinByName(string Name)
        {
            AllSkin = iniSettings.GetAIISkin();

            for (int i = 0; i < AllSkin.Count; i++)
            {
                if (AllSkin[i][0] == Name)
                    return AllSkin[i];
            }
            return null;
        }

    }
}
