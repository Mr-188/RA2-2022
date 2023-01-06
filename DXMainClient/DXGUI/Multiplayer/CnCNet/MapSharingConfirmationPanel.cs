using ClientGUI;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;

namespace DTAClient.DXGUI.Multiplayer.CnCNet
{
    /// <summary>
    /// A panel that is used to verify and display map sharing status.
    /// </summary>
    class MapSharingConfirmationPanel : XNAPanel
    {
        public MapSharingConfirmationPanel(WindowManager windowManager) : base(windowManager)
        {
        }

        private readonly string MapSharingRequestText =
            "房主已经选择了一个在您的本地安装中不存在地图";

        private readonly string MapSharingDownloadText =
            "下载地图中...";

        private readonly string MapSharingFailedText =
            "下载地图失败了。房主" + Environment.NewLine +
            "必须更换地图，否则无法进行游戏";

        public event EventHandler MapDownloadConfirmed;

        private XNALabel lblDescription;
        private XNAClientButton btnDownload;

        public override void Initialize()
        {
            PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.TILED;

            Name = nameof(MapSharingConfirmationPanel);
            BackgroundTexture = AssetLoader.LoadTexture("msgboxform.png");

            lblDescription = new XNALabel(WindowManager);
            lblDescription.Name = nameof(lblDescription);
            lblDescription.X = UIDesignConstants.EMPTY_SPACE_SIDES;
            lblDescription.Y = UIDesignConstants.EMPTY_SPACE_TOP;
            lblDescription.Text = MapSharingRequestText;
            AddChild(lblDescription);

            Width = lblDescription.Right + UIDesignConstants.EMPTY_SPACE_SIDES;

            btnDownload = new XNAClientButton(WindowManager);
            btnDownload.Name = nameof(btnDownload);
            btnDownload.Width = UIDesignConstants.BUTTON_WIDTH_92;
            btnDownload.Y = lblDescription.Bottom + UIDesignConstants.EMPTY_SPACE_TOP * 2;
            btnDownload.Text = "下载";
            btnDownload.LeftClick += (s, e) => MapDownloadConfirmed?.Invoke(this, EventArgs.Empty);
            AddChild(btnDownload);
            btnDownload.CenterOnParentHorizontally();

            Height = btnDownload.Bottom + UIDesignConstants.EMPTY_SPACE_BOTTOM;

            base.Initialize();

            CenterOnParent();

            Disable();
        }

        public void ShowForMapDownload()
        {
            lblDescription.Text = MapSharingRequestText;
            btnDownload.AllowClick = true;
            Enable();
        }

        public void SetDownloadingStatus()
        {
            lblDescription.Text = MapSharingDownloadText;
            btnDownload.AllowClick = false;
        }

        public void SetFailedStatus()
        {
            lblDescription.Text = MapSharingFailedText;
            btnDownload.AllowClick = false;
        }
    }
}
