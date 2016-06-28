﻿using ClientGUI;
using Rampastring.XNAUI.XNAControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rampastring.XNAUI;
using Microsoft.Xna.Framework;
using ClientCore;

namespace DTAClient.DXGUI.Multiplayer
{
    public class CnCNetLoginWindow : XNAWindow
    {
        public CnCNetLoginWindow(WindowManager windowManager) : base(windowManager)
        {
        }

        XNALabel lblConnectToCnCNet;
        XNATextBox tbPlayerName;
        XNALabel lblPlayerName;
        XNACheckBox chkRememberMe;
        XNACheckBox chkPersistentMode;
        XNACheckBox chkAutoConnect;
        XNAButton btnConnect;
        XNAButton btnCancel;

        public event EventHandler Cancelled;
        public event EventHandler Connect;

        public override void Initialize()
        {
            Name = "CnCNetLoginWindow";
            ClientRectangle = new Rectangle(0, 0, 300, 220);
            BackgroundTexture = AssetLoader.LoadTextureUncached("logindialogbg.png");

            lblConnectToCnCNet = new XNALabel(WindowManager);
            lblConnectToCnCNet.Name = "lblConnectToCnCNet";
            lblConnectToCnCNet.FontIndex = 1;
            lblConnectToCnCNet.Text = "CONNECT TO CNCNET";

            AddChild(lblConnectToCnCNet);
            lblConnectToCnCNet.CenterOnParent();
            lblConnectToCnCNet.ClientRectangle = new Rectangle(
                lblConnectToCnCNet.ClientRectangle.X, 12,
                lblConnectToCnCNet.ClientRectangle.Width, 
                lblConnectToCnCNet.ClientRectangle.Height);

            tbPlayerName = new XNATextBox(WindowManager);
            tbPlayerName.Name = "tbPlayerName";
            tbPlayerName.ClientRectangle = new Rectangle(ClientRectangle.Width - 132, 50, 120, 19);
            tbPlayerName.MaximumTextLength = 16;
            string defgame = DomainController.Instance().GetDefaultGame();
            if (defgame == "YR" || defgame == "MO")
                tbPlayerName.MaximumTextLength = 12; // YR can't handle names longer than 12 chars

            tbPlayerName.Text = DomainController.Instance().GetMpHandle();

            lblPlayerName = new XNALabel(WindowManager);
            lblPlayerName.Name = "lblPlayerName";
            lblPlayerName.FontIndex = 1;
            lblPlayerName.Text = "PLAYER NAME:";
            lblPlayerName.ClientRectangle = new Rectangle(12, tbPlayerName.ClientRectangle.Y + 1,
                lblPlayerName.ClientRectangle.Width, lblPlayerName.ClientRectangle.Height);

            chkRememberMe = new XNACheckBox(WindowManager);
            chkRememberMe.Name = "chkRememberMe";
            chkRememberMe.ClientRectangle = new Rectangle(12, tbPlayerName.ClientRectangle.Bottom + 12, 0, 0);
            chkRememberMe.Text = "Remember me";
            chkRememberMe.TextPadding = 7;
            chkRememberMe.CheckSoundEffect = AssetLoader.LoadSound("checkbox.wav");
            chkRememberMe.CheckedChanged += ChkRememberMe_CheckedChanged;

            chkPersistentMode = new XNACheckBox(WindowManager);
            chkPersistentMode.Name = "chkPersistentMode";
            chkPersistentMode.ClientRectangle = new Rectangle(12, chkRememberMe.ClientRectangle.Bottom + 30, 0, 0);
            chkPersistentMode.Text = "Stay connected outside of the CnCNet lobby";
            chkPersistentMode.TextPadding = chkRememberMe.TextPadding;
            chkPersistentMode.CheckSoundEffect = AssetLoader.LoadSound("checkbox.wav");
            chkPersistentMode.CheckedChanged += ChkPersistentMode_CheckedChanged;

            chkAutoConnect = new XNACheckBox(WindowManager);
            chkAutoConnect.Name = "chkAutoConnect";
            chkAutoConnect.ClientRectangle = new Rectangle(12, chkPersistentMode.ClientRectangle.Bottom + 30, 0, 0);
            chkAutoConnect.Text = "Connect automatically on client startup";
            chkAutoConnect.TextPadding = chkRememberMe.TextPadding;
            chkAutoConnect.CheckSoundEffect = AssetLoader.LoadSound("checkbox.wav");
            chkAutoConnect.AllowChecking = false;

            btnConnect = new XNAButton(WindowManager);
            btnConnect.Name = "btnConnect";
            btnConnect.ClientRectangle = new Rectangle(12, ClientRectangle.Height - 35, 110, 23);
            btnConnect.IdleTexture = AssetLoader.LoadTexture("110pxbtn.png");
            btnConnect.HoverTexture = AssetLoader.LoadTexture("110pxbtn_c.png");
            btnConnect.HoverSoundEffect = AssetLoader.LoadSound("button.wav");
            btnConnect.FontIndex = 1;
            btnConnect.Text = "Connect";
            btnConnect.LeftClick += BtnConnect_LeftClick;

            btnCancel = new XNAButton(WindowManager);
            btnCancel.Name = "btnCancel";
            btnCancel.ClientRectangle = new Rectangle(ClientRectangle.Width - 122, btnConnect.ClientRectangle.Y, 110, 23);
            btnCancel.IdleTexture = AssetLoader.LoadTexture("110pxbtn.png");
            btnCancel.HoverTexture = AssetLoader.LoadTexture("110pxbtn_c.png");
            btnCancel.HoverSoundEffect = AssetLoader.LoadSound("button.wav");
            btnCancel.FontIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.LeftClick += BtnCancel_LeftClick;

            AddChild(tbPlayerName);
            AddChild(lblPlayerName);
            AddChild(chkRememberMe);
            AddChild(chkPersistentMode);
            AddChild(chkAutoConnect);
            AddChild(btnConnect);
            AddChild(btnCancel);

            base.Initialize();

            CenterOnParent();
        }

