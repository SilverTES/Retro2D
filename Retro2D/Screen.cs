using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retro2D
{
    public static class Screen
    {

        private static Node _curScreen = new Node();
        private static Node _prevScreen = new Node(); 
        private static Node _showScreen = new Node(); // the Screen showed until the end of transition
        private static Node _transition = new Node(); // the Node played when transition !

        private static bool _onTransition = false;
        private static bool _isTransition = false;
        private static bool _offTransition = false;

        private static bool _onSwap = false;
        private static bool _isSwap = false; // false : Before Swap state / true : After Swap state !


        public static void Init(Node initScreen)
        {
            _curScreen = initScreen;
            _showScreen = initScreen;
            _prevScreen = initScreen;
        }

        public static Node CurScreen()
        {
            return _curScreen;
        }
        public static Node PrevScreen()
        {
            return _prevScreen;
        }

        public static void Update(GameTime gameTime)
        {
            if (_onTransition)
            {
                _onTransition = false;
                _isTransition = true;
            }

            if (_isTransition)
            {
                if (_onSwap)
                {
                    _onSwap = false;
                    _showScreen = _curScreen; // Swap screen when OnSwap is called !
                }

            }

            if (_offTransition)
            {
                _offTransition = false;
                _isTransition = false;

                _transition = null;

                // Navi System
                if (null != _curScreen._naviGate)
                    _curScreen._naviGate.SetNaviGate(true);
            }

            _showScreen.Update(gameTime);

            if (null != _transition)
                _transition.Update(gameTime);


        }
        public static void Render(SpriteBatch batch)
        {
            _showScreen.Render(batch);

            if (null != _transition)
                _transition.Render(batch);
        }
        public static void RenderAdditive(SpriteBatch batch)
        {
            _showScreen.RenderAdditive(batch);

            if (null != _transition)
                _transition.RenderAdditive(batch);
        }
        public static void RenderAlphaBlend(SpriteBatch batch)
        {
            _showScreen.RenderAlphaBlend(batch);

            if (null != _transition)
                _transition.RenderAlphaBlend(batch);
        }
        public static void RenderOpaque(SpriteBatch batch)
        {
            _showScreen.RenderOpaque(batch);

            if (null != _transition)
                _transition.RenderOpaque(batch);
        }

        public static void Swap()
        {
            _onSwap = true;
            _isSwap = true;
        }
        public static bool IsSwap()
        {
            return _isSwap;
        }
        public static bool IsTransition()
        {
            return _isTransition;
        }

        public static void StartTransition()
        {
            
            _onTransition = true;
            _offTransition = false;
        }
        public static void StopTransition()
        {
            
            _onTransition = false;
            _offTransition = true;
        }

        public static void GoTo(Node nextScreen)
        {
            _prevScreen = _curScreen;
            _curScreen = nextScreen;
            _showScreen = _curScreen;
        }
        public static void GoTo(Node nextScreen, Node transition)
        {
            _prevScreen = _curScreen;
            _showScreen = _prevScreen;
            _curScreen = nextScreen;

            _transition = transition;

            //OnTransition();

            _isSwap = false;
            _onSwap = false;

            transition.Start();
        }
        
    }
}
