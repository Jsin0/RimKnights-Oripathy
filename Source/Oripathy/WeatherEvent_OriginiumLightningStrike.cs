using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Originium
{
    [StaticConstructorOnStartup]
    public class WeatherEvent_OriginiumLightningStrike : WeatherEvent_LightningStrike
    {
        public WeatherEvent_OriginiumLightningStrike(Map map)
            : base(map)
        {
        }
        public WeatherEvent_OriginiumLightningStrike(Map map, IntVec3 forcedStrikeLoc)
            : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
        }

        public override void FireEvent()
        {
            WeatherEvent_OriginiumLightningStrike.DoStrike(this.strikeLoc, this.map, ref this.boltMesh);
        }

        public static void DoStrike(IntVec3 strikeLoc, Map map, ref Mesh boltMesh)
        {
            SoundDefOf.Thunder_OffMap.PlayOneShotOnCamera(map);
            if (!strikeLoc.IsValid)
            {
                strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => sq.Standable(map) && !map.roofGrid.Roofed(sq), map, 1000);
            }
            boltMesh = LightningBoltMeshPool.RandomBoltMesh;
            if (!strikeLoc.Fogged(map))
            {
                GenExplosion.DoExplosion(strikeLoc, map, 1.9f, RimWorld.DamageDefOf.Flame, null, -1, -1f, null, null, null, null, Originium.ThingDefOf.RK_OriginiumCluster, 0.15f, 1, null, false, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
                Vector3 vector = strikeLoc.ToVector3Shifted();
                for (int i = 0; i < 4; i++)
                {
                    FleckMaker.ThrowSmoke(vector, map, 1.5f);
                    FleckMaker.ThrowMicroSparks(vector, map);
                    FleckMaker.ThrowLightningGlow(vector, map, 1.5f);
                }
            }
            SoundInfo soundInfo = SoundInfo.InMap(new TargetInfo(strikeLoc, map, false), MaintenanceType.None);
            SoundDefOf.Thunder_OnMap.PlayOneShot(soundInfo);
        }

        public override void WeatherEventDraw()
        {
            Graphics.DrawMesh(this.boltMesh, this.strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(WeatherEvent_OriginiumLightningStrike.LightningMat, this.LightningBrightness), 0);
        }

        private IntVec3 strikeLoc = IntVec3.Invalid;

        private Mesh boltMesh;

        private static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt", -1);
    }
}
