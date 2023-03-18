using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retro2D
{
    public class ButtonEvent
    {
        bool[] _prevButtons;
        bool[] _buttons;

        public ButtonEvent(int nbButtons)
        {
            _prevButtons = new bool[nbButtons];
            _buttons = new bool[nbButtons];
        }

        public void Clear()
        {
            for (int i = 0; i < _buttons.Length; ++i)
                _prevButtons[i] = _buttons[i];
        }
        public void Set(int button, bool isButton)
        {
            _buttons[button] = isButton;
        }
        public bool On(int button)
        {
            return !_prevButtons[button] && _buttons[button];
        }
        public bool Off(int button)
        {
            return _prevButtons[button] && !_buttons[button];
        }
        public bool Is(int button)
        {
            return _buttons[button];
        }
    }
}
