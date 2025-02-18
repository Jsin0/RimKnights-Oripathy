using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace Originium
{
    [StaticConstructorOnStartup]
    public class WeatherEvent_OriginiumLightningStrike : WeatherEvent_LightningFlash
    {
        public WeatherEvent_OriginiumLightningStrike(Map map)
            : base(map)
        {
        }

        // Token: 0x06006B9E RID: 27550 RVA: 0x00246628 File Offset: 0x00244828
        public WeatherEvent_OriginiumLightningStrike(Map map, IntVec3 forcedStrikeLoc)
            : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
        }

        // Token: 0x06006B9F RID: 27551 RVA: 0x00246643 File Offset: 0x00244843
        public override void FireEvent()
        {
            WeatherEvent_OriginiumLightningStrike.DoStrike(this.strikeLoc, this.map, ref this.boltMesh);
        }

        // Token: 0x06006BA0 RID: 27552 RVA: 0x0024665C File Offset: 0x0024485C
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
                GenExplosion.DoExplosion(strikeLoc, map, 1.9f, RimWorld.DamageDefOf.Flame, null, -1, -1f, null, null, null, null, Originium.ThingDefOf.RM_OriginiumCluster, 0.1f, 1, null, false, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
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

        // Token: 0x06006BA1 RID: 27553 RVA: 0x00246792 File Offset: 0x00244992
        public override void WeatherEventDraw()
        {
            Graphics.DrawMesh(this.boltMesh, this.strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(WeatherEvent_OriginiumLightningStrike.LightningMat, this.LightningBrightness), 0);
        }

        // Token: 0x04004166 RID: 16742
        private IntVec3 strikeLoc = IntVec3.Invalid;

        // Token: 0x04004167 RID: 16743
        private Mesh boltMesh;

        // Token: 0x04004168 RID: 16744
        private static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt", -1);
    }
}
