using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClassicOOXX
{
    public class GamePad
    {
        public static int NoneId
        {
            get { return 0; }
        }
        public static int PlayerId
        {
            get { return 1; }
        }
        public static int ComId
        {
            get { return -1; }
        }

        public static int PadSize
        {
            get { return 3; }
        }

        public int[][] PadValues { get; }

        public GamePad()
        {
            PadValues = new int[PadSize][];
            for (int i = 0; i < PadSize; i++)
            {
                PadValues[i] = new int[PadSize];
                for (int j = 0; j < PadSize; j++)
                {
                    PadValues[i][j] = NoneId;
                }
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
        public int Select(int x, int y, int id)
        {
            if (!Selectable(x, y)) { return EvaluateWinner(); }

            PadValues[x][y] = id;
            return EvaluateWinner();
        }

        // 找贏家
        private int EvaluateWinner()
        {
            // 先找2條對角
            // 左上->右下
            int x = 0;
            int y = 0;
            int initVal = PadValues[x][y];
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
                if (successCnt == PadSize) { return initVal; }
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
                if (successCnt == PadSize) { return initVal; }
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
                        if (successCnt == PadSize) { return initVal; }
                    }
                    else
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
                        if (successCnt == PadSize) { return initVal; }
                    }
                }
            }

            return NoneId;
        }
        
    }

    

}
