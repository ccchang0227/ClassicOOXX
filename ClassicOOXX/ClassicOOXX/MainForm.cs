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
    public partial class MainForm : Form, IGamePadCallback, ISimpleAIAction
    {

        private GamePad MainGamePad;

        private SimpleAI ComAI;
        
        public MainForm()
        {
            InitializeComponent();

            // Source: https://www.codeproject.com/Articles/26071/Draw-Over-WinForms-Controls
            graphicalOverlay.Owner = this;

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ComAI = new SimpleAI(this);

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

            SelectButton(x, y);
        }
        
        private void 關於ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("作者：\n張智傑\nChih-chieh Chang\nccch.realtouch@gmail.com", "關於", MessageBoxButtons.OK);
        }

        private void 結束ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        private void 開始新遊戲ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Restart();
        }

        private void SelectButton(int x, int y)
        {
            Button button = (Button)tableLayout.GetControlFromPosition(y, x);

            if (!MainGamePad.Selectable(x, y)) { return; }

            if (MainGamePad.Turn == MainGamePad.Players[0])
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

        private void Restart()
        {
            MainGamePad = new GamePad(this);

            foreach (Button item in tableLayout.Controls.Cast<Button>())
            {
                item.Text = "";
                item.BackgroundImage = null;
                item.Enabled = true;
            }
            Invalidate(true);

            OnTurnChanged(MainGamePad.Turn);

            Timer delayLoadAI = new Timer();
            delayLoadAI.Interval = 500; //0.5 sec.
            delayLoadAI.Tick += LoadAI;
            delayLoadAI.Enabled = true;

        }

        private void LoadAI(object sender, EventArgs e)
        {
            ((Timer)sender).Enabled = false;

            ComAI.Load(MainGamePad);
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
            ComAI.GameOver();

            foreach (Button item in tableLayout.Controls.Cast<Button>())
            {
                item.Enabled = false;
            }

            Invalidate(true);

            /*
            List<List<Point>> winnerLines = MainGamePad.GetWinLines();
            if (winnerLines.Count == 0) { return; }

            Graphics graphics = this.CreateGraphics();
            foreach (List<Point> points in winnerLines)
            {
                if (points.Count != 2) { continue; }

                Point start = points.ElementAt(0);
                Point end = points.ElementAt(1);

                Console.Out.WriteLine("start = " + start.ToString());
                Console.Out.WriteLine("end = " + end.ToString());

                Control startItem = tableLayout.GetControlFromPosition(start.Y, start.X);
                Control endItem = tableLayout.GetControlFromPosition(end.Y, end.X);

                start.X = startItem.Coordinates().X + startItem.Size.Width / 2;
                start.Y = startItem.Coordinates().Y + startItem.Size.Height / 2;
                end.X = endItem.Coordinates().X + endItem.Size.Width / 2;
                end.Y = endItem.Coordinates().Y + endItem.Size.Height / 2;

                Console.Out.WriteLine("start = " + start.ToString());
                Console.Out.WriteLine("end = " + end.ToString());

                Pen pen = new Pen(Color.Red, 10);
                graphics.DrawLine(pen, start, end);

            }
            graphics.Dispose();
            //*/

        }
        
        private void graphicalOverlay_Paint(object sender, PaintEventArgs e)
        {
            List<List<Point>> winnerLines = MainGamePad.GetWinLines();
            if (winnerLines.Count == 0) { return; }
            
            foreach (List<Point> points in winnerLines)
            {
                if (points.Count != 2) { continue; }

                Point start = points.ElementAt(0);
                Point end = points.ElementAt(1);

                Control startItem = tableLayout.GetControlFromPosition(start.Y, start.X);
                Control endItem = tableLayout.GetControlFromPosition(end.Y, end.X);
                
                start.X = startItem.Coordinates().X + startItem.Size.Width / 2;
                start.Y = startItem.Coordinates().Y + startItem.Size.Height / 2;
                end.X = endItem.Coordinates().X + endItem.Size.Width / 2;
                end.Y = endItem.Coordinates().Y + endItem.Size.Height / 2;

                Pen pen = new Pen(Color.Green, 10);
                e.Graphics.DrawLine(pen, start, end);
            }
        }

        // MARK - IGamePadCallback

        public void OnTurnChanged(int newTurn)
        {

            if (newTurn == GamePad.Player1Id)
            {
                toolStripStatusLabel.Text = "輪到玩家1";

                EnableAllUnclickButtons();
            }
            else if (newTurn == GamePad.Player2Id)
            {
                toolStripStatusLabel.Text = "輪到玩家2";

                EnableAllUnclickButtons();
            }
            else if (newTurn == GamePad.ComId)
            {
                toolStripStatusLabel.Text = "輪到電腦";

                DisableAllUnclickButtons();
                ComAI.MyTurn();
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
        
        // MARK - ISimpleAIAction

        public void OnLoad()
        {
            Console.Out.WriteLine("OnLoad: "+MainGamePad.Players[0]+","+ MainGamePad.Players[1]);
            Console.Out.WriteLine("OnLoad: " + MainGamePad.Turn);
            if (MainGamePad.Turn == GamePad.ComId)
            {
                ComAI.MyTurn();
            }
        }

        public void OnMeasuring()
        {
            Console.Out.WriteLine("OnMeasuring");
        }

        public void OnSelect(int x, int y)
        {
            EnableAllUnclickButtons();

            Console.Out.WriteLine("OnSelect: " + x +","+y);
            SelectButton(x, y);
        }
    }

}
