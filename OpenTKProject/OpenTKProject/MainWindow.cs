using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace ConsoleApplication1
{
    class MainWindow : GameWindow
    {
        float[] ambientLight = new float[4] { 0.2f, 0.2f, 0.2f, 1.0f };
        float[] diffuseLight = new float[4] { 0.7f, 0.7f, 0.7f, 1.0f };
        float[] noLight = new float[4] { 0.0f, 0.0f, 0.0f, 1.0f };
        float[] lightPos = new float[4] { 200.0f, 300.0f, 100.0f, 1.0f };
        float[] cameraPos = new float[4] { 100.0f, 150.0f, 200.0f, 1.0f };

        float factor = 5.0f;

        float[] lightModelview = new float[16], lightProjection = new float[16];

        int windowWidth = 512;                // window size
        int windowHeight = 512;

        IntPtr esfera;

        float rot = 0;

        public MainWindow()
            : base(800, 600)
        {
            esfera = Glu.NewQuadric();
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
            
            // Draw green sphere
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.PushMatrix();
            GL.Translate(-60.0f, 0.0f, 0.0f);
            Glu.Sphere(esfera, 25.0f, 50, 50);
            GL.PopMatrix();

            
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
            int i;

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
            Glu.Perspective(45.0f, aspect, 1.0f, 1000.0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Glu.LookAt(cameraPos[0], cameraPos[1], cameraPos[2],
                      0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
            GL.Viewport(0, 0, windowWidth, windowHeight);

            GL.Rotate(rot, 0, 1, 0);

            // Track light position
            GL.Light(LightName.Light0, LightParameter.Position, lightPos);

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

            Glu.DeleteQuadric(esfera);
        }
}
}