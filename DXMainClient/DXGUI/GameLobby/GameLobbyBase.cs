﻿using ClientGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Rampastring.XNAUI.DXControls;
using Rampastring.XNAUI;
using Rampastring.Tools;
using ClientCore;
using DTAClient.domain.CnCNet;
using System.IO;
using ClientCore.Statistics;

namespace DTAClient.DXGUI.GameLobby
{
    /// <summary>
    /// A generic base for all game lobbies (Skirmish, LAN and CnCNet).
    /// Contains the common logic for parsing game options and handling player info.
    /// </summary>
    public abstract class GameLobbyBase : DXWindow
    {
        protected const int PLAYER_COUNT = 8;
        protected const int PLAYER_OPTION_VERTICAL_MARGIN = 12;
        protected const int PLAYER_OPTION_HORIZONTAL_MARGIN = 3;
        protected const int PLAYER_OPTION_CAPTION_Y = 6;
        const int DROP_DOWN_HEIGHT = 21;

        /// <summary>
        /// Creates a new instance of the game lobby base.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="iniName">The name of the lobby in GameOptions.ini.</param>
        public GameLobbyBase(WindowManager windowManager, string iniName, List<GameMode> GameModes) : base(windowManager)
        {
            _iniSectionName = iniName;
            this.GameModes = GameModes;
        }

        private string _iniSectionName;

        protected DXPanel PlayerOptionsPanel;

        protected DXPanel GameOptionsPanel;

        protected List<MultiplayerColor> MPColors = new List<MultiplayerColor>();

        protected List<GameLobbyCheckBox> CheckBoxes = new List<GameLobbyCheckBox>();
        protected List<GameLobbyDropDown> DropDowns = new List<GameLobbyDropDown>();

        /// <summary>
        /// The list of multiplayer game modes.
        /// </summary>
        protected List<GameMode> GameModes;

        /// <summary>
        /// The currently selected game mode.
        /// </summary>
        protected GameMode GameMode { get; set; }

        /// <summary>
        /// The currently selected map.
        /// </summary>
        protected Map Map { get; set; }

        protected DXDropDown[] ddPlayerNames;
        protected DXDropDown[] ddPlayerSides;
        protected DXDropDown[] ddPlayerColors;
        protected DXDropDown[] ddPlayerStarts;
        protected DXDropDown[] ddPlayerTeams;

        protected DXLabel lblName;
        protected DXLabel lblSide;
        protected DXLabel lblColor;
        protected DXLabel lblStart;
        protected DXLabel lblTeam;

        protected DXButton btnLeaveGame;
        protected DXButton btnLaunchGame;
        protected DXLabel lblMapName;
        protected DXLabel lblMapAuthor;
        protected DXLabel lblGameMode;

        protected MapPreviewBox MapPreviewBox;

        protected List<PlayerInfo> Players = new List<PlayerInfo>();
        protected List<PlayerInfo> AIPlayers = new List<PlayerInfo>();

        protected bool PlayerUpdatingInProgress { get; set; }

        protected bool GameInProgress { get; set; }

        /// <summary>
        /// The seed used for randomizing player options.
        /// </summary>
        protected int RandomSeed { get; set; }

        private int _sideCount;

        private MatchStatistics matchStatistics;

        IniFile _gameOptionsIni;
        protected IniFile GameOptionsIni
        {
            get { return _gameOptionsIni; }
        }

