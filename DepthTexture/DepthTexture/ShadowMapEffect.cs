#region File Description
//-----------------------------------------------------------------------------
// ShadowMapEffect.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMapping
{
    public class ShadowMapEffect
    {
        public Effect effect;
        public EffectParameter mWorld;
        public EffectParameter mCameraView;
        public EffectParameter CameraPos;
        public EffectParameter mCameraProj;
        public EffectParameter MeshTexture;
        public EffectParameter LightPos;
        public EffectParameter ShadowMapTexture;
        public EffectParameter mLightView;
        public EffectParameter mLightProj;

        public EffectTechnique texture;
        public EffectTechnique shadows;
        public EffectTechnique shadowMap;
        private ShadowMapEffect(Effect effect)
        {
            this.effect = effect;

            // These values do not change, so we set them once
            effect.Parameters["g_LightDiffuse"].SetValue(
                Color.LightGray.ToVector4());
            effect.Parameters["g_LightAmbient"].SetValue(
                new Vector4(0.05f, 0.05f, 0.05f, 1.0f));

            // These values change every frame, so we assign them
            // to EffectParameters to speed up access
            
            mWorld = effect.Parameters["g_mWorld"];
            mCameraView = effect.Parameters["g_mCameraView"];
            //CameraPos = effect.Parameters["g_CameraPos"];
            mCameraProj = effect.Parameters["g_mCameraProj"];
            MeshTexture = effect.Parameters["g_MeshTexture"];
            ShadowMapTexture = effect.Parameters["g_ShadowMapTexture"];
            LightPos = effect.Parameters["g_LightPos"];
            mLightView = effect.Parameters["g_mLightView"];
            mLightProj = effect.Parameters["g_mLightProj"];

            texture = effect.Techniques["TextureRender"];
            shadowMap = effect.Techniques["ShadowMapRender"];
            shadows = effect.Techniques["ShadowRender"];
        }
        public void RemapModel(Model model)
        {
            RemapModel(model, effect);
        }
        public static void RemapModel(Model model, Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }
        }
        public static ShadowMapEffect LoadEffect(ContentManager content)
        {
            return new ShadowMapEffect(content.Load<Effect>("ShadowMap"));
        }

    }
}
