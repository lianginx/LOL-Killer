using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace WindowsServiceTest1
{
    public partial class Service1 : ServiceBase
    {
        public Service1() => InitializeComponent();

        protected override void OnStart(string[] args)
        {
            var killThread = new Thread(new ParameterizedThreadStart(Kill));
            killThread.Start(19);
        }

        private void Kill(object timeValue)
        {
            var lol = "LeagueClient";
            var tgp = "tgp_daemon";
            var delay = 1000 * 30;
            var time = (int)timeValue;
            var MessageStr = "你好像一不小心开启了「禁忌之门」，怎么这么不小心，幸亏有我在~";

            do
            {
                if (DateTime.Now.Hour < time)
                {
                    var items = Process.GetProcesses();
                    foreach (var item in items)
                    {
                        var temp = item.ProcessName == lol ||
                                   item.ProcessName == tgp;
                        if (temp)
                        {
                            item.Kill();
                            MessageBox.Show(MessageStr);
                        }
                    }
                }
                Thread.Sleep(delay);
            } while (true);
        }

        protected override void OnStop()
        {
        }
    }
}
