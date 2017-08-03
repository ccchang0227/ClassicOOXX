using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicOOXX
{

    public interface ISimpleAIAction
    {
        void OnLoad();
        void OnSelect(int x, int y);
    }

    public class SimpleAI
    {

        private ISimpleAIAction callback = null;

        public SimpleAI(ISimpleAIAction callback)
        {
            this.callback = callback;
        }

        private GamePad gamePad = null;
        public void Load(GamePad gamePad)
        {
            this.gamePad = gamePad;
            if (null != callback)
            {
                callback.OnLoad();
            }
        }

        public void MyTurn()
        {

        }



    }
}