        public override void Initialize()
        {
            Name = _iniSectionName;
            //if (WindowManager.RenderResolutionY < 800)
            //    ClientRectangle = new Rectangle(0, 0, WindowManager.RenderResolutionX, WindowManager.RenderResolutionY);
            //else
                ClientRectangle = new Rectangle(0, 0, WindowManager.RenderResolutionX - 60, WindowManager.RenderResolutionY - 32);
            WindowManager.CenterControlOnScreen(this);
            BackgroundTexture = AssetLoader.LoadTexture("gamelobbybg.png");

            _gameOptionsIni = new IniFile(ProgramConstants.GetBaseResourcePath() + "GameOptions.ini");

            GameOptionsPanel = new DXPanel(WindowManager);
            GameOptionsPanel.Name = "GameOptionsPanel";
            GameOptionsPanel.BackgroundTexture = AssetLoader.LoadTexture("gamelobbyoptionspanelbg.png");
            GameOptionsPanel.ClientRectangle = new Rectangle(ClientRectangle.Width - 411, 12, 399, 289);
            GameOptionsPanel.BackgroundTexture = AssetLoader.CreateTexture(new Color(0, 0, 0, 192), 1, 1);
            GameOptionsPanel.DrawMode = PanelBackgroundImageDrawMode.STRETCHED;

            PlayerOptionsPanel = new DXPanel(WindowManager);
            PlayerOptionsPanel.Name = "PlayerOptionsPanel";
            PlayerOptionsPanel.BackgroundTexture = AssetLoader.LoadTexture("gamelobbypanelbg.png");
            PlayerOptionsPanel.ClientRectangle = new Rectangle(GameOptionsPanel.ClientRectangle.Left - 401, 12, 395, GameOptionsPanel.ClientRectangle.Height);
            PlayerOptionsPanel.LeftClick += PlayerOptionsPanel_LeftClick;
            PlayerOptionsPanel.BackgroundTexture = AssetLoader.CreateTexture(new Color(0, 0, 0, 192), 1, 1);
            PlayerOptionsPanel.DrawMode = PanelBackgroundImageDrawMode.STRETCHED;

            btnLeaveGame = new DXButton(WindowManager);
            btnLeaveGame.Name = "btnLeaveGame";
            btnLeaveGame.IdleTexture = AssetLoader.LoadTexture("133pxbtn.png");
            btnLeaveGame.HoverTexture = AssetLoader.LoadTexture("133pxbtn_c.png");
            btnLeaveGame.HoverSoundEffect = AssetLoader.LoadSound("button.wav");
            btnLeaveGame.ClientRectangle = new Rectangle(ClientRectangle.Width - 143, ClientRectangle.Height - 28, 133, 23);
            btnLeaveGame.FontIndex = 1;
            btnLeaveGame.Text = "Leave Game";
            btnLeaveGame.LeftClick += BtnLeaveGame_LeftClick;

            btnLaunchGame = new DXButton(WindowManager);
            btnLaunchGame.Name = "btnLaunchGame";
            btnLaunchGame.IdleTexture = AssetLoader.LoadTexture("133pxbtn.png");
            btnLaunchGame.HoverTexture = AssetLoader.LoadTexture("133pxbtn_c.png");
            btnLaunchGame.HoverSoundEffect = AssetLoader.LoadSound("button.wav");
            btnLaunchGame.ClientRectangle = new Rectangle(12, btnLeaveGame.ClientRectangle.Y, 133, 23);
            btnLaunchGame.FontIndex = 1;
            btnLaunchGame.Text = "Launch Game";
            btnLaunchGame.LeftClick += BtnLaunchGame_LeftClick;

            MapPreviewBox = new MapPreviewBox(WindowManager, Players, AIPlayers, MPColors, 
                _gameOptionsIni.GetStringValue("General", "Sides", String.Empty).Split(','),
                _gameOptionsIni);
            MapPreviewBox.Name = "MapPreviewBox";
            MapPreviewBox.ClientRectangle = new Rectangle(PlayerOptionsPanel.ClientRectangle.X,
                PlayerOptionsPanel.ClientRectangle.Bottom + 6,
                GameOptionsPanel.ClientRectangle.Right - PlayerOptionsPanel.ClientRectangle.Left,
                ClientRectangle.Height - PlayerOptionsPanel.ClientRectangle.Bottom - 65);
            MapPreviewBox.FontIndex = 1;
            MapPreviewBox.DrawMode = PanelBackgroundImageDrawMode.STRETCHED;
            MapPreviewBox.BackgroundTexture = AssetLoader.CreateTexture(new Color(0, 0, 0, 128), 1, 1);
            MapPreviewBox.StartingLocationApplied += MapPreviewBox_StartingLocationApplied;

            lblMapName = new DXLabel(WindowManager);
            lblMapName.Name = "lblMapName";
            lblMapName.ClientRectangle = new Rectangle(MapPreviewBox.ClientRectangle.X,
                MapPreviewBox.ClientRectangle.Bottom + 3, 0, 0);
            lblMapName.FontIndex = 1;
            lblMapName.Text = "Map:";

            lblMapAuthor = new DXLabel(WindowManager);
            lblMapAuthor.Name = "lblMapAuthor";
            lblMapAuthor.ClientRectangle = new Rectangle(MapPreviewBox.ClientRectangle.Right,
                lblMapName.ClientRectangle.Y, 0, 0);
            lblMapAuthor.FontIndex = 1;
            lblMapAuthor.Text = "By ";

            lblGameMode = new DXLabel(WindowManager);
            lblGameMode.Name = "lblGameMode";
            lblGameMode.ClientRectangle = new Rectangle(lblMapName.ClientRectangle.X,
                lblMapName.ClientRectangle.Bottom + 3, 0, 0);
            lblGameMode.FontIndex = 1;
            lblGameMode.Text = "Game mode:";

            AddChild(lblMapName);
            AddChild(lblMapAuthor);
            AddChild(lblGameMode);
            AddChild(MapPreviewBox);

            SharedUILogic.GameProcessExited += GameProcessExited;

            // Load multiplayer colors
            List<string> colorKeys = GameOptionsIni.GetSectionKeys("MPColors");

            foreach (string key in colorKeys)
            {
                string[] values = GameOptionsIni.GetStringValue("MPColors", key, "255,255,255,0").Split(',');

                try
                {
                    MultiplayerColor mpColor = MultiplayerColor.CreateFromStringArray(key, values);

                    MPColors.Add(mpColor);
                }
                catch
                {
                    throw new Exception("Invalid MPColor specified in GameOptions.ini: " + key);
                }
            }

            AddChild(GameOptionsPanel);

            string[] checkBoxes = GameOptionsIni.GetStringValue(_iniSectionName, "CheckBoxes", String.Empty).Split(',');

            foreach (string chkName in checkBoxes)
            {
                GameLobbyCheckBox chkBox = new GameLobbyCheckBox(WindowManager);
                chkBox.Name = chkName;
                chkBox.GetAttributes(GameOptionsIni);
                CheckBoxes.Add(chkBox);
                AddChild(chkBox);
            }

            string[] labels = GameOptionsIni.GetStringValue(_iniSectionName, "Labels", String.Empty).Split(',');

            foreach (string labelName in labels)
            {
                DXLabel label = new DXLabel(WindowManager);
                label.Name = labelName;
                label.GetAttributes(GameOptionsIni);
                AddChild(label);
            }

            string[] dropDowns = GameOptionsIni.GetStringValue(_iniSectionName, "DropDowns", String.Empty).Split(',');

            foreach (string ddName in dropDowns)
            {
                GameLobbyDropDown dropdown = new GameLobbyDropDown(WindowManager);
                dropdown.Name = ddName;
                dropdown.ClickSoundEffect = AssetLoader.LoadSound("dropdown.wav");
                dropdown.GetAttributes(GameOptionsIni);
                DropDowns.Add(dropdown);
                AddChild(dropdown);
            }

            AddChild(PlayerOptionsPanel);
            AddChild(btnLaunchGame);
            AddChild(btnLeaveGame);
        }

        /// <summary>
        /// Initializes the underlying window class.
        /// </summary>
        protected void InitializeWindow()
        {
            base.Initialize();
        }

