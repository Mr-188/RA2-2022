using ClientCore;
using ClientCore.CnCNet5;
using ClientCore.Enums;
using ClientGUI;
using Microsoft.Xna.Framework;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;
using System.Collections.Generic;

namespace DTAConfig.OptionPanels
{
    class CnCNetOptionsPanel : XNAOptionsPanel
    {
        public CnCNetOptionsPanel(WindowManager windowManager, UserINISettings iniSettings,
            GameCollection gameCollection)
            : base(windowManager, iniSettings)
        {
            this.gameCollection = gameCollection;
        }

        XNAClientCheckBox chkPingUnofficialTunnels;
        XNAClientCheckBox chkWriteInstallPathToRegistry;
        XNAClientCheckBox chkPlaySoundOnGameHosted;

        XNAClientCheckBox chkNotifyOnUserListChange;

        XNAClientCheckBox chkSkipLoginWindow;
        XNAClientCheckBox chkPersistentMode;
        XNAClientCheckBox chkConnectOnStartup;
        XNAClientCheckBox chkDiscordIntegration;
        XNAClientCheckBox chkAllowGameInvitesFromFriendsOnly;
        XNAClientCheckBox chkDisablePrivateMessagePopup;

        XNAClientDropDown ddAllowPrivateMessagesFrom;

        GameCollection gameCollection;

        List<XNAClientCheckBox> followedGameChks = new List<XNAClientCheckBox>();

        public override void Initialize()
        {
            base.Initialize();
            Name = "CnCNetOptionsPanel";

            InitOptions();
            InitGameListPanel();
        }