        private void BtnCancel_LeftClick(object sender, EventArgs e)
        {
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        private void ChkRememberMe_CheckedChanged(object sender, EventArgs e)
        {
            CheckAutoConnectAllowance();
        }

        private void ChkPersistentMode_CheckedChanged(object sender, EventArgs e)
        {
            CheckAutoConnectAllowance();
        }

        private void CheckAutoConnectAllowance()
        {
            chkAutoConnect.AllowChecking = chkPersistentMode.Checked && chkRememberMe.Checked;
            if (!chkAutoConnect.AllowChecking)
                chkAutoConnect.Checked = false;
        }

        private void BtnConnect_LeftClick(object sender, EventArgs e)
        {
            string errorMessage = IsNameValid();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                XNAMessageBox.Show(WindowManager, "Invalid Player Name", errorMessage);
                return;
            }

            ProgramConstants.PLAYERNAME = tbPlayerName.Text;

            DomainController.Instance().SaveCnCNetSettings(ProgramConstants.PLAYERNAME, chkRememberMe.Checked,
                chkPersistentMode.Checked, chkAutoConnect.Checked);
            Connect?.Invoke(this, EventArgs.Empty);
        }

        public void LoadSettings()
        {
            chkAutoConnect.Checked = DomainController.Instance().GetCnCNetAutologinStatus();
            chkPersistentMode.Checked = DomainController.Instance().GetCnCNetPersistentModeStatus();
            chkRememberMe.Checked = DomainController.Instance().GetCnCNetConnectDialogSkipStatus();

            if (chkRememberMe.Checked)
                Connect?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Checks if the player's nickname is valid for CnCNet.
        /// </summary>
        /// <returns>Null if the nickname is valid, otherwise a string that tells
        /// what is wrong with the name.</returns>
        private string IsNameValid()
        {
            if (string.IsNullOrEmpty(tbPlayerName.Text))
                return "Please enter a name.";

            int number = -1;
            if (Int32.TryParse(tbPlayerName.Text.Substring(0, 1), out number))
                return "The first character in your player name cannot be a number.";

            if (tbPlayerName.Text[0] == '-')
                return "The first character in your player name cannot be a dash ( - ).";

            // Check that there are no invalid chars
            char[] allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_[]|\\{}^`".ToCharArray();
            char[] nicknameChars = tbPlayerName.Text.ToCharArray();

            foreach (char nickChar in nicknameChars)
            {
                if (!allowedCharacters.Contains(nickChar))
                {
                    return "Your player name has invalid characters in it." + Environment.NewLine +
                    "Allowed characters are anything from A to Z and numbers.";
                }
            }

            return null;
        }
    }
}
