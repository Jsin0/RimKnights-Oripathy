using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace Oripathy
{
    [StaticConstructorOnStartup]
    public class WeatherOverlay_OriginiumFog : SkyOverlay
    {
        public WeatherOverlay_OriginiumFog()
        {
            this.worldOverlayMat = WeatherOverlay_OriginiumFog.FogOverlayWorld;
            this.worldOverlayPanSpeed1 = 0.0005f;
            this.worldOverlayPanSpeed2 = 0.0004f;
            this.worldPanDir1 = new Vector2(1f, 1f);
            this.worldPanDir1.Normalize();
            this.worldPanDir2 = new Vector2(0.5f, -0.1f);
            this.worldPanDir2.Normalize();
            this.forceOverlayColor = true;
            this.forcedColor = new Color(0.5f, 0.5f, 0.5f);
        }

        // Token: 0x04004179 RID: 16761
        private static readonly Material FogOverlayWorld = MatLoader.LoadMat("Weather/FogOverlayWorld", -1);
    }
}
