﻿#pragma warning disable
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;

namespace ConsoleApplication1
{
    class MainWindow : GameWindow
    {
        Vector4 ambientLight = new Vector4(0.2f, 0.2f, 0.2f, 1f);
        Vector4 diffuseLight = new Vector4(0.7f, 0.7f, 0.7f, 1f);
        Vector4 specularLight = new Vector4(0.2f, 0.2f, 0.2f, 1f);
        Vector3 lightPos = new Vector3(-200.0f, -300.0f, -100.0f);
        Vector3 cameraPos = new Vector3(100.0f, 150.0f, 200.0f);
        float k_a = 1f;
        float k_d = 1f;
        float k_s = 1f;
        int alfa = 10;
        float factor = 5.0f;

        float[] lightModelview = new float[16], lightProjection = new float[16];

        int windowWidth = 512;                // window size
        int windowHeight = 512;


        float rot = 0;

        public MainWindow()
            : base(800, 600)
        {
            //Keyboard.KeyDown += Keyboard_KeyDown;
        }

        #region DrawModels()

        void DrawModels()
        {
            float width = 30;
            float height = 30;
            float length = 30;

            width /= 2.0f;
            height /= 2.0f;
            length /= 2.0f;
            Vector3 Center = new Vector3(0, 0, 0);
            float Radius = 32;
            uint Precision = 128;


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

            Vector3 Normal, Position, L, N, R, V;

            Vector4 i_a, i_d, i_s;
            float f;


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

                    N = Normal;
                    N.Normalize();
                    L.X = Position.X - lightPos.X;
                    L.Y = Position.Y - lightPos.Y;
                    L.Z = Position.Z - lightPos.Z;
                    L.Normalize();
                    f = Vector3.Dot(L, N);
                    R = 2 * f * N - L;
                    R.Normalize();
                    V.X = cameraPos.X - Position.X;
                    V.Y = cameraPos.Y - Position.Y;
                    V.Z = cameraPos.Z - Position.Z;
                    V.Normalize();
                    i_a = k_a * ambientLight;
                    f = Vector3.Dot(L, N);
                    i_d = k_d * f * diffuseLight;
                    f = Vector3.Dot(R, V);
                    f = (float)Math.Pow(f, alfa);
                    i_s = k_s * f * specularLight;
                    GL.Color3(i_a[0] + i_d[0] + i_s[0], i_a[1] + i_d[1] + i_s[1], i_a[2] + i_d[2] + i_s[2]);

                    GL.Normal3(Normal);
                    GL.TexCoord2(i * OneThroughPrecision, 2.0f * (j + 1) * OneThroughPrecision);
                    GL.Vertex3(Position);


                    Normal.X = (float)(Math.Cos(theta1) * Math.Cos(theta3));
                    Normal.Y = (float)Math.Sin(theta1);
                    Normal.Z = (float)(Math.Cos(theta1) * Math.Sin(theta3));
                    Position.X = Center.X + Radius * Normal.X;
                    Position.Y = Center.Y + Radius * Normal.Y;
                    Position.Z = Center.Z + Radius * Normal.Z;

                    N = Normal;
                    N.Normalize();
                    L.X = Position.X - lightPos.X;
                    L.Y = Position.Y - lightPos.Y;
                    L.Z = Position.Z - lightPos.Z;
                    L.Normalize();
                    f = Vector3.Dot(L, N);
                    R = 2 * f * N - L;
                    R.Normalize();
                    V.X = cameraPos.X - Position.X;
                    V.Y = cameraPos.Y - Position.Y;
                    V.Z = cameraPos.Z - Position.Z;
                    V.Normalize();
                    i_a = k_a * ambientLight;
                    f = Vector3.Dot(L, N);
                    i_d = k_d * f * diffuseLight;
                    f = Vector3.Dot(R, V);
                    f = (float)Math.Pow(f, alfa);
                    i_s = k_s * f * specularLight;
                    GL.Color3(i_a[0] + i_d[0] + i_s[0], i_a[1] + i_d[1] + i_s[1], i_a[2] + i_d[2] + i_s[2]);

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
            // Red background
            GL.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);

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

            var state = OpenTK.Input.Keyboard.GetState();
            if (state[Key.Escape])
                this.Exit();
            if (state[Key.Left])
                lightPos[2] -= 10f;
            if (state[Key.Right])
                lightPos[2] += 10f;
            if (state[Key.Up])
                lightPos[0] -= 10f;
            if (state[Key.Down])
                lightPos[0] += 10f;
            if (state[Key.A])
            {
                if (k_a >= 1f)
                    k_a = 1f;
                else
                    k_a += 0.01f;
            }
            if (state[Key.Z])
            {
                if (k_a <= 0f)
                    k_a = 0f;
                else
                    k_a -= 0.01f;
            }
            if (state[Key.S])
            {
                if (k_s >= 1f)
                    k_s = 1f;
                else
                    k_s += 0.01f;
            }
            if (state[Key.X])
            {
                if (k_s <= 0f)
                    k_s = 0f;
                else
                    k_s -= 0.01f;
            }
            if (state[Key.D])
            {
                if (k_d >= 1f)
                    k_d = 1f;
                else
                    k_d += 0.01f;
            }
            if (state[Key.C])
            {
                if (k_d <= 0f)
                    k_d = 0f;
                else
                    k_d -= 0.01f;
            }
           /* if (state[Key.F])
            {
                alfa += 1;
            }
            if (state[Key.V])
            {
                if (alfa > 0)
                {
                    alfa -= 1;
                }
            }*/
            //presety
            //1 - 100% rozpraszajacy
            //2 - 100% odbijajacy
            //3 - 50/50
            if (state[Key.Number1])
            {
                k_d = 1;
                k_s = 0;
            }
            if (state[Key.Number2])
            {
                k_d = 0.75f;
                k_s = 1;
            }
            if (state[Key.Number3])
            {
                k_d = 0.5f;
                k_s = 0.5f;
            }

            if ((state[Key.AltLeft] || state[Key.AltRight]) &&
                (state[Key.Enter] || state[Key.KeypadEnter]))
                if (this.WindowState == WindowState.Fullscreen)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Fullscreen;
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