using ClientCore;
using ClientCore.Statistics;
using ClientGUI;
using DTAClient.Domain.Multiplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rampastring.Tools;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DTAClient.DXGUI.Generic
{
    public class StatisticsWindow : XNAWindow
    {
        public StatisticsWindow(WindowManager windowManager) : base(windowManager) { }

        private XNAPanel panelGameStatistics;
        private XNAPanel panelTotalStatistics;
        private XNAPanel panelAchStatistics;

        private XNAClientDropDown cmbGameModeFilter;
        private XNAClientDropDown cmbGameClassFilter;

        private XNAClientCheckBox chkIncludeSpectatedGames;

        private XNAClientTabControl tabControl;

        // Controls for game statistics

        private XNAMultiColumnListBox lbGameList;
        private XNAMultiColumnListBox lbGameStatistics;

        private Texture2D[] sideTextures;

        // *****************************

        private const int TOTAL_STATS_LOCATION_X1 = 40;
        private const int TOTAL_STATS_VALUE_LOCATION_X1 = 240;
        private const int TOTAL_STATS_LOCATION_X2 = 380;
        private const int TOTAL_STATS_VALUE_LOCATION_X2 = 580;
        private const int TOTAL_STATS_Y_INCREASE = 45;
        private const int TOTAL_STATS_FIRST_ITEM_Y = 20;
        private const string Success = "成功";
        // Controls for total statistics

        private XNALabel lblGamesStartedValue;
        private XNALabel lblGamesFinishedValue;
        private XNALabel lblWinsValue;
        private XNALabel lblLossesValue;
        private XNALabel lblWinLossRatioValue;
        private XNALabel lblAverageGameLengthValue;
        private XNALabel lblTotalTimePlayedValue;
        private XNALabel lblAverageEnemyCountValue;
        private XNALabel lblAverageAllyCountValue;
        private XNALabel lblTotalKillsValue;
        private XNALabel lblKillsPerGameValue;
        private XNALabel lblTotalLossesValue;
        private XNALabel lblLossesPerGameValue;
        private XNALabel lblKillLossRatioValue;
        private XNALabel lblTotalScoreValue;
        private XNALabel lblAverageEconomyValue;
        private XNALabel lblFavouriteSideValue;
        private XNALabel lblAverageAILevelValue;

        // *****************************

        //成就
        private Double[,] Value;
        //久经沙场：对局数到200
        private XNAProgressBar PrghardenedValue;
        //杀人如麻：累计击杀10000
        private XNAProgressBar PrgkillValue;
        //常胜将军：累计胜局100
        private XNAProgressBar PrgVictorValue;
        //爱兵如子：损失<10且获胜局10
        private XNAProgressBar PrgSoldierValue;
        //海枯石烂:熬夜局50局
        private XNAProgressBar PrgLongValue;
        //闪电战:快攻局50局
        private XNAProgressBar PrgShortValue;
        //海贼王:海战50局
        private XNAProgressBar PrgNavalValue;
        //以德服人:玩德国50局
        private XNAProgressBar PrgGermanyValue;
        //过关斩将:单挑50局
        private XNAProgressBar PrgOneValue;
        //枪林弹雨：一局对战中的玩家的摧毁数、建造数均大于200
        private XNAProgressBar PrgBulletsValue;
        //法克尤：1Ⅴ1选法国打赢对面尤里5次
        private XNAProgressBar PrgFkyValue;
        //尽力局：评分最高但输20
        private XNAProgressBar PrgMaxValue;
        //躺赢局：评分最低但赢20
        private XNAProgressBar PrgMinValue;
        //马奇诺防线：使用法国且建造数目最高
        private XNAProgressBar PrgMaginotValue;
        private XNAProgressBar PrgBtValue;
        //*******************************
        private StatisticsManager sm;
        private List<int> listedGameIndexes = new List<int>();

        private string[] sides;

        private List<MultiplayerColor> mpColors;

        public override void Initialize()
        {
            sm = StatisticsManager.Instance;

            string strLblEconomy = "ECONOMY";
            string strLblAvgEconomy = "Average economy:";
            if (ClientConfiguration.Instance.UseBuiltStatistic)
            {
                strLblEconomy = "建造";
                strLblAvgEconomy = "平均建造:";
            }

            Name = "StatisticsWindow";
            BackgroundTexture = AssetLoader.LoadTexture("scoreviewerbg.png");
            ClientRectangle = new Rectangle(0, 0, 900, 521);

            tabControl = new XNAClientTabControl(WindowManager);
            tabControl.Name = "tabControl";
            tabControl.ClientRectangle = new Rectangle(12, 10, 0, 0);
            tabControl.ClickSound = new EnhancedSoundEffect("button.wav");
            tabControl.FontIndex = 1;
            tabControl.AddTab("游戏数据", 133);
            tabControl.AddTab("系统数据", 133);
            tabControl.AddTab("成就", 133);
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            XNALabel lblFilter = new XNALabel(WindowManager);
            lblFilter.Name = "lblFilter";
            lblFilter.FontIndex = 1;
            lblFilter.Text = "筛选:";
            lblFilter.ClientRectangle = new Rectangle(660, 12, 0, 0);

            cmbGameClassFilter = new XNAClientDropDown(WindowManager);
            cmbGameClassFilter.ClientRectangle = new Rectangle(785, 11, 105, 21);
            cmbGameClassFilter.Name = "cmbGameClassFilter";
            cmbGameClassFilter.AddItem("所有游戏");
            cmbGameClassFilter.AddItem("在线游戏");
            cmbGameClassFilter.AddItem("在线PvP");
            cmbGameClassFilter.AddItem("在线PVE");
            cmbGameClassFilter.AddItem("遭遇战");
            cmbGameClassFilter.SelectedIndex = 0;
            cmbGameClassFilter.SelectedIndexChanged += CmbGameClassFilter_SelectedIndexChanged;

            XNALabel lblGameMode = new XNALabel(WindowManager);
            lblGameMode.Name = nameof(lblGameMode);
            lblGameMode.FontIndex = 1;
            lblGameMode.Text = "游戏模式:";
            lblGameMode.ClientRectangle = new Rectangle(480, 12, 0, 0);

            cmbGameModeFilter = new XNAClientDropDown(WindowManager);
            cmbGameModeFilter.Name = nameof(cmbGameModeFilter);
            cmbGameModeFilter.ClientRectangle = new Rectangle(581, 11, 114, 21);
            cmbGameModeFilter.SelectedIndexChanged += CmbGameModeFilter_SelectedIndexChanged;

            var btnReturnToMenu = new XNAClientButton(WindowManager);
            btnReturnToMenu.Name = nameof(btnReturnToMenu);
            btnReturnToMenu.ClientRectangle = new Rectangle(700, 486, 160, 23);
            btnReturnToMenu.Text = "返回主菜单";
            btnReturnToMenu.LeftClick += BtnReturnToMenu_LeftClick;

            var btnClearStatistics = new XNAClientButton(WindowManager);
            btnClearStatistics.Name = nameof(btnClearStatistics);
            btnClearStatistics.ClientRectangle = new Rectangle(20, 486, 160, 23);
            btnClearStatistics.Text = "删除数据";
            btnClearStatistics.LeftClick += BtnClearStatistics_LeftClick;
            //btnClearStatistics.Visible = false;

            chkIncludeSpectatedGames = new XNAClientCheckBox(WindowManager);

            AddChild(chkIncludeSpectatedGames);
            chkIncludeSpectatedGames.Name = nameof(chkIncludeSpectatedGames);
            chkIncludeSpectatedGames.Text = "包含观战局";
            chkIncludeSpectatedGames.Checked = true;
            chkIncludeSpectatedGames.ClientRectangle = new Rectangle(
                Width - chkIncludeSpectatedGames.Width - 12,
                cmbGameModeFilter.Bottom + 3,
                chkIncludeSpectatedGames.Width,
                chkIncludeSpectatedGames.Height);
            chkIncludeSpectatedGames.CheckedChanged += ChkIncludeSpectatedGames_CheckedChanged;



            #region Match statistics

            panelGameStatistics = new XNAPanel(WindowManager);
            panelGameStatistics.Name = "panelGameStatistics";
            panelGameStatistics.BackgroundTexture = AssetLoader.LoadTexture("50.png");
            panelGameStatistics.ClientRectangle = new Rectangle(10, 55, 880, 425);

            AddChild(panelGameStatistics);

            XNALabel lblGames = new XNALabel(WindowManager);
            lblGames.Name = nameof(lblGames);
            lblGames.Text = "游戏们:";
            lblGames.FontIndex = 1;
            lblGames.ClientRectangle = new Rectangle(4, 2, 0, 0);

            lbGameList = new XNAMultiColumnListBox(WindowManager);
            lbGameList.Name = nameof(lbGameList);
            lbGameList.ClientRectangle = new Rectangle(2, 25, 876, 250);
            lbGameList.BackgroundTexture = AssetLoader.CreateTexture(new Color(0, 0, 0, 128), 1, 1);
            lbGameList.PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.STRETCHED;
            lbGameList.AddColumn("时间", 130);
            lbGameList.AddColumn("地图", 200);
            lbGameList.AddColumn("游戏模式", 130);
            lbGameList.AddColumn("FPS", 90);
            lbGameList.AddColumn("进行时间", 150);
            lbGameList.AddColumn("完成", 90);
            lbGameList.SelectedIndexChanged += LbGameList_SelectedIndexChanged;
            lbGameList.AllowKeyboardInput = true;

            lbGameStatistics = new XNAMultiColumnListBox(WindowManager);
            lbGameStatistics.Name = nameof(lbGameStatistics);
            lbGameStatistics.ClientRectangle = new Rectangle(2, 280, 876, 143);
            lbGameStatistics.BackgroundTexture = AssetLoader.CreateTexture(new Color(0, 0, 0, 128), 1, 1);
            lbGameStatistics.PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.STRETCHED;
            lbGameStatistics.AddColumn("昵称", 130);
            lbGameStatistics.AddColumn("击杀数", 100);
            lbGameStatistics.AddColumn("死亡数", 100);
            lbGameStatistics.AddColumn(strLblEconomy, 100);
            lbGameStatistics.AddColumn("积分", 100);
            lbGameStatistics.AddColumn("胜利", 80);
            lbGameStatistics.AddColumn("失败", 80);
            lbGameStatistics.AddColumn("团队", 80);

            panelGameStatistics.AddChild(lblGames);
            panelGameStatistics.AddChild(lbGameList);
            panelGameStatistics.AddChild(lbGameStatistics);

            #endregion

            #region Total statistics

            panelTotalStatistics = new XNAPanel(WindowManager);
            panelTotalStatistics.Name = "panelTotalStatistics";
            panelTotalStatistics.BackgroundTexture = AssetLoader.LoadTexture("50.png");
            panelTotalStatistics.ClientRectangle = new Rectangle(10, 55, 880, 425);

            AddChild(panelTotalStatistics);
            panelTotalStatistics.Visible = false;
            panelTotalStatistics.Enabled = false;

            int locationY = TOTAL_STATS_FIRST_ITEM_Y;

            AddTotalStatisticsLabel("lblGamesStarted", "总游戏回合数:", new Point(TOTAL_STATS_LOCATION_X1, locationY));

            lblGamesStartedValue = new XNALabel(WindowManager);
            lblGamesStartedValue.Name = "lblGamesStartedValue";
            lblGamesStartedValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1, locationY, 0, 0);
            lblGamesStartedValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblGamesFinished", "完整的游戏次数:", new Point(TOTAL_STATS_LOCATION_X1, locationY));

            lblGamesFinishedValue = new XNALabel(WindowManager);
            lblGamesFinishedValue.Name = "lblGamesFinishedValue";
            lblGamesFinishedValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1, locationY, 0, 0);
            lblGamesFinishedValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblWins", "获胜次数:", new Point(TOTAL_STATS_LOCATION_X1, locationY));

            lblWinsValue = new XNALabel(WindowManager);
            lblWinsValue.Name = "lblWinsValue";
            lblWinsValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1, locationY, 0, 0);
            lblWinsValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblLosses", "失败次数:", new Point(TOTAL_STATS_LOCATION_X1, locationY));

            lblLossesValue = new XNALabel(WindowManager);
            lblLossesValue.Name = "lblLossesValue";
            lblLossesValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1, locationY, 0, 0);
            lblLossesValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblWinLossRatio", "胜利/失败比例:", new Point(TOTAL_STATS_LOCATION_X1, locationY));

            lblWinLossRatioValue = new XNALabel(WindowManager);
            lblWinLossRatioValue.Name = "lblWinLossRatioValue";
            lblWinLossRatioValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1, locationY, 0, 0);
            lblWinLossRatioValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblAverageGameLength", "平均游戏时间:", new Point(TOTAL_STATS_LOCATION_X1, locationY));

            lblAverageGameLengthValue = new XNALabel(WindowManager);
            lblAverageGameLengthValue.Name = "lblAverageGameLengthValue";
            lblAverageGameLengthValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1, locationY, 0, 0);
            lblAverageGameLengthValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblTotalTimePlayed", "总游戏时间:", new Point(TOTAL_STATS_LOCATION_X1, locationY));

            lblTotalTimePlayedValue = new XNALabel(WindowManager);
            lblTotalTimePlayedValue.Name = "lblTotalTimePlayedValue";
            lblTotalTimePlayedValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1, locationY, 0, 0);
            lblTotalTimePlayedValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblAverageEnemyCount", "平均敌人数量:", new Point(TOTAL_STATS_LOCATION_X1, locationY));

            lblAverageEnemyCountValue = new XNALabel(WindowManager);
            lblAverageEnemyCountValue.Name = "lblAverageEnemyCountValue";
            lblAverageEnemyCountValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1, locationY, 0, 0);
            lblAverageEnemyCountValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblAverageAllyCount", "平均盟友数量:", new Point(TOTAL_STATS_LOCATION_X1, locationY));

            lblAverageAllyCountValue = new XNALabel(WindowManager);
            lblAverageAllyCountValue.Name = "lblAverageAllyCountValue";
            lblAverageAllyCountValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1, locationY, 0, 0);
            lblAverageAllyCountValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            // SECOND COLUMN

            locationY = TOTAL_STATS_FIRST_ITEM_Y;

            AddTotalStatisticsLabel("lblTotalKills", "总歼敌数:", new Point(TOTAL_STATS_LOCATION_X2, locationY));

            lblTotalKillsValue = new XNALabel(WindowManager);
            lblTotalKillsValue.Name = "lblTotalKillsValue";
            lblTotalKillsValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2, locationY, 0, 0);
            lblTotalKillsValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblKillsPerGame", "平均歼敌数:", new Point(TOTAL_STATS_LOCATION_X2, locationY));

            lblKillsPerGameValue = new XNALabel(WindowManager);
            lblKillsPerGameValue.Name = "lblKillsPerGameValue";
            lblKillsPerGameValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2, locationY, 0, 0);
            lblKillsPerGameValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblTotalLosses", "总损失数:", new Point(TOTAL_STATS_LOCATION_X2, locationY));

            lblTotalLossesValue = new XNALabel(WindowManager);
            lblTotalLossesValue.Name = "lblTotalLossesValue";
            lblTotalLossesValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2, locationY, 0, 0);
            lblTotalLossesValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblLossesPerGame", "平均损失数:", new Point(TOTAL_STATS_LOCATION_X2, locationY));

            lblLossesPerGameValue = new XNALabel(WindowManager);
            lblLossesPerGameValue.Name = "lblLossesPerGameValue";
            lblLossesPerGameValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2, locationY, 0, 0);
            lblLossesPerGameValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblKillLossRatio", "歼敌/损失比例:", new Point(TOTAL_STATS_LOCATION_X2, locationY));

            lblKillLossRatioValue = new XNALabel(WindowManager);
            lblKillLossRatioValue.Name = "lblKillLossRatioValue";
            lblKillLossRatioValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2, locationY, 0, 0);
            lblKillLossRatioValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblTotalScore", "总分:", new Point(TOTAL_STATS_LOCATION_X2, locationY));

            lblTotalScoreValue = new XNALabel(WindowManager);
            lblTotalScoreValue.Name = "lblTotalScoreValue";
            lblTotalScoreValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2, locationY, 0, 0);
            lblTotalScoreValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblAverageEconomy", strLblAvgEconomy, new Point(TOTAL_STATS_LOCATION_X2, locationY));

            lblAverageEconomyValue = new XNALabel(WindowManager);
            lblAverageEconomyValue.Name = "lblAverageEconomyValue";
            lblAverageEconomyValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2, locationY, 0, 0);
            lblAverageEconomyValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblFavouriteSide", "偏好阵营:", new Point(TOTAL_STATS_LOCATION_X2, locationY));

            lblFavouriteSideValue = new XNALabel(WindowManager);
            lblFavouriteSideValue.Name = "lblFavouriteSideValue";
            lblFavouriteSideValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2, locationY, 0, 0);
            lblFavouriteSideValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            AddTotalStatisticsLabel("lblAverageAILevel", "平均AI水平:", new Point(TOTAL_STATS_LOCATION_X2, locationY));

            lblAverageAILevelValue = new XNALabel(WindowManager);
            lblAverageAILevelValue.Name = "lblAverageAILevelValue";
            lblAverageAILevelValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2, locationY, 0, 0);
            lblAverageAILevelValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            panelTotalStatistics.AddChild(lblGamesStartedValue);
            panelTotalStatistics.AddChild(lblGamesFinishedValue);
            panelTotalStatistics.AddChild(lblWinsValue);
            panelTotalStatistics.AddChild(lblLossesValue);
            panelTotalStatistics.AddChild(lblWinLossRatioValue);
            panelTotalStatistics.AddChild(lblAverageGameLengthValue);
            panelTotalStatistics.AddChild(lblTotalTimePlayedValue);
            panelTotalStatistics.AddChild(lblAverageEnemyCountValue);
            panelTotalStatistics.AddChild(lblAverageAllyCountValue);

            panelTotalStatistics.AddChild(lblTotalKillsValue);
            panelTotalStatistics.AddChild(lblKillsPerGameValue);
            panelTotalStatistics.AddChild(lblTotalLossesValue);
            panelTotalStatistics.AddChild(lblLossesPerGameValue);
            panelTotalStatistics.AddChild(lblKillLossRatioValue);
            panelTotalStatistics.AddChild(lblTotalScoreValue);
            panelTotalStatistics.AddChild(lblAverageEconomyValue);
            panelTotalStatistics.AddChild(lblFavouriteSideValue);
            panelTotalStatistics.AddChild(lblAverageAILevelValue);



            //成就
            panelAchStatistics = new XNAPanel(WindowManager);
            panelAchStatistics.Name = "panelAchStatistics";
            panelAchStatistics.BackgroundTexture = AssetLoader.LoadTexture("50.png");
            panelAchStatistics.ClientRectangle = new Rectangle(10, 55, 720, 425);
            Value = new Double[15, 2];
            AddChild(panelAchStatistics);
            panelAchStatistics.Visible = false;
            panelAchStatistics.Enabled = false;

            locationY = TOTAL_STATS_FIRST_ITEM_Y;

            Value[0, 1] = 2000;
            PrghardenedValue = new XNAProgressBar(WindowManager);
            PrghardenedValue.Name = "PrghardenedValue";
            PrghardenedValue.Maximum = (int)Value[0, 1];
            PrghardenedValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1 - 110, locationY - 4, 220, 25);
            //PrghardenedValue.FilledColor = new Color(249, 204, 226);
            locationY += TOTAL_STATS_Y_INCREASE;

            Value[1, 1] = 1000000;
            PrgkillValue = new XNAProgressBar(WindowManager);
            PrgkillValue.Name = "PrgkillValue";
            PrgkillValue.Maximum = (int)Value[1, 1];
            PrgkillValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1 - 110, locationY - 4, 220, 25);
            PrgkillValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            Value[2, 1] = 10;
            PrgVictorValue = new XNAProgressBar(WindowManager);
            PrgVictorValue.Name = "PrgVictorValue";
            PrgVictorValue.Maximum = (int)Value[2, 1];
            PrgVictorValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1 - 110, locationY - 4, 220, 25);
            PrgVictorValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;


            Value[3, 1] = 200;
            PrgLongValue = new XNAProgressBar(WindowManager);
            PrgLongValue.Name = "PrgLongValue";
            PrgLongValue.Maximum = (int)Value[3, 1];
            PrgLongValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1 - 110, locationY - 4, 220, 25);
            PrgLongValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            Value[4, 1] = 200;
            PrgShortValue = new XNAProgressBar(WindowManager);
            PrgShortValue.Name = "PrgShortValue";
            PrgShortValue.Maximum = (int)Value[4, 1];
            PrgShortValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1 - 110, locationY - 4, 220, 25);
            PrgShortValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;


            Value[5, 1] = 200;
            PrgSoldierValue = new XNAProgressBar(WindowManager);
            PrgSoldierValue.Name = "PrgSoldierValue";
            PrgSoldierValue.Maximum = (int)Value[5, 1];
            PrgSoldierValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1 - 110, locationY - 4, 220, 25);
            PrgSoldierValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            Value[6, 1] = 200;

            PrgNavalValue = new XNAProgressBar(WindowManager);
            PrgNavalValue.Name = "PrgNavalValue";
            PrgNavalValue.Maximum = (int)Value[6, 1];
            PrgNavalValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1 - 110, locationY - 4, 220, 25);
            PrgNavalValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            Value[7, 1] = 200;
            PrgGermanyValue = new XNAProgressBar(WindowManager);
            PrgGermanyValue.Name = "PrgGermanyValue";
            PrgGermanyValue.Maximum = (int)Value[7, 1];
            PrgGermanyValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1 - 110, locationY - 4, 220, 25);
            PrgGermanyValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;


            Value[8, 1] = 200;
            PrgOneValue = new XNAProgressBar(WindowManager);
            PrgOneValue.Name = " PrgOneValue";
            PrgOneValue.Maximum = (int)Value[8, 1];
            PrgOneValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X1 - 110, locationY - 4, 220, 25);
            PrgOneValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            locationY = TOTAL_STATS_FIRST_ITEM_Y;

            Value[9, 1] = 200;

            PrgBulletsValue = new XNAProgressBar(WindowManager);
            PrgBulletsValue.Name = "PrgBulletsValue";
            PrgBulletsValue.Maximum = (int)Value[9, 1];
            PrgBulletsValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2 - 110, locationY - 4, 220, 25);
            PrgBulletsValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            Value[10, 1] = 10;

            PrgFkyValue = new XNAProgressBar(WindowManager);
            PrgFkyValue.Name = "PrgFkyValue";
            PrgFkyValue.Maximum = (int)Value[10, 1];
            PrgFkyValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2 - 110, locationY - 4, 220, 25);
            PrgFkyValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            Value[11, 1] = 200;
            PrgMaxValue = new XNAProgressBar(WindowManager);
            PrgMaxValue.Name = "PrgMaxValue";
            PrgMaxValue.Maximum = (int)Value[11, 1];
            PrgMaxValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2 - 110, locationY - 4, 220, 25);
            PrgMaxValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            Value[12, 1] = 200;
            PrgMinValue = new XNAProgressBar(WindowManager);
            PrgMinValue.Name = "PrgMinValue";
            PrgMinValue.Maximum = (int)Value[12, 1];
            PrgMinValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2 - 110, locationY - 4, 220, 25);
            PrgMinValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            Value[13, 1] = 20;

            PrgMaginotValue = new XNAProgressBar(WindowManager);
            PrgMaginotValue.Name = "PrgMaginotValue";
            PrgMaginotValue.Maximum = (int)Value[13, 1];
            PrgMaginotValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2 - 110, locationY - 4, 220, 25);
            PrgMaginotValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;


            Value[14, 1] = 20;
            PrgBtValue = new XNAProgressBar(WindowManager);
            PrgBtValue.Name = "PrgBtValue";
            PrgBtValue.Maximum = (int)Value[14, 1];
            PrgBtValue.ClientRectangle = new Rectangle(TOTAL_STATS_VALUE_LOCATION_X2 - 110, locationY - 4, 220, 25);
            PrgBtValue.RemapColor = UISettings.ActiveSettings.AltColor;
            locationY += TOTAL_STATS_Y_INCREASE;

            panelAchStatistics.AddChild(PrghardenedValue);
            panelAchStatistics.AddChild(PrgkillValue);
            panelAchStatistics.AddChild(PrgVictorValue);
            panelAchStatistics.AddChild(PrgSoldierValue);
            panelAchStatistics.AddChild(PrgLongValue);
            panelAchStatistics.AddChild(PrgShortValue);
            panelAchStatistics.AddChild(PrgNavalValue);
            panelAchStatistics.AddChild(PrgGermanyValue);
            panelAchStatistics.AddChild(PrgOneValue);
            panelAchStatistics.AddChild(PrgBulletsValue);
            panelAchStatistics.AddChild(PrgFkyValue);
            panelAchStatistics.AddChild(PrgMaxValue);
            panelAchStatistics.AddChild(PrgMinValue);
            panelAchStatistics.AddChild(PrgMaginotValue);
            panelAchStatistics.AddChild(PrgBtValue);
            #endregion


            AddChild(tabControl);
            AddChild(lblFilter);
            AddChild(cmbGameClassFilter);
            AddChild(lblGameMode);
            AddChild(cmbGameModeFilter);
            AddChild(btnReturnToMenu);
            AddChild(btnClearStatistics);

            base.Initialize();

            CenterOnParent();

            sides = ClientConfiguration.Instance.Sides.Split(',');

            sideTextures = new Texture2D[sides.Length + 1];
            for (int i = 0; i < sides.Length; i++)
                sideTextures[i] = AssetLoader.LoadTexture(sides[i] + "icon.png");

            sideTextures[sides.Length] = AssetLoader.LoadTexture("观察者icon.png");

            mpColors = MultiplayerColor.LoadColors();

            StatisticsManager.Instance.GameAdded += Instance_GameAdded;

            ReadStatistics();
            ListGameModes();
            SetTotalStatistics();
            ListGames();

            locationY = TOTAL_STATS_FIRST_ITEM_Y;
            AddAchBtn("btnhardened", "久经沙场", "十年老玩家，有什么不懂的可以问我！：完成2000场对局", new Point(TOTAL_STATS_LOCATION_X1, locationY), 0);
            locationY += TOTAL_STATS_Y_INCREASE;
            AddAchBtn("lblkillValue", "杀人如麻", "我说我杀人不眨眼，你问我眼睛干不干？：累计摧毁数到达1000000", new Point(TOTAL_STATS_LOCATION_X1, locationY), 1);
            locationY += TOTAL_STATS_Y_INCREASE;
            AddAchBtn("PrgVictorValue", "常胜将军", "运气好而已啦！：胜场/负场比到达10（胜率0.9）", new Point(TOTAL_STATS_LOCATION_X1, locationY), 2);
            locationY += TOTAL_STATS_Y_INCREASE;
            AddAchBtn("PrgLongValue", "熬鹰", "我有两台电脑谁怕谁？：完成200场45分钟的对局", new Point(TOTAL_STATS_LOCATION_X1, locationY), 3);
            locationY += TOTAL_STATS_Y_INCREASE;
            AddAchBtn("PrgShortValue", "闪电战", "乱拳打死老师傅。：完成200场10分钟以内的对局", new Point(TOTAL_STATS_LOCATION_X1, locationY), 4);
            locationY += TOTAL_STATS_Y_INCREASE;
            AddAchBtn("PrgSoldierValue", "爱兵如子", "我们对血量的把控十分精准。：完成200场损失数小于10的对局", new Point(TOTAL_STATS_LOCATION_X1, locationY), 5);
            locationY += TOTAL_STATS_Y_INCREASE;
            AddAchBtn("PrgNavalValue", "海贼王", "我要当上海贼王，不是上海贼王！，完成10场海战", new Point(TOTAL_STATS_LOCATION_X1, locationY), 6);
            locationY += TOTAL_STATS_Y_INCREASE;
            AddAchBtn("PrgGermanyValue", "以德服人", "以德服人（物理）。：使用德国胜利20场", new Point(TOTAL_STATS_LOCATION_X1, locationY), 7);
            locationY += TOTAL_STATS_Y_INCREASE;
            AddAchBtn(" PrgOneValue", "请叫我单挑王", "单挑王重出江湖。：单挑获胜200场", new Point(TOTAL_STATS_LOCATION_X1, locationY), 8);
            locationY = TOTAL_STATS_FIRST_ITEM_Y;
            AddAchBtn("PrgBulletsValue", "枪林弹雨", "报告，驾驶员抱着一个方向盘回来了！：完成损失数+摧毁数大于100的对局200场", new Point(TOTAL_STATS_LOCATION_X2 + 10, locationY), 9);
            locationY += TOTAL_STATS_Y_INCREASE;
            AddAchBtn("PrgFkyValue", "法克尤", "天命如此：使用法国在单挑中获胜尤里20局", new Point(TOTAL_STATS_LOCATION_X2 + 10, locationY), 10);
            locationY += TOTAL_STATS_Y_INCREASE;
            AddAchBtn("PrgMaxValue", "尽力局局长", "队友都是萌新宝宝怎么打？：评分最高却失败100场", new Point(TOTAL_STATS_LOCATION_X2 + 10, locationY), 11);
            locationY += TOTAL_STATS_Y_INCREASE;
            AddAchBtn("PrgMaxValue", "躺赢局局长", "ELO机制受益者。：评分最低却获胜100场", new Point(TOTAL_STATS_LOCATION_X2 + 10, locationY), 12);
            locationY += TOTAL_STATS_Y_INCREASE;
            AddAchBtn("PrgMaginotValue", "马奇诺防线", "有本身你来冲我啊？：使用法国且到达400建造数20场", new Point(TOTAL_STATS_LOCATION_X2 + 10, locationY), 13);
            locationY += TOTAL_STATS_Y_INCREASE;
            AddAchBtn("PrgBtValue", "见冰天知树目", "冰天雪地里有多少棵树我都知道：游玩冰天(大)雪地20场", new Point(TOTAL_STATS_LOCATION_X2 + 10, locationY),14);
            locationY += TOTAL_STATS_Y_INCREASE;
            //冰天雪地里有多少棵树我都知道
        }

        private void Instance_GameAdded(object sender, EventArgs e)
        {
            ListGames();
        }

        private void ChkIncludeSpectatedGames_CheckedChanged(object sender, EventArgs e)
        {
            ListGames();
        }

        private void AddTotalStatisticsLabel(string name, string text, Point location)
        {
            XNALabel label = new XNALabel(WindowManager);
            label.Name = name;
            label.Text = text;
            label.ClientRectangle = new Rectangle(location.X, location.Y, 0, 0);
            panelTotalStatistics.AddChild(label);
        }

        private void AddAchBtn(string name, string Title, string Content, Point location, int i)
        {
            XNAButton btn = new XNAClientButton(WindowManager);
            btn.Name = name;
            btn.Text = Title;
            btn.ClientRectangle = new Rectangle(location.X - 25, location.Y - 5, 100, 30);
            btn.IdleTexture = AssetLoader.LoadTexture("92pxbtn.png");
            btn.HoverTexture = AssetLoader.LoadTexture("92pxbtn_c.png");
            if (Value[i, 0] <= Value[i, 1])
                Content += "          " + Value[i, 0].ToString() + "/" + Value[i, 1].ToString();
            else
                Content += "          已完成";
            btn.LeftClick += (s, e) => Messagebox(Title, Content);
            panelAchStatistics.AddChild(btn);

        }

        private void Messagebox(string Title, string Content)
        {
            XNAMessageBox messageBox = new XNAMessageBox(WindowManager, Title, Content, XNAMessageBoxButtons.OK);
            messageBox.Show();
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == 1)
            {
                panelGameStatistics.Visible = false;
                panelGameStatistics.Enabled = false;
                panelTotalStatistics.Visible = true;
                panelTotalStatistics.Enabled = true;
                panelAchStatistics.Visible = false;
                panelAchStatistics.Enabled = false;
            }
            else if (tabControl.SelectedTab == 2)
            {
                panelGameStatistics.Visible = false;
                panelGameStatistics.Enabled = false;
                panelTotalStatistics.Visible = false;
                panelTotalStatistics.Enabled = false;
                panelAchStatistics.Visible = true;
                panelAchStatistics.Enabled = true;
            }
            else
            {

                panelGameStatistics.Visible = true;
                panelGameStatistics.Enabled = true;
                panelTotalStatistics.Visible = false;
                panelTotalStatistics.Enabled = false;
                panelAchStatistics.Visible = false;
                panelAchStatistics.Enabled = false;
            }
        }

        private void CmbGameClassFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListGames();
        }

        private void CmbGameModeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListGames();
        }

        private void LbGameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbGameStatistics.ClearItems();

            if (lbGameList.SelectedIndex == -1)
                return;

            MatchStatistics ms = sm.GetMatchByIndex(listedGameIndexes[lbGameList.SelectedIndex]);

            List<PlayerStatistics> players = new List<PlayerStatistics>();

            for (int i = 0; i < ms.GetPlayerCount(); i++)
            {
                players.Add(ms.GetPlayer(i));
            }

            players = players.OrderBy(p => p.Score).Reverse().ToList();

            Color textColor = UISettings.ActiveSettings.AltColor;

            for (int i = 0; i < ms.GetPlayerCount(); i++)
            {
                PlayerStatistics ps = players[i];

                //List<string> items = new List<string>();
                List<XNAListBoxItem> items = new List<XNAListBoxItem>();

                if (ps.Color > -1 && ps.Color < mpColors.Count)
                    textColor = mpColors[ps.Color].XnaColor;

                if (ps.IsAI)
                {
                    items.Add(new XNAListBoxItem(AILevelToString(ps.AILevel), textColor));
                }
                else
                    items.Add(new XNAListBoxItem(ps.Name, textColor));

                if (ps.WasSpectator)
                {
                    // Player was a spectator
                    items.Add(new XNAListBoxItem("-", textColor));
                    items.Add(new XNAListBoxItem("-", textColor));
                    items.Add(new XNAListBoxItem("-", textColor));
                    items.Add(new XNAListBoxItem("-", textColor));
                    items.Add(new XNAListBoxItem("-", textColor));
                    XNAListBoxItem spectatorItem = new XNAListBoxItem();
                    spectatorItem.Text = "观察者";
                    spectatorItem.TextColor = textColor;
                    spectatorItem.Texture = sideTextures[sideTextures.Length - 1];
                    items.Add(spectatorItem);
                    items.Add(new XNAListBoxItem("-", textColor));
                }
                else
                {
                    if (!ms.SawCompletion)
                    {
                        // The game wasn't completed - we don't know the stats
                        items.Add(new XNAListBoxItem("-", textColor));
                        items.Add(new XNAListBoxItem("-", textColor));
                        items.Add(new XNAListBoxItem("-", textColor));
                        items.Add(new XNAListBoxItem("-", textColor));
                        items.Add(new XNAListBoxItem("-", textColor));
                    }
                    else
                    {
                        // The game was completed and the player was actually playing
                        items.Add(new XNAListBoxItem(ps.Kills.ToString(), textColor));
                        items.Add(new XNAListBoxItem(ps.Losses.ToString(), textColor));
                        items.Add(new XNAListBoxItem(ps.Economy.ToString(), textColor));
                        items.Add(new XNAListBoxItem(ps.Score.ToString(), textColor));
                        items.Add(new XNAListBoxItem(
                            Conversions.BooleanToString(ps.Won, BooleanStringStyle.YESNO), textColor));
                    }

                    if (ps.Side == 0 || ps.Side > sides.Length)
                        items.Add(new XNAListBoxItem("未知", textColor));
                    else
                    {
                        XNAListBoxItem sideItem = new XNAListBoxItem();
                        sideItem.Text = sides[ps.Side - 1];
                        sideItem.TextColor = textColor;
                        sideItem.Texture = sideTextures[ps.Side - 1];
                        items.Add(sideItem);
                    }

                    items.Add(new XNAListBoxItem(TeamIndexToString(ps.Team), textColor));
                }

                if (!ps.IsLocalPlayer)
                {
                    lbGameStatistics.AddItem(items);

                    items.ForEach(item => item.Selectable = false);
                }
                else
                {
                    lbGameStatistics.AddItem(items);
                    lbGameStatistics.SelectedIndex = i;
                }
            }
        }

        private string TeamIndexToString(int teamIndex)
        {
            switch (teamIndex)
            {
                case 1:
                    return "A";
                case 2:
                    return "B";
                case 3:
                    return "C";
                case 4:
                    return "D";
                default:
                    return "-";
            }
        }

        private string AILevelToString(int aiLevel)
        {
            switch (aiLevel)
            {
                case 2:
                    return "简单的电脑";
                case 1:
                    return "普通的电脑";
                case 0:
                default:
                    return "困难的电脑";
            }
        }

        #region Statistics reading / game listing code

        void ReadStatistics()
        {
            StatisticsManager sm = StatisticsManager.Instance;

            sm.ReadStatistics(ProgramConstants.GamePath);
        }

        void ListGameModes()
        {
            int gameCount = sm.GetMatchCount();

            List<string> gameModes = new List<string>();

            cmbGameModeFilter.Items.Clear();

            cmbGameModeFilter.AddItem("所有");

            for (int i = 0; i < gameCount; i++)
            {
                MatchStatistics ms = sm.GetMatchByIndex(i);
                if (!gameModes.Contains(ms.GameMode))
                    gameModes.Add(ms.GameMode);
            }

            gameModes.Sort();

            foreach (string gm in gameModes)
                cmbGameModeFilter.AddItem(gm);

            cmbGameModeFilter.SelectedIndex = 0;
        }

        void ListGames()
        {
            lbGameList.SelectedIndex = -1;
            lbGameList.SetTopIndex(0);

            lbGameStatistics.ClearItems();
            lbGameList.ClearItems();
            listedGameIndexes.Clear();

            switch (cmbGameClassFilter.SelectedIndex)
            {
                case 0:
                    ListAllGames();
                    break;
                case 1:
                    ListOnlineGames();
                    break;
                case 2:
                    ListPvPGames();
                    break;
                case 3:
                    ListCoOpGames();
                    break;
                case 4:
                    ListSkirmishGames();
                    break;
            }

            listedGameIndexes.Reverse();


            SetAchStatistics();
            foreach (int gameIndex in listedGameIndexes)
            {
                MatchStatistics ms = sm.GetMatchByIndex(gameIndex);
                string dateTime = ms.DateAndTime.ToShortDateString() + " " + ms.DateAndTime.ToShortTimeString();
                List<string> info = new List<string>();
                info.Add(Renderer.GetSafeString(dateTime, lbGameList.FontIndex));
                info.Add(ms.MapName);
                info.Add(ms.GameMode);
                if (ms.AverageFPS == 0)
                    info.Add("-");
                else
                    info.Add(ms.AverageFPS.ToString());
                info.Add(Renderer.GetSafeString(TimeSpan.FromSeconds(ms.LengthInSeconds).ToString(), lbGameList.FontIndex));
                info.Add(Conversions.BooleanToString(ms.SawCompletion, BooleanStringStyle.YESNO));
                lbGameList.AddItem(info, true);
            }
        }

        private void ListAllGames()
        {
            int gameCount = sm.GetMatchCount();

            for (int i = 0; i < gameCount; i++)
            {
                ListGameIndexIfPrerequisitesMet(i);
            }
        }

        private void ListOnlineGames()
        {
            int gameCount = sm.GetMatchCount();

            for (int i = 0; i < gameCount; i++)
            {
                MatchStatistics ms = sm.GetMatchByIndex(i);

                int pCount = ms.GetPlayerCount();
                int hpCount = 0;

                for (int j = 0; j < pCount; j++)
                {
                    PlayerStatistics ps = ms.GetPlayer(j);

                    if (!ps.IsAI)
                    {
                        hpCount++;

                        if (hpCount > 1)
                        {
                            ListGameIndexIfPrerequisitesMet(i);
                            break;
                        }
                    }
                }
            }
        }

        private void ListPvPGames()
        {
            int gameCount = sm.GetMatchCount();

            for (int i = 0; i < gameCount; i++)
            {
                MatchStatistics ms = sm.GetMatchByIndex(i);

                int pCount = ms.GetPlayerCount();
                int pTeam = -1;

                for (int j = 0; j < pCount; j++)
                {
                    PlayerStatistics ps = ms.GetPlayer(j);

                    if (!ps.IsAI && !ps.WasSpectator)
                    {
                        // If we find a single player on a different team than another player,
                        // we'll count the game as a PvP game
                        if (pTeam > -1 && (ps.Team != pTeam || ps.Team == 0))
                        {
                            ListGameIndexIfPrerequisitesMet(i);
                            break;
                        }

                        pTeam = ps.Team;
                    }
                }
            }
        }

        private void ListCoOpGames()
        {
            int gameCount = sm.GetMatchCount();

            for (int i = 0; i < gameCount; i++)
            {
                MatchStatistics ms = sm.GetMatchByIndex(i);

                int pCount = ms.GetPlayerCount();
                int hpCount = 0;
                int pTeam = -1;
                bool add = true;

                for (int j = 0; j < pCount; j++)
                {
                    PlayerStatistics ps = ms.GetPlayer(j);

                    if (!ps.IsAI && !ps.WasSpectator)
                    {
                        hpCount++;

                        if (pTeam > -1 && (ps.Team != pTeam || ps.Team == 0))
                        {
                            add = false;
                            break;
                        }

                        pTeam = ps.Team;
                    }
                }

                if (add && hpCount > 1)
                {
                    ListGameIndexIfPrerequisitesMet(i);
                }
            }
        }

        private void ListSkirmishGames()
        {
            int gameCount = sm.GetMatchCount();

            for (int i = 0; i < gameCount; i++)
            {
                MatchStatistics ms = sm.GetMatchByIndex(i);

                int pCount = ms.GetPlayerCount();
                int hpCount = 0;
                bool add = true;

                foreach (PlayerStatistics ps in ms.Players)
                {
                    if (!ps.IsAI)
                    {
                        hpCount++;

                        if (hpCount > 1)
                        {
                            add = false;
                            break;
                        }
                    }
                }

                if (add)
                {
                    ListGameIndexIfPrerequisitesMet(i);
                }
            }
        }

        private void ListGameIndexIfPrerequisitesMet(int gameIndex)
        {
            MatchStatistics ms = sm.GetMatchByIndex(gameIndex);

            if (cmbGameModeFilter.SelectedIndex != 0)
            {
                string gameMode = cmbGameModeFilter.Items[cmbGameModeFilter.SelectedIndex].Text;

                if (ms.GameMode != gameMode)
                    return;
            }

            PlayerStatistics ps = ms.Players.Find(p => p.IsLocalPlayer);

            if (ps != null && !chkIncludeSpectatedGames.Checked)
            {
                if (ps.WasSpectator)
                    return;
            }

            listedGameIndexes.Add(gameIndex);
        }

        /// <summary>
        /// Adjusts the labels on the "Total statistics" tab.
        /// </summary>
        private void SetTotalStatistics()
        {
            int gamesStarted = 0;
            int gamesFinished = 0;
            int gamesPlayed = 0;
            int wins = 0;
            int gameLosses = 0;
            TimeSpan timePlayed = TimeSpan.Zero;
            int numEnemies = 0;
            int numAllies = 0;
            int totalKills = 0;
            int totalLosses = 0;
            int totalScore = 0;
            int totalEconomy = 0;
            int[] sideGameCounts = new int[sides.Length];
            int numEasyAIs = 0;
            int numMediumAIs = 0;
            int numHardAIs = 0;

            foreach (int gameIndex in listedGameIndexes)
            {
                MatchStatistics ms = sm.GetMatchByIndex(gameIndex);

                gamesStarted++;

                if (ms.SawCompletion)
                    gamesFinished++;

                timePlayed += TimeSpan.FromSeconds(ms.LengthInSeconds);

                PlayerStatistics localPlayer = FindLocalPlayer(ms);

                if (!localPlayer.WasSpectator)
                {
                    totalKills += localPlayer.Kills;
                    totalLosses += localPlayer.Losses;
                    totalScore += localPlayer.Score;
                    totalEconomy += localPlayer.Economy;

                    if (localPlayer.Side > 0 && localPlayer.Side <= sides.Length)
                        sideGameCounts[localPlayer.Side - 1]++;

                    if (!ms.SawCompletion)
                        continue;

                    if (localPlayer.Won)
                        wins++;
                    else
                        gameLosses++;

                    gamesPlayed++;

                    for (int i = 0; i < ms.GetPlayerCount(); i++)
                    {
                        PlayerStatistics ps = ms.GetPlayer(i);

                        if (!ps.WasSpectator && (!ps.IsLocalPlayer || ps.IsAI))
                        {
                            if (ps.Team == 0 || localPlayer.Team != ps.Team)
                                numEnemies++;

                            else
                                numAllies++;

                            if (ps.IsAI)
                            {
                                if (ps.AILevel == 0)
                                    numEasyAIs++;
                                else if (ps.AILevel == 1)
                                    numMediumAIs++;
                                else
                                    numHardAIs++;
                            }
                        }
                    }
                }
            }

            lblGamesStartedValue.Text = gamesStarted.ToString();
            lblGamesFinishedValue.Text = gamesFinished.ToString();
            lblWinsValue.Text = wins.ToString();
            lblLossesValue.Text = gameLosses.ToString();

            if (gameLosses > 0)
            {
                lblWinLossRatioValue.Text = Math.Round(wins / (double)gameLosses, 2).ToString();
                Value[2, 0] = Math.Round(wins / (double)gameLosses, 2);
            }
            else
            {
                Value[2, 0] = 0;
                lblWinLossRatioValue.Text = "-";
            }
            if (gamesStarted > 0)
            {
                lblAverageGameLengthValue.Text = TimeSpan.FromSeconds((int)timePlayed.TotalSeconds / gamesStarted).ToString();
            }
            else
                lblAverageGameLengthValue.Text = "-";

            if (gamesPlayed > 0)
            {
                lblAverageEnemyCountValue.Text = Math.Round(numEnemies / (double)gamesPlayed, 2).ToString();
                lblAverageAllyCountValue.Text = Math.Round(numAllies / (double)gamesPlayed, 2).ToString();
                lblKillsPerGameValue.Text = (totalKills / gamesPlayed).ToString();
                lblLossesPerGameValue.Text = (totalLosses / gamesPlayed).ToString();
                lblAverageEconomyValue.Text = (totalEconomy / gamesPlayed).ToString();
            }
            else
            {
                lblAverageEnemyCountValue.Text = "-";
                lblAverageAllyCountValue.Text = "-";
                lblKillsPerGameValue.Text = "-";
                lblLossesPerGameValue.Text = "-";
                lblAverageEconomyValue.Text = "-";
            }

            if (totalLosses > 0)
            {
                lblKillLossRatioValue.Text = Math.Round(totalKills / (double)totalLosses, 2).ToString();
            }
            else
                lblKillLossRatioValue.Text = "-";
            Value[0, 0] = gamesStarted;
            Value[1, 0] = totalKills;

            lblTotalTimePlayedValue.Text = timePlayed.ToString();
            lblTotalKillsValue.Text = totalKills.ToString();
            lblTotalLossesValue.Text = totalLosses.ToString();
            lblTotalScoreValue.Text = totalScore.ToString();
            lblFavouriteSideValue.Text = sides[GetHighestIndex(sideGameCounts)];

            if (numEasyAIs >= numMediumAIs && numEasyAIs >= numHardAIs)
                lblAverageAILevelValue.Text = "简单";
            else if (numMediumAIs >= numEasyAIs && numMediumAIs >= numHardAIs)
                lblAverageAILevelValue.Text = "一般";
            else
                lblAverageAILevelValue.Text = "困难";
        }



        private void SetAchStatistics()
        {
            TimeSpan timePlayed = TimeSpan.Zero;
            Value[3, 0] = 0;
            Value[4, 0] = 0;
            Value[5, 0] = 0;
            Value[6, 0] = 0;
            Value[7, 0] = 0;    //以德服人
            Value[8, 0] = 0;    //过关斩将
            Value[9, 0] = 0;    //枪林弹雨
            Value[10, 0] = 0;  //法克尤
            Value[11, 0] = 0;  //尽力局
            Value[12, 0] = 0;  //躺赢局
            Value[13, 0] = 0;//马奇诺防线
            Value[14, 0] = 0;//冰天

            foreach (int gameIndex in listedGameIndexes)
            {
                MatchStatistics ms = sm.GetMatchByIndex(gameIndex);
                PlayerStatistics localPlayer = FindLocalPlayer(ms);
                if (ms.GameMode == "海战")
                {
                    Value[6, 0]++;
                }
                if (string.Compare(TimeSpan.FromSeconds(ms.LengthInSeconds).ToString(), "00:45:00") > 0)
                    Value[3, 0]++;
                if (string.Compare(TimeSpan.FromSeconds(ms.LengthInSeconds).ToString(), "00:10:00") < 0)
                    Value[4, 0]++;
                if (!localPlayer.WasSpectator)//玩家不是观察者
                {
                    if (localPlayer.Won)//玩家胜利
                    {
                        if (localPlayer.Side == 4)//玩家德国
                            Value[7, 0]++;
                        if (ms.MapName[1] == '2') // 单人图
                            Value[8, 0]++;
                        if (ms.MapName == "[8]冰天大雪地" || ms.MapName == "[8]冰天雪地")
                            Value[14, 0]++;
                        if (localPlayer.Side == 3)
                        {//玩家法国
                            if ((ms.GetPlayer(0).Side == 3 && ms.GetPlayer(1).Side == 10) || (ms.GetPlayer(1).Side == 3 && ms.GetPlayer(0).Side == 10)) //两人为法国和尤里
                                Value[10, 0]++;
                            if (localPlayer.Economy > 500)
                                Value[13, 0]++;
                        }
                        int Max = 0, Min = 9999999;
                        for (int c = 0; c < ms.GetPlayerCount(); c++)
                        {
                            if (ms.GetPlayer(c).Score > Max)
                                Max = ms.GetPlayer(c).Score;
                            if (ms.GetPlayer(c).Score < Min)
                                Min = ms.GetPlayer(c).Score;
                        }
                        if (localPlayer.Score == Max && !localPlayer.Won)
                            Value[11, 0]++;
                        if (localPlayer.Score == Min && localPlayer.Won)
                            Value[12, 0]++;
                        if (localPlayer.Losses <= 10) 
                        Value[5, 0]++;
                        if (localPlayer.Losses + localPlayer.Kills > 1000)
                            Value[9, 0]++;
                    }
                }
            }
            PrghardenedValue.Value = (int)Value[0, 0];
            PrgkillValue.Value = (int)Value[1, 0];
            PrgVictorValue.Value = (int)Value[2, 0];
            PrgLongValue.Value = (int)Value[3, 0];
            PrgShortValue.Value = (int)Value[4, 0];
            PrgSoldierValue.Value = (int)Value[5, 0];
            PrgNavalValue.Value = (int)Value[6, 0];
            PrgGermanyValue.Value = (int)Value[7, 0];
            PrgOneValue.Value = (int)Value[8, 0];
            PrgBulletsValue.Value = (int)Value[9, 0];
            PrgFkyValue.Value = (int)Value[10, 0];
            PrgMaxValue.Value = (int)Value[11, 0];
            PrgMinValue.Value = (int)Value[12, 0];
            PrgMaginotValue.Value = (int)Value[13, 0];
            PrgBtValue.Value = (int)Value[14, 0];
        }

        private PlayerStatistics FindLocalPlayer(MatchStatistics ms)
        {
            int pCount = ms.GetPlayerCount();

            for (int pId = 0; pId < pCount; pId++)
            {
                PlayerStatistics ps = ms.GetPlayer(pId);

                if (!ps.IsAI && ps.IsLocalPlayer)
                    return ps;
            }

            return null;
        }

        private int GetHighestIndex(int[] t)
        {
            int highestIndex = -1;
            int highest = Int32.MinValue;

            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] > highest)
                {
                    highest = t[i];
                    highestIndex = i;
                }
            }

            return highestIndex;
        }

        private void ClearAllStatistics()
        {
            StatisticsManager.Instance.ClearDatabase();
            ReadStatistics();
            ListGameModes();
            ListGames();
        }

        #endregion

        private void BtnReturnToMenu_LeftClick(object sender, EventArgs e)
        {
            // To hide the control, just set Enabled=false
            // and MainMenuDarkeningPanel will deal with the rest
            Enabled = false;
        }

        private void BtnClearStatistics_LeftClick(object sender, EventArgs e)
        {
            var msgBox = new XNAMessageBox(WindowManager, "清除所有数据",
                "所有统计数据将从数据库中清除." +
                Environment.NewLine + Environment.NewLine +
                "您确定要继续吗?", XNAMessageBoxButtons.YesNo);
            msgBox.Show();
            msgBox.YesClickedAction = ClearStatisticsConfirmation_YesClicked;
        }

        private void ClearStatisticsConfirmation_YesClicked(XNAMessageBox messageBox)
        {
            ClearAllStatistics();
        }
    }
}
