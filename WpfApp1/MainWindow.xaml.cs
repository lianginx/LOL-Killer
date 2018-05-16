using System;
using System.ServiceProcess;
using System.Windows;
using System.Configuration.Install;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        const string SERVICE_NAME = "LOL Killer";
        const string INSTALL = "安 装";
        const string UNINSTALL = "卸 载";
        const string START = "启 动";
        const string STOP = "停 止";
        const string Pause = "暂 停";
        const string CONTINUE = "继 续";
        const string CHECK = "检查状态";

        System.Windows.Forms.Timer timer;

        public MainWindow()
        {
            InitializeComponent();

            timer = new System.Windows.Forms.Timer() { Interval = 100 };
            timer.Tick += Timer_Tick;
        }

        private void MainFrom_Loaded(object sender, RoutedEventArgs e)
        {
            SetUI(SERVICE_NAME);
        }
        private void BtnInstall_Click(object sender, EventArgs e)
        {
            string[] install = { $@".\Service\{SERVICE_NAME}.exe" };
            string[] uninstall = { "/u", $@".\Service\{SERVICE_NAME}.exe" };

            try
            {
                btnInstall.IsEnabled = false;
                switch (btnInstall.Content as string)
                {
                    case INSTALL:
                        if (!IsServiceExisted(SERVICE_NAME))
                        {
                            ManagedInstallerClass.InstallHelper(install);
                        }
                        else
                            MessageBox.Show("该服务已经存在，不用重复安装。");
                        break;
                    case UNINSTALL:
                        if (IsServiceExisted(SERVICE_NAME))
                        {
                            ManagedInstallerClass.InstallHelper(uninstall);
                        }
                        else
                            MessageBox.Show("该服务未安装，请稍后重试。");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                SetUI(SERVICE_NAME);
                btnInstall.IsEnabled = true;
            }
        }
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            var serviceController = new ServiceController(SERVICE_NAME);

            try
            {
                switch (btnStart.Content as string)
                {
                    case START:
                        serviceController.Start();
                        MessageBox.Show($"{SERVICE_NAME} 已启动。");
                        SetUI(SERVICE_NAME);
                        break;
                    case STOP:
                        if (serviceController.CanStop)
                        {
                            serviceController.Stop();
                            MessageBox.Show($"{SERVICE_NAME} 已停止。");
                            SetUI(SERVICE_NAME);
                        }
                        else
                            MessageBox.Show("无法停止！");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(new ServiceController(SERVICE_NAME).Status.ToString());
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            var nowTime = DateTime.Now;
            var countDown = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 22, 55, 00);

            if (nowTime > countDown)
            {
                txtTime.Content = "Game Time";
                return;
            }
            var t = countDown - nowTime;
            txtTime.Content = $"{t.ToString("hh")}:{t.ToString("mm")}:{t.ToString("ss")}";

        }

        #region Tools

        /// <summary>   
        /// 检查指定的服务是否存在
        /// </summary>   
        /// <param name="serviceName">要查找的服务名字</param>    
        private bool IsServiceExisted(string svcName)
        {
            var services = ServiceController.GetServices();
            foreach (var item in services)
            {
                if (item.ServiceName == svcName) return true;
            }
            return false;
        }

        /// <summary>
        /// 根据传入参数，设置 UI 状态
        /// </summary>
        /// <param name="agr"></param>
        private void SetUI(string serviceName)
        {
            if (!IsServiceExisted(serviceName))
            {
                timer.Stop();
                btnInstall.Content = INSTALL;
                btnStart.Content = START;
                btnCheck.Content = CHECK;
                txtTime.Content = "Not Installed";
                btnStart.IsEnabled = false;
                btnCheck.IsEnabled = false;
            }
            else
            {
                btnInstall.Content = UNINSTALL;
                btnStart.IsEnabled = true;
                btnCheck.IsEnabled = true;
                switch (new ServiceController(serviceName).Status)
                {
                    case ServiceControllerStatus.Running:
                    case ServiceControllerStatus.StartPending:
                        timer.Start();
                        btnStart.Content = STOP;
                        break;
                    case ServiceControllerStatus.Stopped:
                    case ServiceControllerStatus.StopPending:
                        timer.Stop();
                        txtTime.Content = "Not Start";
                        btnStart.Content = START;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }
}
