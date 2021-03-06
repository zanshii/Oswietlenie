using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using ShadowMapping;

namespace DepthTexture
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix projection;
        Matrix view;
        Texture2D map;
        bool showDepthBuffer = true;

        ShadowMapEffect MyEffect;
        RenderTarget2D shadowRenderTarget;

        Viewport PIPViewport;
        Viewport defaultViewport;
        BoundingSphere bounds;

        /*Model terrain;
        Matrix terrainWorld;
        Texture2D terrainTex;*/

        Model redtorus;
        Matrix torusWorld;
        Texture2D torusTex;

        /*Model sphere;
        Matrix sphereWorld;
        Texture2D lightTex;*/

        // Camera and light positions
        Vector3 CameraPos;
        Vector3 LightPos;


        // Our onscreen font        
        SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Components.Add(new GfxComponent(this, graphics));
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            shadowRenderTarget = new RenderTarget2D(GraphicsDevice,
                graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height);

            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            CameraPos = new Vector3(1, 1, 1);

            defaultViewport = GraphicsDevice.Viewport;

            LightPos = Vector3.One * 10;

            LoadModels();

            MyEffect = ShadowMapEffect.LoadEffect(Content);

            // Change models to use our loaded effect
            //MyEffect.RemapModel(terrain);
            MyEffect.RemapModel(redtorus);
            //MyEffect.RemapModel(sphere);

            // Load font
            font = Content.Load<SpriteFont>("CourierNew");

            // Calculate bounds of scene
            bounds = new BoundingSphere();
            /*foreach (ModelMesh m in terrain.Meshes)
            {
                bounds = BoundingSphere.CreateMerged(bounds, m.BoundingSphere);
            }*/
            foreach (ModelMesh m in redtorus.Meshes)
            {
                bounds = BoundingSphere.CreateMerged(bounds, m.BoundingSphere);
            }

            // Place camera so that entire scene is visible
            CameraPos.X = bounds.Radius * 2;
            CameraPos.Y = bounds.Radius * 2;
            CameraPos.Z = bounds.Radius * 2;

            // Create view and projection matrices
            view = Matrix.CreateLookAt(CameraPos, Vector3.Zero, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                (float)defaultViewport.Width / defaultViewport.Height, 0.1f, 500);

        }

        private void LoadModels()
        {
            /*terrain = Content.Load<Model>("terrain");
            terrainWorld = Matrix.Identity;
            terrainTex = Content.Load<Texture2D>("TerrainTex");*/

            redtorus = Content.Load<Model>("SphereHighPoly");
            torusWorld = Matrix.CreateTranslation(new Vector3(2));
            torusTex = Content.Load<Texture2D>("redtexture");

            /*sphere = Content.Load<Model>("SphereHighPoly");
            lightTex = Content.Load<Texture2D>("whitetexture");*/
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

#if WINDOWS
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
#endif

            if (GamePad.GetState(PlayerIndex.One).Buttons.A ==
                ButtonState.Pressed)
                showDepthBuffer = true;

            if (GamePad.GetState(PlayerIndex.One).Buttons.B ==
                ButtonState.Pressed)
                showDepthBuffer = false;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                showDepthBuffer = true;

            if (Keyboard.GetState().IsKeyDown(Keys.B))
                showDepthBuffer = false;

            // TODO: Add your update logic here
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            LightPos.X += state.ThumbSticks.Left.X;
            LightPos.Y += state.ThumbSticks.Left.Y;
            LightPos.Z += state.ThumbSticks.Right.X;
            //sphereWorld = Matrix.CreateScale(new Vector3(0.1f)) * Matrix.CreateTranslation(LightPos);

            //MyEffect.CameraPos.SetValue(CameraPos);
            MyEffect.mCameraView.SetValue(view);
            MyEffect.mCameraProj.SetValue(projection);
            MyEffect.LightPos.SetValue(LightPos);
            MyEffect.mLightView.SetValue(Matrix.CreateLookAt(LightPos,
                bounds.Center, Vector3.Up));

            float angle = (float)3.14 / 4;
            float near = 8.5f;// 0.0f - bounds.Radius;
            float far = 22.9f;// bounds.Radius;

            Matrix proj = Matrix.CreatePerspectiveFieldOfView(angle,
                (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height,
                near, far);

            MyEffect.mLightProj.SetValue(proj);
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            map = CreateShadowMap();
            Texture2D texture = map;
            Color[] pixels = new Color[texture.Width * texture.Height];
            int[] pixelsGray = new int[texture.Width * texture.Height];
            texture.GetData<Color>(pixels);
            //   rysujemy po pikselkach w pixels
            for (int i = 0;i<pixels.GetLength(0);i++)
            {
                pixelsGray[i] = (pixels[i].R + pixels[i].G + pixels[i].B)/3; // mniejsza wartość = głębiej(?)
            }
            //Console.WriteLine("done");
            texture.SetData(pixelsGray);
            

            // Render the depth texture
            Rectangle rect = new Rectangle(
                graphics.GraphicsDevice.Viewport.X,
                graphics.GraphicsDevice.Viewport.Y,
                graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);

            spriteBatch.Begin();
            spriteBatch.Draw(map, rect, Color.White);
            spriteBatch.End();


            //           DrawInstructions();
            base.Draw(gameTime);
        }

        /// <summary>
        /// This function draws helpful instructions on screen.
        /// </summary>
        private void DrawInstructions()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Press A to show depth map, B to hide" +
                "\r\nUse Thumbsticks to move light", Vector2.One * 50, Color.LightGreen);
            spriteBatch.End();
        }

        /// <summary>
        /// This function creates a shadow map using DrawScene.
        /// </summary>
        /// <returns>The shadow map as a texture</returns>
        private Texture2D CreateShadowMap()
        {
            DepthStencilState depthStencilState = new DepthStencilState();

            depthStencilState.DepthBufferFunction = CompareFunction.LessEqual;
            GraphicsDevice.DepthStencilState = depthStencilState;

            GraphicsDevice.SetRenderTarget(shadowRenderTarget);

            DrawScene(MyEffect.shadowMap);

            GraphicsDevice.SetRenderTarget(null);

            return shadowRenderTarget;
        }

        /// <summary>
        /// This function draws the scene using the specified technique
        /// </summary>
        /// <param name="view">The View matrix</param>
        /// <param name="projection">The Projection matrix</param>
        /// <param name="technique">The EffectTechnique to use</param>
        private void DrawScene(EffectTechnique technique)
        {
            /*MyEffect.mWorld.SetValue(terrainWorld);
            MyEffect.MeshTexture.SetValue(terrainTex);
            foreach (ModelMesh mesh in terrain.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = technique;
                    mesh.Draw();
                }
            }*/

            MyEffect.mWorld.SetValue(torusWorld);
            MyEffect.MeshTexture.SetValue(torusTex);
            foreach (ModelMesh mesh in redtorus.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = technique;
                    mesh.Draw();
                }
            }
        }
    }
}