        private void MapPreviewBox_StartingLocationApplied(object sender, EventArgs e)
        {
            CopyPlayerDataToUI();
        }

        private void PlayerOptionsPanel_LeftClick(object sender, EventArgs e)
        {
            Logger.Log("Clicked!");
        }

        /// <summary>
        /// Initializes the player option drop-down controls.
        /// </summary>
        protected void InitPlayerOptionDropdowns()
        {
            ddPlayerNames = new DXDropDown[PLAYER_COUNT];
            ddPlayerSides = new DXDropDown[PLAYER_COUNT];
            ddPlayerColors = new DXDropDown[PLAYER_COUNT];
            ddPlayerStarts = new DXDropDown[PLAYER_COUNT];
            ddPlayerTeams = new DXDropDown[PLAYER_COUNT];

            int playerOptionVecticalMargin = GameOptionsIni.GetIntValue(Name, "PlayerOptionVerticalMargin", PLAYER_OPTION_VERTICAL_MARGIN);
            int playerOptionHorizontalMargin = GameOptionsIni.GetIntValue(Name, "PlayerOptionHorizontalMargin", PLAYER_OPTION_HORIZONTAL_MARGIN);
            int playerOptionCaptionLocationY = GameOptionsIni.GetIntValue(Name, "PlayerOptionCaptionLocationY", PLAYER_OPTION_CAPTION_Y);
            int playerNameWidth = GameOptionsIni.GetIntValue(Name, "PlayerNameWidth", 136);
            int sideWidth = GameOptionsIni.GetIntValue(Name, "SideWidth", 91);
            int colorWidth = GameOptionsIni.GetIntValue(Name, "ColorWidth", 79);
            int startWidth = GameOptionsIni.GetIntValue(Name, "StartWidth", 49);
            int teamWidth = GameOptionsIni.GetIntValue(Name, "TeamWidth", 46);
            int locationX = GameOptionsIni.GetIntValue(Name, "PlayerOptionLocationX", 25);
            int locationY = GameOptionsIni.GetIntValue(Name, "PlayerOptionLocationY", 24);

            // InitPlayerOptionDropdowns(136, 91, 79, 49, 46, new Point(25, 24));

            string[] sides = GameOptionsIni.GetStringValue("General", "Sides", String.Empty).Split(',');
            _sideCount = sides.Length;

            for (int i = PLAYER_COUNT - 1; i > -1; i--)
            {
                DXDropDown ddPlayerName = new DXDropDown(WindowManager);
                ddPlayerName.Name = "ddPlayerName" + i;
                ddPlayerName.ClientRectangle = new Rectangle(locationX,
                    locationY + (DROP_DOWN_HEIGHT + playerOptionVecticalMargin) * i,
                    playerNameWidth, DROP_DOWN_HEIGHT);
                ddPlayerName.AddItem(String.Empty);
                ddPlayerName.AddItem("Easy AI");
                ddPlayerName.AddItem("Medium AI");
                ddPlayerName.AddItem("Hard AI");
                ddPlayerName.AllowDropDown = AllowPlayerDropdown();
                ddPlayerName.ClickSoundEffect = AssetLoader.LoadSound("dropdown.wav");
                ddPlayerName.SelectedIndexChanged += CopyPlayerDataFromUI;

                DXDropDown ddPlayerSide = new DXDropDown(WindowManager);
                ddPlayerSide.Name = "ddPlayerSide" + i;
                ddPlayerSide.ClientRectangle = new Rectangle(
                    ddPlayerName.ClientRectangle.Right + playerOptionHorizontalMargin,
                    ddPlayerName.ClientRectangle.Y, sideWidth, DROP_DOWN_HEIGHT);
                ddPlayerSide.AddItem("Random", AssetLoader.LoadTexture("randomicon.png"));
                foreach (string sideName in sides)
                    ddPlayerSide.AddItem(sideName, AssetLoader.LoadTexture(sideName + "icon.png"));
                ddPlayerSide.AllowDropDown = false;
                ddPlayerSide.ClickSoundEffect = AssetLoader.LoadSound("dropdown.wav");
                ddPlayerSide.SelectedIndexChanged += CopyPlayerDataFromUI;

                DXDropDown ddPlayerColor = new DXDropDown(WindowManager);
                ddPlayerColor.Name = "ddPlayerColor" + i;
                ddPlayerColor.ClientRectangle = new Rectangle(
                    ddPlayerSide.ClientRectangle.Right + playerOptionHorizontalMargin,
                    ddPlayerName.ClientRectangle.Y, colorWidth, DROP_DOWN_HEIGHT);
                ddPlayerColor.AddItem("Random", Color.White);
                foreach (MultiplayerColor mpColor in MPColors)
                    ddPlayerColor.AddItem(mpColor.Name, mpColor.XnaColor);
                ddPlayerColor.AllowDropDown = false;
                ddPlayerColor.ClickSoundEffect = AssetLoader.LoadSound("dropdown.wav");
                ddPlayerColor.SelectedIndexChanged += CopyPlayerDataFromUI;

                DXDropDown ddPlayerStart = new DXDropDown(WindowManager);
                ddPlayerStart.Name = "ddPlayerStart" + i;
                ddPlayerStart.ClientRectangle = new Rectangle(
                    ddPlayerColor.ClientRectangle.Right + playerOptionHorizontalMargin,
                    ddPlayerName.ClientRectangle.Y, startWidth, DROP_DOWN_HEIGHT);
                for (int j = 1; j < 9; j++)
                    ddPlayerStart.AddItem(j.ToString());
                ddPlayerStart.AllowDropDown = false;
                ddPlayerStart.ClickSoundEffect = AssetLoader.LoadSound("dropdown.wav");
                ddPlayerStart.SelectedIndexChanged += CopyPlayerDataFromUI;
                ddPlayerStart.Visible = false;
                ddPlayerStart.Enabled = false;

                DXDropDown ddPlayerTeam = new DXDropDown(WindowManager);
                ddPlayerTeam.Name = "ddPlayerTeam" + i;
                ddPlayerTeam.ClientRectangle = new Rectangle(
                    ddPlayerColor.ClientRectangle.Right + playerOptionHorizontalMargin,
                    ddPlayerName.ClientRectangle.Y, teamWidth, DROP_DOWN_HEIGHT);
                ddPlayerTeam.AddItem("-");
                ddPlayerTeam.AddItem("A");
                ddPlayerTeam.AddItem("B");
                ddPlayerTeam.AddItem("C");
                ddPlayerTeam.AddItem("D");
                ddPlayerTeam.AllowDropDown = false;
                ddPlayerTeam.ClickSoundEffect = AssetLoader.LoadSound("dropdown.wav");
                ddPlayerTeam.SelectedIndexChanged += CopyPlayerDataFromUI;

                ddPlayerNames[i] = ddPlayerName;
                ddPlayerSides[i] = ddPlayerSide;
                ddPlayerColors[i] = ddPlayerColor;
                ddPlayerStarts[i] = ddPlayerStart;
                ddPlayerTeams[i] = ddPlayerTeam;

                PlayerOptionsPanel.AddChild(ddPlayerName);
                PlayerOptionsPanel.AddChild(ddPlayerSide);
                PlayerOptionsPanel.AddChild(ddPlayerColor);
                PlayerOptionsPanel.AddChild(ddPlayerStart);
                PlayerOptionsPanel.AddChild(ddPlayerTeam);
            }

            lblName = new DXLabel(WindowManager);
            lblName.Name = "lblName";
            lblName.Text = "PLAYER";
            lblName.FontIndex = 1;
            lblName.ClientRectangle = new Rectangle(ddPlayerNames[0].ClientRectangle.X, playerOptionCaptionLocationY, 0, 0);

            lblSide = new DXLabel(WindowManager);
            lblSide.Name = "lblSide";
            lblSide.Text = "SIDE";
            lblSide.FontIndex = 1;
            lblSide.ClientRectangle = new Rectangle(ddPlayerSides[0].ClientRectangle.X, playerOptionCaptionLocationY, 0, 0);

            lblColor = new DXLabel(WindowManager);
            lblColor.Name = "lblColor";
            lblColor.Text = "COLOR";
            lblColor.FontIndex = 1;
            lblColor.ClientRectangle = new Rectangle(ddPlayerColors[0].ClientRectangle.X, playerOptionCaptionLocationY, 0, 0);

            lblStart = new DXLabel(WindowManager);
            lblStart.Name = "lblStart";
            lblStart.Text = "START";
            lblStart.FontIndex = 1;
            lblStart.ClientRectangle = new Rectangle(ddPlayerStarts[0].ClientRectangle.X, playerOptionCaptionLocationY, 0, 0);
            lblStart.Visible = false;

            lblTeam = new DXLabel(WindowManager);
            lblTeam.Name = "lblTeam";
            lblTeam.Text = "TEAM";
            lblTeam.FontIndex = 1;
            lblTeam.ClientRectangle = new Rectangle(ddPlayerTeams[0].ClientRectangle.X, playerOptionCaptionLocationY, 0, 0);

            PlayerOptionsPanel.AddChild(lblName);
            PlayerOptionsPanel.AddChild(lblSide);
            PlayerOptionsPanel.AddChild(lblColor);
            PlayerOptionsPanel.AddChild(lblStart);
            PlayerOptionsPanel.AddChild(lblTeam);
        }

