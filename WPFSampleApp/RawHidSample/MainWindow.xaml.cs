using HidLibrary;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace RawHidSample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private HidDevice usb;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var vendorId = int.Parse(VID.Text, System.Globalization.NumberStyles.HexNumber);
            var productId = int.Parse(PID.Text, System.Globalization.NumberStyles.HexNumber);
            // VID/PIDで複数取得出来るのでさらにUsagePageとUsageでフィルタリングして最初の一つを使う
            usb = HidDevices.Enumerate(vendorId, productId)
                .Where(x => (UInt16)x.Capabilities.UsagePage == 0xFF60 && x.Capabilities.Usage == 0x61)
                .FirstOrDefault();

            if (usb == null)
            {
                return;
            }

            usb.OpenDevice();
            
            if (!usb.IsConnected)
            {
                return;
            }

            SettingGrid.IsEnabled = false;

            usb.Inserted += DeviceAttachedHandler;
            usb.Removed += DeviceRemovedHandler;
            usb.MonitorDeviceEvents = true;

            // QMKのRawHID機能は32バイトを受送信可能
            var report = new HidReport(32);
            report.ReportId = 0;    // IDは適当で良い模様
            report.Data[0] = (byte)('h');
            usb.WriteReport(report);
            usb.ReadReport(OnReport);
        }

        private void OnReport(HidReport report)
        {
            if (!usb.IsConnected) { return; }
            var data = Encoding.ASCII.GetString(report.Data);
            System.Diagnostics.Debug.WriteLine(data);
            Dispatcher.Invoke(() =>
            {
                OutputText.Text = string.IsNullOrEmpty(OutputText.Text) ? data : $"{OutputText.Text}\n{data}";
                SettingGrid.IsEnabled = true;
            });

            // このサンプルでは一度受信したら終了
            usb.CloseDevice();
        }

        private static void DeviceAttachedHandler()
        {
        }

        private static void DeviceRemovedHandler()
        {
        }
    }
}
