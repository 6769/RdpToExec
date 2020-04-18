using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdpRun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RdpRun.Tests
{
    [TestClass()]
    public class NativeKeyBoardTests
    {
        //[Ignore]
        [TestMethod()]
        public void WinRTest()
        {
            
            //对Mstsc有效，但对FreeRDP无效
            Thread.Sleep(1500);
            NativeKeyBoard.WinR();
            Thread.Sleep(500);
            NativeKeyBoard.CtrlV();
            Thread.Sleep(500);
            NativeKeyBoard.Enter();
        }

        [Ignore]
        [TestMethod()]
        public void RunCmdTest()
        {
            CMD.RunCmd("cmd.exe");
        }
    }
}