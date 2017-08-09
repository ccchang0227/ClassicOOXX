using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;

namespace ClassicOOXX
{

    public interface ISimpleAIAction
    {
        void OnLoad();
        void OnMeasuring();
        void OnSelect(int x, int y);
    }

    public class SimpleAI
    {

        private ISimpleAIAction callback = null;

        private bool loaded = false;

        public SimpleAI(ISimpleAIAction callback)
        {
            this.callback = callback;
        }

        private GamePad gamePad = null;
        public void Load(GamePad gamePad)
        {
            this.gamePad = gamePad;

            PadEstimation = new int[GamePad.PadSize][];
            for (int i = 0; i < GamePad.PadSize; i++)
            {
                PadEstimation[i] = new int[GamePad.PadSize];
                for (int j = 0; j < GamePad.PadSize; j++)
                {
                    PadEstimation[i][j] = 0;
                }
            }

            PointRecord.Clear();

            loaded = true;

            if (null != callback)
            {
                callback.OnLoad();
            }
        }

        public void GameOver()
        {
            loaded = false;

            PointRecord.Clear();
            PadEstimation = null;
            gamePad = null;
        }

        public void MyTurn()
        {
            if (!loaded)
            {
                return;
            }
            if (gamePad.GameState != State.Playing)
            {
                return;
            }

            if (callback != null)
            {
                callback.OnMeasuring();
            }

            Thread.Sleep(500);

            Measure();
        }

        public int[][] PadEstimation { get; private set; }
        private List<Point> PointRecord = new List<Point>();

        private void Measure()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int x, y;
            int PadSize = GamePad.PadSize;

            int[][] corners = { new int[]{ 0, 0},
                                new int[] { 0, PadSize - 1 },
                                new int[] { PadSize - 1, 0 },
                                new int[] { PadSize - 1, PadSize - 1 } };

            Console.Out.WriteLine("allBlank = " + gamePad.IsAllBlank());
            if (gamePad.IsAllBlank())
            {
                // 全空白的情況，從角開始下
                int[] possibleX = { 0, PadSize - 1};
                int[] possibleY = { 0, PadSize - 1 };
                x = random.Next(possibleX.Length);
                y = random.Next(possibleY.Length);
                x = possibleX[x];
                y = possibleY[y];

                PointRecord.Add(new Point(x, y));

                if (callback != null)
                {
                    callback.OnSelect(x, y);
                }

                return;
            }
            //if (gamePad.TurnCount == 2)
            //{
                // 後手，第一次下 (隨機)
                int rand = -1;
                int val;
                do
                {
                    rand = random.Next((int)Math.Pow(PadSize, 2));
                    x = rand / PadSize;
                    y = rand % PadSize;
                    val = gamePad.PadValues[x][y];
                } while (val != GamePad.NoneId);

                PointRecord.Add(new Point(x, y));

                if (callback != null)
                {
                    callback.OnSelect(x, y);
                }

            //}
            if (gamePad.TurnCount == 3)
            {
                // 第二次下
                
            }

            Reset();
            
            
        }

        private void Reset()
        {
            for (int i = 0; i < GamePad.PadSize; i++)
            {
                for (int j = 0; j < GamePad.PadSize; j++)
                {
                    PadEstimation[i][j] = 0;
                }
            }
        }

        private void EstimateRecord(int padVal, int x, int y)
        {

        }

    }
}
