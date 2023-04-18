using BetterDesktop.Models;
using RegistryUtils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using static BetterDesktop.IconBackgroundStuff.Externs;

namespace BetterDesktop.IconBackgroundStuff
{
    /// <summary>Use the <see cref="Externs"/> functions to paint the desktop.</summary>
    internal class IconBackgroundManager
    {
        private Point[] points;
        private IntPtr workerW;

        public Settings settings;

        public IconBackgroundManager()
        {
            // when the desktop is changed, repaint.
            var regMonitor = new RegistryMonitor("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\Shell\\Bags\\1\\Desktop");
            regMonitor.RegChanged += Start;
            regMonitor.Start();
        }

        /// <summary>Setup the manager and paint.</summary>
        public void Start(object sender = null, EventArgs e = null)
        {
            Reset();

            // Get the folder view.
            var handle = FindDesktopFolderView();
            points = GetIconPositions(handle);
            workerW = GetWorkerW();
            Paint();
        }

        /// <summary>Paint underneath the desktop icons.</summary>
        private void Paint()
        {
            // Get the Device Context of the WorkerW
            IntPtr dc = GetDCEx(workerW, IntPtr.Zero, (DeviceContextValues)0x403);
            if (dc != IntPtr.Zero)
            {
                Bitmap b = new Bitmap(1920, 1080);
                using (Graphics bg = Graphics.FromImage(b))
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        var solid = Color.FromArgb(255, settings.PaintColor.R, settings.PaintColor.G, settings.PaintColor.B);
                        // Use the Graphics instance to draw a white rectangle in the upper 
                        // left corner. In case you have more than one monitor think of the 
                        // drawing area as a rectangle that spans across all monitors, and 
                        // the 0,0 coordinate being in the upper left corner.
                        bg.FillRectangle(new SolidBrush(solid),
                            points[i].X - 18, points[i].Y - 2, 76 + /*offset =>*/ 25, 100);
                    }
                }
                b = ChangeOpacity(b, settings.PaintColor.A / 255f);
                // Create a Graphics instance from the Device Context
                using (Graphics g = Graphics.FromHdc(dc))
                {
                    // Use the Graphics instance to draw a white rectangle in the upper 
                    // left corner. In case you have more than one monitor think of the 
                    // drawing area as a rectangle that spans across all monitors, and 
                    // the 0,0 coordinate being in the upper left corner.
                    g.DrawImage(b, 0, 0);
                }
                // make sure to release the device context after use.
                ReleaseDC(workerW, dc);
            }
        }

        /// <summary>Find the folder view window handle.</summary>
        /// <returns>An <see cref="IntPtr"/> of the window's handle.</returns>
        /// <exception cref="Exception">When handle is not found.</exception>
        private IntPtr FindDesktopFolderView()
        {
            // get the handle of the desktop listview
            // https://social.msdn.microsoft.com/Forums/en-US/4af734fb-d2c1-414b-a9f1-759b76692802/drawing-on-desktop-background-before-background-icons-are-drawn-is-difficult?forum=vcgeneral
            IntPtr vHandle = FindWindow("Progman", "Program Manager");
            vHandle = FindWindowEx(vHandle, IntPtr.Zero, "SHELLDLL_DefView", null);
            vHandle = FindWindowEx(vHandle, IntPtr.Zero, "SysListView32", "FolderView");

            // if cannot find shelldll under progman, it could have been moved under
            // a WorkerW when the "spawn WorkerW" signal was sent.
            // Check for list view as a child of a WorkerW window with a shelldll child.
            if (vHandle == IntPtr.Zero)
            {
                // We enumerate all Windows, until we find one, that has the SHELLDLL_DefView 
                // as a child. 
                // If we found that window, we take its next sibling and assign it to workerw.
                EnumWindows(new EnumWindowsProc((tophandle, topparamhandle) =>
                {
                    IntPtr p = FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            null);

                    if (p != IntPtr.Zero)
                    {
                        // Gets the child of the shell.
                        vHandle = FindWindowEx(p, IntPtr.Zero, "SysListView32", "FolderView");

                        // no need to keep looping.
                        return false;
                    }

                    return true;
                }), IntPtr.Zero);

                // if we still havn't found the window.
                if (vHandle == IntPtr.Zero)
                {
                    throw new Exception("Could not find the FolderView window.");
                }
            }

            // return the window's handle.
            return vHandle;
        }

        /// <summary>Get points of desktop icons.</summary>
        /// <param name="folderViewHandle">The folder view handle.</param>
        /// <returns>An array of <see cref="Point"/>s.</returns>
        private Point[] GetIconPositions(IntPtr folderViewHandle)
        {
            //Get total count of the icons on the desktop
            int vItemCount = SendMessage(folderViewHandle, LVM_GETITEMCOUNT, 0, 0);

            uint vProcessId;
            GetWindowThreadProcessId(folderViewHandle, out vProcessId);

            IntPtr vProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ |
                PROCESS_VM_WRITE, false, vProcessId);

            IntPtr vPointer = VirtualAllocEx(vProcess, IntPtr.Zero, 4096,
                MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);

            Point[] points = new Point[vItemCount];

            try
            {
                for (int j = 0; j < vItemCount; j++)
                {
                    byte[] vBuffer = new byte[256];
                    LVITEMA[] vItem = new LVITEMA[1];
                    vItem[0].mask = LVIF_TEXT;
                    vItem[0].iItem = j;
                    vItem[0].iSubItem = 0;
                    vItem[0].cchTextMax = vBuffer.Length;
                    vItem[0].pszText = (IntPtr)((int)vPointer + Marshal.SizeOf(typeof(LVITEMA)));

                    uint vNumberOfBytesRead = 0;

                    WriteProcessMemory(vProcess, vPointer,
                        Marshal.UnsafeAddrOfPinnedArrayElement(vItem, 0),
                        Marshal.SizeOf(typeof(LVITEMA)), ref vNumberOfBytesRead);

                    SendMessage(folderViewHandle, LVM_GETITEMW, j, vPointer.ToInt32());

                    ReadProcessMemory(vProcess,
                        (IntPtr)((int)vPointer + Marshal.SizeOf(typeof(LVITEMA))),
                        Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0),
                        vBuffer.Length, ref vNumberOfBytesRead);

                    //Get icon location
                    SendMessage(folderViewHandle, LVM_GETITEMPOSITION, j, vPointer.ToInt32());

                    Point[] vPoint = new Point[1];

                    ReadProcessMemory(vProcess, vPointer,
                        Marshal.UnsafeAddrOfPinnedArrayElement(vPoint, 0),
                        Marshal.SizeOf(typeof(Point)), ref vNumberOfBytesRead);

                    points[j] = vPoint[0];
                }
            }
            finally
            {
                VirtualFreeEx(vProcess, vPointer, 0, MEM_RELEASE);
                CloseHandle(vProcess);
            }

            return points;
        }

        /// <summary>Get the handle of WorkerW, the window underneath the desktop icons.</summary>
        /// <returns>An <see cref="IntPtr"/> to WorkerW's handle.</returns>
        /// <exception cref="Exception">When handle is not found.</exception>
        private IntPtr GetWorkerW()
        {
            // https://www.codeproject.com/Articles/856020/Draw-Behind-Desktop-Icons-in-Windows-plus
            IntPtr progman = FindWindow("Progman", null);
            IntPtr result = IntPtr.Zero;

            // Send 0x052C to Progman. This message directs Progman to spawn a 
            // WorkerW behind the desktop icons. If it is already there, nothing 
            // happens.
            SendMessageTimeout(progman,
                                   0x052C,
                                   IntPtr.Zero,
                                   IntPtr.Zero,
                                   SendMessageTimeoutFlags.SMTO_NORMAL,
                                   1000,
                                   out result);

            IntPtr workerw = IntPtr.Zero;

            // We enumerate all Windows, until we find one, that has the SHELLDLL_DefView 
            // as a child. 
            // If we found that window, we take its next sibling and assign it to workerw.
            EnumWindows(new EnumWindowsProc((tophandle, topparamhandle) =>
            {
                IntPtr p = FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            null);

                if (p != IntPtr.Zero)
                {
                    // Gets the WorkerW Window after the current one.
                    workerw = FindWindowEx(IntPtr.Zero,
                                               tophandle,
                                               "WorkerW",
                                               null);
                }

                return true;
            }), IntPtr.Zero);

            if (workerw == IntPtr.Zero) throw new Exception("WorkerW not found.");

            return workerw;
        }

        /// <summary>Get the path of the desktop wallpaper.</summary>
        /// <returns>A path to the wallpaper.</returns>
        private static string GetPathOfWallpaper()
        {
            // https://stackoverflow.com/questions/35023390/get-path-to-current-desktop-wallpaper#comment57773579_35023504
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Microsoft\\Windows\\Themes\\TranscodedWallpaper");
        }

        /// <summary>Refreshes desktop wallpaper, undoing everything.</summary>
        public void Reset()
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                GetPathOfWallpaper(),
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        /// <summary>Change the opacity of an image.</summary>
        /// <param name="img">The image.</param>
        /// <param name="opacityvalue">The new opacity.</param>
        /// <returns>A <see cref="Bitmap"/>.</returns>
        // https://www.codeproject.com/Tips/201129/Change-Opacity-of-Image-in-C
        public static Bitmap ChangeOpacity(Image img, float opacityvalue)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height); // Determining Width and Height of Source Image
            Graphics graphics = Graphics.FromImage(bmp);
            ColorMatrix colormatrix = new ColorMatrix();
            colormatrix.Matrix33 = opacityvalue;
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();   // Releasing all resource used by graphics 
            return bmp;
        }
    }
}
