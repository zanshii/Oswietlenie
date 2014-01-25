using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
#pragma warning disable 618
namespace ConsoleApplication1
{
    class MainWindow : GameWindow
    {
        Vector4 ambientLight = new Vector4(0.2f, 0.2f, 0.2f, 1f);
        Vector4 diffuseLight = new Vector4(0.7f, 0.7f, 0.7f, 1f);
        Vector4 noLight = new Vector4(0.0f, 0.0f, 0.0f, 1f);
        Vector3 lightPos = new Vector3(200.0f, 300.0f, 100.0f);
        Vector3 cameraPos = new Vector3(100.0f, 150.0f, 200.0f);

        float factor = 5.0f;

        float[] lightModelview = new float[16], lightProjection = new float[16];

        int windowWidth = 512;                // window size
        int windowHeight = 512;


        float rot = 0;

        public MainWindow()
            : base(800, 600)
        {
            Keyboard.KeyDown += Keyboard_KeyDown;
        }

        #region Keyboard_KeyDown

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <param name="sender">The KeyboardDevice which generated this event.</param>
        /// <param name="key">The key that was pressed.</param>
        void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs key)
        {
            if (key.Key == Key.Escape)
                this.Exit();
            if (key.Key == Key.Left)
                lightPos[2] += 5f;
            if (key.Key == Key.Right)
                lightPos[2] -= 5f;
            if (key.Key == Key.Up)
                lightPos[0] += 5f;
            if (key.Key == Key.Down)
                lightPos[0] -= 5f;

            if ((key.Key == Key.AltLeft || key.Key == Key.AltRight) &&
                (key.Key == Key.Enter || key.Key == Key.KeypadEnter))
                if (this.WindowState == WindowState.Fullscreen)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Fullscreen;
        }

        #endregion

        #region DrawModels()

        void DrawModels()
        {
            float width = 30;
            float height = 30;
            float length = 30;

            width /= 2.0f;
            height /= 2.0f;
            length /= 2.0f;
            Vector3 Center = new Vector3(100, 0, 0);
            float Radius = 32;
            uint Precision = 64;


            if (Radius < 0f)
                Radius = -Radius;
            if (Radius == 0f)
                throw new DivideByZeroException("DrawSphere: Radius cannot be 0f.");
            if (Precision == 0)
                throw new DivideByZeroException("DrawSphere: Precision of 8 or greater is required.");

            const float HalfPI = (float)(Math.PI * 0.5);
            float OneThroughPrecision = 1.0f / Precision;
            float TwoPIThroughPrecision = (float)(Math.PI * 2.0 * OneThroughPrecision);

            float theta1, theta2, theta3;

            Vector3 Normal, Position;

            for (uint j = 0; j < Precision / 2; j++)
            {
                theta1 = (j * TwoPIThroughPrecision) - HalfPI;
                theta2 = ((j + 1) * TwoPIThroughPrecision) - HalfPI;

                GL.Begin(BeginMode.TriangleStrip);
                for (uint i = 0; i <= Precision; i++)
                {
                    theta3 = i * TwoPIThroughPrecision;

                    Normal.X = (float)(Math.Cos(theta2) * Math.Cos(theta3));
                    Normal.Y = (float)Math.Sin(theta2);
                    Normal.Z = (float)(Math.Cos(theta2) * Math.Sin(theta3));
                    Position.X = Center.X + Radius * Normal.X;
                    Position.Y = Center.Y + Radius * Normal.Y;
                    Position.Z = Center.Z + Radius * Normal.Z;
                    
                    //tutaj zmienić kolor na policzony z phonga
                    //GL.Color4(a,b,c,d)

                    GL.Normal3(Normal);
                    GL.TexCoord2(i * OneThroughPrecision, 2.0f * (j + 1) * OneThroughPrecision);
                    GL.Vertex3(Position);


                    Normal.X = (float)(Math.Cos(theta1) * Math.Cos(theta3));
                    Normal.Y = (float)Math.Sin(theta1);
                    Normal.Z = (float)(Math.Cos(theta1) * Math.Sin(theta3));
                    Position.X = Center.X + Radius * Normal.X;
                    Position.Y = Center.Y + Radius * Normal.Y;
                    Position.Z = Center.Z + Radius * Normal.Z;

                    //tutaj zmienić kolor na policzony z phonga
                    //GL.Color4(a,b,c,d)

                    GL.Normal3(Normal);
                    GL.TexCoord2(i * OneThroughPrecision, 2.0f * j * OneThroughPrecision);
                    GL.Vertex3(Position);


                }
                GL.End();
            }

        }
        #endregion DrawModels()

        #region OnLoad

        /// <summary>
        /// Setup OpenGL and load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            // Black background
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            // Hidden surface removal
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.PolygonOffset(factor, 0.0f);
        }

        #endregion

        #region OnResize

        /// <summary>
        /// Respond to resize events here.
        /// </summary>
        /// <param name="e">Contains information on the new GameWindow size.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnResize(EventArgs e)
        {
            windowWidth = Width;
            windowHeight = Height;
        }

        #endregion

        #region OnUpdateFrame

        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            rot += 0.5f;
        }

        #endregion

        #region OnRenderFrame

        /// <summary>
        /// Add your game rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            double aspect = (double)windowWidth / (double)windowHeight;
            // Track camera angle
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            OpenTK.Graphics.Glu.Perspective(45.0f, aspect, 1.0f, 1000.0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            OpenTK.Graphics.Glu.LookAt(cameraPos[0], cameraPos[1], cameraPos[2],
                      0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
            GL.Viewport(0, 0, windowWidth, windowHeight);

            GL.Rotate(rot, 0, 1, 0);

            // Clear the window with current clearing color
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Draw objects in the scene
            DrawModels();

            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.TextureGenS);
            GL.Disable(EnableCap.TextureGenT);
            GL.Disable(EnableCap.TextureGenR);
            GL.Disable(EnableCap.TextureGenQ);

            this.SwapBuffers();
        }

        #endregion

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }
    }
}