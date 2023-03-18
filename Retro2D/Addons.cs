using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retro2D
{
    public static class Addon
    {
        public static class StaticType<T>
        {
            public static int _type = 0;
        }
        public static class StaticDefine<T>
        {
            public static bool _define = false;
        }
        public class Type
        {
            public static Dictionary<string, int> _mapType = new Dictionary<string, int>();
            public static int Id(string componentName, bool createNewComponentType = false)
            {
                // if Name of component don't exist then create a new component name
                // by increase +1 with the highest id of component !
                if (!_mapType.ContainsKey(componentName))
                {
                    if (createNewComponentType)
                    {
                        int lastComponent = 0;

                        if (_mapType.Count > 0)
                        {
                            // Get the highest element values in the map !
                            lastComponent = _mapType.Values.Last();
                        }

                        _mapType[componentName] = lastComponent + 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                return _mapType[componentName];
            }
            public static void ConsoleLogAll()
            {
                foreach (KeyValuePair<string, int> entry in _mapType)
                {
                    Console.WriteLine(" - " + entry.Value + " : " + entry.Key + "");
                }
            }
        }

        public class Base
        {
            public Node _node = null;
            public int _type = 0;
            public virtual T Clone<T>(Node node) where T : Base
            {
                T clone = (T)MemberwiseClone();
                clone.SetNode(node);
                return clone;
            }
            public Node SetNode(Node node)
            {
                _node = node;
                return _node;
            }
        }
        public class Helper<T> : Base where T : new()
        {
            protected Helper()
            {
                if (!StaticDefine<T>._define) // If not defined then define !
                {
                    StaticDefine<T>._define = true;
                    _type = StaticType<T>._type = Addon.Type.Id(typeof(T).Name, true);
                }
                else
                    _type = StaticType<T>._type;
            }

            //public T clone()
            //{
            //    T clone = new T();
            //    clone = clone.cloneMe();
            //    return clone;
            //}
        }
        public class Navi : Helper<Navi>
        {

            public bool _onOver = false;  // trigger :Mouse over the Node
            public bool _isOver = false;  // status : Mouse over the Node

            public bool _onOut = false;   // trigger : Mouse out of Node
            public bool _isOut = false;   // statuc : Mouse out of Node

            public bool _isClickable = false; // status : Node is Clickable 
            public bool _onClick = false; // trigger : Node is Clicked
            public bool _isClick = false; // status : Node is Clicked

            public bool _onPress = false; // trigger : on pressed navi
            public bool _isPress = false; // status : is pressed navi

            public bool _onRelease = false; // trigger : on pressed navi
            public bool _isRelease = false; // status : is pressed navi

            public bool _isFocusable = true; // focusable by default !
            public bool _onFocus = false; // trigger : is focused navi
            public bool _isFocus = false; // status : is focused navi

            public Navi Clone()
            {
                return (Navi)MemberwiseClone();
            }
        }
        public class Loop : Helper<Loop>
        {

            bool _run = false;
            bool _onEnd = false;
            bool _onBegin = false;
            public float _current = 0;   // current position
            public float _begin = 0;     // begin at
            public float _end = 0;       // end at
            float _direction = 0; // direction of the loop
            Loops _loopType = 0;

            public Node SetLoop(float current, float begin, float end, float direction, Loops loopType)
            {
                _current = current;
                _begin = begin;
                _end = end;
                _direction = direction;
                _loopType = loopType;

                return _node;
            }
            public Node Start()
            {
                _run = true;
                return _node;
            }
            public Node ReStart()
            {
                _run = true;
                _current = _begin;
                return _node;
            }
            public Node Stop()
            {
                _run = false;
                return _node;
            }
            public bool OnBegin() { return _onBegin; }
            public bool OnEnd() { return _onEnd; }
            public void Update()
            {
                if (_run)
                {
                    _current += _direction;

                    _onEnd = false;
                    _onBegin = false;

                    if (_direction > 0 && _current > _end)
                    {
                        _current = _end;
                        _onEnd = true;
                    }

                    if (_direction < 0 && _current < _begin)
                    {
                        _current = _begin;
                        _onBegin = true;
                    }

                    if (_loopType == Loops.ONCE)
                    {
                        if (_onEnd || _onBegin)
                            Stop();
                    }    
                    else if (_loopType == Loops.REPEAT)
                    {
                        if (_onEnd)
                            _current = _begin;
                        if (_onBegin)
                            _current = _end;
                    }
                    else if (_loopType == Loops.PINGPONG)
                    {
                        if (_onEnd || _onBegin)
                            _direction = -_direction;
                    }

                }
            }
        }
        public class Draggable : Helper<Draggable>
        {
            bool _isDraggable = false;
            public bool IsDraggable => _isDraggable;
            bool _onDrag = false;
            public bool OnDrag => _onDrag;
            bool _offDrag = false;
            public bool OffDrag => _offDrag;
            bool _isDrag = false;
            public bool IsDrag => _isDrag;
            bool _isDragRectNode = true;
            bool _isLimitRect = false;
            public bool IsLimitRect => _isLimitRect;
            float _dragX = 0;
            float _dragY = 0;
            float relX = 0; // parent X
            float relY = 0; // parent Y

            public RectangleF _dragRect { get; private set; }
            public RectangleF _limitRect = new RectangleF();
            public Node _containerNode{get; private set;}

            //Node _nodeLimitRect;
            public Node SetDraggable(bool isDraggable)
            {
                _isDraggable = isDraggable;
                return _node;
            }
            public Node SetLimitRect(bool isLimitRect)
            {
                _isLimitRect = isLimitRect;
                return _node;
            }
            public Node SetLimitRect(RectangleF limitRect)
            {
                _isLimitRect = true;
                _limitRect = Gfx.CloneRelRectF(limitRect);
                return _node;
            }
            public Node SetLimitRect(Node containerNode)
            {
                if (null != containerNode)
                {
                    _isLimitRect = true;
                    _containerNode = containerNode;
                    _limitRect = Gfx.CloneRelRectF(_containerNode._rect);
                }
                return _node;
            }
            public RectangleF DragRect()
            {
                return _dragRect;
            }
            public Node SetDragRect(RectangleF dragRect)
            {
                //_dragRect.X = dragRect.X;
                //_dragRect.Y = dragRect.Y;
                //_dragRect.Width = dragRect.Width;
                //_dragRect.Height = dragRect.Height;
                _dragRect = dragRect;

                return _node;
            }
            public Node SetDragRectNode(bool isDragRectNode)
            {
                _isDragRectNode = isDragRectNode;

                return _node;
            }
            //public Draggable SetLimitRect(Node nodeLimitRect)
            //{
            //    _nodeLimitRect = nodeLimitRect;
            //    return this;
            //}
            public void Update(Input.Mouse mouse)
            {
                _onDrag = false;
                _offDrag = false;

                if (_isDragRectNode)
                {
                    _node.UpdateRect();
                    _dragRect = _node.AbsRect;

                }

                relX = 0;
                relY = 0;

                if (null != _node._parent)
                {
                    relX = _node._parent.AbsX;
                    relY = _node._parent.AbsY;

                    //RectangleF relLimitRect = new RectangleF(_limitRect.X + relX, _limitRect.Y + relY, _limitRect.Width, _limitRect.Height);
                    //_limitRect = relLimitRect;
                }

                if (null != _containerNode) // Update Rect of container if exist
                {
                    relX = _containerNode.AbsX;
                    relY = _containerNode.AbsY;

                    //_limitRect = Gfx.CloneRelRectF(_containerNode._rect);
                    //_containerNode.UpdateRect();
                    //_limitRect = _containerNode.AbsRect;

                }

                _node.UpdateRect();

                if (null != mouse && _isDraggable)
                {
                    _node._navi._isOver = Collision2D.PointInRect(new Vector2(mouse._x, mouse._y), _dragRect);

                    if (_node._navi._isOver)
                    {
                        //Console.Write("< isOverDraggable >");
                        mouse._isOver = true;
                    }

                    if (mouse._isMove && _isDrag && _node._navi._isFocus)
                    {
                        _node._x = mouse._x - _dragX;
                        _node._y = mouse._y - _dragY;
                    }

                    if (mouse._down)
                    {

                        if (_node._navi._isOver)
                        {
                            if (_isDrag) _node._navi._isFocus = true;

                            if (!_isDrag && (mouse._button == 1) &&
                                !mouse._drag && !mouse._reSize)
                            {
                                _onDrag = true;
                                _isDrag = true;
                                mouse._drag = true;
                                //log(0,"< Draggable is On >");
                            }
                        }
                    }
                    else
                    {
                        if (_isDrag)
                        {
                            _isDrag = false;
                            _offDrag = true;
                        }
                    }


                    if (_isDrag)
                    {
                        _dragX = mouse._x - _node._x;
                        _dragY = mouse._y - _node._y;

                        _node.UpdateRect();
                    }
                }

                if (_isLimitRect)
                {
                    if (null != _limitRect)
                    {
                        if (_node._rect.X < _limitRect.X)
                        {
                            _node._x = _limitRect.X + _node._oX;
                            //_mouse->_x = _rect.x + _dragX;
                            _node.UpdateRect();
                            //std::cout << "< Out of LimitRect LEFT >";
                        }
                        if (_node._rect.Y < _limitRect.Y)
                        {
                            _node._y = _limitRect.Y + _node._oY;
                            //_mouse->_y = _rect.y + _dragY;
                            _node.UpdateRect();
                            //std::cout << "< Out of LimitRect TOP >";
                        }
                        if (_node._rect.X > _limitRect.Width - _node._rect.Width)
                        {
                            _node._x = _limitRect.Width - _node._rect.Width + _node._oX;
                            //_mouse->_x = _rect.x + _dragX;
                            _node.UpdateRect();
                            //std::cout << "< Out of LimitRect RIGHT >";
                        }
                        if (_node._rect.Y > _limitRect.Height - _node._rect.Height)
                        {
                            _node._y = _limitRect.Height - _node._rect.Height + _node._oY;
                            //_mouse->_y = _rect.y + _dragY;
                            _node.UpdateRect();
                            //std::cout << "< Out of LimitRect BOTTOM >";
                        }

                    }

                }

            }
            public void Render(SpriteBatch batch)
            {
                if (null != _limitRect)
                {
                    RectangleF rect = _limitRect;
                    rect.Offset(relX, relY);
                    //batch.DrawRectangle(_limitRect, Color.Red, 1);
                    Draw.Rectangle(batch, rect, Color.Red, 1);
                    Draw.Rectangle(batch, _dragRect, Color.YellowGreen, 1);
                }
            }

        }
        public class Resizable : Helper<Resizable>
        {
            bool _isResizable = false;

            bool _isResize = false;
            int _tolerance = 4;

            int _x1 = 0;
            int _y1 = 0;
            int _x2 = 0;
            int _y2 = 0;

            //float _dragX = 0;
            //float _dragY = 0;

            bool _isOverN = false;
            bool _isOverS = false;
            bool _isOverW = false;
            bool _isOverE = false;
            bool _isOverNW = false;
            bool _isOverNE = false;
            bool _isOverSW = false;
            bool _isOverSE = false;

            bool _isDragN = false;
            bool _isDragS = false;
            bool _isDragW = false;
            bool _isDragE = false;
            bool _isDragNW = false;
            bool _isDragNE = false;
            bool _isDragSW = false;
            bool _isDragSE = false;

            public bool IsResize()
            {
                return _isResize;
            }
            public Node SetResizable(bool isResizable)
            {
                _isResizable = isResizable;
                return _node;
            }
            public Node Init(int tolerance)
            {
                _tolerance = tolerance;
                SetResize();
                return _node;
            }
            public Node SetTolerance(int tolerance)
            {
                _tolerance = tolerance;
                return _node;
            }
            private Node SetResize()
            {
                _x1 = (int)_node._rect.X;
                _y1 = (int)_node._rect.Y;
                _x2 = (int)_node._rect.X + (int)_node._rect.Width;
                _y2 = (int)_node._rect.Y + (int)_node._rect.Height;

                return _node;
            }
            private Node GetResize()
            {
                _node._x = _node._rect.X = _x1 - ((null != _node._parent) ? (int)_node._parent._x : 0);
                _node._y = _node._rect.Y = _y1 - ((null != _node._parent) ? (int)_node._parent._y : 0);

                _node._rect.Width = _x2 - _node._rect.X - ((null != _node._parent) ? (int)_node._parent._x : 0);
                _node._rect.Height = _y2 - _node._rect.Y - ((null != _node._parent) ? (int)_node._parent._y : 0);

                return _node;

            }
            public void Update(Input.Mouse mouse, int minW = 8, int minH = 8)
            {
                if (_isResizable)
                {
                    if (mouse._reSize)
                    {
                        //if (null != _node._parent)
                        //{
                        //    if (null != _node._parent._navigate)
                        //        if (_node._parent._navigate.IsNaviGate())
                        //            _node._navigate.SetFocus(_node._index);
                        //}

                        GetResize();
                    }
                    else
                        SetResize();

                    if (mouse._up)
                    {
                        _isResize = false;

                        _isOverN = false;
                        _isOverS = false;
                        _isOverW = false;
                        _isOverE = false;
                        _isOverNW = false;
                        _isOverNE = false;
                        _isOverSW = false;
                        _isOverSE = false;

                        _isDragN = false;
                        _isDragS = false;
                        _isDragW = false;
                        _isDragE = false;
                        _isDragNW = false;
                        _isDragNE = false;
                        _isDragSW = false;
                        _isDragSE = false;
                    }

                    int x = (int)mouse._x;
                    int y = (int)mouse._y;

                    // Test isOVER 
                    bool isOver = false;

                    if (Misc.InRange(x, _node._rect.X + _tolerance*2, _node._rect.X + _node._rect.Width - _tolerance*2) &&
                        Misc.InRange(y, _node._rect.Y - _tolerance, _node._rect.Y + _tolerance) &&
                        !mouse._reSize)
                        isOver = _isOverN = true;

                    if (Misc.InRange(x, _node._rect.X + _tolerance*2, _node._rect.X + _node._rect.Width - _tolerance*2) &&
                        Misc.InRange(y, _node._rect.Y + _node._rect.Height - _tolerance, _node._rect.Y + _node._rect.Height + _tolerance) &&
                        !mouse._reSize)
                        isOver = _isOverS = true;

                    if (Misc.InRange(x, _node._rect.X - _tolerance, _node._rect.X + _tolerance) &&
                        Misc.InRange(y, _node._rect.Y + _tolerance*2, _node._rect.Y + _node._rect.Height - _tolerance*2) &&
                        !mouse._reSize)
                        isOver = _isOverW = true;

                    if (Misc.InRange(x, _node._rect.X + _node._rect.Width - _tolerance, _node._rect.X + _node._rect.Width + _tolerance) &&
                        Misc.InRange(y, _node._rect.Y + _tolerance*2, _node._rect.Y + _node._rect.Height - _tolerance*2) &&
                        !mouse._reSize)
                        isOver = _isOverE = true;

                    if (Misc.InCircle(x, y, 0, _node._rect.X, _node._rect.Y, _tolerance * 2) && !mouse._reSize)
                        isOver = _isOverNW = true;

                    if (Misc.InCircle(x, y, 0, _node._rect.X + _node._rect.Width, _node._rect.Y, _tolerance * 2) && !mouse._reSize)
                        isOver = _isOverNE = true;

                    if (Misc.InCircle(x, y, 0, _node._rect.X, _node._rect.Y + _node._rect.Height, _tolerance * 2) && !mouse._reSize)
                        isOver = _isOverSW = true;

                    if (Misc.InCircle(x, y, 0, _node._rect.X + _node._rect.Width, _node._rect.Y + _node._rect.Height, _tolerance * 2) && !mouse._reSize)
                        isOver = _isOverSE = true;

                    // Test isCLICK
                    if (_node._navi._isFocus) // Resizable only if focused
                    {
                        if (_isOverN && mouse._down && !_isResize && !mouse._drag)
                        {
                            _isDragN = true;
                            _isResize = true;
                            mouse._reSize = true;
                        }
                        if (_isOverS && mouse._down && !_isResize && !mouse._drag)
                        {
                            _isDragS = true;
                            _isResize = true;
                            mouse._reSize = true;
                        }
                        if (_isOverW && mouse._down && !_isResize && !mouse._drag)
                        {
                            _isDragW = true;
                            _isResize = true;
                            mouse._reSize = true;
                        }
                        if (_isOverE && mouse._down && !_isResize && !mouse._drag)
                        {
                            _isDragE = true;
                            _isResize = true;
                            mouse._reSize = true;
                        }

                        if (_isOverNW && mouse._down && !_isResize && !mouse._drag)
                        {
                            _isDragNW = true;
                            _isResize = true;
                            mouse._reSize = true;
                        }
                        if (_isOverNE && mouse._down && !_isResize && !mouse._drag)
                        {
                            _isDragNE = true;
                            _isResize = true;
                            mouse._reSize = true;
                        }
                        if (_isOverSW && mouse._down && !_isResize && !mouse._drag)
                        {
                            _isDragSW = true;
                            _isResize = true;
                            mouse._reSize = true;
                        }
                        if (_isOverSE && mouse._down && !_isResize && !mouse._drag)
                        {
                            _isDragSE = true;
                            _isResize = true;
                            mouse._reSize = true;
                        }
                    }
                    
                    // test isDRAG
                    if (_isDragN && mouse._isMove && _isResize)
                    {
                        if (_node._rect.Height > minH || mouse._moveUP) // Limit Vertical Resize 
                            _y1 = mouse._y;
                    }

                    if (_isDragS && mouse._isMove && _isResize)
                    {
                        if (_node._rect.Height > minH || mouse._moveDOWN) // Limit Vertical Resize 
                            _y2 = mouse._y;
                    }

                    if (_isDragW && mouse._isMove && _isResize)
                    {
                        if (_node._rect.Width > minW || mouse._moveLEFT) // Limit Horizontal Resize 
                            _x1 = mouse._x;
                    }

                    if (_isDragE && mouse._isMove && _isResize)
                    {
                        if (_node._rect.Width > minW || mouse._moveRIGHT) // Limit Horizontal Resize 
                            _x2 = mouse._x;
                    }

                    if (_isDragNW && mouse._isMove && _isResize)
                    {
                        if ((_node._rect.Height > minH || mouse._moveUP) &&
                            (_node._rect.Width > minW || mouse._moveLEFT))
                        {
                            _x1 = mouse._x;
                            _y1 = mouse._y;
                        }
                    }
                    if (_isDragNE && mouse._isMove && _isResize)
                    {
                        if ((_node._rect.Height > minH || mouse._moveUP) &&
                            (_node._rect.Width > minW || mouse._moveRIGHT))
                        {
                            _x2 = mouse._x;
                            _y1 = mouse._y;
                        }
                    }
                    if (_isDragSW && mouse._isMove && _isResize)
                    {
                        if ((_node._rect.Height > minH || mouse._moveDOWN) &&
                            (_node._rect.Width > minW || mouse._moveLEFT))
                        {
                            _x1 = mouse._x;
                            _y2 = mouse._y;
                        }
                    }
                    if (_isDragSE && mouse._isMove && _isResize)
                    {
                        if ((_node._rect.Height > minH || mouse._moveDOWN) &&
                            (_node._rect.Width > minW || mouse._moveRIGHT))
                        {
                            _x2 = mouse._x;
                            _y2 = mouse._y;
                        }
                    }

                }

            }
            public void Render(SpriteBatch batch, Input.Mouse mouse, SpriteFont font)
            {
                //if (_isResize && Node._showNodeInfo)
                //    batch.DrawRectangle
                //    (
                //        new RectangleF((float)_x1 + .5f, (float)_y1 + .5f, (float)_x2-_x1 + 1.5f, (float)_y2-_y1 + 1.5f), 
                //        new Color(85,250,0,55), 4
                //    );

                if (_node._navi._isFocus && _isResizable && Node._showNodeInfo)
                {
                    string str = String.Format("x1 {0}, y1 {1} : x2 {2}, y2 {3} : {4}", _x1, _y1, _x2, _y2,
                        (mouse._reSize)? "GetResize()" : "SetResize()");
                    Draw.FillRectangle(batch, new Rectangle(mouse._x,mouse._y, (int)font.MeasureString(str).X+2, (int)font.MeasureString(str).Y+2), Color.Black);
                    batch.DrawString(font, str, new Vector2(mouse._x+4, mouse._y+4), Color.Gold);
                }

                if (!mouse._down)// && _node._navi._isFocus)
                {
                    int padding = 6;
                    int W = _x2 - _x1;
                    int H = _y2 - _y1;
                    int x = mouse._x;
                    int y = mouse._y;

                    if (_isOverN)
                    {
                        Draw.Triangle(batch, x + .5f, _y1 + .5f - padding,
                                                x + .5f - 8, _y1 + .5f - padding + 8,
                                                x + .5f + 8, _y1 + .5f - padding + 8,
                                                new Color(255, 0, 0));
                    }
                    if (_isOverS)
                    {
                        Draw.Triangle(batch, x + .5f, _y2 + .5f + padding,
                                                x + .5f - 8, _y2 + .5f + padding - 8,
                                                x + .5f + 8, _y2 + .5f + padding - 8,
                                                new Color(255, 0, 0));
                    }
                    if (_isOverW)
                    {
                        Draw.Triangle(batch, _x1 + .5f - padding, y + .5f,
                                                _x1 + .5f - padding + 8, y + .5f - 8,
                                                _x1 + .5f - padding + 8, y + .5f + 8,
                                                new Color(255, 0, 0));
                    }
                    if (_isOverE)
                    {
                        Draw.Triangle(batch, _x2 + .5f + padding, y + .5f,
                                                _x2 + .5f + padding - 8, y + .5f - 8,
                                                _x2 + .5f + padding - 8, y + .5f + 8,
                                                new Color(255, 0, 0));
                    }


                    if (_isOverNW)
                    {
                        Draw.Triangle(batch, _x1 + .5f - padding, _y1 + .5f - padding,
                                                _x1 + .5f - padding + 10, _y1 + .5f - padding,
                                                _x1 + .5f - padding, _y1 + .5f - padding + 10,
                                                new Color(255, 0, 0));
                    }
                    if (_isOverNE)
                    {
                        Draw.Triangle(batch, _x2 + .5f + padding, _y1 + .5f - padding,
                                                _x2 + .5f + padding - 10, _y1 + .5f - padding,
                                                _x2 + .5f + padding, _y1 + .5f - padding + 10,
                                                new Color(255, 0, 0));
                    }
                    if (_isOverSW)
                    {
                        Draw.Triangle(batch, _x1 + .5f - padding, _y2 + .5f + padding,
                                                _x1 + .5f - padding + 10, _y2 + .5f + padding,
                                                _x1 + .5f - padding, _y2 + .5f + padding - 10,
                                                new Color(255, 0, 0));
                    }
                    if (_isOverSE)
                    {
                        Draw.Triangle(batch, _x2 + .5f + padding, _y2 + .5f + padding,
                                                _x2 + .5f + padding - 10, _y2 + .5f + padding,
                                                _x2 + .5f + padding, _y2 + .5f + padding - 10,
                                                new Color(255, 0, 0));
                    }
                }

            }
        }
        public class ViewZone : Helper<ViewZone>
        {
            class View
            {
                public RenderTarget2D _renderTarget2D;
                public SpriteBatch _spriteBatch;
                public Rectangle _rect;

                public View CloneMe()
                {
                    View clone = (View)MemberwiseClone();

                    //clone._renderTarget2D = _renderTarget2D;
                    //clone._spriteBatch = _spriteBatch;

                    return clone;
                }
            }

            Dictionary<int, View> _views = new Dictionary<int, View>();

            public SpriteBatch Batch(int viewId)
            {
                return _views[viewId]._spriteBatch;
            }
            public Node SetViewZone(Window window, int viewId, Rectangle rect)
            {
                View view = new View();
                view._renderTarget2D = new RenderTarget2D(window._graphics.GraphicsDevice, rect.Width, rect.Height, false, SurfaceFormat.Rgba64, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
                view._spriteBatch = new SpriteBatch(window._graphics.GraphicsDevice);
                view._rect = rect;

                if (!_views.ContainsKey(viewId))
                    _views.Add(viewId, view);

                return _node;
            }
            public Node Update(int viewId, Rectangle rect)
            {
                _views[viewId]._rect = rect;
                return _node;
            }
            public Node BeginRender(Window window, int viewId)
            {
                window._graphics.GraphicsDevice.SetRenderTarget(_views[viewId]._renderTarget2D);
                _views[viewId]._spriteBatch.Begin
                (
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    DepthStencilState.None,
                    RasterizerState.CullCounterClockwise
                );
                window._graphics.GraphicsDevice.Clear(Color.Transparent);

                return _node;
            }
            public Node EndRender(Window window, int viewId)
            {
                _views[viewId]._spriteBatch.End();
                window._graphics.GraphicsDevice.SetRenderTarget(window.NativeRenderTarget);
                window.Batch.Draw
                (
                    _views[viewId]._renderTarget2D,
                    _views[viewId]._rect,
                    new Rectangle(0, 0, _views[viewId]._rect.Width, _views[viewId]._rect.Height),
                    Color.White
                );

                return _node;
            }


        }
        public class ConfigGamepad : Helper<ConfigGamepad>
        {
            #region Attributes

            public bool _isWait = true;
            public bool _isTest = false;
            public bool _isRec = false;
            public bool _isRecOK = false;
            public bool _isRecSafe = false; // if Record button is safe , ex: just after select "setup"
            public bool _endConfig = true; // true if config/test button is finish !

            int _prevRecButton;
            int _nextRecButton;
            int _currentRecButton;

            int _tempoEndTest = 0; // Hold any buttons to quit Test config !

            Color _color = new Color(250, 200, 0);
            int _alpha = 0;
            int _vAlpha = 25;

            Dictionary<int, bool> _mapButton = new Dictionary<int, bool>();
            Dictionary<int, Rectangle> _mapRect = new Dictionary<int, Rectangle>();

            public Player _player;
            public Controller _controller;
            Texture2D _bitmap;

            #endregion

            public Node SetBitmap(Texture2D bitmap)
            {
                _bitmap = bitmap;
                return _node;
            }
            public Node SetButton(int buttonId, Rectangle rect, bool isButtonPress = false)
            {
                _mapRect.Add(buttonId, rect);
                _mapButton.Add(buttonId, isButtonPress);
                return _node;
            }
            public Node SetPlayer(Player player)
            {
                _player = player;
                _controller = _player._controllers[Controller.MAIN];
                return _node;
            }
            public Node SetController(Controller controller)
            {
                _player.SetController(Controller.MAIN, controller);
                return _node;
            }
            public Node SetButtonExec(bool buttonExec)
            {
                if (!_isRecSafe)
                    _isRecSafe = !buttonExec;

                return _node;
            }

            public Node Wait()
            {
                _isRec = false;
                _isTest = false;
                _isWait = true;
                return _node;
            }

            public Node RecButton()
            {
                _isRec = true;
                _isTest = false;

                _currentRecButton = 1;
                _prevRecButton = 1;

                Controller.MapButton(ref _player, Controller.MAIN, _currentRecButton);

                _endConfig = false;

                //Console.WriteLine("Begin Record !! ");

                return _node;
            }
            public Node TestButton()
            {
                _isRec = false;
                _isTest = true;
                _currentRecButton = 0;

                Controller.CancelMapButton(ref _player, Controller.MAIN, _currentRecButton);

                _endConfig = false;

                return _node;
            }
            public void Update(int maxButton, bool stopTest = false)
            {
                _isRecOK = false;
                _isWait = !(_isRec || _isTest);

                if (_isRec)
                {

                    if (_isRecSafe) // || _controller->mapIdButton() == PAD_A)
                        _controller.PollButton(Controller.MAIN);

                    _color = new Color(255, 10, 110, _alpha);

                    if (null != _controller)
                    {
                        // Check if is not the prev recorded button & next button == 1
                        if (_controller.GetButton(_prevRecButton)==0 && _nextRecButton == 1)
                        {
                            _nextRecButton = 0;
                            Controller.MapButton(ref _player, Controller.MAIN, _currentRecButton);
                        }

                        if (_controller.IsAssignButtonOK && _nextRecButton == 0)
                        {
                            _isRecOK = true;

                            //Console.WriteLine("Rec Ok CallBack !");

                            if (_currentRecButton < maxButton - 1)
                            {
                                _nextRecButton = 1;
                                _prevRecButton = _currentRecButton;
                                ++_currentRecButton;
                            }
                            else
                            {
                                // Stop REC !
                                _isRec = false;
                                _isRecSafe = false;
                                _endConfig = true;
                                //std::cout << " END RECORD \n";
                                //Console.WriteLine("End Record !! ");
                            }


                        }
                    }

                }

                if (_isTest)
                {

                    int _anyButton = 0;

                    for (int i = 0; i < _mapButton.Count; ++i)
                    {
                        if (null != _controller)
                        {
                            _mapButton[i] = _controller.GetButton(i)!=0;

                            if (_mapButton[i])
                                ++_anyButton;
                        }

                    }

                    if (_anyButton > 0)
                        _tempoEndTest += _anyButton;
                    else
                        _tempoEndTest = 0;


                    if (_tempoEndTest > 256)
                    {
                        _tempoEndTest = 0;
                        // Stop TEST !
                        _isTest = false;
                        _endConfig = true;
                    }

                    _color = new Color(200, 250, 250, 200);

                    //if (_controller.GetButton((int)SNES_PAD.L) &&
                    //    _controller.GetButton((int)SNES_PAD.START))
                    //{
                    //    // Stop TEST !
                    //    _isTest = false;
                    //    _endConfig = true;
                    //}

                    if (stopTest)
                    {
                        // Stop TEST !
                        _isTest = false;
                        _endConfig = true;
                    }

                }


                _alpha += _vAlpha;

                if (_alpha < 0)
                {
                    _alpha = 0;
                    _vAlpha = 25;
                }

                if (_alpha > 255)
                {
                    _alpha = 255;
                    _vAlpha = -25;
                }


            }
            public void Render(SpriteBatch batch)
            {
                if (null != _bitmap)
                    batch.Draw(_bitmap, new Vector2(_node.AbsRect.X, _node.AbsRect.Y), Color.White);

                if (_isRec)
                {

                    Rectangle rect = Gfx.AddRect(_mapRect[_currentRecButton], new Rectangle( _node.AbsRect.X, _node.AbsRect.Y,0,0));

                    Draw.FillRectangle(batch, rect, _color);

                    float mx = rect.X + rect.Width / 2;
                    float my = rect.Y + rect.Height / 2;

                    Rectangle rect2 = Gfx.AddRect(rect, new Rectangle( -4,-4,8,8));

                    Color color = new Color(250, 50, 10, 255 - _alpha);

                    Draw.Rectangle(batch,rect2, color);

                    float cenX = rect2.Width / 2 + _alpha / 40;
                    float cenY = rect2.Height / 2 + _alpha / 40;
                    float len = 8;
                    float sz = 3;

                    Draw.Line(batch, mx - cenX, my, mx - cenX - len, my, color, sz);
                    Draw.Line(batch, mx + cenX, my, mx + cenX + len, my, color, sz);

                    Draw.Line(batch, mx, my - cenY - 1, mx, my - cenY - len - 1, color, sz);
                    Draw.Line(batch, mx, my + cenY, mx, my + cenY + len, color, sz);
                }

                if (_isTest)
                {

                    Draw.FillRectangle(batch, new Rectangle(_node.AbsRect.X, _node.AbsRect.Y-5,_tempoEndTest,4), new Color(250, 200, 50));

                    Draw.Rectangle(batch, new Rectangle(_node.AbsRect.X, _node.AbsRect.Y-5, 256, 4), new Color(50, 100, 150));

                    foreach (var it in _mapButton)
                    {
                        if (_mapButton.ContainsKey(it.Key))
                            if (it.Value)
                                if (_mapRect.ContainsKey(it.Key))
                                    Draw.FillRectangle(batch, Gfx.AddRect(_mapRect[it.Key], new Rectangle(_node.AbsRect.X, _node.AbsRect.Y, 0, 0)), _color);
                    }

                    foreach (var it in _mapButton)
                    {
                        if (_mapButton.ContainsKey(it.Key))
                            if (it.Value)
                                if (_mapRect.ContainsKey(it.Key))
                                {
                                    Rectangle rect = Gfx.AddRect(_mapRect[it.Key], new Rectangle(_node.AbsRect.X, _node.AbsRect.Y,0,0));

                                    Draw.FillRectangle(batch, rect, _color);
                                    batch.Draw(_bitmap, new Vector2(_node.AbsRect.X, _node.AbsRect.Y), Color.White);


                                    float mx = rect.X + rect.Width / 2;
                                    float my = rect.Y + rect.Height / 2;

                                    Rectangle rect2 = Gfx.AddRect(rect, new Rectangle ( -4,-4,8,8));

                                    Color color = new Color(50, 50, 150, 255 - _alpha);

                                    Draw.Rectangle(batch, rect2, color);

                                    float cenX = rect2.Width / 2 + _alpha / 40;
                                    float cenY = rect2.Width / 2 + _alpha / 40;
                                    float len = 8;
                                    float sz = 3;

                                    Draw.Line(batch, mx - cenX, my, mx - cenX - len, my, color, sz);
                                    Draw.Line(batch, mx + cenX, my, mx + cenX + len, my, color, sz);
                                    Draw.Line(batch, mx, my - cenY - 1, mx, my - cenY - len - 1, color, sz);
                                    Draw.Line(batch, mx, my + cenY, mx, my + cenY + len, color, sz);
                                }
                    }
                }

                
            }
        }

    }
    
}