        protected abstract void BtnLaunchGame_LeftClick(object sender, EventArgs e);

        protected abstract void BtnLeaveGame_LeftClick(object sender, EventArgs e);

        protected abstract bool AllowPlayerDropdown();


        /// <summary>
        /// Randomizes options of both human and AI players
        /// and returns the options as an array of PlayerHouseInfos.
        /// </summary>
        /// <returns>An array of PlayerHouseInfos.</returns>
        protected virtual PlayerHouseInfo[] Randomize()
        {
            int totalPlayerCount = Players.Count + AIPlayers.Count;
            PlayerHouseInfo[] houseInfos = new PlayerHouseInfo[totalPlayerCount];

            for (int i = 0; i < totalPlayerCount; i++)
                houseInfos[i] = new PlayerHouseInfo();

            // Gather list of spectators
            for (int i = 0; i < Players.Count; i++)
            {
                houseInfos[i].IsSpectator = Players[i].SideId == _sideCount + 1;
            }

            // Gather list of available colors

            List<int> freeColors = new List<int>();

            for (int cId = 0; cId < MPColors.Count; cId++)
                freeColors.Add(cId);

            if (Map.CoopInfo != null)
            {
                foreach (int colorIndex in Map.CoopInfo.DisallowedPlayerColors)
                    freeColors.RemoveAt(colorIndex);
            }

            foreach (PlayerInfo player in Players)
                freeColors.Remove(player.ColorId - 1); // The first color is Random

            foreach (PlayerInfo aiPlayer in AIPlayers)
                freeColors.Remove(aiPlayer.ColorId - 1);

            // Gather list of available starting locations

            List<int> freeStartingLocations = new List<int>();

            for (int i = 0; i < Map.MaxPlayers; i++)
                freeStartingLocations.Add(i);

            for (int i = 0; i < Players.Count; i++)
            {
                if (!houseInfos[i].IsSpectator)
                    freeStartingLocations.Remove(Players[i].StartingLocation - 1);
            }

            // Randomize options

            Random random = new Random(RandomSeed);

            for (int i = 0; i < totalPlayerCount; i++)
            {
                PlayerInfo pInfo;
                PlayerHouseInfo pHouseInfo = houseInfos[i];

                if (i < Players.Count)
                {
                    pInfo = Players[i];
                }
                else
                    pInfo = AIPlayers[i - Players.Count];

                pHouseInfo.RandomizeSide(pInfo, Map, _sideCount, random);
                pHouseInfo.RandomizeColor(pInfo, freeColors, MPColors, random);
                pHouseInfo.RandomizeStart(pInfo, Map, freeStartingLocations, random);
            }

            return houseInfos;
        }

