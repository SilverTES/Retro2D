﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Retro2D
{
    public interface IClone<T>
    {
        T Clone();
    }

    public class Node : ZIndex, IClone<Node>
    {
        #region Attributes

        //Game _game;

        public static Node _root = new Node("root");

        // Node dictionary
        public Dictionary<string, Node> _node = new Dictionary<string, Node>();

        // Addon dictionary
        Dictionary<int, Addon.Base> _addon = new Dictionary<int, Addon.Base>();

        // Debug 
        public static bool _showNodeInfo = false;

        // Identity 
        public int _type = Const.NoIndex; // Type of Node
        public int _subType = Const.NoIndex; // SubType of Node 
        public List<int> _class = new List<int>(); // List of Class Selector 
        public string _name = ""; // name of the clip

        // Animation
        public int _curFrame { get; private set; } // Current frame of Clip
        public int _nbFrame = 0;

        // Status
        public bool _isPlay = true;    // if Clip is playing
        public bool _isActive = true;  // if Clip is active -> update()
        public bool _isVisible = true; // if Clip is visible -> render()
        public bool _isAppend = false; // If append to an Parent Clip !

        // Gui
        public Addon.Navi _navi = new Addon.Navi();

        // NaviNode - Navigate
        public NaviGate _naviGate = null; // need to create new instance in node or derived !!
        public Dictionary<Position, Node> _naviNodes = new Dictionary<Position, Node>();

        // Legacy
        public Node _parent = null;
        public Node _master = null;
        public Node _original = null;

        // World2D
        //public float _cameraMoveFactor;
        public float _x;
        public float _y;
        public float _alpha = 1f;

        public float AbsXF;
        public float AbsYF;

        public int AbsX;
        public int AbsY;

        public Vector2 XY = new Vector2(); // Relative position for update
        public Vector2 AbsXY = new Vector2(); // Absolute position for render


        //public ZIndex _zIndex = new ZIndex {_z = 0, _index = 0};
        public RectangleF _rect = new RectangleF();
        public Rectangle AbsRect = new Rectangle();
        public RectangleF AbsRectF = new RectangleF();

        public RectangleF _rectView = new RectangleF(); // Rect View : Camera or other

        // Offset : _rect._x, _rect._y <-> _x, _y
        public float _oX = 0;
        public float _oY = 0;
        public Vector2 OXY = new Vector2();

        // Collision Check
        //public bool _isCollide = false;
        //public bool _isCollidable = false;
        //public int _collideLevel = 0;  // Can make collide or not by choose Level !
        //public int _idCollideName = -1;
        //public int _idCollideIndex = -1;
        public HashSet<int> _vecCollideBy = new HashSet<int>(); // std::set for avoid duplicate elements

        // Messaging
        public Message _message = null;
        public bool HasMessage()
        {
            return null != _message;
        }
        public void EndMessage()
        {
            _message = null;
        }
        public void PollMessage()
        {
            if (null != _message)
            {
                OnMessage();
                _message = null;
            }
        }
        protected virtual void OnMessage() { }

        // map of Collide Zone(Rect) of Clip
        public Dictionary<int,  Collide.Zone> _collideZones = new Dictionary<int, Collide.Zone>();

        public IContainer<Node> _childs = new IContainer<Node>();

        public List<ZIndex> _vecZIndex = new List<ZIndex>();
        public ZIndex _frontZ = new ZIndex { _z = int.MaxValue, _index = 0 };
        public ZIndex _backZ = new ZIndex { _z = 0, _index = 0 };

        public Animate _animate = new Animate();
        public AnimateVec2 _animateVec2 = new AnimateVec2();

        // Action queue script Minor methods
        public Action<Node> _createAction = null;
        public Action<Node> _initAction = null;
        public Action<Node> _updateAction = null;
        public Action<Node, SpriteBatch> _renderAction = null;

        #endregion

        #region Methodes
        // --- Navigation
        public Node SetAsNaviNodeFocus()
        {
            if (null != _parent)
                if (null != _parent._naviGate)
                    _parent._naviGate.SetNaviNodeFocusAt(_index);

            return this;
        }
        public Node SetNaviNode(Position direction, Node node)
        {
            _naviNodes[direction] = node;
            return this;
        }
        public static void SetNaviNodeHorizontal(Node nodeLeft, Node nodeRight)
        {
            nodeLeft._naviNodes[Position.RIGHT] = nodeRight;
            nodeRight._naviNodes[Position.LEFT] = nodeLeft;
        }
        public static void SetNaviNodeVertical(Node nodeUp, Node nodeDown)
        {
            nodeUp._naviNodes[Position.DOWN] = nodeDown;
            nodeDown._naviNodes[Position.UP] = nodeUp;
        }

        // --- Constructor
        public Node() { }
        public Node(string name) { _name = name; }
        ~Node()
        {
            //Console.WriteLine("Kill Node");
        }
        //public Node SetGame<T>(T game) where T : Game
        //{   
        //    _game = game;
        //    return this;
        //}
        //public T GetGame<T>() where T : Game
        //{
        //    return (T)_game;
        //}

        // --- Clonage
        public Node GetMaster(Node node)
        {
            Node master = null;

            if (null != node._original)
                master = GetMaster(node._original);
            else
                master = node;

            return master;
        }
        public virtual Node Clone()
        {
            Node clone = (Node)MemberwiseClone();

            clone._initAction = _initAction;
            clone._updateAction = _updateAction;

            clone._animate = new Animate();

            clone._navi = _navi.Clone();

            //clone._button = _button.Clone();

            clone._childs = new IContainer<Node>();
            for (int i = 0; i < _childs.Count(); i++)
            {
                //clone.Append(_childs.At(i).Clone());
                if (null != _childs.At(i))
                    _childs.At(i).Clone().AppendTo(clone);
            }

            clone._node = new Dictionary<string, Node>();
            foreach (KeyValuePair<string, Node> node in _node)
            {
                clone._node[node.Key] = node.Value.Clone();
            }

            clone._collideZones = new Dictionary<int, Collide.Zone>();
            foreach (KeyValuePair<int, Collide.Zone> it in _collideZones)
            {
                clone._collideZones[it.Key] = it.Value.Clone();
            }

            clone._vecZIndex = new List<ZIndex>();
            for (int i = 0; i < _vecZIndex.Count(); i++)
            {
                clone._vecZIndex.Add(_vecZIndex[i]);
            }

            // -- Clone Addons
            clone._addon = new Dictionary<int, Addon.Base>();
            foreach (KeyValuePair<int, Addon.Base> entry in _addon)
            {
                clone._addon[entry.Key] = entry.Value.Clone<Addon.Base>(clone);

                //clone._addon[entry.Key]._node = clone; // Change component owner !! Important !
            }

            return clone;
        }
        public Node Add(Node node)
        {
            if (null != node)
            {
                node._parent = this;
                _childs.Add(node);
            }
            return this;
        }
        //private Node SetParent(Node parent)
        //{
        //    _parent = parent;
        //    return this;
        //}
        public virtual Node AppendTo(Node parent) // Child Append to Parent
        {
            if (null != parent)
            {
                _isAppend = true;
                _parent = parent;
                _parent.Add(this);
            }

            UpdateRect();
            return this;
        }
        public virtual Node Append(Node child) // Parent Append Child
        {
            if (null != child)
            {
                child._isAppend = true;
                _parent = this;
                Add(child);
            }

            UpdateRect();
            return this;
        }

        public Node this[string key]
        {
            get
            {
                return _node[key];
            }
            set
            {
                _node[key] = value;
                _node[key]._name = key;
            }
        }

        // --- Addons
        public T CreateAddon<T>() where T : Addon.Base, new()
        {
            T addon = new T();
            addon._node = this;
            return (T)addon;
        }

        public Node Attach(Addon.Base addon)
        {
            _addon.Add(addon._type, addon);
            _addon[addon._type]._node = this;
            return this;
        }
        public Node Attach<T>() where T : Addon.Base, new()
        {
            T addon = new T();

            _addon.Add(addon._type, addon);
            _addon[addon._type]._node = this;

            return this;
        }
        public Node Detach(int id)
        {
            _addon.Remove(id);
            return this;
        }
        public T Get<T>() where T : Addon.Base, new()
        {
            return (T)_addon[Addon.StaticType<T>._type];

        }
        public T GetVerif<T>() where T : Addon.Base, new()
        {
            T component = new T();
            Addon.Base c;
            if (_addon.TryGetValue(component._type, out c))
                return (T)_addon[component._type];
            else
                return null;
        }

        // --- Derived Node 
        public T This<T>() where T : Node
        {
            return (T)this;
        }
        // --- Minor Action
        public Node OnCreateAction(Action<Node> createAction)
        {
            _createAction = createAction;
            _createAction?.Invoke(this);
            return this;
        }
        public Node CreateAction()
        {
            _createAction?.Invoke(this);
            return this;
        }
        public Node OnInitAction(Action<Node> initAction)
        {
            _initAction = initAction;
            return this;
        }
        public Node InitAction()
        {
            _initAction?.Invoke(this);
            return this;
        }
        public Node OnUpdateAction(Action<Node> updateAction)
        {
            _updateAction = updateAction;
            return this;
        }
        public void UpdateAction()
        {
            _updateAction?.Invoke(this);
        }
        public Node OnRenderAction(Action<Node, SpriteBatch> renderAction)
        {
            _renderAction = renderAction;
            return this;
        }
        public void RenderAction(SpriteBatch batch)
        {
            _renderAction?.Invoke(this, batch);
        }

        public Node AddAnimate(String name, Func<float, float, float, float, float> easing, Tweening tweening)
        {
            _animate.Add(name, easing, tweening);
            return this;
        }

        // --- Identity
        public Node SetType(int type)
        {
            _type = type;
            return this;
        }
        public Node SetName(String name)
        {
            _name = name;
            return this;
        }

        // --- Childs
        public void KillMe()
        {
            //Misc.log(_index + " begin Kill !\n");
            //_parent._childs.Del(_index);
            _parent._childs.Del(this);
            //Misc.log(_index + " end Kill !\n");
            //_parent._vecClip.del(this);

        }
        public IContainer<Node> Childs()
        {
            return _childs;
        }
        public int NbNode()
        {
            return _childs.Count();
        }
        public int NbActive()
        {
            return _childs.NbActive();
        }
        public Node Index(int index)
        {
            return _childs.At(index);
        }

        public int KillAll()
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                        {
                            _childs.At(i).KillMe();
                            nbKill++;
                        }
                    }
                }

            return nbKill;
        }
        public int KillAllAndKeep(string name)
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                        {
                            if (_childs.At(i)._name != name)
                            {
                                _childs.At(i).KillMe();
                                nbKill++;
                            }
                        }
                    }
                }

            return nbKill;
        }
        public int KillAll(string name)
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i=0; i< _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                            if (_childs.At(i)._name == name)
                            {
                                _childs.At(i).KillMe();
                                nbKill++;
                            }
                    }
                }
            return nbKill;
        }
        public int KillAllAndKeep(int type)
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                            if (_childs.At(i)._type != type)
                            {
                                _childs.At(i).KillMe();
                                nbKill++;
                            }
                    }
                }
            return nbKill;
        }
        public int KillAll(int type)
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                            if (_childs.At(i)._type == type)
                            { 
                                _childs.At(i).KillMe();
                                nbKill++;
                            }
                    }
                }
            return nbKill;
        }
        public int KillAllAndKeep(int[] types)
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                        {
                            bool keep = false;

                            for (int t = 0; t < types.Length; t++)
                            {
                                if (_childs.At(i)._type == types[t])
                                {
                                    keep = true;
                                    break;
                                }
                            }

                            if (!keep)
                            {
                                _childs.At(i).KillMe();
                                nbKill++;
                            }
                        }

                    }
                }
            return nbKill;
        }
        public int KillAll(int[] types)
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        for (int t = 0; t < types.Length; t++)
                        {
                            if (null != _childs.At(i))
                                if (_childs.At(i)._type == types[t])
                                {
                                    _childs.At(i).KillMe();
                                    nbKill++;
                                }

                        }
                    }
                }
            return nbKill;
        }
        public int IndexByName(String name)
        {
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                            if (_childs.At(i)._name == name)
                                return i;
                    }
                }
            return 0;
        }


        public List<Node> GroupAll()
        {
            List<Node> nodes = new List<Node>();
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    if (null != _childs.At(i))
                        nodes.Add(_childs.At(i));
                }
            return nodes;
        }
        public List<Node> GroupOf(String name)
        {
            List<Node> nodes = new List<Node>();
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    if (null != _childs.At(i))
                        if (_childs.At(i)._name == name)
                            nodes.Add(_childs.At(i));
                }
            return nodes;
        }
        public List<Node> GroupOf(int type)
        {
            List<Node> nodes = new List<Node>();
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    if (null != _childs.At(i))
                        if (_childs.At(i)._type == type)
                            nodes.Add(_childs.At(i));
                }
            return nodes;
        }
        public List<Node> GroupOf(int[] types)
        {
            List<Node> nodes = new List<Node>();
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    for (int t=0; t<types.Length; t++)
                    {
                        if (null != _childs.At(i))
                            if (_childs.At(i)._type == types[t])
                                nodes.Add(_childs.At(i));
                    }
                }
            return nodes;
        }
        public List<Node> SubGroupOf(int subType)
        {
            List<Node> nodes = new List<Node>();
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    if (null != _childs.At(i))
                        if (_childs.At(i)._subType == subType)
                            nodes.Add(_childs.At(i));
                }
            return nodes;
        }
        public List<Node> SubGroupOf(int[] subTypes)
        {
            List<Node> nodes = new List<Node>();
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    for (int t = 0; t < subTypes.Length; t++)
                    {
                        if (null != _childs.At(i))
                            if (_childs.At(i)._subType == subTypes[t])
                                nodes.Add(_childs.At(i));
                    }
                }
            return nodes;
        }

        public void LogAll()
        {
            Console.WriteLine("- Node: name = \"{0}\" | index = {1} | type = {2}", _name, _index, _type);
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    if (null != _childs.At(i))
                        Console.WriteLine("   - Child Node: name = \"{0}\" | index = {1} | type = {2}", _childs.At(i)._name, _childs.At(i)._index, _childs.At(i)._type);
                }
        }


        // --- Status
        //public Node SetCollidable(bool isCollidable)
        //{
        //    _isCollidable = isCollidable;
        //    return this;
        //}
        //public Node SetCameraMoveFactor(float cameraMoveFactor)
        //{
        //    _cameraMoveFactor = cameraMoveFactor;
        //    return this;
        //}
        public Node SetRectView(RectangleF rectView)
        {
            _rectView = rectView;
            return this;
        }
        public Node UpdateRectView(float x, float y, float width, float height)
        {
            _rectView.X = x;
            _rectView.Y = x;
            _rectView.Width = width;
            _rectView.Height = height;
            return this;
        }
        public bool InRectView()
        {
            if (null != _parent)
                return Misc.InRect(AbsX, AbsY, _parent._rectView);

            return false;
        }
        public bool InRectView(RectangleF rectView)
        {
            return Misc.InRect(AbsX, AbsY, rectView);
        }
        public Node SetActive(bool isActive)
        {
            _isActive = isActive;

            if (_isActive)
                _curFrame = 0;

            return this;
        }
        public Node SetVisible(bool isVisible)
        {
            _isVisible = isVisible;
            return this;
        }
        //public Node SetMouse(Input.Mouse mouse)
        //{
        //    _mouse = mouse;
        //    return this;
        //}

        // --- World 2D
        public void UpdateRect()
        {
            _rect.X = _x - _oX;
            _rect.Y = _y - _oY;

            OXY.X = _oX;
            OXY.Y = _oY;
            // For determinate relative & absolute Clip position !
            if (null != _parent)
            {
                AbsRectF.X = _rect.X + _parent.AbsRectF.X;
                AbsRectF.Y = _rect.Y + _parent.AbsRectF.Y;

                AbsX = (int)(AbsXF = _parent.AbsRectF.X + _x);
                AbsY = (int)(AbsYF = _parent.AbsRectF.Y + _y);
            }
            else
            {
                AbsRectF.X = _rect.X;
                AbsRectF.Y = _rect.Y;

                AbsX = (int)(AbsXF = _x);
                AbsY = (int)(AbsYF = _y);

            }

            XY.X = _x;
            XY.Y = _y;

            AbsRectF.Width = _rect.Width;
            AbsRectF.Height = _rect.Height;

            AbsXY.X = AbsX;
            AbsXY.Y = AbsY;

            AbsRect.X = (int)AbsRectF.X;
            AbsRect.Y = (int)AbsRectF.Y;
            AbsRect.Width = (int)AbsRectF.Width;
            AbsRect.Height = (int)AbsRectF.Height;


            // Update Childs Rect !
            for (int index = 0; index < _childs.Count(); index++)
                if (null != _childs.At(index))
                    _childs.At(index).UpdateRect();

        }

        public Node SetPosition(float x, float y)
        {
            _x = x;
            _y = y;
            UpdateRect();
            return this;
        }
        public Node SetPosition(Vector2 position)
        {
            _x = position.X;
            _y = position.Y;
            UpdateRect();
            return this;
        }
        public Node SetPosition(Position position, Node parent = null)
        {
            float pW = 1, pH = 1;

            if (null == parent)
            {
                if (null != _parent)
                {
                    pW = _parent._rect.Width;
                    pH = _parent._rect.Height;
                }
                else
                {
                    return this;
                }
            }
            else
            {
                pW = parent._rect.Width;
                pH = parent._rect.Height;
            }

            switch (position)
            {
                case Position.M:
                    _x = pW / 2;
                    _y = pH / 2;
                    break;
                case Position.NW:
                    _x = 0;
                    _y = 0;
                    break;
                case Position.NE:
                    _x = pW;
                    _y = 0;
                    break;
                case Position.SW:
                    _x = 0;
                    _y = pH;
                    break;
                case Position.SE:
                    _x = pW;
                    _y = pH;
                    break;
                case Position.N:
                    _y = 0;
                    break;
                case Position.S:
                    _y = pH;
                    break;
                case Position.W:
                    _x = 0;
                    break;
                case Position.E:
                    _x = pW;
                    break;
                case Position.NM:
                    _x = pW / 2;
                    _y = 0;
                    break;
                case Position.SM:
                    _x = pW / 2;
                    _y = pH;
                    break;
                case Position.WM:
                    _x = 0;
                    _y = pH / 2;
                    break;
                case Position.EM:
                    _x = pW;
                    _y = pH / 2;
                    break;
                default:
                    break;
            }

            UpdateRect();
            return this;
        }
        public Node SetSize(float w, float h)
        {
            AbsRectF.Width = _rect.Width = w;
            AbsRectF.Height = _rect.Height = h;
            UpdateRect();
            return this;
        }
        public Node SetSize(Vector2 size)
        {
            SetSize(size.X, size.Y);
            return this;
        }
        public Node SetX(Position position)
        {
            if (null != _parent)
            {
                RectangleF pR = _parent._rect;

                switch (position)
                {
                    case Position.M:
                        _x = pR.Width / 2;
                        break;
                    case Position.W:
                        _x = pR.X;
                        break;
                    case Position.E:
                        _x = pR.Width;
                        break;
                    default:
                        break;
                }
            }
            return this;
        }
        public Node SetY(Position position)
        {
            if (null != _parent)
            {
                RectangleF pR = _parent._rect;

                switch (position)
                {
                    case Position.M:
                        _y = pR.Height / 2;
                        break;
                    case Position.N:
                        _y = pR.Y;
                        break;
                    case Position.S:
                        _y = pR.Height;
                        break;
                    default:
                        break;
                }
            }
            return this;
        }
        
        //public Node SetPosition(Position positionX , Position positionY)
        //{
        //    SetX(positionX);
        //    SetY(positionY);
        //    return this;
        //}
        //public Node SetPosition(float x, Position positionY)
        //{
        //    SetX(x);
        //    SetY(positionY);
        //    return this;
        //}
        //public Node SetPosition(Position positionX, float y)
        //{
        //    SetX(positionX);
        //    SetY(y);
        //    return this;
        //}

        public Node SetX(float x)
        {
            _x = x;
            UpdateRect();
            return this;
        }
        public Node SetY(float y)
        {
            _y = y;
            UpdateRect();
            return this;
        }
        public Node SetZ(int z)
        {
            _z = z;
            return this;
        }
        public Node SetPivotX(float oX)
        {
            _oX = oX;
            UpdateRect();
            return this;
        }
        public Node SetPivotY(float oY)
        {
            _oY = oY;
            UpdateRect();
            return this;
        }
        public Node SetPivot(float oX, float oY)
        {
            SetPivotX(oX);
            SetPivotY(oY);
            return this;
        }
        public Node SetPivot(Position position)
        {
            if (null != _rect)
            {
                float pW = _rect.Width;
                float pH = _rect.Height;

                switch (position)
                {
                    case Position.M:
                        _oX = pW / 2;
                        _oY = pH / 2;
                        break;
                    case Position.NW:
                        _oX = 0;
                        _oY = 0;
                        break;
                    case Position.NE:
                        _oX = pW;
                        _oY = 0;
                        break;
                    case Position.SW:
                        _oX = 0;
                        _oY = pH;
                        break;
                    case Position.SE:
                        _oX = pW;
                        _oY = pH;
                        break;
                    case Position.N:
                        _oY = 0;
                        break;
                    case Position.S:
                        _oY = pH;
                        break;
                    case Position.W:
                        _oX = 0;
                        break;
                    case Position.E:
                        _oX = pW;
                        break;
                    case Position.NM:
                        _oX = pW / 2;
                        _oY = 0;
                        break;
                    case Position.SM:
                        _oX = pW / 2;
                        _oY = pH;
                        break;
                    case Position.WM:
                        _oX = 0;
                        _oY = pH / 2;
                        break;
                    case Position.EM:
                        _oX = pW;
                        _oY = pH / 2;
                        break;
                    default:
                        break;
                }
            }
            return this;
        }
        // Main methods 

        public virtual Node Init() { return this; }
        public virtual Node Done() { return this; }
        public Node InitChilds()
        {
            for (int i = 0; i < _childs.Count(); i++)
                _childs.At(i).Init();

            return this;
        }
        public Node DoneChilds()
        {
            for (int i = 0; i < _childs.Count(); i++)
                _childs.At(i).Done();

            return this;
        }

        public virtual Node Update(GameTime gameTime) { return this; }
        public virtual Node Render(SpriteBatch batch) { return this; } // NonPremultiplied
        public virtual Node RenderAdditive(SpriteBatch batch) { return this; } // Additive
        public virtual Node RenderAlphaBlend(SpriteBatch batch) { return this; } // AlphaBlend 
        public virtual Node RenderOpaque(SpriteBatch batch) { return this; } // Opaque


        public void GotoFront(int index, int type = -1)
        {
            if (null != _childs.At(index))
            {
                List<Node> listNode;

                if (type == -1)
                    listNode = GroupAll();
                else
                    listNode = GroupOf(type);

                listNode = listNode.OrderByDescending(v => v._z).ToList(); // sort group by z !!

                int indexNode = _childs.At(index)._index;

                int indexSwap = 0;
                int it = 0;

                for (int i=0; i<listNode.Count; i++)
                {
                    if (listNode[i]._index == indexNode)
                    {
                        indexSwap = it;
                        break;
                    }
                    ++it;
                }

                for (int i = indexSwap; i < listNode.Count - 1; ++i)
                {
                    float tmpZ = listNode[i]._z;
                    listNode[i]._z = listNode[i + 1]._z;
                    listNode[i + 1]._z = tmpZ;

                    if (listNode[i]._z == listNode[i + 1]._z) // Avoid conflict same z
                        --listNode[i]._z; 

                    Misc.Swap<Node>(listNode, i, i + 1);
                }

                for (int i = indexSwap; i < listNode.Count; ++i)
                {
                    _childs.SetAt(listNode[i]._index, listNode[i]);
                }
            }
            else
            {
                Console.WriteLine("GotoFront error at index : " + index);
            }

        }


        public ZIndex FrontZ()
        {
            return _frontZ;
        }
        public ZIndex BackZ()
        {
            return _backZ;
        }
        public Node UpdateChilds(GameTime gameTime)
        {
            for (int index = 0; index < _childs.Count(); index++)
                if (null !=_childs.At(index))
                    if (_childs.At(index)._isActive) 
                        _childs.At(index).Update(gameTime);
            return this;
        }
        public Node UpdateChildsSort(GameTime gameTime)
        {
            for (int index = 0; index < _childs.Count(); ++index)
            {
                if (!_childs.IsEmpty())
                    if (index < _vecZIndex.Count)
                    {
                        if (null != _childs.At(ZIndex(index)))
                        {
                            if (_childs.At(index)._isActive)
                            {
                                // --- Find the front Z
                                if (_childs.At(ZIndex(index))._z < _frontZ._z)
                            {
                                _frontZ._z = _childs.At(ZIndex(index))._z;
                                _frontZ._index = index;
                            }
                            // --- Find the back Z
                            if (_childs.At(ZIndex(index))._z > _backZ._z)
                            {
                                _backZ._z = _childs.At(ZIndex(index))._z;
                                _backZ._index = index;
                            }

                            _childs.At(ZIndex(index)).Update(gameTime);
                            }
                        }
                    }
            }
            return this;
        }
        public Node RenderChilds(SpriteBatch batch)
        {
            for (int index = 0; index < _childs.Count(); ++index)
            {
                if (!_childs.IsEmpty())
                    if (index < _vecZIndex.Count)
                    {
                        if (null != _childs.At(ZIndex(index)))
                            if (_childs.At(ZIndex(index))._isVisible)
                                _childs.At(ZIndex(index)).Render(batch);
                    }
                    else
                    {
                        if (null != _childs.At(index))
                            if (_childs.At(index)._isVisible)
                                _childs.At(index).Render(batch);
                    }
            }
            return this;
        }
        public Node RenderAdditiveChilds(SpriteBatch batch)
        {
            for (int index = 0; index < _childs.Count(); ++index)
            {
                if (!_childs.IsEmpty())
                    if (index < _vecZIndex.Count)
                    {
                        if (null != _childs.At(ZIndex(index)))
                            if (_childs.At(ZIndex(index))._isVisible)
                                _childs.At(ZIndex(index)).RenderAdditive(batch);
                    }
                    else
                    {
                        if (null != _childs.At(index))
                            if (_childs.At(index)._isVisible)
                                _childs.At(index).RenderAdditive(batch);
                    }
            }
            return this;
        }
        public Node RenderAlphaBlendChilds(SpriteBatch batch)
        {
            for (int index = 0; index < _childs.Count(); ++index)
            {
                if (!_childs.IsEmpty())
                    if (index < _vecZIndex.Count)
                    {
                        if (null != _childs.At(ZIndex(index)))
                            if (_childs.At(ZIndex(index))._isVisible)
                                _childs.At(ZIndex(index)).RenderAlphaBlend(batch);
                    }
                    else
                    {
                        if (null != _childs.At(index))
                            if (_childs.At(index)._isVisible)
                                _childs.At(index).RenderAlphaBlend(batch);
                    }
            }
            return this;
        }
        public Node RenderOpaqueChilds(SpriteBatch batch)
        {
            for (int index = 0; index < _childs.Count(); ++index)
            {
                if (!_childs.IsEmpty())
                    if (index < _vecZIndex.Count)
                    {
                        if (null != _childs.At(ZIndex(index)))
                            if (_childs.At(ZIndex(index))._isVisible)
                                _childs.At(ZIndex(index)).RenderOpaque(batch);
                    }
                    else
                    {
                        if (null != _childs.At(index))
                            if (_childs.At(index)._isVisible)
                                _childs.At(index).RenderOpaque(batch);
                    }
            }
            return this;
        }
        public int ZIndex(int index)
        {
            return _vecZIndex[index]._index;
        }
        public Node SortZD()  // Descending Sort
        {
            SortZIndexD(_childs._objects);
            return this;
        }
        public Node SortZA() // Ascending Sort
        {
            SortZIndexA(_childs._objects);
            return this;
        }
        private void SortZIndexD(List<Node> vecEntity)
        {
            // Resize listZIndex if smaller than listObj
            if (_vecZIndex.Count < vecEntity.Count)
            {
                //mlog("- Resize ZIndex !\n");
                for (int index = _vecZIndex.Count; index < vecEntity.Count; ++index)
                {
                    _vecZIndex.Add(new ZIndex());
                }
            }

            for (int index = 0; index < _vecZIndex.Count; ++index)
            {
                if (null != _vecZIndex[index])
                    _vecZIndex[index]._index = index;
                else
                    continue;

                if (index >= 0 && index < vecEntity.Count)
                    if (null != vecEntity[index])
                    {
                        _vecZIndex[index]._z = vecEntity[index]._z;
                    }
                    else
                        _vecZIndex[index]._z = 0;
            }

            _vecZIndex = _vecZIndex.OrderByDescending(v => v._z).ToList();
        }
        private void SortZIndexA(List<Node> vecEntity)
        {
            // Resize listZIndex if smaller than listObj
            if (_vecZIndex.Count < vecEntity.Count)
            {
                //mlog("- Resize ZIndex !\n");
                for (int index = _vecZIndex.Count; index < vecEntity.Count; ++index)
                {
                    _vecZIndex.Add(new ZIndex());
                }
            }

            for (int index = 0; index < _vecZIndex.Count; ++index)
            {
                if (null != _vecZIndex[index])
                    _vecZIndex[index]._index = index;
                else
                    continue;

                if (index >= 0 && index < vecEntity.Count)
                    if (null != vecEntity[index])
                    {
                        _vecZIndex[index]._z = vecEntity[index]._z;
                    }
                    else
                        _vecZIndex[index]._z = 0;
            }

            _vecZIndex = _vecZIndex.OrderBy(v => v._z).ToList();
        }
        // Collisions methods !
        public Node SetCollideZone(int index, RectangleF rect)
        {
            Collide.Zone collideZone = new Collide.Zone
            (
                index,
                new RectangleF
                (
                    AbsX + rect.X,
                    AbsY + rect.Y,
                    rect.Width,
                    rect.Height
                ),
                this
            );
            _collideZones[index] = collideZone;

            return this;
        }
        public Collide.Zone GetCollideZone(int index)
        {
            if (null != _collideZones[index])
                return _collideZones[index];

            return null;
        }
        public Node UpdateCollideZone(int index, RectangleF rect, bool isCollidable = true, int collideLevel = 0)
        {
            if (null != GetCollideZone(index))
            {
                GetCollideZone(index)._rect = rect;
                GetCollideZone(index)._isCollidable = isCollidable;
                GetCollideZone(index)._collideLevel = collideLevel;
            }

            return this;
        }
        public Node RenderCollideZone(SpriteBatch batch, int index, Color color)
        {
            if (null != GetCollideZone(index) && _showNodeInfo)
                Draw.Rectangle(batch, GetCollideZone(index)._rect, color, 0);

            return this;
        }

        // Frames methods !
        //public int CurFrame()
        //{
        //    return _curFrame;
        //}
        public bool IsPlay()
        {
            return _isPlay;
        }
        public void NextFrame()
        {
            if (_isPlay)
                ++_curFrame;
        }
        public void PrevFrame()
        {
            if (_isPlay)
                --_curFrame;
        }
        public bool OnFrame(int frame)
        {
            return _curFrame == frame;
        }
        public void Start()
        {
            _isPlay = true;
            _curFrame = 0;
        }
        public void Stop()
        {
            _isPlay = false;
            _curFrame = 0;
        }
        public void Pause()
        {
            _isPlay = false;
        }
        public void Resume()
        {
            _isPlay = true;
        }
        public void StartAt(int frame)
        {
            _curFrame = frame;
            Start();
        }
        public void StopAt(int frame)
        {
            _curFrame = frame;
            Stop();
        }

        #endregion

    }
}
