using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Retro2D
{
    public class Button : IClone<Button>
    {
        public int _id;      // Button id enum !
        public int _idKey;     // Keyboard Id
        public int _idJoy;     // Gamepad Id
        public int _idHat;   // Hat of Gamepad Id
        public int _idAxis;   // Axis of Stick Id
        public float _idDirection; // Direction of the Axis Id
        public int _idButton;  // Button of Gamepad Id
        [XmlIgnore] public bool _isPressed = false; // if button is pressed !

        public Button() { }
        public Button(int id = 0, int idKey = -1, int idJoy = 0, int idHat = 0, int idAxis = 0, float idDirection = 0, int idButton = -1)
        {
            _id = id;
            _idKey = idKey;
            _idJoy = idJoy;
            _idHat = idHat;
            _idAxis = idAxis;
            _idDirection = idDirection;
            _idButton = idButton;
        }
        public Button Clone()
        {
            return (Button)MemberwiseClone();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat
            (
                "[ _id = {0}, _idKey = {1}, _idJoy = {2}, _idHat = {3}, _idAxis = {4}, _idDirection = {5}, _idButton = {6} ]",
                _id, _idKey, _idJoy, _idHat, _idAxis, _idDirection, _idButton
            );
            return sb.ToString();
        }
    }

    public class Pad
    {
        List<bool> _onButtons = new List<bool>();
        List<bool> _offButtons = new List<bool>();
        List<bool> _isButtons = new List<bool>();
        List<bool> _isPushButtons = new List<bool>();

        int _nButton = 0;

        public Pad(int nButton)
        {
            _nButton = nButton;

            for (int i = 0; i < _nButton; ++i)
            {
                _onButtons.Add(false);
                _offButtons.Add(false);
                _isButtons.Add(false);
                _isPushButtons.Add(false);
            }
        }

        public Pad SetButton(int button, bool isButton)
        {
            _isButtons[button] = isButton;

            return this;
        }
        public bool IS(int button)
        {
            return _isButtons[(int)button];
        }
        public bool ON(int button)
        {
            return _onButtons[button];
        }
        public bool OFF(int button)
        {
            return _offButtons[button];
        }

        public void PollButton()
        {
            for (int i = 0; i < _nButton; ++i)
            {
                _onButtons[i] = false;
                _offButtons[i] = false;

                if (!_isButtons[i])
                {
                    if (_isPushButtons[i])
                        _offButtons[i] = true;

                    _isPushButtons[i] = false;
                }

                if (_isButtons[i] && !_isPushButtons[i])
                {
                    _isPushButtons[i] = true;
                    _onButtons[i] = true;
                }
            }
        }
    }

    public class SNES
    {
        public enum BUTTONS
        {
            NULL = 0,
            START,
            SELECT,
            UP,
            DOWN,
            LEFT,
            RIGHT,
            A, B, X, Y,
            L, R,
            MAX_BUTTONS
        }

        public static Dictionary<string, BUTTONS> SNESButtonDicoMap = new Dictionary<string, BUTTONS>
        {
            {"NULL", BUTTONS.NULL },
            {"START", BUTTONS.START },
            {"SELECT", BUTTONS.SELECT },
            {"UP", BUTTONS.UP },
            {"DOWN", BUTTONS.DOWN },
            {"LEFT", BUTTONS.LEFT },
            {"RIGHT", BUTTONS.RIGHT },
            {"B", BUTTONS.B },
            {"X", BUTTONS.X },
            {"A", BUTTONS.A },
            {"Y", BUTTONS.Y },
            {"L", BUTTONS.L },
            {"R", BUTTONS.R },
            {"MAX_BUTTONS", BUTTONS.MAX_BUTTONS },
        };

        public static List<string> SNESButtonDico = new List<string>
        {
            "NULL",
            "START",
            "SELECT",
            "UP",
            "DOWN",
            "LEFT",
            "RIGHT",
            "A",
            "B",
            "X",
            "Y",
            "L",
            "R",
            "MAX_BUTTONS"
        };

        public static string GetButtonDico(SNES.BUTTONS id)
        {
            if (id >= 0 && id < SNES.BUTTONS.MAX_BUTTONS)
                return SNESButtonDico[(int)id];

            return "NULL";
        }
    }


    public class Controller
    {
        #region Attributes

        public const int MAIN = 0;

        public const int MaxButtons = (int)SNES.BUTTONS.MAX_BUTTONS;
        public const int DEAD_ZONE = 3200;
        int _numJoystick;

        public Button[] _buttons; // = new Button[MaxButtons];
        //public List<Button> _buttons = new List<Button>(MAX_BUTTONS);

        [XmlIgnore] public bool IsAssignButton { get; private set; } = false;    // if Button need to be assigned
        [XmlIgnore] public bool IsAssignButtonOK { get; private set; } = false;  // if button is assigned
        [XmlIgnore] public int CurrentAssignIdButton { get; private set; } = -1;    // Current button id need to be assigned

        Player CurrentAssignPlayer = null;    // Current player who own the current button to be assigned

        #endregion

        public Controller()
        {

            _buttons = new Button[MaxButtons];

            _numJoystick = NumJoystick();

            for (int i=0; i<MaxButtons; ++i)
            {
                _buttons[i] = new Button(i, -1, 0, 0, 0, 0, -1); // don't forget -1 for idKey and idButton for avoid A button 
            }

            //Console.WriteLine("NumJoystick connected = " + _numJoystick);
        }

        public Controller AppendTo(Player player)
        {
            player.AddController(this);
            return this;
        }
        public Controller AsMainController(Player player)
        {
            player.SetController(MAIN, this);
            return this;
        }
        public void Copy(Controller controllerSrc) // Copy the buttons from another controller !
        {
            // iterate all valid buttons of  controller source !
            for (int i = 0; i < MaxButtons; ++i)
            {
                int id = controllerSrc._buttons[i]._id; // id of the button to clone
                Button buttonSrc = controllerSrc._buttons[id];

                // Check if joystick is connected or not, if not don"t assign this button
                //if (Joystick.GetState(buttonSrc._idJoy).IsConnected)
                {
                    _buttons[id] = buttonSrc.Clone();
                }
                //else
                //    Console.WriteLine("Joystick {0} not connected", buttonSrc._idJoy);
            }
        }

        // Debug
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i=0; i< MaxButtons; i++)
            {
                sb.Append(_buttons[i] + "\n");
            }

            return sb.ToString();
        }

        public int NumJoy()
        {
            return _numJoystick;
        }

        public static int NumJoystick()
        {
            int numJoystick = 0;
            for (int i=0;i<MaxButtons;i++)
            {
                if (Joystick.GetState(i).IsConnected)
                    numJoystick++;
            }
            
            return numJoystick;
        }

        public Controller SetButton(int id, int idKey, int idJoy, int idHat, int idAxis, float idDirection, int idButton)
        {
            _buttons[id] = new Button(id, idKey, idJoy, idHat, idAxis, idDirection, idButton);
            return this;
        }
        public Controller SetButton(Button button)
        {
            _buttons[button._id] = button;
            return this;
        }
        public void ForceButton(int id, bool pressed)
        {
            _buttons[id]._isPressed = pressed;
        }
        public int GetButton(int id)
        {
            
            //Console.Write(" _buttons.Length = " + _buttons.Length);
            //Console.Write(" id = " + id);
            //Console.WriteLine(" GetButton = " + _buttons[id]);

            if (_buttons[id]._isPressed)
            {
                _buttons[id]._isPressed = false;
                return 1;
            }

            // Check Keyboard !
            if (Keyboard.GetState().IsKeyDown((Keys)_buttons[id]._idKey))
                return 1;

            // Check Joystick !
            //if (_numJoystick > 0)
            if (Joystick.GetState(_buttons[id]._idJoy).IsConnected)
            {
                // Get Buttons
                if (_buttons[id]._idButton >= 0 && _buttons[id]._idButton < Joystick.GetState(_buttons[id]._idJoy).Buttons.Length)
                    if (Joystick.GetState(_buttons[id]._idJoy).Buttons[_buttons[id]._idButton] == ButtonState.Pressed)
                        return 1;

                // Get Hat or DPAD
                if (Joystick.GetCapabilities(_buttons[id]._idJoy).HatCount > 0 && _buttons[id]._idHat > 1)
                {
                    if (Joystick.GetState(_buttons[id]._idJoy).Hats[0].Up == ButtonState.Pressed && _buttons[id]._idHat == 8)
                        return 1;
                    if (Joystick.GetState(_buttons[id]._idJoy).Hats[0].Down == ButtonState.Pressed && _buttons[id]._idHat == 2)
                        return 1;
                    if (Joystick.GetState(_buttons[id]._idJoy).Hats[0].Left == ButtonState.Pressed && _buttons[id]._idHat == 4)
                        return 1;
                    if (Joystick.GetState(_buttons[id]._idJoy).Hats[0].Right == ButtonState.Pressed && _buttons[id]._idHat == 6)
                        return 1;
                }

                
                // Get Axis
                if (_buttons[id]._idDirection > 0)
                    if (Joystick.GetState(_buttons[id]._idJoy).Axes[_buttons[id]._idAxis] > DEAD_ZONE)
                        return Joystick.GetState(_buttons[id]._idJoy).Axes[_buttons[id]._idAxis];

                if (_buttons[id]._idDirection < 0)
                    if (Joystick.GetState(_buttons[id]._idJoy).Axes[_buttons[id]._idAxis] < -DEAD_ZONE)
                        return Joystick.GetState(_buttons[id]._idJoy).Axes[_buttons[id]._idAxis];


            }

            return 0;
        }
        public Button GetControllerButton(int id)
        {
            for (int i = 0; i < Enum.GetNames(typeof(Keys)).Length; i++)
            {
                if (Keyboard.GetState().IsKeyDown((Keys)i))
                {
                    //Console.WriteLine("GetButtonController : " + (Keys)i);

                    //return new Button( id, i, 0, 0, 0, 0, -1 ); // -1 evite le conflit avec le button0 du gamepad !
                    SetButton( id, i, 0, 0, 0, 0, -1 ); // -1 evite le conflit avec le button0 du gamepad !
                    return _buttons[id];
                }
            }

            for (int i = 0; i < _numJoystick; i++) // All Joysticks
            {
                if (Joystick.GetState(i).IsConnected) // if Joystick connected
                {
                    // Get Buttons
                    for (int j = 0; j < Joystick.GetCapabilities(i).ButtonCount; j++) // All Buttons of the Joystick
                    {
                        if (Joystick.GetState(i).Buttons[j] == ButtonState.Pressed)
                        {
                            //Console.WriteLine("GetButtonController : Joystick Button id = {0} Button = {1}", i, j);

                            //return new Button(id, 0, i, 0, 0, 0, j);
                            SetButton(id, -1, i, 0, 0, 0, j);
                            return _buttons[id];
                        }
                    }

                    // Get Hat or DPAD
                    if (Joystick.GetCapabilities(i).HatCount > 0)
                    {
                        if (Joystick.GetState(i).Hats[0].Up == ButtonState.Pressed) { SetButton(id, -1, i, 8, 0, 0, -1); return _buttons[id]; }
                        if (Joystick.GetState(i).Hats[0].Down == ButtonState.Pressed) {SetButton(id, -1, i, 2, 0, 0, -1); return _buttons[id]; }
                        if (Joystick.GetState(i).Hats[0].Left == ButtonState.Pressed) {SetButton(id, -1, i, 4, 0, 0, -1); return _buttons[id]; }
                        if (Joystick.GetState(i).Hats[0].Right == ButtonState.Pressed) {SetButton(id, -1, i, 6, 0, 0, -1); return _buttons[id]; }
                        //if (Joystick.GetState(i).Hats[0].Up == ButtonState.Pressed) return new Button(id, 0, i, 8, 0, 0, -1);
                        //if (Joystick.GetState(i).Hats[0].Down == ButtonState.Pressed) return new Button(id, 0, i, 2, 0, 0, -1);
                        //if (Joystick.GetState(i).Hats[0].Left == ButtonState.Pressed) return new Button(id, 0, i, 4, 0, 0, -1);
                        //if (Joystick.GetState(i).Hats[0].Right == ButtonState.Pressed) return new Button(id, 0, i, 6, 0, 0, -1);
                    }

                    //for (int l = 0; l < Joystick.GetCapabilities(i).AxisCount; l++)
                    for (int l = 0; l < 2; l++)
                    {
                        //if (Joystick.hasAxis(i, sf::Joystick::Axis(l))) // if Axis l exist !
                        {
                            if (Joystick.GetState(i).Axes[l] > DEAD_ZONE ||
                                Joystick.GetState(i).Axes[l] < -DEAD_ZONE)
                            {
                                //Console.WriteLine("GetButtonController : Joystick Button id = {0} Axes = {1}", i, l);

                                //return new Button(id, 0, i, 0, l, Joystick.GetState(i).Axes[l], -1); // -1 evite le conflit avec le button0 du gamepad !
                                SetButton(id, -1, i, 0, l, Joystick.GetState(i).Axes[l], -1); // -1 evite le conflit avec le button0 du gamepad !
                                return _buttons[id];
                            }
                        }
                    }
                }
            }
            return null;
        }
        //public void AssignButton(Controller controller, int id, Action<int, int> run) // Fonction bloquante , wait KEY or JOY inputs
        //{

        //    Misc.Log(" > Assign "+ SNESButtonDico[id] +" Button < ");

        //    while (true)
        //    {
        //        Button button = GetControllerButton(id);
        //        if (button != null)
        //        {
        //            //player._controller.SetButton(id, button);
        //            controller.SetButton(button);
        //            break;
        //        }

        //        //run(player._id, id);

        //    }

        //    Misc.Log(" * OK * \n");

        //    while (controller.GetButton(id))
        //    {
        //        //sf::Joystick::update();
        //    }

        //}
        public static void MapButton(ref Player mapPlayer,int idController, int idButton)
        {
            mapPlayer._controllers[idController].IsAssignButton = true;
            mapPlayer._controllers[idController].IsAssignButtonOK = false;
            mapPlayer._controllers[idController].CurrentAssignPlayer = mapPlayer;
            mapPlayer._controllers[idController].CurrentAssignIdButton = idButton;
        }
        //public static void MapButton(ref Player mapPlayer, int idButton)
        //{
        //    mapPlayer._controller._isAssignButton = true;
        //    mapPlayer._controller._isAssignButtonOK = false;
        //    mapPlayer._controller._currentAssignPlayer = mapPlayer;
        //    mapPlayer._controller._currentAssignIdButton = idButton;
        //}
        public static void CancelMapButton(ref Player mapPlayer, int idController, int idButton)
        {
            mapPlayer._controllers[idController].IsAssignButton = false;
            mapPlayer._controllers[idController].IsAssignButtonOK = true;
            mapPlayer._controllers[idController].CurrentAssignPlayer = mapPlayer;
            mapPlayer._controllers[idController].CurrentAssignIdButton = idButton;
        }

        public void PollButton(int idController)
        {
            if (CurrentAssignPlayer != null)
                if (CurrentAssignPlayer._controllers[idController].IsAssignButton)
                {
                    if (!CurrentAssignPlayer._controllers[idController].IsAssignButtonOK)
                    {
                        Button buttonMap = GetControllerButton(CurrentAssignPlayer._controllers[idController].CurrentAssignIdButton);
                        if (buttonMap != null)
                        {
                            //_currentAssignPlayer._controller.SetButton(_currentAssignPlayer._controller._currentAssignIdButton, buttonMap);
                            CurrentAssignPlayer._controllers[idController].SetButton(buttonMap);
                            CurrentAssignPlayer._controllers[idController].IsAssignButtonOK = true;
                        }
                    }
                    else
                    {
                        //sf::Joystick::update();
                        if (CurrentAssignPlayer._controllers[idController].GetButton(CurrentAssignPlayer._controllers[idController].CurrentAssignIdButton)==0)
                        {
                            CurrentAssignPlayer._controllers[idController].IsAssignButton = false;
                            CurrentAssignPlayer._controllers[idController].IsAssignButtonOK = false;
                        }
                    }
                }
        }
        public Player MapPlayer()
        {
            return CurrentAssignPlayer;
        }
        //public int MapIdButton()
        //{
        //    return _currentAssignIdButton;
        //}
        //public bool IsAssignButton()
        //{
        //    return _isAssignButton;
        //}
        //public bool IsAssignButtonOK()
        //{
        //    return _isAssignButtonOK;
        //}
    }
}