        /// <summary>
        /// Writes spawn.ini.
        /// </summary>
        protected virtual void WriteSpawnIni()
        {
            Logger.Log("Writing spawn.ini");

            File.Delete(ProgramConstants.GamePath + ProgramConstants.SPAWNER_SETTINGS);

            if (Map.IsCoop)
            {
                foreach (PlayerInfo pInfo in Players)
                    pInfo.TeamId = 1;

                foreach (PlayerInfo pInfo in AIPlayers)
                    pInfo.TeamId = 1;
            }

            PlayerHouseInfo[] houseInfos = Randomize();

            IniFile spawnIni = new IniFile(ProgramConstants.GamePath + ProgramConstants.SPAWNER_SETTINGS);

            spawnIni.SetStringValue("Settings", "Name", ProgramConstants.PLAYERNAME);
            spawnIni.SetStringValue("Settings", "Scenario", ProgramConstants.SPAWNMAP_INI);
            spawnIni.SetStringValue("Settings", "UIGameMode", GameMode.UIName);
            spawnIni.SetStringValue("Settings", "UIMapName", Map.Name);
            spawnIni.SetIntValue("Settings", "PlayerCount", Players.Count);
            int myIndex = Players.FindIndex(c => c.Name == ProgramConstants.PLAYERNAME);
            spawnIni.SetIntValue("Settings", "Side", houseInfos[myIndex].SideIndex);
            spawnIni.SetBooleanValue("Settings", "IsSpectator", houseInfos[myIndex].IsSpectator);
            spawnIni.SetIntValue("Settings", "Color", houseInfos[myIndex].ColorIndex);
            spawnIni.SetStringValue("Settings", "CustomLoadScreen", LoadingScreenController.GetLoadScreenName(houseInfos[myIndex].SideIndex));
            spawnIni.SetIntValue("Settings", "AIPlayers", AIPlayers.Count);
            spawnIni.SetIntValue("Settings", "Seed", RandomSeed);
            WriteSpawnIniAdditions(spawnIni);

            foreach (GameLobbyCheckBox chkBox in CheckBoxes)
            {
                chkBox.ApplySpawnINICode(spawnIni);
            }

            foreach (GameLobbyDropDown dd in DropDowns)
            {
                dd.ApplySpawnIniCode(spawnIni);
            }

            // Apply forced options from GameOptions.ini

            List<string> forcedKeys = GameOptionsIni.GetSectionKeys("ForcedSpawnIniOptions");

            if (forcedKeys != null)
            {
                foreach (string key in forcedKeys)
                {
                    spawnIni.SetStringValue("Settings", key,
                        GameOptionsIni.GetStringValue("ForcedSpawnIniOptions", key, String.Empty));
                }
            }

            GameMode.ApplySpawnIniCode(spawnIni); // Forced options from the game mode
            Map.ApplySpawnIniCode(spawnIni, Players.Count + AIPlayers.Count, 
                AIPlayers.Count, GameMode.CoopDifficultyLevel); // Forced options from the map

            // Player options

            int otherId = 1;

            for (int pId = 0; pId < Players.Count; pId++)
            {
                PlayerInfo pInfo = Players[pId];
                PlayerHouseInfo pHouseInfo = houseInfos[pId];

                if (pInfo.Name == ProgramConstants.PLAYERNAME)
                    continue;

                string sectionName = "Other" + otherId;

                spawnIni.SetStringValue(sectionName, "Name", pInfo.Name);
                spawnIni.SetIntValue(sectionName, "Side", pHouseInfo.SideIndex);
                spawnIni.SetBooleanValue(sectionName, "IsSpectator", pHouseInfo.IsSpectator);
                spawnIni.SetIntValue(sectionName, "Color", pHouseInfo.ColorIndex);
                spawnIni.SetStringValue(sectionName, "Ip", GetIPAddressForPlayer(pInfo));
                spawnIni.SetIntValue(sectionName, "Port", pInfo.Port);

                otherId++;
            }

            List<int> multiCmbIndexes = new List<int>();

            for (int cId = 0; cId < MPColors.Count; cId++)
            {
                for (int pId = 0; pId < Players.Count; pId++)
                {
                    if (houseInfos[pId].ColorIndex == MPColors[cId].GameColorIndex)
                        multiCmbIndexes.Add(pId);
                }
            }

            if (AIPlayers.Count > 0)
            {
                for (int aiId = 0; aiId < AIPlayers.Count; aiId++)
                {
                    int multiId = multiCmbIndexes.Count + aiId + 1;

                    string keyName = "Multi" + multiId;

                    spawnIni.SetIntValue("HouseHandicaps", keyName, AIPlayers[aiId].AILevel);
                    spawnIni.SetIntValue("HouseCountries", keyName, houseInfos[Players.Count + aiId].SideIndex);
                    spawnIni.SetIntValue("HouseColors", keyName, houseInfos[Players.Count + aiId].ColorIndex);
                }
            }

            for (int multiId = 0; multiId < multiCmbIndexes.Count; multiId++)
            {
                int pIndex = multiCmbIndexes[multiId];
                if (houseInfos[pIndex].IsSpectator)
                    spawnIni.SetBooleanValue("IsSpectator", "Multi" + (multiId + 1), true);
            }

            // Write alliances, the code is pretty big so let's take it to another class
            AllianceHolder.WriteInfoToSpawnIni(Players, AIPlayers, multiCmbIndexes, spawnIni);

            for (int pId = 0; pId < Players.Count; pId++)
            {
                int multiIndex = pId + 1;
                spawnIni.SetIntValue("SpawnLocations", "Multi" + multiIndex,
                    houseInfos[multiCmbIndexes[pId]].StartingWaypoint);
            }

            for (int aiId = 0; aiId < AIPlayers.Count; aiId++)
            {
                int multiIndex = Players.Count + aiId + 1;
                spawnIni.SetIntValue("SpawnLocations", "Multi" + multiIndex,
                    houseInfos[Players.Count + aiId].StartingWaypoint);
            }

            spawnIni.WriteIniFile();

            InitializeMatchStatistics(houseInfos);
        }

