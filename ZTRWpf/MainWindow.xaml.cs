using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZTRWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Variables for tracking the position of two points.
        int lPos = 0;
        int rPos = 0;
        RobotConnection rc = null;
        string lastMsg = string.Empty;

        const int numFactor = 11;
        const int demFactor = 10;

        public int LeftAdj { get; private set; }
        public int RightAdj { get; private set; }
        public int Speed { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            rc = new ZTRWpf.RobotConnection();
            lastMsg = string.Empty;
            txtIpAddress.Text = "192.168.1.102";
            LeftAdj = 0;
            RightAdj = 0;
            Speed = 0;
        }

        private void SetText(int lPos, int rPos)
        {
            SetText(string.Format("L{0:000},R{1:000}", lPos, rPos));
        }

        private void SetText(string text)
        {
            txtSend.Text = text;
            if (!lastMsg.Equals(text))
            {
                txtEcho.Text = rc.Send(text);
                lastMsg = text;
            }
        }

        private void GetLeftPosition(TouchEventArgs e)
        {
            var h = (int)(canvas1.ActualHeight) / 2;
            lPos = (h - (int)e.GetTouchPoint(canvas1).Position.Y) * numFactor / h * demFactor;
            if (Math.Abs(lPos) > 100) lPos = 100 * lPos / Math.Abs(lPos);
            SetText(lPos, rPos);
        }
        private void GetRightPosition(TouchEventArgs e)
        {
            var h = (int)(canvas2.ActualHeight * 0.90d) / 2;
            rPos = (h - (int)e.GetTouchPoint(canvas2).Position.Y) * numFactor / h * demFactor;
            if (Math.Abs(rPos) > 100) rPos = 100 * rPos / Math.Abs(rPos);
            SetText(lPos, rPos);
        }
        private void SetSpeed(TouchEventArgs e)
        {
            var h = (int)(canvas1.ActualHeight) / 2;
            lPos = (h - (int)e.GetTouchPoint(canvas1).Position.Y) * numFactor / h * demFactor;
            if (Math.Abs(lPos) > 100) lPos = 100 * lPos / Math.Abs(lPos);
            Speed = lPos;
            SendSpeed();
        }
        private void SetRatio(TouchEventArgs e)
        {
            var w = (int)(canvas2.ActualWidth * 0.90d) / 2;
            rPos = (w - (int)e.GetTouchPoint(canvas2).Position.X) * numFactor / w * demFactor;
            if (Math.Abs(rPos) > 100) rPos = 100 * rPos / Math.Abs(rPos);

            if (Speed != 0)
            {
                LeftAdj = 0;
                RightAdj = 0;
                if (rPos > 0)
                    LeftAdj = rPos;
                else if (rPos < 0)
                    RightAdj = -rPos;
            }
            else
            {
                RightAdj = rPos;
                LeftAdj = -rPos;
            }
            SendSpeed();
        }

        private void SendSpeed()
        {
            if (Speed != 0)
            {
                lPos = Speed * (100 - LeftAdj) / 100;
                rPos = Speed * (100 - RightAdj) / 100;
            }
            else
            {
                lPos = LeftAdj;
                rPos = RightAdj;
            }
            SetText(lPos, rPos);
        }

        private void canvas1_TouchDown(object sender, TouchEventArgs e)
        {
            SetSpeed(e);
        }

        private void canvas1_TouchMove(object sender, TouchEventArgs e)
        {
            SetSpeed(e);
        }

        private void canvas1_TouchUp(object sender, TouchEventArgs e)
        {
            Speed = 0;
            LeftAdj = 0;
            RightAdj = 0;
            SendSpeed();
        }

        private void canvas2_TouchDown(object sender, TouchEventArgs e)
        {
            SetRatio(e);
        }

        private void canvas2_TouchMove(object sender, TouchEventArgs e)
        {
            SetRatio(e);
        }

        private void canvas2_TouchUp(object sender, TouchEventArgs e)
        {
            LeftAdj = 0;
            RightAdj = 0;
            SendSpeed();
        }

        //private void txtIpAddress_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        var ipaddr = txtIpAddress.Text;
        //        if (Regex.IsMatch(ipaddr, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
        //        {
        //            imgConnection.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/connected.png"));
        //        }
        //        else
        //        {
        //            imgConnection.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/disconnected.png"));
        //            ipaddr = string.Empty;
        //        }
        //        rc.IpAddress = ipaddr;
        //        SetText("Ready");
        //    }
        //}

        private void btnMotor_Checked(object sender, RoutedEventArgs e)
        {
            if (!rc.IsReady)
            {
                btnMotor.IsChecked = false;
                SetText("Not Connected");
            }
            else
            {
                var msg = btnMotor?.IsChecked ?? false ? "MOTOR-START" : "MOTOR-STOP";
                SetText(msg);
            }
        }
        private void btnNetwork_Checked(object sender, RoutedEventArgs e)
        {
            var ipaddr = txtIpAddress.Text;
            if ((btnNetwork?.IsChecked ?? false) && Regex.IsMatch(ipaddr, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
            {
                rc.IpAddress = ipaddr;
                SetText("Ready");
            }
            else
            {
                btnNetwork.IsChecked = false;
                rc.IpAddress = string.Empty;
                SetText("DISCONNECT");
            }
        }

        //void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        //{
        //    if (this.canvas1 != null)
        //    {
        //        foreach (TouchPoint _touchPoint in e.GetTouchPoints(this.canvas1))
        //        {
        //            if (_touchPoint.Action == TouchAction.Down)
        //            {
        //                // Clear the canvas and capture the touch to it.
        //                this.canvas1.Children.Clear();
        //                _touchPoint.TouchDevice.Capture(this.canvas1);
        //            }

        //            else if (_touchPoint.Action == TouchAction.Move && e.GetPrimaryTouchPoint(this.canvas1) != null)
        //            {
        //                // This is the first (primary) touch point. Just record its position.
        //                if (_touchPoint.TouchDevice.Id == e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id)
        //                {
        //                    pt1.X = _touchPoint.Position.X;
        //                    pt1.Y = _touchPoint.Position.Y;
        //                }

        //                // This is not the first touch point. Draw a line from the first point to this one.
        //                else if (_touchPoint.TouchDevice.Id != e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id)
        //                {
        //                    pt2.X = _touchPoint.Position.X;
        //                    pt2.Y = _touchPoint.Position.Y;

        //                    Line _line = new Line();
        //                    _line.Stroke = new RadialGradientBrush(Colors.White, Colors.Black);
        //                    _line.X1 = pt1.X;
        //                    _line.X2 = pt2.X;
        //                    _line.Y1 = pt1.Y;
        //                    _line.Y2 = pt2.Y;
        //                    _line.StrokeThickness = 2;
        //                    this.canvas1.Children.Add(_line);
        //                }
        //            }

        //            else if (_touchPoint.Action == TouchAction.Up)
        //            {
        //                // If this touch is captured to the canvas, release it.
        //                if (_touchPoint.TouchDevice.Captured == this.canvas1)
        //                {
        //                    this.canvas1.ReleaseTouchCapture(_touchPoint.TouchDevice);
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
