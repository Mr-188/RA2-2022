using ClientGUI;
using Microsoft.Xna.Framework;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;


namespace DTAClient.DXGUI.Generic
{
    public class ThankWindow : XNAWindow
    {

        public ThankWindow(WindowManager windowManager) : base(windowManager)
        {
        }

        public event EventHandler YesClicked;
        public XNAListBox lblThankList;
        public override void Initialize()
        {
            //Name = "ThankWindow";
            ClientRectangle = new Rectangle(0, 0, 334, 453);
            //BackgroundTexture = AssetLoader.LoadTexture("cheaterbg.png");

            var lblCheater = new XNALabel(WindowManager);
            lblCheater.Name = "lblCheater";
            lblCheater.ClientRectangle = new Rectangle(0, 0, 0, 0);
            lblCheater.FontIndex = 1;
            lblCheater.Text = "鸣谢列表";

            lblThankList = new XNAListBox(WindowManager);
            lblThankList.Name = nameof(lblThankList);
            lblThankList.ClientRectangle = new Rectangle(30, lblCheater.Y + 60, 280, 350);
            lblThankList.FontIndex = 1;
            lblThankList.LineHeight = 30;
 
            lblThankList.AddItem("CNC平台：CNCNet");
            lblThankList.AddItem("游戏平台：Ares，Phobos");
            lblThankList.AddItem("随机地图：囧hán方序囧(hán字无法显示)");
            lblThankList.AddItem("原版战役适配：双杀步枪");
            lblThankList.AddItem("二次元主题：Blue623");
            lblThankList.AddItem("原版模式：泳池里的潜艇");
            lblThankList.AddItem("第三帝国：疾风丶Yy5871");
            lblThankList.AddItem("地图编辑器：FA2SP制作组");
            lblThankList.AddItem("中文语音包：蚂蚁制作组");
            lblThankList.AddItem("(皮肤)雷达样式2：=Star=");
            lblThankList.AddItem("(皮肤)连续血条：雷德克里莫");
            lblThankList.AddItem("(皮肤)水面箱子样式2：雷德克里莫");
            lblThankList.AddItem("(皮肤)超时空动画样式2：ppap11404");
            lblThankList.AddItem("(皮肤)无畏级战舰样式2：snmiglight");
            lblThankList.AddItem("(皮肤)恐怖机器人样式2：BoundaryHand");
            lblThankList.AddItem("(皮肤)基洛夫空艇样式2：布加迪");
            lblThankList.AddItem("(皮肤)入侵者战机样式2：Creator");
            lblThankList.AddItem("(皮肤)黑鹰战机样式2：Creator");
            lblThankList.AddItem("(皮肤)建造UI：Aaron_Kka");

            lblThankList.AddItem("(皮肤)盟军高科样式2：雷德克里莫");
            lblThankList.AddItem("(皮肤)盟军重工样式2：雷德克里莫");
            lblThankList.AddItem("(皮肤)盟军电厂样式2：雷德克里莫");
            lblThankList.AddItem("(皮肤)盟军矿场样式2：雷德克里莫");
            lblThankList.AddItem("(皮肤)盟军兵营样式2：雷德克里莫 lalalayuan77");

            lblThankList.AddItem("(皮肤)苏军高科样式2：雷德克里莫");
            lblThankList.AddItem("(皮肤)苏军重工样式2：雷德克里莫");
            lblThankList.AddItem("(皮肤)苏军电厂样式2：雷德克里莫");
            lblThankList.AddItem("(皮肤)苏军矿场样式2：雷德克里莫");
            lblThankList.AddItem("(皮肤)苏军兵营样式2：雷德克里莫");
            lblThankList.AddItem("(皮肤)苏军雷达样式3：雷德克里莫");

            lblThankList.AddItem("(皮肤)苏军基地样式2：xuetianyi");
            lblThankList.AddItem("(皮肤)盟军基地样式2：ruanhuhu,qwqwq");
            lblThankList.AddItem("(皮肤)盟军基地样式3：xuetianyi");

            lblThankList.AddItem("(皮肤)灯光：雷德克里莫");

            lblThankList.AddItem("(皮肤)脑车样式2：cyanideT");

            lblThankList.AddItem("(皮肤)城市地形：凌..");
            lblThankList.AddItem("(皮肤)防空炮样式2：HG_SCIPCION deathreaperz");
            lblThankList.AddItem("(皮肤)爱国者样式2：HG_SCIPCION deathreaperz");
            lblThankList.AddItem("(皮肤)哨戒炮样式2：HG_SCIPCION deathreaperz");
            lblThankList.AddItem("(皮肤)神盾样式2：13220379104");

            lblThankList.AddItem("冷场AI：囧hán方序囧(hán字无法显示)");
            lblThankList.AddItem("liyupengAI：liyupeng");

            var btnYes = new XNAClientButton(WindowManager);
            btnYes.Name = "btnYes";
            btnYes.ClientRectangle = new Rectangle((Width - UIDesignConstants.BUTTON_WIDTH_92) / 2,
                Height - 35, UIDesignConstants.BUTTON_WIDTH_92, UIDesignConstants.BUTTON_HEIGHT);
            btnYes.Text = "是";
            btnYes.LeftClick += BtnYes_LeftClick;

            AddChild(lblThankList);
            AddChild(lblCheater);
            AddChild(btnYes);

            lblCheater.CenterOnParent();
            lblCheater.ClientRectangle = new Rectangle(lblCheater.X, 12,
                lblCheater.Width, lblCheater.Height);

            base.Initialize();
        }



        private void BtnYes_LeftClick(object sender, EventArgs e)
        {
            Disable();
        }
    }

}
