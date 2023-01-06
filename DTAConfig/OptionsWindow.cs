using ClientCore;
using ClientCore.CnCNet5;
using ClientGUI;
using DTAClient.DXGUI.Generic;
using DTAConfig.OptionPanels;
using Microsoft.Xna.Framework;
using Rampastring.Tools;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;
using Updater;

namespace DTAConfig
{
    public class OptionsWindow : XNAWindow
    {
        public OptionsWindow(WindowManager windowManager, GameCollection gameCollection, XNAControl topBar) : base(windowManager)
        {
            this.gameCollection = gameCollection;
            this.topBar = topBar;


        }
        public static void AddAndInitializeWithControl(WindowManager wm, XNAControl control)
        {
            var dp = new DarkeningPanel(wm);
            wm.AddAndInitializeControl(dp);
            dp.AddChild(control);
        }
        public event EventHandler OnForceUpdate;

        private XNAClientTabControl tabControl;
        private ThankWindow thankWindow;
        private XNAOptionsPanel[] optionsPanels;
        private ComponentsPanel componentsPanel;

        private DisplayOptionsPanel displayOptionsPanel;
        private XNAControl topBar;

        private GameCollection gameCollection;

        public override void Initialize()
        {

            Name = "OptionsWindow";
            ClientRectangle = new Rectangle(0, 0, 800, 475);
            BackgroundTexture = AssetLoader.LoadTextureUncached("optionsbg.png");

            tabControl = new XNAClientTabControl(WindowManager);
            tabControl.Name = "tabControl";
            tabControl.ClientRectangle = new Rectangle(12, 12, 0, 23);
            tabControl.FontIndex = 1;
            tabControl.ClickSound = new EnhancedSoundEffect("button.wav");
            tabControl.AddTab("显示", UIDesignConstants.BUTTON_WIDTH_92);
            tabControl.AddTab("音频", UIDesignConstants.BUTTON_WIDTH_92);
            tabControl.AddTab("游戏", UIDesignConstants.BUTTON_WIDTH_92);
            tabControl.AddTab("CnCNet", UIDesignConstants.BUTTON_WIDTH_92);
            tabControl.AddTab("局内皮肤", UIDesignConstants.BUTTON_WIDTH_92);
            tabControl.AddTab("更新", UIDesignConstants.BUTTON_WIDTH_92);
            tabControl.AddTab("组件", UIDesignConstants.BUTTON_WIDTH_92);
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            var btnCancel = new XNAClientButton(WindowManager);
            btnCancel.Name = "btnCancel";
            btnCancel.ClientRectangle = new Rectangle(Width - 104,
                Height - 35, UIDesignConstants.BUTTON_WIDTH_92, UIDesignConstants.BUTTON_HEIGHT);
            btnCancel.Text = "取消";
            btnCancel.LeftClick += BtnBack_LeftClick;

            var btnSave = new XNAClientButton(WindowManager);
            btnSave.Name = "btnSave";
            btnSave.ClientRectangle = new Rectangle(12, btnCancel.Y, UIDesignConstants.BUTTON_WIDTH_92, UIDesignConstants.BUTTON_HEIGHT);
            btnSave.Text = "保存";
            btnSave.LeftClick += BtnSave_LeftClick;

            var btnThank = new XNAClientButton(WindowManager);
            btnThank.Name = "btnThank";
            btnThank.ClientRectangle = new Rectangle((btnSave.X + btnCancel.X) / 2, btnSave.Y, UIDesignConstants.BUTTON_WIDTH_92, UIDesignConstants.BUTTON_HEIGHT);
            btnThank.Text = "鸣谢列表";
            btnThank.LeftClick += btnThank_LeftClick;

            thankWindow = new ThankWindow(WindowManager);
            AddAndInitializeWithControl(WindowManager, thankWindow);
            thankWindow.Disable();

            displayOptionsPanel = new DisplayOptionsPanel(WindowManager, UserINISettings.Instance);
            componentsPanel = new ComponentsPanel(WindowManager, UserINISettings.Instance);
            var updaterOptionsPanel = new UpdaterOptionsPanel(WindowManager, UserINISettings.Instance);
            updaterOptionsPanel.OnForceUpdate += (s, e) => { Disable(); OnForceUpdate?.Invoke(this, EventArgs.Empty); };

            optionsPanels = new XNAOptionsPanel[]
            {
                displayOptionsPanel,
                new AudioOptionsPanel(WindowManager, UserINISettings.Instance),
                new GameOptionsPanel(WindowManager, UserINISettings.Instance, topBar),
                new CnCNetOptionsPanel(WindowManager, UserINISettings.Instance, gameCollection),
                new LocalSkinPanel(WindowManager, UserINISettings.Instance),
                updaterOptionsPanel,
                componentsPanel
            };

            //禁用cncnet选项
            //tabControl.MakeUnselectable(3);
            if (ClientConfiguration.Instance.ModMode || CUpdater.UPDATEMIRRORS == null || CUpdater.UPDATEMIRRORS.Count < 1)
            {
                tabControl.MakeUnselectable(5);
                tabControl.MakeUnselectable(6);
            }
            else if (CUpdater.CustomComponents == null || CUpdater.CustomComponents.Length < 1)
                tabControl.MakeUnselectable(6);

            foreach (var panel in optionsPanels)
            {
                AddChild(panel);
                panel.Load();
                panel.Disable();
            }

            optionsPanels[0].Enable();

            AddChild(tabControl);
            AddChild(btnCancel);
            AddChild(btnSave);
            AddChild(btnThank);
            base.Initialize();

            CenterOnParent();
        }
        private void btnThank_LeftClick(object sender, EventArgs e)
        {
            thankWindow.CenterOnParent();
            thankWindow.Enable();
        }