        private void InitOptions()
        {
            // LEFT COLUMN

            chkPingUnofficialTunnels = new XNAClientCheckBox(WindowManager);
            chkPingUnofficialTunnels.Name = nameof(chkPingUnofficialTunnels);
            chkPingUnofficialTunnels.ClientRectangle = new Rectangle(12, 12, 0, 0);
            chkPingUnofficialTunnels.Text = "检测非官方服务器的延迟";

            AddChild(chkPingUnofficialTunnels);

            chkWriteInstallPathToRegistry = new XNAClientCheckBox(WindowManager);
            chkWriteInstallPathToRegistry.Name = nameof(chkWriteInstallPathToRegistry);
            chkWriteInstallPathToRegistry.ClientRectangle = new Rectangle(
                chkPingUnofficialTunnels.X,
                chkPingUnofficialTunnels.Bottom + 12, 0, 0);
            chkWriteInstallPathToRegistry.Text = "将安装路径写入注册表";


            AddChild(chkWriteInstallPathToRegistry);

            chkPlaySoundOnGameHosted = new XNAClientCheckBox(WindowManager);
            chkPlaySoundOnGameHosted.Name = nameof(chkPlaySoundOnGameHosted);
            chkPlaySoundOnGameHosted.ClientRectangle = new Rectangle(
                chkPingUnofficialTunnels.X,
                chkWriteInstallPathToRegistry.Bottom + 12, 0, 0);
            chkPlaySoundOnGameHosted.Text = "创建游戏房间后扔播放背景音乐";

            AddChild(chkPlaySoundOnGameHosted);

            chkNotifyOnUserListChange = new XNAClientCheckBox(WindowManager);
            chkNotifyOnUserListChange.Name = nameof(chkNotifyOnUserListChange);
            chkNotifyOnUserListChange.ClientRectangle = new Rectangle(
                chkPingUnofficialTunnels.X,
                chkPlaySoundOnGameHosted.Bottom + 12, 0, 0);
            chkNotifyOnUserListChange.Text = "显示玩家进出信息";

            AddChild(chkNotifyOnUserListChange);

            chkDisablePrivateMessagePopup = new XNAClientCheckBox(WindowManager);
            chkDisablePrivateMessagePopup.Name = nameof(chkDisablePrivateMessagePopup);
            chkDisablePrivateMessagePopup.ClientRectangle = new Rectangle(
                chkNotifyOnUserListChange.X,
                chkNotifyOnUserListChange.Bottom + 8, 0, 0);
            chkDisablePrivateMessagePopup.Text = "禁用从私人消息弹出窗口";

            AddChild(chkDisablePrivateMessagePopup);

            InitAllowPrivateMessagesFromDropdown();

            // RIGHT COLUMN

            chkSkipLoginWindow = new XNAClientCheckBox(WindowManager);
            chkSkipLoginWindow.Name = nameof(chkSkipLoginWindow);
            chkSkipLoginWindow.ClientRectangle = new Rectangle(
                276,
                12, 0, 0);
            chkSkipLoginWindow.Text = "跳过登录对话框";
            chkSkipLoginWindow.CheckedChanged += ChkSkipLoginWindow_CheckedChanged;

            AddChild(chkSkipLoginWindow);

            chkPersistentMode = new XNAClientCheckBox(WindowManager);
            chkPersistentMode.Name = nameof(chkPersistentMode);
            chkPersistentMode.ClientRectangle = new Rectangle(
                chkSkipLoginWindow.X,
                chkSkipLoginWindow.Bottom + 12, 0, 0);
            chkPersistentMode.Text = "离开CnCNet大厅后不断开连接";
            chkPersistentMode.CheckedChanged += ChkPersistentMode_CheckedChanged;

            AddChild(chkPersistentMode);

            chkConnectOnStartup = new XNAClientCheckBox(WindowManager);
            chkConnectOnStartup.Name = nameof(chkConnectOnStartup);
            chkConnectOnStartup.ClientRectangle = new Rectangle(
                chkSkipLoginWindow.X,
                chkPersistentMode.Bottom + 12, 0, 0);
            chkConnectOnStartup.Text = "在客户端启动时自动连接";
            chkConnectOnStartup.AllowChecking = false;

            AddChild(chkConnectOnStartup);

            chkDiscordIntegration = new XNAClientCheckBox(WindowManager);
            chkDiscordIntegration.Name = nameof(chkDiscordIntegration);
            chkDiscordIntegration.ClientRectangle = new Rectangle(
                chkSkipLoginWindow.X,
                chkConnectOnStartup.Bottom + 12, 0, 0);
            chkDiscordIntegration.Text = "在Discord状态显示本游戏信息";

            if (String.IsNullOrEmpty(ClientConfiguration.Instance.DiscordAppId))
            {
                chkDiscordIntegration.AllowChecking = false;
                chkDiscordIntegration.Checked = false;
            }
            else
            {
                chkDiscordIntegration.AllowChecking = true;
            }

            AddChild(chkDiscordIntegration);

            chkAllowGameInvitesFromFriendsOnly = new XNAClientCheckBox(WindowManager);
            chkAllowGameInvitesFromFriendsOnly.Name = nameof(chkAllowGameInvitesFromFriendsOnly);
            chkAllowGameInvitesFromFriendsOnly.ClientRectangle = new Rectangle(
                chkDiscordIntegration.X,
                chkDiscordIntegration.Bottom + 12, 0, 0);
            chkAllowGameInvitesFromFriendsOnly.Text = "只允许好友邀请我";

            AddChild(chkAllowGameInvitesFromFriendsOnly);
        }

