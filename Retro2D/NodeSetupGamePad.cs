using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using MonoGame.Extended;
using Retro2D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retro2D
{
    public class SetupGamePad : Gui.Base
    {

        #region Attributes

        MessageQueue _messageQueue = new MessageQueue();
        string _info = "";
        float _infoX = 0;
        float _infoY = 0;
        Color _infoColor = Color.Silver;
        Style _buttonStyle = new Style();
        public Addon.ConfigGamepad _configGamepad;

        Player _mainPlayer; // Main player who can controller the setup !

        bool submitButton = false;

        #endregion

        public Node SetButtonStyle(Style buttonStyle)
        {
            if (null != buttonStyle)
                _buttonStyle = buttonStyle;

            return this;
        }
        public SetupGamePad(Player mainPlayer, Style buttonStyle, Input.Mouse mouse) : base(mouse)
        {
            _mainPlayer = mainPlayer;
            //_nodeGui = nodeGui;
            _name = "ConfigGamePad";
            _type = UID.Get<Gui.Base>();

            _configGamepad = CreateAddon<Addon.ConfigGamepad>();
            _drag = CreateAddon<Addon.Draggable>();

            _naviGate = new NaviGate(this);

            SetSize(256, 100);
            SetPivot(128, 50);

            _buttonStyle = buttonStyle;

            this["Setup"] =
                new Gui.Button(mouse)
                .SetName("Setup")
                .This<Gui.Button>().SetStyle(_buttonStyle)
                .This<Gui.Button>()._drag.SetDraggable(false)
                .This<Gui.Button>().SetLabel("SETUP")
                .SetSize(64, 8)
                .SetPosition(128 - 128, -24)
                .This<Gui.Button>().OnMessage((m) => 
                {
                    if (m._message == "ON_PRESS")
                    {
                        _configGamepad.RecButton();
                        _naviGate.SetNaviGate(false);
                        //Console.WriteLine("Setup is pressed ! " + _navigate.IsNaviGate());
                        PostMessage("ON_SETUP");

                    }
                })
                .AppendTo(this);

            this["Ok"] =
                new Gui.Button(mouse)
                .SetName("Ok")
                .This<Gui.Button>().SetStyle(_buttonStyle)
                .This<Gui.Button>()._drag.SetDraggable(false)
                .This<Gui.Button>().SetLabel("OK")
                .SetSize(24, 8)
                .SetPosition(128 - 12, -24)
                //.OnUpdateAction(this["Setup"]._updateAction)
                .This<Gui.Button>().OnMessage((m) =>
                {
                    if (m._message == "ON_PRESS")
                    {
                        _configGamepad.Wait();
                        _naviGate.SetNaviGate(false);
                        //_navigate.SetFocus(-1);
                        _parent._naviGate.SetNaviGate(true);
                        //Console.WriteLine("Ok is pressed ! " + _navigate.IsNaviGate());
                        PostMessage("ON_OK");
                    }
                })
                .AppendTo(this);

            this["Test"] =
                new Gui.Button(mouse)
                .SetName("Test")
                .This<Gui.Button>().SetStyle(_buttonStyle)
                .This<Gui.Button>()._drag.SetDraggable(false)
                .This<Gui.Button>().SetLabel("TEST")
                .SetSize(64, 8)
                .SetPosition(128 + 70, -24)
                //.OnUpdateAction(this["Setup"]._updateAction)
                .This<Gui.Button>().OnMessage((m) => 
                {
                    if (m._message == "ON_PRESS")
                    {
                        _configGamepad.TestButton();
                        _naviGate.SetNaviGate(false);
                        //Console.WriteLine("Test is pressed ! " + _navigate.IsNaviGate());
                        PostMessage("ON_TEST");
                    }
                })
                .AppendTo(this);


            //Texture2D imgBase64 = null;

            //if (null != _window)
            //{
            //    byte[] data = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAQAAAABkCAYAAABpYO6eAAAAB3RJTUUH4QUJFBYkn5D41wAAABd0RVh0U29mdHdhcmUAR0xEUE5HIHZlciAzLjRxhaThAAAACHRwTkdHTEQzAAAAAEqAKR8AAAAEZ0FNQQAAsY8L / GEFAAAABmJLR0QA / wD / AP + gvaeTAAAKQ0lEQVR4nO1dObIeNRB + CycwCRzAkEGOAw4ARWgHUHAKOAQRV6DKiXMiYnwAQp4PQII5Ac9DyaBHo1 / SaOlV6q76yn7zz0i9fiNptqvjOK604eln3x4WIO2nUXz66Oej9q81SOeB5XwRVyAXQGl9evSW1mEUKxW / tA49umrLcxUOkXbCrA3SOvQiFHsJ0rqt7vtUf2kbtjUc0xZpHXrhBKALkvWwjaGUNknr0AsnAJ2QqA91ht18chxYqCW6w9EKzJzURgQsxd9T8NT6OEk4asXNnX8ztaOeAGoGzDr87vkNinNyRIDJ + A49yMV4NPdSzOafFAmwEgAMxky7s06vBQJLR4ceYMY2l3sY + VjSjZoAbq4Y5PbJ1RFw//LqOoKj3x7RrBuVxLgESOvCIZpjDHVjjQkVs4RhVmAvijMpxQigxMrUfUhjdRux7cMc/rfoHmoo1BJVHySNzs6xWoNA5ZQ0CKsXyYqgiluaexwkENcvKNpHbQwusFAGl5MAYCA4+3O7dNpEtQbQYlOsL8x20RqiPuvnHM41FINBWLFgVgFHfKQIANqHSQIojXCc9UuB4CQAGAhOW90WXXZIEQC0FYsEphuQKH4N2NFmrdgxFlgkMHXwrsUPgyCtw+42WNd/1vZZEhg+cPfih0GQ1mFXuO/nSWDoIC/+yyBI67Cb7lb1pvLFKAl0H+DFXw6CtA67wH2d98kICXTtLFn8vQ9+SOkonQir6+z5V9exlwSad+Qu/lmHSgXEWkFZgudfW789JNC0E1fxUzqMMxiWSMCKrp5/fX21ksDpDhzFL8GSHDZx2bM6PP/G+mghgdOGKAlA+tZayv6tEIAFPT3/xtqeJgDq4pdyPJcummy0quMOsZEkgeIPuxQ/RxCkbSvppWHl2mNCb2uNBIoHUhCA1kSj1E+rvaleGvX0/MNps5sAqIp/tg2ub7Bpsr33+rM2rBCDnhzUpCtsr0QCFxusF/8qJKD9bEVph7TvS+D8CCgXCVzsiE0A2MWPuS9nEHraWqHwtdiPXfxY+0n44ZQArBf/zDFcQRjdR/oDGr3Atl8ydqP5p8mG2FYam4sk0xaAUWdaJIDc7xTvgeNASe8RH0jGLpdHH3z510W7YRvcrjX/2AiAg33vr14fAb3HSdnSm/wWCz9FagMXAVDlXyx0WOy5bVrzr0gAGou/5shY/CUSwP7AIrVvViz+ki2tPpCOVUBpBAALPi3+0nEabEpJAP3LQPELQNjtQnlz/efFV1Ny2zAlfrGFsg+XeeHIv9+e3z60/+FX90f4G26jELL8iwytkX1zTJo76+e2UXximdJHcNtKZ/+cTWf2a4hRRC2PckP/luOkbYOjANQRADX7wrP8zfHoOvd/ypGAjwJ0C8fZP0g468czP9xG3S9F/rF8HBRLQqFHhL9DsceCT39zcaGQtNC5SQBb0AiAi32jwDM99fwfio8CdAp3/sHCp57/Q0HPP6z5P9Xda9L3AVDZmrax8xqA5vwLkLwRiMrWuA5gagqQCpwCuLhQyYufvn97hn/2+XenuRb3iceoF4wRgBT7ct0IRGGzjwDofEkFzoeBqG02NQIosWlt0c8MA7uYkZacspZ3rAQQhke9qLXXMgXA7lNKpG2Q7h9LZvUOBV4Ddf/owrUA0/s8PxxKjS4CYvRNOeyEbdSmANKLUBj9l6YAnAuA0kN4zP6x/MYy/8Jwbm8CYgSJOggtBCB9FQSrfyoC6C1+Lr9x9I/hO/IpABzWlIZILSv54djWVdiWoVipT3isxqHsmx++OGpYvf8R6VmZ71nxt9J/TdjWAGaKH7ZRc0xr8Z/1bW0hx6UsueL7+OX72bjH7ZhFmOv/1e2zI4f4OysJUA/BWi7jlS7ltbQ7OmQ767ulXZ8CtB8nNQVI9fjol/eOgHS/dDuV/+5unh4RtW1c+Sd2GTD3YE/raCDHjK1sCZ8dyOmiTUbOBpg3o0j3v6o8vn9xnfs/t4gQwFnxw8t7M8UJ28g9O2CRBFovwWEWn3T/FAKnAaUpwQ7yjrQCLm0iXVDS/WPJr09+v44FnxZ++I1LDzjnlxSREUDuzJs+3x//nnm8F7aRe39A6f0CLmtLrtA5iz9IGPZDcPYNRWwN4IwEapI7G7Weobz4XYLAgucufk0i+izASOHBOWZ6C+bIZRMvfhdOkTzbZ4XrMkztkkbrZcDWO6la2qr12Xp5keMy4CqQvBNwhVupR20/O95vBZ7Q1QmgHX4rsM5bgf1hoMEHMjgfBloB/jCQ0oeBOF8IMlKIZ1OH1qE8ZvFjBODspRgrkcDZh0E4XwgiVfzY/WO9EMTEG4EkjuUIQO1vJwC+WPRAmkAwfWaGAEqOXemVYLltK5BAy2fBNOcfhPQUAttnaARAGQSNBEDlr9y2Hb4OrDn/IqQXESn8ZZ4ApAJB6a/S9lhQVoBhs4b8G8kl7aNPmE8Pf2gdhpWcyf11YExbz47nnNNygdpmzvy7++PrI0XLcVy+bDke/duA3F/MKX0bkFo4vkATfbnCF4iiHVw+o+wjyKvX37zt4/G7P15DxO3Ugu1LE68FjxIf602/B5j7zbqEIEMisIpoh7Q/XQoShgH+eXBe2zQM8bEvxWnxCXX+5Yb/XFMAzMW/+PfDD1oXA3OOTOf/uXUAzQHA1GlGjxQadNLQBkSJANL90m2a8+9/d2Vijyg45mK5OT/1OgD312epBQ7NVxqmS369mXIdgCz/IDtonApouA9Am09WhjZft04B0pGB1vy7uDELnVH+Fa6RgJ/552SFKw05ocq/sOKP3WZNyPMPsgHmCACLuUZuscS8LVObP1aHRn9L3giE7Y90BHCxk3US8OK3r5dGv0vcCkxd/AEXO2ITAJYhFI/0cjmfor2VodX3nA8DUfigiQACrJOABl0p29tBN60xWKn4A4oHUZGA1oSj0k+zvdrh+YfTZu1p0uKBFARAFVjNOmm01Yp+O8WE0tYhAgjYhQR2sNEqdoiNVPEHnDayOgmsbNsqelLGSNJ+yv5bij/gtCFKAoBO4AwER58WCssKqH0plX/UfaAQQAA1CaSOoeiLk2gsFb8VXT3/+vpqfY1cc6NcJJBz2Ei/Uk+6WSkoi/D8a+u35x2SXY1zk0DNoWeQ0lGi35109vyr69j7AtnuTiRJQDPcJ+5raZ+MvD16qDMngUvnS+uwm+5W9abyxeir44c7dRL4z/nSOuyqv2XdMX0w892Iqc53JwHrtkvPWT0G87bPfjRmWoldSWBHm7Vix1hgFH8AijK7kUDvirCDB9J5wZl/WJ+LQ1MqksDKgYj2SX92y5HHTvmH1SaqgjAQ0s6icL50gjvaiUA6XyjzD7NdEmVXYmMK1nXQwvOvHaRBsM7GVKzroIfnXxtYAmGNjf2svw48/+pgCwIMhMZgQN38rL8WPP/KEAmEJlbOrexL6+SggeffJcSDwX0tN+3Pi35PeP79AzWBKAVkJii5ttL+pO13yGL3/BMPwFlASkFpQa4tafscurFb/v0NpX2mWtSxtbcAAAAASUVORK5CYII=");
            //    MemoryStream ms = new MemoryStream(data);
            //    imgBase64 = Texture2D.FromStream(_window._graphics.GraphicsDevice, ms);
            //}

            _configGamepad.SetBitmap(Draw._gamePadSNES);

            _configGamepad.SetButton((int)SNES.BUTTONS.START, new Rectangle(138, 45, 12, 6));
            _configGamepad.SetButton((int)SNES.BUTTONS.SELECT, new Rectangle(106, 45, 12, 6));
            _configGamepad.SetButton((int)SNES.BUTTONS.UP, new Rectangle(42, 29, 12, 13));
            _configGamepad.SetButton((int)SNES.BUTTONS.DOWN, new Rectangle(42, 61, 12, 13));
            _configGamepad.SetButton((int)SNES.BUTTONS.LEFT, new Rectangle(26, 46, 13, 12));
            _configGamepad.SetButton((int)SNES.BUTTONS.RIGHT, new Rectangle(57, 46, 13, 12));
            _configGamepad.SetButton((int)SNES.BUTTONS.A, new Rectangle(218, 46, 12, 12));
            _configGamepad.SetButton((int)SNES.BUTTONS.B, new Rectangle(202, 62, 12, 12));
            _configGamepad.SetButton((int)SNES.BUTTONS.X, new Rectangle(202, 30, 12, 12));
            _configGamepad.SetButton((int)SNES.BUTTONS.Y, new Rectangle(186, 46, 12, 12));
            _configGamepad.SetButton((int)SNES.BUTTONS.L, new Rectangle(37, 0, 42, 5));
            _configGamepad.SetButton((int)SNES.BUTTONS.R, new Rectangle(177, 0, 42, 5));

            _naviGate.SetNaviNodeFocusAt(1);

        }
        public Node SetPlayer(Player player)
        {
            _configGamepad.SetPlayer(player);
            return this;
        }
        public Node SetMainPlayer(Player mainPlayer)
        {
            _mainPlayer = mainPlayer;
            return this;
        }
        public Node SetController(Controller controller)
        {
            _configGamepad.SetController(controller);
            return this;
        }
        public Node SetInfo(string info, Color infoColor, float infoX = 0, float infoY = 0)
        {
            _info = info;
            _infoColor = infoColor;
            _infoX = infoX;
            _infoY = infoY;

            return this;
        }
        public override Node Init()
        {
 

            return this;
        }
        public override Node Update(GameTime gameTime)
        {
            _naviGate.Update(UID.Get<Gui.Base>());

            // Ne PAS déplacer cette line ! pour l'enregistrement des buttons
            submitButton = _mainPlayer.GetButton((int)SNES.BUTTONS.A)!=0 || _configGamepad._player.GetButton((int)SNES.BUTTONS.A)!=0;

            if (_naviGate.IsNaviGate)
            {
                _naviGate.UpdateSubmitButton(submitButton);

                Input.Button.OnPress("Left", _mainPlayer.GetButton((int)SNES.BUTTONS.LEFT)!=0 || _configGamepad._player.GetButton((int)SNES.BUTTONS.LEFT)!=0);
                Input.Button.OnPress("Right", _mainPlayer.GetButton((int)SNES.BUTTONS.RIGHT)!=0 || _configGamepad._player.GetButton((int)SNES.BUTTONS.RIGHT)!=0);

                this["Setup"].SetVisible(true);
                this["Test"].SetVisible(true);
                this["Ok"].SetVisible(true);

                if (_configGamepad._isWait)
                {
                    if (Input.Button.OnPress("Left")) _naviGate.ToPrevNaviNode(UID.Get<Gui.Base>());
                    if (Input.Button.OnPress("Right")) _naviGate.ToNextNaviNode(UID.Get<Gui.Base>());
                    if (_naviGate.OnChangeFocus) PostMessage("ON_FOCUS_CHANGE");
                }
            }
            else
            {
                this["Setup"].SetVisible(false);
                this["Test"].SetVisible(false);
                this["Ok"].SetVisible(false);
            }

            if (_configGamepad._isRec || _configGamepad._isTest) // On config controller !
            {
                if (_configGamepad._isRec) // Evite d'enregistrer le bouton de confirmation lors du premier button à enregistrer !
                    //_configGamepad.SetButtonExec(_navigate.IsSubmit()); // Don't forget this for make a safe record controller button
                    _configGamepad.SetButtonExec(submitButton); // Don't forget this for make a safe record controller button


               bool stopTest = (_mainPlayer.GetButton((int)SNES.BUTTONS.L)!=0 && _mainPlayer.GetButton((int)SNES.BUTTONS.START)!=0) ||
                               (_configGamepad._controller.GetButton((int)SNES.BUTTONS.L) != 0 && _configGamepad._controller.GetButton((int)SNES.BUTTONS.START) != 0);

                _configGamepad.Update(Controller.MaxButtons, stopTest);

                if (_configGamepad._endConfig) // End config gamepad button !
                {
                    _configGamepad.Wait();
                    _naviGate.SetNaviGate(true);

                    if (!_configGamepad._isTest)
                        PostMessage("ON_END_REC");
                    else
                        PostMessage("ON_STOP_TEST");
                }

                if (_configGamepad._isRecOK) // Record button OK !
                {
                    PostMessage("ON_REC_OK");
                }

            }

            UpdateChilds(gameTime);

            return base.Update(gameTime);
        }
        public override Node Render(SpriteBatch batch)
        {
            base.Render(batch);

            _configGamepad.Render(batch);

            RenderChilds(batch);


            //if (null != _parent)
            //{
            //    if (null != _parent._navigate)
            //    {
            //        if (_parent._navigate.IsNaviGate())
            //        {
            //            if (_navi._isFocus)
            //                batch.DrawRectangle(Gfx.AddRect(AbsRectF(), new Rectangle(-4, -4, 8, 8)), Color.YellowGreen, 1);

            //            if (_navi._isOver && !_navi._isClick && !_navi._isFocus)
            //                batch.DrawRectangle(AbsRectF(), Color.YellowGreen * .8f);
            //        }

            //    }

            //}

            //if (Node._showNodeInfo)
            //{
            //    _drag.Render(batch);
            //    batch.DrawRectangle(AbsRectF(), Color.Red);
            //}


            //Draw.CenterStringXY(batch, _nodeGui._font, "isNAVIGATE = " + _navigate.IsNaviGate(), (int)AbsX(), (int)AbsY() + 50, Color.Silver);
            Draw.CenterStringXY(batch, _style._font, _info, AbsX+_infoX, AbsY+_infoY , _infoColor);


            return this;
        }

    }
}
