using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Retro2D
{
    public class ControllerX
    {
        public class Button
        {
            public int _id;     // id of this button
            public int _idKeyB; // keyboard button id
            public int _idPad;  // gamepad id        : -1 to avoid conflict with gamepad0 
            public int _idPadB; // gamepad button id : -1 to avoid conflict with button0 

            public bool _isPressed; // if button is pressed !

            public Button(int id, int idKeyB, int idPad, int idPadB)
            {
                _id = id;
                _idKeyB = idKeyB;
                _idPad = idPad;
                _idPadB = idPadB;
            }
        }
        
        Button[] _arrayButton = new Button[Microsoft.Xna.Framework.Input.GamePad.MaximumGamePadCount];
        static int _numGamePad;
        public bool _isAssignButton = false;    // if Button need to be assigned
        public bool _isAssignButtonOK = false;  // if button is assigned
        public int _currentAssignIdButton = -1;    // Current button id need to be assigned
        Player _currentAssignPlayer = null;    // Current player who own the current button to be assigned

        public ControllerX()
        {
            _numGamePad = NumGamePad();
        }
        public static int NumGamePad()
        {
            int numGamePad = 0;
            for (int i = 0; i < Microsoft.Xna.Framework.Input.GamePad.MaximumGamePadCount; i++)
            {
                if (Microsoft.Xna.Framework.Input.GamePad.GetState(i).IsConnected)
                    numGamePad++;
            }
            return numGamePad;
        }
        public void SetButton(int id, int idKeyB, int idPad, int idPadB)
        {
            _arrayButton[id] = new Button(id,idKeyB,idPad,idPadB);
        }
        public void SetButton(int id, Button button)
        {
            _arrayButton[id] = button;
        }
        public void ForceButton(int id, bool pressed)
        {
            _arrayButton[id]._isPressed = pressed;
        }
        public bool GetButton(int id)
        {
            if (_arrayButton[id]._isPressed)
            {
                _arrayButton[id]._isPressed = false;
                return true;
            }
            // Check Keyboard !
            if (Keyboard.GetState().IsKeyDown((Keys)_arrayButton[id]._idKeyB))
                return true;

            //if (_numGamePad > 0)
            {
                if (_arrayButton[id]._idPadB >= 0)
                    if (Microsoft.Xna.Framework.Input.GamePad.GetState(_arrayButton[id]._idPad).IsButtonDown((Buttons)_arrayButton[id]._idPadB))
                        return true;


                //if (_arrayButton[id]._idDirection > 0)
                //    if (Joystick.GetState(_arrayButton[id]._idJoy).Axes[_arrayButton[id]._idAxis] > DEAD_ZONE)
                //        return true;

                //if (_arrayButton[id]._idDirection < 0)
                //    if (Joystick.GetState(_arrayButton[id]._idJoy).Axes[_arrayButton[id]._idAxis] < -DEAD_ZONE)
                //        return true;


            }

            return false;
        }
        public static Button GetControllerButton(int id)
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(Keys)).Length; i++)
            {
                if (Keyboard.GetState().IsKeyDown((Keys)i)) return new Button(id, i, -1, -1); // -1 evite le conflit avec le button0 du gamepad !
            }

            for (int i = 0; i < _numGamePad; i++) // All Joysticks
            {
                if (Microsoft.Xna.Framework.Input.GamePad.GetState(i).IsConnected) // if Joystick connected
                {
                    for (int j = 0; j < Enum.GetValues(typeof(Buttons)).Length; j++) // All Buttons of the Joystick
                    {
                        if (Microsoft.Xna.Framework.Input.GamePad.GetState(i).IsButtonDown((Buttons)j))
                            return new Button(id, 0, i, j);
                    }

                    //for (int l = 0; l < GamePad.GetCapabilities(i).AxisCount; l++)
                    //{
                    //    //if (Joystick.hasAxis(i, sf::Joystick::Axis(l))) // if Axis l exist !
                    //    {
                    //        if (GamePad.GetState(i).Axes[l] > DEAD_ZONE ||
                    //            GamePad.GetState(i).Axes[l] < -DEAD_ZONE)
                    //        {
                    //            return new Button(id, 0, i, 0, l, GamePad.GetState(i).Axes[l], -1); // -1 evite le conflit avec le button0 du gamepad !
                    //        }
                    //    }
                    //}
                }
            }
            return null;
        }

        public static void MapButton(ref Player mapPlayer, int idButton)
        {
            mapPlayer._controllerX._isAssignButton = true;
            mapPlayer._controllerX._isAssignButtonOK = false;
            mapPlayer._controllerX._currentAssignPlayer = mapPlayer;
            mapPlayer._controllerX._currentAssignIdButton = idButton;
        }
        public static void CancelMapButton(ref Player mapPlayer, int idButton)
        {
            mapPlayer._controllerX._isAssignButton = false;
            mapPlayer._controllerX._isAssignButtonOK = true;
            mapPlayer._controllerX._currentAssignPlayer = mapPlayer;
            mapPlayer._controllerX._currentAssignIdButton = idButton;
        }
        public void PollButton()
        {
            if (_currentAssignPlayer != null)
                if (_currentAssignPlayer._controllerX._isAssignButton)
                {
                    if (!_currentAssignPlayer._controllerX._isAssignButtonOK)
                    {
                        Button buttonMap = GetControllerButton(_currentAssignPlayer._controllerX._currentAssignIdButton);
                        if (buttonMap != null)
                        {
                            _currentAssignPlayer._controllerX.SetButton(_currentAssignPlayer._controllerX._currentAssignIdButton, buttonMap);
                            _currentAssignPlayer._controllerX._isAssignButtonOK = true;
                        }
                    }
                    else
                    {
                        //sf::Joystick::update();
                        if (!_currentAssignPlayer._controllerX.GetButton(_currentAssignPlayer._controllerX._currentAssignIdButton))
                        {
                            _currentAssignPlayer._controllerX._isAssignButton = false;
                            _currentAssignPlayer._controllerX._isAssignButtonOK = false;
                        }
                    }
                }
        }
        public Player MapPlayer()
        {
            return _currentAssignPlayer;
        }
        public int MapIdButton()
        {
            return _currentAssignIdButton;
        }
        public bool IsAssignButton()
        {
            return _isAssignButton;
        }
        public bool IsAssignButtonOK()
        {
            return _isAssignButtonOK;
        }

    }
}
