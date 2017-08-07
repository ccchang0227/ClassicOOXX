using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClassicOOXX
{

    public enum State
    {
        Playing,
        Draw,
        Finished
    }

    public interface IGamePadCallback
    {
        void OnTurnChanged(int newTurn);
        void OnGameStateChanged(int winnerId, State newState);
    }

    public class GamePad
    {
        
        public State GameState { get; private set; }

        // 空白
        public static int NoneId
        {
            get { return 0; }
        }
        // 玩家1
        public static int Player1Id
        {
            get { return 1; }
        }
        // 玩家2
        public static int Player2Id
        {
            get { return 2; }
        }
        // 電腦
        public static int ComId
        {
            get { return -1; }
        }

        // 棋盤大小
        public static int PadSize
        {
            get { return 3; }
        }

        public int[][] PadValues { get; }
        private int BlankCnt = PadSize ^ 2;

        public int[] Players = { Player1Id, Player2Id };
        public int Turn
        {
            get;
            private set;
        }

        private IGamePadCallback Callback = null;

        public GamePad(IGamePadCallback callback)
        {
            this.Callback = callback;

            PadValues = new int[PadSize][];
            for (int i = 0; i < PadSize; i++)
            {
                PadValues[i] = new int[PadSize];
                for (int j = 0; j < PadSize; j++)
                {
                    PadValues[i][j] = NoneId;
                }
            }
            GameState = State.Playing;
            BlankCnt = (int)Math.Pow(PadSize, 2);

            Turn = Players[0];
        }

        // 換回合
        public void TurnChange()
        {
            if (Turn == Players[0])
            {
                Turn = Players[1];
            }
            else
            {
                Turn = Players[0];
            }

            if (Callback != null)
            {
                Callback.OnTurnChanged(Turn);
            }
        }

        // 是否可以選點
        public bool Selectable(int x, int y)
        {
            if (x < 0 || x >= PadSize || y < 0 || y >= PadSize) { return false; }
            if (PadValues[x][y] != NoneId) { return false; }

            return true;
        }

        // 選點
        public bool Select(int x, int y, int id)
        {
            if (!Selectable(x, y)) { return EvaluateWinner(); }
            if (id == NoneId) { return EvaluateWinner(); }

            PadValues[x][y] = id;
            BlankCnt--;
            if (BlankCnt == 0)
            {
                GameState = State.Draw;
            }

            return EvaluateWinner();
        }

        private List<List<Point>> successLines = new List<List<Point>>();

        /**
         * 找贏家
         * 
         * @return 回傳遊戲是否結束，true/false
         */
        private bool EvaluateWinner()
        {
            // 先找2條對角
            // 左上->右下
            int x = 0;
            int y = 0;
            int initVal = PadValues[x][y];

            int winnerId = NoneId;

            int successCnt = 0;
            if (initVal != 0)
            {
                successCnt++;
                while (x + 1 < PadSize && y + 1 < PadSize)
                {
                    x++;
                    y++;
                    int val = PadValues[x][y];
                    if (initVal == val) { successCnt++; }
                    else { break; }
                }
                if (successCnt == PadSize) {
                    winnerId = initVal;
                    GameState = State.Finished;

                    List<Point> points = new List<Point>
                    {
                        new Point(0, 0),
                        new Point(PadSize - 1, PadSize - 1)
                    };
                    successLines.Add(points);
                }
            }
            // 右上->左下
            x = 0;
            y = PadSize-1;
            initVal = PadValues[x][y];
            successCnt = 0;
            if (initVal != 0)
            {
                successCnt++;
                while (x + 1 < PadSize && y - 1 < PadSize)
                {
                    x++;
                    y--;
                    int val = PadValues[x][y];
                    if (initVal == val) { successCnt++; }
                    else { break; }
                }
                if (successCnt == PadSize)
                {
                    if (winnerId == NoneId)
                    {
                        winnerId = initVal;
                        GameState = State.Finished;
                    }

                    List<Point> points = new List<Point>
                    {
                        new Point(0, PadSize - 1),
                        new Point(PadSize - 1, 0)
                    };
                    successLines.Add(points);
                }
            }

            for (int i = 0; i < PadSize; i++)
            {
                for (int j = 0; j < PadSize; j++)
                {
                    if (i > 0 && j > 0)
                    {
                        break;
                    }

                    int val = PadValues[i][j];
                    if (val == 0) {
                        continue;
                    }

                    successCnt = 1;
                    initVal = val;
                    if (i == 0)
                    {
                        // 往下找
                        x = i;
                        while (x + 1 < PadSize)
                        {
                            x++;
                            val = PadValues[x][j];
                            if (initVal == val) { successCnt++; }
                            else { break; }
                        }
                        if (successCnt == PadSize)
                        {
                            if (winnerId == NoneId)
                            {
                                winnerId = initVal;
                                GameState = State.Finished;
                            }

                            List<Point> points = new List<Point>
                            {
                                new Point(0, j),
                                new Point(PadSize - 1, j)
                            };
                            successLines.Add(points);
                        }
                    }

                    successCnt = 1;
                    if (i >= 0 && j == 0)
                    {
                        // 往右找
                        y = j;
                        while (y + 1 < PadSize)
                        {
                            y++;
                            val = PadValues[i][y];
                            if (initVal == val) { successCnt++; }
                            else { break; }
                        }
                        if (successCnt == PadSize)
                        {
                            if (winnerId == NoneId)
                            {
                                winnerId = initVal;
                                GameState = State.Finished;
                            }

                            List<Point> points = new List<Point>
                            {
                                new Point(i, 0),
                                new Point(i, PadSize - 1)
                            };
                            successLines.Add(points);
                        }
                    }
                }
            }

            if (GameState == State.Draw)
            {
                if (Callback != null)
                {
                    Callback.OnGameStateChanged(NoneId, GameState);
                }
                return true;
            }
            else if (GameState == State.Finished)
            {
                if (Callback != null)
                {
                    Callback.OnGameStateChanged(winnerId, GameState);
                }
                return true;
            }

            return false;
        }

        public List<List<Point>> GetWinLines() { return successLines; }

    }

    

}
