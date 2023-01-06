using ClientGUI;
using Microsoft.Xna.Framework;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;

namespace DTAClient.DXGUI.Generic
{
    public class CheaterWindow : XNAWindow
    {
        public CheaterWindow(WindowManager windowManager) : base(windowManager)
        {
        }

        public event EventHandler YesClicked;

        public override void Initialize()
        {
            Name = "CheaterScreen";
            ClientRectangle = new Rectangle(0, 0, 334, 453);
            BackgroundTexture = AssetLoader.LoadTexture("cheaterbg.png");

            var lblCheater = new XNALabel(WindowManager);
            lblCheater.Name = "lblCheater";
            lblCheater.ClientRectangle = new Rectangle(0, 0, 0, 0);
            lblCheater.FontIndex = 1;
            lblCheater.Text = "CHEATER!";

            var lblDescription = new XNALabel(WindowManager);
            lblDescription.Name = "lblDescription";
            lblDescription.ClientRectangle = new Rectangle(12, 40, 0, 0);
            lblDescription.Text = "检测到游戏文件被修改. They could affect" + Environment.NewLine +
                "它们可能会影响到游戏." +
                Environment.NewLine + Environment.NewLine +
                "如果觉得游戏过难,可以尝试简单难度或去网上寻找攻略.";

            var imagePanel = new XNAPanel(WindowManager);
            imagePanel.Name = "imagePanel";
            imagePanel.PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.STRETCHED;
            imagePanel.ClientRectangle = new Rectangle(lblDescription.X,
                lblDescription.Bottom + 12, Width - 24,
                Height - (lblDescription.Bottom + 59));
            imagePanel.BackgroundTexture = AssetLoader.LoadTextureUncached("cheater.png");

            var btnCancel = new XNAClientButton(WindowManager);
            btnCancel.Name = "btnCancel";
            btnCancel.ClientRectangle = new Rectangle(Width - 104,
                Height - 35, UIDesignConstants.BUTTON_WIDTH_92, UIDesignConstants.BUTTON_HEIGHT);
            btnCancel.Text = "取消";
            btnCancel.LeftClick += BtnCancel_LeftClick;

            var btnYes = new XNAClientButton(WindowManager);
            btnYes.Name = "btnYes";
            btnYes.ClientRectangle = new Rectangle(12, btnCancel.Y,
                btnCancel.Width, btnCancel.Height);
            btnYes.Text = "是";
            btnYes.LeftClick += BtnYes_LeftClick;

            AddChild(lblCheater);
            AddChild(lblDescription);
            AddChild(imagePanel);
            AddChild(btnCancel);
            AddChild(btnYes);

            lblCheater.CenterOnParent();
            lblCheater.ClientRectangle = new Rectangle(lblCheater.X, 12,
                lblCheater.Width, lblCheater.Height);

            base.Initialize();
        }

        private void BtnCancel_LeftClick(object sender, EventArgs e)
        {
            Disable();
        }

        private void BtnYes_LeftClick(object sender, EventArgs e)
        {
            Disable();
            YesClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
