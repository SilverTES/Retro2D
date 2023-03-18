using Microsoft.Xna.Framework;
//using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retro2D
{
    public class Camera
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        public float DeltaX { get; private set; }
        public float DeltaY { get; private set; }

        public float DeltaLimitX { get; private set; }
        public float DeltaLimitY { get; private set; }


        private Vector2 _offset = new Vector2();

        public RectangleF View; // View of the camera
        public RectangleF Zone; // limit zone when active move camera
        public RectangleF Limit; // limit the total camera move
        private RectangleF oldLimit; // limit the total camera move

        //private bool _isOutLimitX = false;
        //private bool _isOutLimitY = false;

        public void SetView(RectangleF view) { View = view; }
        public void SetZone(RectangleF zone) { Zone = zone; }
        public void SetLimit(RectangleF limit) 
        {
            oldLimit = Limit;
            Limit = limit; 
        }

        //public void SetZonePosition(float x, float y)
        //{

        //}

        public void SetPosition(Vector2 position)
        {
            // Center the position in the Zone
            Zone.X = position.X - Zone.Width / 2;
            Zone.Y = position.Y - Zone.Height / 2;

            // Set Camera final position
            X = -Zone.X + View.Width / 2 - Zone.Width / 2;
            Y = -Zone.Y + View.Height / 2 - Zone.Height / 2;
        }
        //public void CameraMoveX(float vx) 
        //{
        //    DeltaX = vx;
        //}
        //public void CameraMoveY(float vy) 
        //{
        //    DeltaY = vy;
        //}

        public Vector2 Offset() { _offset.X = X; _offset.Y = Y; return _offset; }

        public void Update(float x, float y, float smoothFactorX = 10f, float smoothFactorY = 10f)
        {


            // Manage Camera Movement 
            if (x < Zone.X)
                DeltaX = Zone.X - x;

            if (x > Zone.X + Zone.Width)
                DeltaX = Zone.X + Zone.Width - x;

            if (y < Zone.Y)
                DeltaY =  Zone.Y - y;

            if (y > Zone.Y + Zone.Height)
                DeltaY = Zone.Y + Zone.Height - y;


            if (Math.Abs(DeltaX) >= 0) DeltaX /= smoothFactorX; else DeltaX = 0;
            if (Math.Abs(DeltaY) >= 0) DeltaY /= smoothFactorY; else DeltaY = 0;

            X += DeltaX;
            Zone.X -= DeltaX;

            Y += DeltaY;
            Zone.Y -= DeltaY;

            // Set Camera final position
            X = -Zone.X + View.Width / 2 - Zone.Width / 2;
            Y = -Zone.Y + View.Height / 2 - Zone.Height / 2;


            // Test if out of limit
            // Limit the camera movement in limit size of the map
            //if (X > Limit.X) X = Limit.X;
            //if (Y > Limit.Y) Y = Limit.Y;

            //if (X < -(Limit.X + Limit.Width - View.Width)) X = -(Limit.X + Limit.Width - View.Width);
            //if (Y < -(Limit.X + Limit.Height - View.Height)) Y = -(Limit.X + Limit.Height - View.Height);

            if (X > -Limit.X)
            {
                X = -Limit.X;
            }
            if (Y > -Limit.Y)
            {
                Y = -Limit.Y;
            }
            if (X < -(Limit.X + Limit.Width - View.Width))
            {
                X = -(Limit.X + Limit.Width - View.Width);
            }
            if (Y < -(Limit.Y + Limit.Height - View.Height))
            {
                Y = -(Limit.Y + Limit.Height - View.Height);
            }


        }
    }
}