        private void InitAllowPrivateMessagesFromDropdown()
        {
            XNALabel lblAllPrivateMessagesFrom = new XNALabel(WindowManager);
            lblAllPrivateMessagesFrom.Name = nameof(lblAllPrivateMessagesFrom);
            lblAllPrivateMessagesFrom.Text = "允许从私信到达:";
            lblAllPrivateMessagesFrom.ClientRectangle = new Rectangle(
                chkDisablePrivateMessagePopup.X,
                chkDisablePrivateMessagePopup.Bottom + 12, 165, 0);

            AddChild(lblAllPrivateMessagesFrom);

            ddAllowPrivateMessagesFrom = new XNAClientDropDown(WindowManager);
            ddAllowPrivateMessagesFrom.Name = nameof(ddAllowPrivateMessagesFrom);
            ddAllowPrivateMessagesFrom.ClientRectangle = new Rectangle(
                lblAllPrivateMessagesFrom.Right,
                lblAllPrivateMessagesFrom.Y - 2, 65, 0);

            ddAllowPrivateMessagesFrom.AddItem(new XNADropDownItem()
            {
                Text = "所有",
                Tag = AllowPrivateMessagesFromEnum.All
            });

            ddAllowPrivateMessagesFrom.AddItem(new XNADropDownItem()
            {
                Text = "好友",
                Tag = AllowPrivateMessagesFromEnum.Friends
            });

            ddAllowPrivateMessagesFrom.AddItem(new XNADropDownItem()
            {
                Text = "无",
                Tag = AllowPrivateMessagesFromEnum.None
            });

            AddChild(ddAllowPrivateMessagesFrom);
        }

        private void InitGameListPanel()
        {
            const int gameListPanelHeight = 185;
            XNAPanel gameListPanel = new XNAPanel(WindowManager);
            gameListPanel.DrawBorders = false;
            gameListPanel.Name = nameof(gameListPanel);
            gameListPanel.ClientRectangle = new Rectangle(0, Bottom - gameListPanelHeight, Width, gameListPanelHeight);

            AddChild(gameListPanel);

            var lblFollowedGames = new XNALabel(WindowManager);
            lblFollowedGames.Name = nameof(lblFollowedGames);
            lblFollowedGames.ClientRectangle = new Rectangle(12, 12, 0, 0);
            lblFollowedGames.Text = "展示以下游戏房间:";

            gameListPanel.AddChild(lblFollowedGames);

            int chkCount = 0;
            int chkCountPerColumn = 4;
            int nextColumnXOffset = 0;
            int columnXOffset = 0;
            foreach (CnCNetGame game in gameCollection.GameList)
            {
                if (!game.Supported || string.IsNullOrEmpty(game.GameBroadcastChannel))
                    continue;

                if (chkCount == chkCountPerColumn)
                {
                    chkCount = 0;
                    columnXOffset += nextColumnXOffset + 6;
                    nextColumnXOffset = 0;
                }

                var panel = new XNAPanel(WindowManager);
                panel.Name = "panel" + game.InternalName;
                panel.ClientRectangle = new Rectangle(lblFollowedGames.X + columnXOffset,
                    lblFollowedGames.Bottom + 12 + chkCount * 22, 16, 16);
                panel.DrawBorders = false;
                panel.BackgroundTexture = game.Texture;

                var chkBox = new XNAClientCheckBox(WindowManager);
                chkBox.Name = game.InternalName.ToUpper();
                chkBox.ClientRectangle = new Rectangle(
                    panel.Right + 6,
                    panel.Y, 0, 0);
                chkBox.Text = game.UIName;

                chkCount++;

                gameListPanel.AddChild(panel);
                gameListPanel.AddChild(chkBox);
                followedGameChks.Add(chkBox);

                if (chkBox.Right > nextColumnXOffset)
                    nextColumnXOffset = chkBox.Right;
            }
        }

        private void ChkSkipLoginWindow_CheckedChanged(object sender, EventArgs e)
        {
            CheckConnectOnStartupAllowance();
        }

        private void ChkPersistentMode_CheckedChanged(object sender, EventArgs e)
        {
            CheckConnectOnStartupAllowance();
        }

        private void CheckConnectOnStartupAllowance()
        {
            if (!chkSkipLoginWindow.Checked || !chkPersistentMode.Checked)
            {
                chkConnectOnStartup.AllowChecking = false;
                chkConnectOnStartup.Checked = false;
                return;
            }

            chkConnectOnStartup.AllowChecking = true;
        }

