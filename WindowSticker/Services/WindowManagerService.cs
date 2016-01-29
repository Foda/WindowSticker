using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowSticker.Services
{
    public class WindowManagerService
    {
        private List<IntPtr> _activeWindows = new List<IntPtr>();

        public List<IntPtr> GetAllActiveWindows()
        {
            _activeWindows.Clear();
            EnumWindows(new EnumWindowsProc(GetActiveWindows), IntPtr.Zero);

            return _activeWindows;
        }

        public void SetWindowPosition(IntPtr handle, Rect pos, bool isMax)
        {
            if (IsWindowVisible(handle))
            {
                //Check to see if the window is going to be off the screen, and if so just put it on the left side
                if (pos.Right > System.Windows.SystemParameters.VirtualScreenWidth)
                    SetWindowPos(handle, IntPtr.Zero, 0, pos.Top, Math.Abs(pos.Left - pos.Right), Math.Abs(pos.Top - pos.Bottom), SetWindowPosFlags.SWP_SHOWWINDOW);
                else
                {
                    SetWindowPos(handle, IntPtr.Zero, pos.Left, pos.Top, Math.Abs(pos.Left - pos.Right), Math.Abs(pos.Top - pos.Bottom), SetWindowPosFlags.SWP_SHOWWINDOW);
                    if (isMax)
                        ShowWindow(handle, 3);
                }
            }
        }

        private bool GetActiveWindows(IntPtr hWnd, IntPtr lParam)
        {
            if ((GetWindowLongA(hWnd, GWL_STYLE) & TARGETWINDOW) == TARGETWINDOW)
            {
                int style = GetWindowLong(hWnd, GWL_STYLE);
                _activeWindows.Add(hWnd);
            }

            return true;
        }

        public static bool IsWindowMaximized(IntPtr hWnd)
        {
            return (GetWindowLong(hWnd, GWL_STYLE) & WS_MAXIMIZE) == WS_MAXIMIZE;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern ulong GetWindowLongA(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        protected static extern bool IsWindowVisible(IntPtr hWnd); 

        [DllImport("user32.dll", CharSet = CharSet.Unicode)] 
        protected static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        protected delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")] 
        protected static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags wFlags);

        [DllImport("User32")]
        static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            SWP_ASYNCWINDOWPOS = 0x4000,

            SWP_DEFERERASE = 0x2000,

            SWP_DRAWFRAME = 0x0020,

            SWP_FRAMECHANGED = 0x0020,

            SWP_HIDEWINDOW = 0x0080,

            SWP_NOACTIVATE = 0x0010,

            SWP_NOCOPYBITS = 0x0100,

            SWP_NOMOVE = 0x0002,

            SWP_NOOWNERZORDER = 0x0200,

            SWP_NOREDRAW = 0x0008,

            SWP_NOREPOSITION = 0x0200,

            SWP_NOSENDCHANGING = 0x0400,

            SWP_NOSIZE = 0x0001,

            SWP_NOZORDER = 0x0004,

            SWP_SHOWWINDOW = 0x0040,
        }

        public enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }

        static readonly int GWL_STYLE = -16;
        static readonly ulong WS_VISIBLE = 0x10000000L;
        static readonly ulong WS_BORDER = 0x00800000L;
        static readonly ulong TARGETWINDOW = WS_BORDER | WS_VISIBLE;

        static readonly UInt32 WS_MINIMIZE = 0x20000000;
        static readonly UInt32 WS_MAXIMIZE = 0x1000000;

    }
    public struct Rect
    {
        public int Left { get; set; }

        public int Top { get; set; }

        public int Right { get; set; }

        public int Bottom { get; set; }
    }
}