        protected virtual string GetIPAddressForPlayer(PlayerInfo player)
        {
            return "0.0.0.0";
        }

        /// <summary>
        /// Override this in a derived class to write game lobby specific code to
        /// spawn.ini. For example, CnCNet game lobbies should write tunnel info
        /// in this method.
        /// </summary>
        /// <param name="iniFile">The spawn INI file.</param>
        protected virtual void WriteSpawnIniAdditions(IniFile iniFile)
        {
            // Do nothing by default
        }

        protected virtual void InitializeMatchStatistics(PlayerHouseInfo[] houseInfos)
        {
            matchStatistics = new MatchStatistics(ProgramConstants.GAME_VERSION, Map.Name, GameMode.UIName, Players.Count);

            for (int pId = 0; pId < Players.Count; pId++)
            {
                PlayerInfo pInfo = Players[pId];
                matchStatistics.AddPlayer(pInfo.Name, pInfo.Name == ProgramConstants.PLAYERNAME,
                    false, pInfo.SideId == _sideCount + 1, houseInfos[pId].SideIndex + 1, pInfo.TeamId, 10);
            }

            for (int aiId = 0; aiId < AIPlayers.Count; aiId++)
            {
                PlayerInfo aiInfo = AIPlayers[aiId];
                matchStatistics.AddPlayer("Computer", false, true, false, 
                    houseInfos[Players.Count + aiId].SideIndex + 1, aiInfo.TeamId, aiInfo.ReversedAILevel);
            }
        }

        /// <summary>
        /// Writes spawnmap.ini.
        /// </summary>
        protected virtual void WriteMap()
        {
            File.Delete(ProgramConstants.GamePath + ProgramConstants.SPAWNMAP_INI);

            Logger.Log("Writing map.");

            IniFile mapIni = new IniFile(ProgramConstants.GamePath + Map.BaseFilePath + ".map");

            IniFile globalCodeIni = new IniFile(ProgramConstants.GamePath + "INI\\Map Code\\GlobalCode.ini");

            IniFile.ConsolidateIniFiles(mapIni, GameMode.GetMapRulesIniFile());
            IniFile.ConsolidateIniFiles(mapIni, globalCodeIni);

            foreach (GameLobbyCheckBox checkBox in CheckBoxes)
                checkBox.ApplyMapCode(mapIni);

            mapIni.MoveSectionToFirst("MultiplayerDialogSettings"); // Required by YR

            mapIni.WriteIniFile(ProgramConstants.GamePath + ProgramConstants.SPAWNMAP_INI);
        }

        /// <summary>
        /// Writes spawn.ini, writes the map file, initializes statistics and
        /// starts the game process.
        /// </summary>
        protected virtual void StartGame()
        {
            WriteSpawnIni();
            WriteMap();

            GameInProgress = true;

            SharedUILogic.StartGameProcess(0);
        }

        protected virtual void GameProcessExited()
        {
            if (!GameInProgress)
                return;

            GameInProgress = false;

            Logger.Log("GameProcessExited: Parsing statistics.");

            matchStatistics.ParseStatistics(ProgramConstants.GamePath, DomainController.Instance().GetDefaultGame());

            Logger.Log("GameProcessExited: Adding match to statistics.");

            StatisticsManager.Instance.AddMatchAndSaveDatabase(true, matchStatistics);
        }

        /// <summary>
        /// "Copies" player information from the UI to internal memory,
        /// applying users' player options changes.
        /// </summary>
        protected virtual void CopyPlayerDataFromUI(object sender, EventArgs e)
        {
            if (PlayerUpdatingInProgress)
                return;

            for (int pId = 0; pId < Players.Count; pId++)
            {
                PlayerInfo pInfo = Players[pId];

                pInfo.ColorId = ddPlayerColors[pId].SelectedIndex;
                pInfo.SideId = ddPlayerSides[pId].SelectedIndex;
                pInfo.StartingLocation = ddPlayerStarts[pId].SelectedIndex;
                pInfo.TeamId = ddPlayerTeams[pId].SelectedIndex;

                if (pInfo.SideId == _sideCount + 1)
                    pInfo.StartingLocation = 0;

                DXDropDown ddName = ddPlayerNames[pId];
                if (ddName.SelectedIndex == 1)
                    ddName.SelectedIndex = 0;
                else if (ddName.SelectedIndex == 2)
                    KickPlayer(pId);
                else
                    BanPlayer(pId);
            }

            AIPlayers.Clear();
            for (int cmbId = Players.Count; cmbId < 8; cmbId++)
            {
                DXDropDown dd = ddPlayerNames[cmbId];
                dd.Items[0].Text = "-";

                if (dd.SelectedIndex < 1)
                    continue;

                PlayerInfo aiPlayer = new PlayerInfo();
                aiPlayer.Name = dd.Items[dd.SelectedIndex].Text;
                aiPlayer.AILevel = 2 - (dd.SelectedIndex - 1);
                aiPlayer.SideId = Math.Max(ddPlayerSides[cmbId].SelectedIndex, 0);
                aiPlayer.ColorId = Math.Max(ddPlayerColors[cmbId].SelectedIndex, 0);
                aiPlayer.StartingLocation = Math.Max(ddPlayerStarts[cmbId].SelectedIndex, 0);
                aiPlayer.TeamId = Math.Max(ddPlayerTeams[cmbId].SelectedIndex, 0);

                AIPlayers.Add(aiPlayer);
            }

            CopyPlayerDataToUI();
        }