        public override void Load()
        {
            base.Load();

            chkPingUnofficialTunnels.Checked = IniSettings.PingUnofficialCnCNetTunnels;
            chkWriteInstallPathToRegistry.Checked = IniSettings.WritePathToRegistry;
            chkPlaySoundOnGameHosted.Checked = IniSettings.PlaySoundOnGameHosted;
            chkNotifyOnUserListChange.Checked = IniSettings.NotifyOnUserListChange;
            chkDisablePrivateMessagePopup.Checked = IniSettings.DisablePrivateMessagePopups;
            SetAllowPrivateMessagesFromState(IniSettings.AllowPrivateMessagesFromState);
            chkConnectOnStartup.Checked = IniSettings.AutomaticCnCNetLogin;
            chkSkipLoginWindow.Checked = IniSettings.SkipConnectDialog;
            chkPersistentMode.Checked = IniSettings.PersistentMode;

            chkDiscordIntegration.Checked = !String.IsNullOrEmpty(ClientConfiguration.Instance.DiscordAppId)
                && IniSettings.DiscordIntegration;

            chkAllowGameInvitesFromFriendsOnly.Checked = IniSettings.AllowGameInvitesFromFriendsOnly;

            string localGame = ClientConfiguration.Instance.LocalGame;

            foreach (var chkBox in followedGameChks)
            {
                if (chkBox.Name == localGame)
                {
                    chkBox.AllowChecking = false;
                    chkBox.Checked = true;
                    IniSettings.SettingsIni.SetBooleanValue("Channels", localGame, true);
                    continue;
                }

                chkBox.Checked = IniSettings.IsGameFollowed(chkBox.Name);
            }
        }

        public override bool Save()
        {
            bool restartRequired = base.Save();

            IniSettings.PingUnofficialCnCNetTunnels.Value = chkPingUnofficialTunnels.Checked;
            IniSettings.WritePathToRegistry.Value = chkWriteInstallPathToRegistry.Checked;
            IniSettings.PlaySoundOnGameHosted.Value = chkPlaySoundOnGameHosted.Checked;
            IniSettings.NotifyOnUserListChange.Value = chkNotifyOnUserListChange.Checked;
            IniSettings.DisablePrivateMessagePopups.Value = chkDisablePrivateMessagePopup.Checked;
            IniSettings.AllowPrivateMessagesFromState.Value = GetAllowPrivateMessagesFromState();
            IniSettings.AutomaticCnCNetLogin.Value = chkConnectOnStartup.Checked;
            IniSettings.SkipConnectDialog.Value = chkSkipLoginWindow.Checked;
            IniSettings.PersistentMode.Value = chkPersistentMode.Checked;

            if (!String.IsNullOrEmpty(ClientConfiguration.Instance.DiscordAppId))
            {
                IniSettings.DiscordIntegration.Value = chkDiscordIntegration.Checked;
            }

            IniSettings.AllowGameInvitesFromFriendsOnly.Value = chkAllowGameInvitesFromFriendsOnly.Checked;

            foreach (var chkBox in followedGameChks)
            {
                IniSettings.SettingsIni.SetBooleanValue("Channels", chkBox.Name, chkBox.Checked);
            }

            return restartRequired;
        }

        private void SetAllowPrivateMessagesFromState(int state)
        {
            var selectedIndex = ddAllowPrivateMessagesFrom.Items.FindIndex(i => (int)i.Tag == state);
            if (selectedIndex < 0)
                selectedIndex = ddAllowPrivateMessagesFrom.Items.FindIndex(i => (AllowPrivateMessagesFromEnum)i.Tag == AllowPrivateMessagesFromEnum.All);

            ddAllowPrivateMessagesFrom.SelectedIndex = selectedIndex;
        }

        private int GetAllowPrivateMessagesFromState()
        {
            return (int)(ddAllowPrivateMessagesFrom.SelectedItem?.Tag ?? AllowPrivateMessagesFromEnum.All);
        }
    }
}
