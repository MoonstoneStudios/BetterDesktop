using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BetterDesktop.IconBackgroundStuff
{
    /// <summary>All extern functions and their required constants and structs.</summary>
    internal static class Externs
    {
        // https://social.msdn.microsoft.com/Forums/windows/en-US/d7df8a4d-fc0f-4b62-80c9-7768756456e6/how-can-i-get-desktops-icons-information-
        public const uint LVM_FIRST = 0x1000;
        public const uint LVM_GETITEMCOUNT = LVM_FIRST + 4;
        public const uint LVM_GETITEMW = LVM_FIRST + 75;
        public const uint LVM_GETITEMPOSITION = LVM_FIRST + 16;
        public const uint PROCESS_VM_OPERATION = 0x0008;
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;
        public const uint MEM_COMMIT = 0x1000;
        public const uint MEM_RELEASE = 0x8000;
        public const uint MEM_RESERVE = 0x2000;
        public const uint PAGE_READWRITE = 4;
        public const int LVIF_TEXT = 0x0001;

        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
           uint dwSize, uint dwFreeType);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess,
            bool bInheritHandle, uint dwProcessId);

        [DllImport("user32.DLL")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.DLL")]
        public static extern IntPtr FindWindow(string lpszClass, string lpszWindow);

        [DllImport("user32.DLL")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent,
            IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd,
            out uint dwProcessId);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr windowHandle,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags flags,
            uint timeout,
            out IntPtr result);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, DeviceContextValues flags);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        // https://stackoverflow.com/a/1061682
        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPIF_UPDATEINIFILE = 0x01;
        public const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        // https://stackoverflow.com/a/60352109
        [StructLayout(LayoutKind.Sequential)]
        public struct LVITEMA
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            public IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public uint iGroupId;
            public UIntPtr puColumns;
        }

        // https://www.pinvoke.net/default.aspx/Enums/SendMessageTimeoutFlags.html
        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // <summary>Values to pass to the GetDCEx method.</summary>
        [Flags()]
        public enum DeviceContextValues : uint
        {
            /// <summary>DCX_WINDOW: Returns a DC that corresponds to the window rectangle rather
            /// than the client rectangle.</summary>
            Window = 0x00000001,
            /// <summary>DCX_CACHE: Returns a DC from the cache, rather than the OWNDC or CLASSDC
            /// window. Essentially overrides CS_OWNDC and CS_CLASSDC.</summary>
            Cache = 0x00000002,
            /// <summary>DCX_NORESETATTRS: Does not reset the attributes of this DC to the
            /// default attributes when this DC is released.</summary>
            NoResetAttrs = 0x00000004,
            /// <summary>DCX_CLIPCHILDREN: Excludes the visible regions of all child windows
            /// below the window identified by hWnd.</summary>
            ClipChildren = 0x00000008,
            /// <summary>DCX_CLIPSIBLINGS: Excludes the visible regions of all sibling windows
            /// above the window identified by hWnd.</summary>
            ClipSiblings = 0x00000010,
            /// <summary>DCX_PARENTCLIP: Uses the visible region of the parent window. The
            /// parent's WS_CLIPCHILDREN and CS_PARENTDC style bits are ignored. The origin is
            /// set to the upper-left corner of the window identified by hWnd.</summary>
            ParentClip = 0x00000020,
            /// <summary>DCX_EXCLUDERGN: The clipping region identified by hrgnClip is excluded
            /// from the visible region of the returned DC.</summary>
            ExcludeRgn = 0x00000040,
            /// <summary>DCX_INTERSECTRGN: The clipping region identified by hrgnClip is
            /// intersected with the visible region of the returned DC.</summary>
            IntersectRgn = 0x00000080,
            /// <summary>DCX_EXCLUDEUPDATE: Unknown...Undocumented</summary>
            ExcludeUpdate = 0x00000100,
            /// <summary>DCX_INTERSECTUPDATE: Unknown...Undocumented</summary>
            IntersectUpdate = 0x00000200,
            /// <summary>DCX_LOCKWINDOWUPDATE: Allows drawing even if there is a LockWindowUpdate
            /// call in effect that would otherwise exclude this window. Used for drawing during
            /// tracking.</summary>
            LockWindowUpdate = 0x00000400,
            /// <summary>DCX_VALIDATE When specified with DCX_INTERSECTUPDATE, causes the DC to
            /// be completely validated. Using this function with both DCX_INTERSECTUPDATE and
            /// DCX_VALIDATE is identical to using the BeginPaint function.</summary>
            Validate = 0x00200000,
        }

    }
}