        /// <summary>
        /// Applies player information changes done in memory to the UI.
        /// </summary>
        protected virtual void CopyPlayerDataToUI()
        {
            PlayerUpdatingInProgress = true;

            // Human players
            for (int pId = 0; pId < Players.Count; pId++)
            {
                PlayerInfo pInfo = Players[pId];

                pInfo.Index = pId;

                DXDropDown ddPlayerName = ddPlayerNames[pId];
                ddPlayerName.Items[0].Text = pInfo.Name;
                ddPlayerName.Items[1].Text = string.Empty;
                ddPlayerName.Items[2].Text = "Kick";
                ddPlayerName.Items[3].Text = "Ban";
                ddPlayerName.SelectedIndex = 0;
                ddPlayerName.AllowDropDown = false;

                ddPlayerSides[pId].SelectedIndex = pInfo.SideId;
                ddPlayerSides[pId].AllowDropDown = true;

                ddPlayerColors[pId].SelectedIndex = pInfo.ColorId;
                ddPlayerColors[pId].AllowDropDown = true;

                ddPlayerStarts[pId].SelectedIndex = pInfo.StartingLocation;
                ddPlayerStarts[pId].AllowDropDown = true;

                ddPlayerTeams[pId].SelectedIndex = pInfo.TeamId;
                ddPlayerTeams[pId].AllowDropDown = true;
            }

            // AI players
            for (int aiId = 0; aiId < AIPlayers.Count; aiId++)
            {
                PlayerInfo aiInfo = AIPlayers[aiId];

                int index = Players.Count + aiId;

                aiInfo.Index = index;

                DXDropDown ddPlayerName = ddPlayerNames[index];
                ddPlayerName.Items[0].Text = "-";
                ddPlayerName.Items[1].Text = "Easy AI";
                ddPlayerName.Items[2].Text = "Medium AI";
                ddPlayerName.Items[3].Text = "Hard AI";
                ddPlayerName.SelectedIndex = 3 - aiInfo.AILevel;
                ddPlayerName.AllowDropDown = true;

                ddPlayerSides[index].SelectedIndex = aiInfo.SideId;
                ddPlayerSides[index].AllowDropDown = true;

                ddPlayerColors[index].SelectedIndex = aiInfo.ColorId;
                ddPlayerColors[index].AllowDropDown = true;

                ddPlayerStarts[index].SelectedIndex = aiInfo.StartingLocation;
                ddPlayerStarts[index].AllowDropDown = true;

                ddPlayerTeams[index].SelectedIndex = aiInfo.TeamId;
                ddPlayerTeams[index].AllowDropDown = true;
            }

            // Unused player slots
            for (int ddIndex = Players.Count + AIPlayers.Count; ddIndex < PLAYER_COUNT; ddIndex++)
            {
                DXDropDown ddPlayerName = ddPlayerNames[ddIndex];
                ddPlayerName.AllowDropDown = false;
                ddPlayerName.Items[0].Text = string.Empty;
                ddPlayerName.Items[1].Text = "Easy AI";
                ddPlayerName.Items[2].Text = "Medium AI";
                ddPlayerName.Items[3].Text = "Hard AI";
                ddPlayerName.SelectedIndex = -1;

                ddPlayerSides[ddIndex].SelectedIndex = -1;
                ddPlayerSides[ddIndex].AllowDropDown = false;

                ddPlayerColors[ddIndex].SelectedIndex = -1;
                ddPlayerColors[ddIndex].AllowDropDown = false;

                ddPlayerStarts[ddIndex].SelectedIndex = -1;
                ddPlayerStarts[ddIndex].AllowDropDown = false;

                ddPlayerTeams[ddIndex].SelectedIndex = -1;
                ddPlayerTeams[ddIndex].AllowDropDown = false;
            }

            if (Players.Count + AIPlayers.Count < PLAYER_COUNT)
                ddPlayerNames[Players.Count + AIPlayers.Count].AllowDropDown = true;

            MapPreviewBox.UpdateStartingLocationTexts();

            PlayerUpdatingInProgress = false;
        }

        /// <summary>
        /// Override this in a derived class to kick players.
        /// </summary>
        /// <param name="playerIndex">The index of the player that should be kicked.</param>
        protected virtual void KickPlayer(int playerIndex)
        {
            // Do nothing by default
        }

        /// <summary>
        /// Override this in a derived class to ban players.
        /// </summary>
        /// <param name="playerIndex">The index of the player that should be banned.</param>
        protected virtual void BanPlayer(int playerIndex)
        {
            // Do nothing by default
        }

