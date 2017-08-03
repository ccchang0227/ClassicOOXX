using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ClassicOOXX
{
    public partial class MainForm : Form
    {

        private GamePad MainGamePad;
        
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Restart();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            
            Size tableSize = tableLayout.Size;
            Point tableLocation = tableLayout.Location;
            tableSize.Width = Math.Min(toolStripContainer.ContentPanel.Width - 24, toolStripContainer.ContentPanel.Height - 24);
            tableSize.Height = tableSize.Width;
            tableLayout.Size = tableSize;
            tableLocation.X = (toolStripContainer.ContentPanel.Width - tableSize.Width) / 2;
            tableLocation.Y = (toolStripContainer.ContentPanel.Height - tableSize.Height) / 2;
            tableLayout.Location = tableLocation;
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int x = tableLayout.GetRow(button);
            int y = tableLayout.GetColumn(button);
            System.Console.Out.WriteLine("x=" + x + ", y=" + y);
            if (MainGamePad.Selectable(x, y))
            {
                button.Text = "X";
                button.Enabled = false;
                int winnerId = MainGamePad.Select(x, y, GamePad.PlayerId);
                if (winnerId != GamePad.NoneId)
                {
                    MessageBox.Show("" + winnerId, "贏家", MessageBoxButtons.OK);
                }
            }
        }
        
        private void 關於ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("作者：\n張智傑\nChih-chieh Chang\nccch.realtouch@gmail.com", "關於", MessageBoxButtons.OK);
        }

        private void 結束ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Restart()
        {
            MainGamePad = new GamePad();
            foreach (Button item in tableLayout.Controls.Cast<Button>())
            {
                item.Text = "";
                item.Enabled = true;
            }
        }

        private void 開始新遊戲ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Restart();
        }
    }
    
}
