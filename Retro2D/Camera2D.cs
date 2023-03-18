using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retro2D
{
    public class Camera2D
    {
        public Matrix _transform;
        protected float _zoom;
        public Vector2 _position;
        protected float _rotation;

        public Camera2D()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _position = Vector2.Zero;
        }
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negtive zoom will flip image
        }
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }
        public void Move(Vector2 amount)
        {
            _position += amount;
        }
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Matrix GetTransformation(GraphicsDevice graphicsDevice)
        {
            Viewport viewport = graphicsDevice.Viewport;

            _transform =       // Thanks to o KB o for this solution
            Matrix.CreateTranslation(new Vector3(-_position.X, -_position.Y, 0)) *
                                        Matrix.CreateRotationZ(Rotation) *
                                        Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                        Matrix.CreateTranslation(new Vector3(viewport.Width * 0.5f, viewport.Height * 0.5f, 0));
            return _transform;
        }

    }
}

//So now how can we use it?
//Simple on your sprite batch begin you must add the camera transformation.

//Camera2d cam = new Camera2d();
//cam.Pos = new Vector2(500.0f,200.0f);
//// cam.Zoom = 2.0f // Example of Zoom in
//// cam.Zoom = 0.5f // Example of Zoom out

////// if using XNA 3.1
//spriteBatch.Begin(SpriteBlendMode.AlphaBlend,
//                        SpriteSortMode.Immediate,
//                        SaveStateMode.SaveState,
//                        cam.get_transformation(device /*Send the variable that has your graphic device here*/));
 
////// if using XNA 4.0
//spriteBatch.Begin(SpriteSortMode.BackToFront,
//                        BlendState.AlphaBlend,
//                        null,
//                        null,
//                        null,
//                        null,
//                        cam.get_transformation(device /*Send the variable that has your graphic device here*/));
 
//// Draw Everything
//// You can draw everything in their positions since the cam matrix has already done the maths for you 
 
//spriteBatch.End(); // Call Sprite Batch End