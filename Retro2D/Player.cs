using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retro2D
{
    public class Player
    {
        Pad _pad;
        public Pad Pad => _pad;
        int _maxButtons = 0;

        public int _id { get; private set; }
        public string _name { get; private set; }
        //public Controller _controller { get; private set; }
        public ControllerX _controllerX { get; private set; }

        public List<Controller> _controllers { get; private set; }

        public Player(int id, string name = "", int maxButtons = 0)
        {
            _id = id;
            _name = name;
            _maxButtons = maxButtons;
            //_controller = new Controller();
            _controllerX = new ControllerX();

            _controllers = new List<Controller>();

            AddController(new Controller()); // Main Controller 

            _pad = new Pad(maxButtons);

        }
        public void PollButton()
        {
            for (int i=0; i<_maxButtons; ++i)
            {
                _pad.SetButton(i, GetButton(i)!=0);
            }
            _pad.PollButton();

        }
        public Player AddController(Controller controller)
        {
            _controllers.Add(controller);
            return this;
        }
        public Player RemoveController(Controller controller)
        {
            _controllers.Remove(controller);
            return this;
        }
        public int GetButton(int idButton)
        {
            int result = 0;

            //if (_controllers.Count > 0)
                for (int i=0; i<_controllers.Count; i++)
                {
                    //if (i < 0 || i > _controllers.Count)
                    //{
                    //    Console.WriteLine(" Error Player.GetButton() : i is out of limit !");
                    //    return false;
                    //}
                    //Console.WriteLine("Player id = " + _id);
                    result = result + _controllers[i].GetButton(idButton);
                }

            return result;
        }
        public Player SetController(int idController, Controller controller)
        {
            if (null != _controllers.ElementAt(idController))
                _controllers[idController] = controller;

            return this;
        }

        public Player Get()
        {
            return this;
        }
        public void SetId(int id)
        {
            _id = id;
        }
        public void SetName(string name)
        {
            _name = name;
        }
        //public void SetController(Controller controller)
        //{
        //    _controller = controller;
        //}
        //public bool GetButton(int button) { return _controller.GetButton(button);}
        public void SetControllerX(ControllerX controllerX)
        {
            _controllerX = controllerX;
        }

    }
}