        /// <summary>
        /// Parses extra options defined by the modder
        /// from an INI file. Called from XNAWindow.SetAttributesFromINI.
        /// </summary>
        /// <param name="iniFile">The INI file.</param>
        protected override void GetINIAttributes(IniFile iniFile)
        {
            base.GetINIAttributes(iniFile);

            foreach (var panel in optionsPanels)
                panel.ParseUserOptions(iniFile);
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var panel in optionsPanels)
                panel.Disable();

            optionsPanels[tabControl.SelectedTab].Enable();
            optionsPanels[tabControl.SelectedTab].RefreshPanel();
        }

        private void BtnBack_LeftClick(object sender, EventArgs e)
        {
            if (CustomComponent.IsDownloadInProgress())
            {
                var msgBox = new XNAMessageBox(WindowManager, "下载过程中",
                    "可选组件下载正在进行中。如果退出“选项”菜单，下载将被取消." +
                    Environment.NewLine + Environment.NewLine +
                    "您确定要继续吗?", XNAMessageBoxButtons.YesNo);
                msgBox.Show();
                msgBox.YesClickedAction = ExitDownloadCancelConfirmation_YesClicked;

                return;
            }

            WindowManager.SoundPlayer.SetVolume(Convert.ToSingle(UserINISettings.Instance.ClientVolume));
            Disable();
        }

        private void ExitDownloadCancelConfirmation_YesClicked(XNAMessageBox messageBox)
        {
            componentsPanel.CancelAllDownloads();
            WindowManager.SoundPlayer.SetVolume(Convert.ToSingle(UserINISettings.Instance.ClientVolume));
            Disable();
        }

        private void BtnSave_LeftClick(object sender, EventArgs e)
        {
            if (CustomComponent.IsDownloadInProgress())
            {
                var msgBox = new XNAMessageBox(WindowManager, "下载过程中",
                    "可选组件下载正在进行中。如果退出“选项”菜单，下载将被取消." +
                    Environment.NewLine + Environment.NewLine +
                    "您确定要继续吗?", XNAMessageBoxButtons.YesNo);
                msgBox.Show();
                msgBox.YesClickedAction = SaveDownloadCancelConfirmation_YesClicked;

                return;
            }

            SaveSettings();
        }

        private void SaveDownloadCancelConfirmation_YesClicked(XNAMessageBox messageBox)
        {
            componentsPanel.CancelAllDownloads();

            SaveSettings();
        }

        private void SaveSettings()
        {
            if (RefreshOptionPanels())
                return;

            bool restartRequired = false;

            try
            {
                foreach (var panel in optionsPanels)
                    restartRequired = panel.Save() || restartRequired;

                UserINISettings.Instance.SaveSettings();
            }
            catch (Exception ex)
            {
                Logger.Log("Saving settings failed! Error message: " + ex.Message);
                XNAMessageBox.Show(WindowManager, "保存设置失败",
                    "保存设置失败!错误消息: " + ex.Message);
            }

            Disable();

            if (restartRequired)
            {
                var msgBox = new XNAMessageBox(WindowManager, "重新启动要求",
                    "客户端需要重新启动，一些更改才能生效." +
                    Environment.NewLine + Environment.NewLine +
                    "你想现在重新启动吗?", XNAMessageBoxButtons.YesNo);
                msgBox.Show();
                msgBox.YesClickedAction = RestartMsgBox_YesClicked;
            }
        }

        private void RestartMsgBox_YesClicked(XNAMessageBox messageBox) => WindowManager.RestartGame();

        /// <summary>
        /// Refreshes the option panels to account for possible
        /// changes that could affect theirs functionality.
        /// Shows the popup to inform the user if needed.
        /// </summary>
        /// <returns>A bool that determines whether the 
        /// settings values were changed.</returns>
        private bool RefreshOptionPanels()
        {
            bool optionValuesChanged = false;

            foreach (var panel in optionsPanels)
                optionValuesChanged = panel.RefreshPanel() || optionValuesChanged;

            if (optionValuesChanged)
            {
                XNAMessageBox.Show(WindowManager, "设置值(s)改变",
                    "一个或多个设置值不再可用，已被更改." +
                    Environment.NewLine + Environment.NewLine +
                    "您可能需要验证新设置客户端选项窗口中的值");

                return true;
            }

            return false;
        }

        public void RefreshSettings()
        {
            foreach (var panel in optionsPanels)
                panel.Load();

            RefreshOptionPanels();

            foreach (var panel in optionsPanels)
                panel.Save();

            UserINISettings.Instance.SaveSettings();
        }

        public void Open()
        {
            foreach (var panel in optionsPanels)
                panel.Load();

            RefreshOptionPanels();

            componentsPanel.Open();

            Enable();
        }

        public void ToggleMainMenuOnlyOptions(bool enable)
        {
            foreach (var panel in optionsPanels)
            {
                panel.ToggleMainMenuOnlyOptions(enable);
            }
        }

        public void SwitchToCustomComponentsPanel()
        {
            foreach (var panel in optionsPanels)
                panel.Disable();

            tabControl.SelectedTab = 5;
        }

        public void InstallCustomComponent(int id) => componentsPanel.InstallComponent(id);

        public void PostInit()
        {
#if TS
            displayOptionsPanel.PostInit();
#endif
        }
    }
}