        /// <summary>
        /// Changes the current map and game mode.
        /// </summary>
        /// <param name="gameMode">The new game mode.</param>
        /// <param name="map">The new map.</param>
        protected virtual void ChangeMap(GameMode gameMode, Map map)
        {
            if (GameMode == null || !object.ReferenceEquals(gameMode, GameMode))
            {
                // TODO: Load the new game mode's default settings
            }

            GameMode = gameMode;

            Map = map;

            lblMapName.Text = "Map: " + map.Name;
            lblMapAuthor.Text = "By " + map.Author;
            lblGameMode.Text = "Game mode: " + gameMode.UIName;

            lblMapAuthor.ClientRectangle = new Rectangle(MapPreviewBox.ClientRectangle.Right - lblMapAuthor.ClientRectangle.Width,
                lblMapAuthor.ClientRectangle.Y, lblMapAuthor.ClientRectangle.Width, lblMapAuthor.ClientRectangle.Height);

            // Clear forced options
            foreach (DXDropDown ddGameOption in DropDowns)
                ddGameOption.AllowDropDown = true;

            foreach (DXCheckBox checkBox in CheckBoxes)
                checkBox.AllowChecking = true;

            // We could either pass the CheckBoxes and DropDowns of this class
            // to the Map and GameMode instances and let them apply their forced
            // options, or we could do it in this class with helper functions.
            // I think the second approach is clearer.

            ApplyForcedCheckBoxOptions(gameMode.ForcedCheckBoxValues);
            ApplyForcedCheckBoxOptions(map.ForcedCheckBoxValues);

            ApplyForcedDropDownOptions(gameMode.ForcedDropDownValues);
            ApplyForcedDropDownOptions(map.ForcedDropDownValues);

            // Enable all sides by default
            foreach (DXDropDown ddSide in ddPlayerSides)
            {
                foreach (DXDropDownItem item in ddSide.Items)
                    item.Selectable = true;
            }

            // Enable all colors by default
            foreach (DXDropDown ddColor in ddPlayerColors)
            {
                foreach (DXDropDownItem item in ddColor.Items)
                    item.Selectable = true;
            }

            // Apply starting locations
            foreach (DXDropDown ddStart in ddPlayerStarts)
            {
                ddStart.Items.Clear();

                ddStart.AddItem("???");

                for (int i = 1; i <= Map.MaxPlayers; i++)
                    ddStart.AddItem(i.ToString());
            }

            foreach (PlayerInfo pInfo in Players)
            {
                if (pInfo.StartingLocation > Map.MaxPlayers)
                    pInfo.StartingLocation = 0;
            }

            foreach (PlayerInfo aiInfo in AIPlayers)
            {
                if (aiInfo.StartingLocation > Map.MaxPlayers)
                    aiInfo.StartingLocation = 0;
            }

            if (map.CoopInfo != null)
            {
                // Co-Op map disallowed side logic

                List<int> disallowedSides = map.CoopInfo.DisallowedPlayerSides;

                bool disallowRandom = _sideCount == disallowedSides.Count + 1; // Disallow Random if only 1 side is allowed
                int defaultSideIndex = 0; // The side to switch to if we're currently using a disallowed side. 0 = random

                if (disallowRandom)
                {
                    for (int sideIndex = 0; sideIndex < _sideCount; sideIndex++)
                    {
                        if (!disallowedSides.Contains(sideIndex))
                        {
                            defaultSideIndex = sideIndex + 1;
                            break;
                        }
                    }

                    foreach (DXDropDown dd in ddPlayerSides)
                        dd.Items[0].Selectable = false;

                    foreach (PlayerInfo pInfo in Players)
                    {
                        if (pInfo.SideId == 0)
                            pInfo.SideId = defaultSideIndex;
                    }

                    foreach (PlayerInfo aiInfo in AIPlayers)
                    {
                        if (aiInfo.SideId == 0)
                            aiInfo.SideId = defaultSideIndex;
                    }
                }

                foreach (int disallowedSideIndex in disallowedSides)
                {
                    if (disallowedSideIndex >= _sideCount)
                        continue; // Let's not crash the client

                    foreach (DXDropDown ddSide in ddPlayerSides)
                    {
                        ddSide.Items[disallowedSideIndex + 1].Selectable = false;
                    }

                    foreach (PlayerInfo pInfo in Players)
                    {
                        if (pInfo.SideId == disallowedSideIndex + 1)
                            pInfo.SideId = defaultSideIndex;
                    }

                    foreach (PlayerInfo aiInfo in AIPlayers)
                    {
                        if (aiInfo.SideId == disallowedSideIndex + 1)
                            aiInfo.SideId = defaultSideIndex;
                    }
                }

                // Co-Op map disallowed color logic
                foreach (int disallowedColorIndex in map.CoopInfo.DisallowedPlayerColors)
                {
                    if (disallowedColorIndex >= MPColors.Count)
                        continue;

                    foreach (DXDropDown ddColor in ddPlayerColors)
                        ddColor.Items[disallowedColorIndex + 1].Selectable = false;

                    foreach (PlayerInfo pInfo in Players)
                    {
                        if (pInfo.ColorId == disallowedColorIndex + 1)
                            pInfo.ColorId = 0;
                    }

                    foreach (PlayerInfo aiInfo in AIPlayers)
                    {
                        if (aiInfo.ColorId == disallowedColorIndex + 1)
                            aiInfo.ColorId = 0;
                    }
                }
            }

            CopyPlayerDataToUI();

            MapPreviewBox.Map = map;
        }

        protected void ApplyForcedCheckBoxOptions(List<KeyValuePair<string, bool>> forcedOptions)
        {
            foreach (KeyValuePair<string, bool> option in forcedOptions)
            {
                GameLobbyCheckBox checkBox = CheckBoxes.Find(chk => chk.Name == option.Key);
                if (checkBox != null)
                {
                    checkBox.Checked = option.Value;
                    checkBox.AllowChecking = false;
                }
            }
        }

        protected void ApplyForcedDropDownOptions(List<KeyValuePair<string, int>> forcedOptions)
        {
            foreach (KeyValuePair<string, int> option in forcedOptions)
            {
                GameLobbyDropDown dropDown = DropDowns.Find(dd => dd.Name == option.Key);
                if (dropDown != null)
                {
                    dropDown.SelectedIndex = option.Value;
                    dropDown.AllowDropDown = false;
                }
            }
        }
    }
}
