
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClientGUI
{
    public enum CompositionAttributes
    {
        /// <summary>
        /// Character being entered by the user.
        /// The IME has yet to convert this character.
        /// </summary>
        Input = 0x00,
        /// <summary>
        /// Character selected by the user and then converted by the IME.
        /// </summary>
        TargetConverted = 0x01,
        /// <summary>
        /// Character that the IME has already converted.
        /// </summary>
        Converted = 0x02,
        /// <summary>
        /// Character being converted. The user has selected this character
        /// but the IME has not yet converted it.
        /// </summary>
        TargetNotConverted = 0x03,
        /// <summary>
        /// An error character that the IME cannot convert. For example,
        /// the IME cannot put together some consonants.
        /// </summary>
        InputError = 0x04,
        /// <summary>
        /// Characters that the IME will no longer convert.
        /// </summary>
        FixedConverted = 0x05,
    }

    /// <summary>
    /// Special event arguemnt class stores new character that IME sends in.
    /// </summary>
    public class IMEResultEventArgs : EventArgs
    {

        internal IMEResultEventArgs(char result)
        {
            this.Result = result;
        }

        /// <summary>
        /// The result character
        /// </summary>
        public char Result { get; private set; }
    }

    /// <summary>
    /// Native window class that handles IME.
    /// </summary>
    public sealed class IMENativeWindow : NativeWindow, IDisposable
    {

        private IMMCompositionString
            _compstr, _compclause, _compattr,
            _compread, _compreadclause, _compreadattr,
            _resstr, _resclause,
            _resread, _resreadclause;
        private IMMCompositionInt _compcurpos;
        private bool _disposed;
        //private bool _showIMEWin;
        private IntPtr _context;

        /// <summary>
        /// Gets the state if the IME should be enabled
        /// </summary>
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// Composition String
        /// </summary>
        public string CompositionString { get { return _compstr.ToString(); } }

        /// <summary>
        /// Composition Clause
        /// </summary>
        public string CompositionClause { get { return _compclause.ToString(); } }

        /// <summary>
        /// Composition String Reads
        /// </summary>
        public string CompositionReadString { get { return _compread.ToString(); } }

        /// <summary>
        /// Composition Clause Reads
        /// </summary>
        public string CompositionReadClause { get { return _compreadclause.ToString(); } }

        /// <summary>
        /// Result String
        /// </summary>
        public string ResultString { get { return _resstr.ToString(); } }

        /// <summary>
        /// Result Clause
        /// </summary>
        public string ResultClause { get { return _resclause.ToString(); } }

        /// <summary>
        /// Result String Reads
        /// </summary>
        public string ResultReadString { get { return _resread.ToString(); } }

        /// <summary>
        /// Result Clause Reads
        /// </summary>
        public string ResultReadClause { get { return _resreadclause.ToString(); } }

        /// <summary>
        /// Caret position of the composition
        /// </summary>
        public int CompositionCursorPos { get { return _compcurpos.Value; } }

        /// <summary>
        /// Array of the candidates
        /// </summary>
        public string[] Candidates { get; private set; }

        /// <summary>
        /// First candidate index of current page
        /// </summary>
        public uint CandidatesPageStart { get; private set; }

        /// <summary>
        /// How many candidates should display per page
        /// </summary>
        public uint CandidatesPageSize { get; private set; }

        /// <summary>
        /// The selected canddiate index
        /// </summary>
        public uint CandidatesSelection { get; private set; }

        /// <summary>
        /// Get the composition attribute at character index.
        /// </summary>
        /// <param name="index">Character Index</param>
        /// <returns>Composition Attribute</returns>
        public CompositionAttributes GetCompositionAttr(int index)
        {
            return (CompositionAttributes)_compattr[index];
        }

        /// <summary>
        /// Get the composition read attribute at character index.
        /// </summary>
        /// <param name="index">Character Index</param>
        /// <returns>Composition Attribute</returns>
        public CompositionAttributes GetCompositionReadAttr(int index)
        {
            return (CompositionAttributes)_compreadattr[index];
        }

        /// <summary>
        /// Called when the candidates updated
        /// </summary>
        public event EventHandler CandidatesReceived;

        /// <summary>
        /// Called when the composition updated
        /// </summary>
        public event EventHandler CompositionReceived;

        /// <summary>
        /// Called when a new result character is coming
        /// </summary>
        public event EventHandler<IMEResultEventArgs> ResultReceived;

        /// <summary>
        /// Constructor, must be called when the window create.
        /// </summary>
        /// <param name="handle">Handle of the window</param>
        /// <param name="showDefaultIMEWindow">True if you want to display the default IME window</param>
        public IMENativeWindow(IntPtr handle)
        {
            this._context = IntPtr.Zero;
            this.Candidates = new string[0];
            this._compcurpos = new IMMCompositionInt(IMM.GCSCursorPos);
            this._compstr = new IMMCompositionString(IMM.GCSCompStr);
            this._compclause = new IMMCompositionString(IMM.GCSCompClause);
            this._compattr = new IMMCompositionString(IMM.GCSCompAttr);
            this._compread = new IMMCompositionString(IMM.GCSCompReadStr);
            this._compreadclause = new IMMCompositionString(IMM.GCSCompReadClause);
            this._compreadattr = new IMMCompositionString(IMM.GCSCompReadAttr);
            this._resstr = new IMMCompositionString(IMM.GCSResultStr);
            this._resclause = new IMMCompositionString(IMM.GCSResultClause);
            this._resread = new IMMCompositionString(IMM.GCSResultReadStr);
            this._resreadclause = new IMMCompositionString(IMM.GCSResultReadClause);
            //this._showIMEWin = showDefaultIMEWindow;
            AssignHandle(handle);
        }

        /// <summary>
        /// Enable the IME
        /// </summary>
        public void EnableIME()
        {
            IsEnabled = true;
 
            if (_context != IntPtr.Zero)
            {
                IMM.ImmAssociateContext(Handle, _context);
                IMM.ImmReleaseContext(Handle, _context);
        //        IMM.ShowReadingWindow(Handle, true);
                return;
            }

            // This fix the bug that _context is 0 on fullscreen mode.
            ImeContext.Enable(Handle);
        }

        /// <summary>
        /// Disable the IME
        /// </summary>
        public void DisableIME()
        {
            IsEnabled = false;

            IMM.ImmAssociateContext(Handle, IntPtr.Zero);
            IMM.ImmReleaseContext(Handle, _context);
        }

        /// <summary>
        /// Dispose everything
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                ReleaseHandle();
                _disposed = true;
            }
        }

        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case IMM.ImeSetContext:
                    IMESetContext(ref msg);
                    break;
                case IMM.InputLanguageChange:
                    return;
                case IMM.ImeNotify:
                    IMENotify(msg.WParam.ToInt32());
                   // if (!_showIMEWin)
                    return;
                    break;
                case IMM.ImeStartCompostition:
                    IMEStartComposion(msg.LParam.ToInt32());
                    return;
                case IMM.ImeComposition:
                    IMEComposition(msg.LParam.ToInt32());
                    break;
                case IMM.ImeEndComposition:
                    IMEEndComposition(msg.LParam.ToInt32());
                  //  if (!_showIMEWin)
                    return;
                    break;
                case IMM.Char:
                    CharEvent(msg.WParam.ToInt32());
                    break;
            }
            base.WndProc(ref msg);
        }

        private void ClearComposition()
        {
            _compstr.Clear();
            _compclause.Clear();
            _compattr.Clear();
            _compread.Clear();
            _compreadclause.Clear();
            _compreadattr.Clear();
        }

        private void ClearResult()
        {
            _resstr.Clear();
            _resclause.Clear();
            _resread.Clear();
            _resreadclause.Clear();
        }

        #region IME Message Handlers

        private void IMESetContext(ref Message msg)
        {
            if (msg.WParam.ToInt32() == 1)
            {
                IntPtr ptr = IMM.ImmGetContext(Handle);
                if (_context == IntPtr.Zero)
                    _context = ptr;
                else if (ptr == IntPtr.Zero && IsEnabled)
                    EnableIME();

                _compcurpos.IMEHandle = _context;
                _compstr.IMEHandle = _context;
                _compclause.IMEHandle = _context;
                _compattr.IMEHandle = _context;
                _compread.IMEHandle = _context;
                _compreadclause.IMEHandle = _context;
                _compreadattr.IMEHandle = _context;
                _resstr.IMEHandle = _context;
                _resclause.IMEHandle = _context;
                _resread.IMEHandle = _context;
                _resreadclause.IMEHandle = _context;

               // if (!_showIMEWin)
                //    msg.LParam = (IntPtr)0;
            }
        }

        private void IMENotify(int WParam)
        {
            switch (WParam)
            {
                case IMM.ImnOpenCandidate:
                case IMM.ImnChangeCandidate:
                    IMEChangeCandidate();
                    break;
                case IMM.ImnCloseCandidate:
                    IMECloseCandidate();
                    break;
                case IMM.ImnPrivate:
                    break;
                default:
                    break;
            }
        }

        private void IMEChangeCandidate()
        {
            uint length = IMM.ImmGetCandidateList(_context, 0, IntPtr.Zero, 0);
            if (length > 0)
            {
                IntPtr pointer = Marshal.AllocHGlobal((int)length);
                length = IMM.ImmGetCandidateList(_context, 0, pointer, length);
                IMM.CandidateList cList = (IMM.CandidateList)Marshal.PtrToStructure(pointer, typeof(IMM.CandidateList));
                //IMM.CandidateList cList = IMM.GetCandidateList(_context, 0);
                CandidatesSelection = cList.dwSelection;
                CandidatesPageStart = cList.dwPageStart;
                CandidatesPageSize = cList.dwPageSize;

                if (cList.dwCount > 1)
                {
                    Candidates = new string[cList.dwCount];
                    for (int i = 0; i < cList.dwCount; i++)
                    {
                        int sOffset = Marshal.ReadInt32(pointer, 24 + 4 * i);
                        Candidates[i] = Marshal.PtrToStringUni((IntPtr)(pointer.ToInt32() + sOffset));
                    }

                    if (CandidatesReceived != null)
                        CandidatesReceived(this, EventArgs.Empty);
                }
                else
                    IMECloseCandidate();

                Marshal.FreeHGlobal(pointer);
            }
        }

        private void IMECloseCandidate()
        {
            CandidatesSelection = CandidatesPageStart = CandidatesPageSize = 0;
            Candidates = new string[0];

            if (CandidatesReceived != null)
                CandidatesReceived(this, EventArgs.Empty);
        }

        private void IMEStartComposion(int lParam)
        {
            ClearComposition();
            ClearResult();

            if (CompositionReceived != null)
                CompositionReceived(this, EventArgs.Empty);
        }

        private void IMEComposition(int lParam)
        {
            if (_compstr.Update(lParam))
            {
                _compclause.Update();
                _compattr.Update();
                _compread.Update();
                _compreadclause.Update();
                _compreadattr.Update();
                _compcurpos.Update();

                if (CompositionReceived != null)
                    CompositionReceived(this, EventArgs.Empty);
            }
        }

        private void IMEEndComposition(int lParam)
        {
            ClearComposition();

            if (_resstr.Update(lParam))
            {
                _resclause.Update();
                _resread.Update();
                _resreadclause.Update();
            }

            if (CompositionReceived != null)
                CompositionReceived(this, EventArgs.Empty);
        }

        private void CharEvent(int wParam)
        {
            if (ResultReceived != null)
                ResultReceived(this, new IMEResultEventArgs((char)wParam));

            if (IsEnabled)
                IMECloseCandidate();
        }

        #endregion
    }

}
