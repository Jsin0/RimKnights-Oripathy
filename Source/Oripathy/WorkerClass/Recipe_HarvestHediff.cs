using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using static HarmonyLib.Code;

namespace Originium
{
    public class Recipe_HarvestHediff : Recipe_Surgery
    {
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if(!base.AvailableOnNow(thing, part))
            {
                return false; 
            }
            Pawn pawn;
            if ((pawn = thing as Pawn) == null)
            {
                return false;
            }

            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            for(int i = 0; i < hediffs.Count; i++)
            {
                if((!this.recipe.targetsBodyPart || hediffs[i].Part != null) && (hediffs[i].TryGetComp<HediffComp_Harvestable>()?.ready ?? false) && hediffs[i].Visible)
                {
                    //Log.Message($"Found {hediffs[i].Label} on {hediffs[i].Part}");
                    return true;
                }
            }
            return false;
        }
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = 0; hediffs.Count > i; i++)
            {
                if (hediffs[i].Part != null && hediffs[i].TryGetComp<HediffComp_Harvestable>() != null && hediffs[i].Visible)
                {
                    //Log.Message($"GetPartsToApplyOn: {hediffs[i].Label} on {hediffs[i].Part}");
                    yield return hediffs[i].Part;
                }
            }
            yield break;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if(billDoer != null)
            {
                if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[] { billDoer, pawn });
                
                Hediff hediff = pawn.health.hediffSet.hediffs.Find((Hediff h) => h.Part == part && h.TryGetComp<HediffComp_Harvestable>() != null && h.Visible);
                //Log.Message($"Doing surgery on {hediff.Label} on {hediff.Part}");

                HediffComp_Harvestable comp = hediff.TryGetComp<HediffComp_Harvestable>();

                GenSpawn.Spawn(comp.Props.resource, billDoer.Position, billDoer.Map);
                hediff.Severity += comp.Props.severityOffset;
                comp.ResetCooldown();
                pawn.health.AddHediff(HediffDefOf.RK_HarvestShock);
            }
        }
    }
}
