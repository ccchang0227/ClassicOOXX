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
    public partial class MainForm : Form, IGamePadCallback
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
            
            if (!MainGamePad.Selectable(x, y)) { return; }

            if (MainGamePad.Turn == GamePad.Player1Id)
            {
                //button.Text = "O";
                button.BackgroundImage = Properties.Resources.O;
            }
            else
            {
                //button.Text = "X";
                button.BackgroundImage = Properties.Resources.X;
            }
            button.Enabled = false;

            if (!MainGamePad.Select(x, y, MainGamePad.Turn))
            {
                MainGamePad.TurnChange();
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
            MainGamePad = new GamePad(this);
            foreach (Button item in tableLayout.Controls.Cast<Button>())
            {
                item.Text = "";
                item.BackgroundImage = null;
                item.Enabled = true;
            }
        }

        private void 開始新遊戲ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Restart();
        }

        private void EnableAllUnclickButtons()
        {
            foreach (Button item in tableLayout.Controls.Cast<Button>())
            {
                if (item.BackgroundImage == null)
                {
                    item.Enabled = true;
                }
            }
        }

        private void DisableAllUnclickButtons()
        {
            foreach (Button item in tableLayout.Controls.Cast<Button>())
            {
                if (item.BackgroundImage == null)
                {
                    item.Enabled = false;
                }
            }
        }

        private void GameOver()
        {
            foreach (Button item in tableLayout.Controls.Cast<Button>())
            {
                item.Enabled = false;
            }
        }

        // MARK - IGamePadCallback

        public void OnTurnChanged(int newTurn)
        {

            if (newTurn == GamePad.Player1Id)
            {
                toolStripStatusLabel.Text = "輪到玩家1";
            }
            else if (newTurn == GamePad.Player2Id)
            {
                toolStripStatusLabel.Text = "輪到玩家2";
            }
            else if (newTurn == GamePad.ComId)
            {
                toolStripStatusLabel.Text = "輪到電腦";
            }
            
        }

        public void OnGameStateChanged(int winnerId, State newState)
        {
            switch (newState)
            {
                case State.Draw:
                    {
                        GameOver();
                        toolStripStatusLabel.Text = "平手，遊戲結束";
                        MessageBox.Show("平手", "遊戲結束", MessageBoxButtons.OK);

                        break;
                    }
                case State.Finished:
                    {
                        GameOver();
                        if (winnerId != GamePad.NoneId)
                        {
                            String winner = "";
                            if (winnerId == GamePad.Player1Id) { winner = "玩家1"; }
                            else if (winnerId == GamePad.Player2Id) { winner = "玩家2"; }
                            else if (winnerId == GamePad.ComId) { winner = "電腦"; }

                            toolStripStatusLabel.Text = "遊戲結束，"+winner+"贏了";
                            MessageBox.Show("贏家:\n" + winner, "遊戲結束", MessageBoxButtons.OK);
                        }
                        else
                        {
                            toolStripStatusLabel.Text = "平手，遊戲結束";
                            MessageBox.Show("平手", "遊戲結束", MessageBoxButtons.OK);
                        }
                        break;
                    }
                default:
                    break;
            }
        }
        
    }

}
