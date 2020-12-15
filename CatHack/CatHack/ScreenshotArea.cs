﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace CatHack
{
    public partial class ScreenshotArea : Form
    {
        private static string userName = Environment.UserName;
        private String path = @"C:\Users\" + userName + @"\Documents\userData.txt";
        private int xInput, yInput, widthInput, heightInput;
        private Size sizeInput;

        public ScreenshotArea()
        {
            InitializeComponent();

            this.Opacity = .5D; // make trasparent
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true); // this is to avoid visual artifact
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17;

        const int _ = 10; // you can rename this variable if you like

        Rectangle Top { get { return new Rectangle(0, 0, this.ClientSize.Width, _); } }
        Rectangle Left { get { return new Rectangle(0, 0, _, this.ClientSize.Height); } }
        Rectangle Bottom { get { return new Rectangle(0, this.ClientSize.Height - _, this.ClientSize.Width, _); } }
        Rectangle Right { get { return new Rectangle(this.ClientSize.Width - _, 0, _, this.ClientSize.Height); } }

        Rectangle TopLeft { get { return new Rectangle(0, 0, _, _); } }

        private void captureThis_KeyDown(object sender, KeyEventArgs e)
        {

            CatHackMain cathack = new CatHackMain();

            if(cathack.getAttackSpeedScreenshot()) // TODO: check if file exists, if not - make one
            {
                xInput = this.Location.X;
                yInput = this.Location.Y;
                widthInput = this.Width;
                heightInput = this.Height;
                sizeInput = this.Size;

                File.WriteAllText(path, String.Empty);

                using (StreamWriter sr = File.AppendText(path)) // saving X,Y,W,H,S coordinates
                {
                    sr.WriteLine(xInput);
                    sr.WriteLine(yInput);
                    sr.WriteLine(widthInput);
                    sr.WriteLine(heightInput);
                    sr.WriteLine(sizeInput);
                    sr.Close();
                }
            }

            try
            {
                if (e.KeyCode == Keys.F)
                {
                    this.Hide();
                    SaveImage save = new SaveImage(this.Location.X, this.Location.Y, this.Width, this.Height, this.Size);
                    save.Show();
                }
            }
            catch (System.ArgumentException error)
            {               
                Console.WriteLine("caught");
            }
        }

        private void panelDrag_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.F)
            {
                this.Hide();
                SaveImage save = new SaveImage(this.Location.X, this.Location.Y, this.Width, this.Height, this.Size);
                save.Show();
            }
        }

        Rectangle TopRight { get { return new Rectangle(this.ClientSize.Width - _, 0, _, _); } }
        Rectangle BottomLeft { get { return new Rectangle(0, this.ClientSize.Height - _, _, _); } }
        Rectangle BottomRight { get { return new Rectangle(this.ClientSize.Width - _, this.ClientSize.Height - _, _, _); } }


        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == 0x84) // WM_NCHITTEST
            {
                var cursor = this.PointToClient(Cursor.Position);

                if (TopLeft.Contains(cursor)) message.Result = (IntPtr)HTTOPLEFT;
                else if (TopRight.Contains(cursor)) message.Result = (IntPtr)HTTOPRIGHT;
                else if (BottomLeft.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMLEFT;
                else if (BottomRight.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (Top.Contains(cursor)) message.Result = (IntPtr)HTTOP;
                else if (Left.Contains(cursor)) message.Result = (IntPtr)HTLEFT;
                else if (Right.Contains(cursor)) message.Result = (IntPtr)HTRIGHT;
                else if (Bottom.Contains(cursor)) message.Result = (IntPtr)HTBOTTOM;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            this.panelDrag.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top 
                | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));

            this.panelDrag.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelDrag.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.panelDrag.Name = "panelDrag";

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void captureThis_Click(object sender, EventArgs e)
        {
            this.Hide();
            SaveImage save = new SaveImage(this.Location.X, this.Location.Y, this.Width, this.Height, this.Size);
            save.Show();
        }
    }
}
