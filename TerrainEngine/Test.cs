using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace TerrainEngine
{
    class Test
    {
        [STAThread]
        static void Main() 
        {
            Console.Out.WriteLine("Starting up");
            Terrain.Generate();
            Console.Out.WriteLine("Completed");
        }

        public static void Show(Image img)
        {
            Form frm = new Form()
            {
                ClientSize = new Size(800, 800),
                BackColor = Color.Black,
                WindowState = FormWindowState.Maximized
            };
            PictureBox pic;
            frm.Controls.Add(pic = new PictureBox()
            {
                Dock = DockStyle.Fill,
                Image = img,
                SizeMode = PictureBoxSizeMode.Zoom
            });
            pic.DoubleClick += (sender, e) =>
            {
                Clipboard.SetImage(img);
            };
            frm.ShowDialog();
        }
    }
}
