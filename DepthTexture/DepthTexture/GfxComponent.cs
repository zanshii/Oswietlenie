#region File Description
//-----------------------------------------------------------------------------
// GfxComponent.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMapping
{
    /// <summary>
    /// This class is designed to isolate graphics differences between the 
    /// Xbox and PC in these projects.  This class has logic for choosing a
    /// back buffer and depth stencils on PC.
    /// </summary>
    class GfxComponent : GameComponent
    {
        public GfxComponent(Game game, GraphicsDeviceManager graphics)
            : base(game)
        {
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreparingDeviceSettings +=
                new EventHandler<PreparingDeviceSettingsEventArgs>(
                    graphics_PreparingDeviceSettings);
        }

        void graphics_PreparingDeviceSettings(object sender,
            PreparingDeviceSettingsEventArgs e)
        {
//            int quality = 0;
            GraphicsAdapter adapter = e.GraphicsDeviceInformation.Adapter;
            SurfaceFormat format = adapter.CurrentDisplayMode.Format;
            DisplayMode currentmode = adapter.CurrentDisplayMode;

            PresentationParameters pp =
                e.GraphicsDeviceInformation.PresentationParameters;

#if XBOX
            pp.MultiSampleQuality = 0;
            pp.MultiSampleType =
                MultiSampleType.FourSamples;
            pp.BackBufferWidth = 1280;
            pp.BackBufferHeight = 720;
            pp.BackBufferFormat = SurfaceFormat.Bgr32;
            pp.AutoDepthStencilFormat = DepthFormat.Depth24Stencil8Single;
            pp.EnableAutoDepthStencil = true;
            return;
#endif

            // Set a window size compatible with the current screen
            if (currentmode.Width < 800)
            {
                pp.BackBufferWidth = 640;
                pp.BackBufferHeight = 480;
            }
            else if (currentmode.Width < 1024)
            {
                pp.BackBufferWidth = 800;
                pp.BackBufferHeight = 600;
            }
            else if (currentmode.Width < 1280)
            {
                pp.BackBufferWidth = 1024;
                pp.BackBufferHeight = 768;
            }
            else // Xbox, or a PC with a big screen
            {
                pp.BackBufferWidth = 1280;
                pp.BackBufferHeight = 720;
            }
            return;
        }
        
        public static RenderTarget2D CloneRenderTarget(RenderTarget2D original, int numberLevels)
        {
            return new RenderTarget2D(
                original.GraphicsDevice,
                original.Width, 
                original.Height);
        }

        public static RenderTarget2D CloneRenderTarget(GraphicsDevice device, int numberLevels)
        {
            return new RenderTarget2D(device,
                device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight);
        }

        public static bool CheckTextureSize(int width, int height,
            out int newwidth, out int newheight)
        {
            bool retval = false;

            if (true)
            {
                retval = true;  // Return true to indicate the numbers changed

                // Find the nearest base two log of the current width, 
                // and go up to the next integer                
                double exp = Math.Ceiling(Math.Log(width) / Math.Log(2));
                // and use that as the exponent of the new width
                width = (int)Math.Pow(2, exp);
                // Repeat the process for height
                exp = Math.Ceiling(Math.Log(height) / Math.Log(2));
                height = (int)Math.Pow(2, exp);
            }
            newwidth = width;
            newheight = height;

            return retval;
        }

        public static RenderTarget2D CreateRenderTarget(GraphicsDevice device,
            int numberLevels, SurfaceFormat surface)
        {
            surface = device.DisplayMode.Format;

            int width, height;

            // See if we can use our buffer size as our texture
            CheckTextureSize(device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight,
                out width, out height);

            // Create our render target
            return new RenderTarget2D(device, width, height);
        }
    }
}
