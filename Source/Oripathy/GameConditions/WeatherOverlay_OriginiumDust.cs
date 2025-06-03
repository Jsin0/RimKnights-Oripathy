using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace RimKnights
{
    [StaticConstructorOnStartup]
    public class WeatherOverlay_OriginiumDust : SkyOverlay
    {
        public WeatherOverlay_OriginiumDust()
        {
            this.worldOverlayMat = WeatherOverlay_OriginiumDust.DustOverlayWorld;
            this.worldOverlayPanSpeed1 = panSpeed1.RandomInRange;
            this.worldOverlayPanSpeed2 = panSpeed2.RandomInRange;
            this.worldPanDir1 = new Vector2(1f, 1f);
            this.worldPanDir1.Normalize();
            this.worldPanDir2 = new Vector2(0.5f, -0.1f);
            this.worldPanDir2.Normalize();
            this.forceOverlayColor = true;
            this.forcedColor = new Color(1f, 0.733f, 0f, 1f);
        }


        private FloatRange panSpeed1 = new FloatRange(0.001f, 0.02f);

        private FloatRange panSpeed2 = new FloatRange(0.001f, 0.02f);

        private static readonly Material DustOverlayWorld = MatLoader.LoadMat("Weather/FogOverlayWorld", -1);
        //private static readonly Material DustOverlayWorld = MaterialPool.MatFrom("Originium/Textures/Weather/DustStorm", ShaderDatabase.WorldOverlayTransparent);
        //private static readonly Material DustOverlayWorld = MatLoader.LoadMat("Originium/Textures/Weather/DustStorm", -1);
    }
}
