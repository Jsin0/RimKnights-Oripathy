using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    [StaticConstructorOnStartup]
    public class WeatherOverlay_OriginiumRain : WeatherOverlayDualPanner
    {
        public WeatherOverlay_OriginiumRain()
        {

            this.worldOverlayMat = WeatherOverlay_OriginiumRain.OriginiumRainMat;
            this.worldOverlayPanSpeed1 = 0.015f;
            this.worldPanDir1 = new Vector2(-0.25f, -1f);
            this.worldPanDir1.Normalize();
            this.worldOverlayPanSpeed2 = 0.022f;
            this.worldPanDir2 = new Vector2(-0.24f, -1f);
            this.worldPanDir2.Normalize();
            base.ForcedOverlayColor = new Color(0f, 0f, 0f);
        }

        private static readonly Material OriginiumRainMat = MatLoader.LoadMat("Weather/RainOverlayWorld", -1);
    }
}
