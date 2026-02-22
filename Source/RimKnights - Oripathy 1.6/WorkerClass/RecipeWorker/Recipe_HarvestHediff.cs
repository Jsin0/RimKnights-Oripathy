using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RimKnights.Oripathy
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
                if((!this.recipe.targetsBodyPart || hediffs[i].Part != null) && (hediffs[i].TryGetComp<HediffComp_Harvestable>()?.Harvestable ?? false))
                {
                    if(OripathyMod.settings.debugMode) Log.Message($"Found {hediffs[i].Label} on {hediffs[i].Part}");
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
                if (hediffs[i].Part != null && (hediffs[i].TryGetComp<HediffComp_Harvestable>()?.Harvestable ?? false))
                {
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
                
                Hediff hediff = pawn.health.hediffSet.hediffs.Find((Hediff h) => h.Part == part && (h.TryGetComp<HediffComp_Harvestable>()?.Harvestable ?? false));
                
                HediffComp_Harvestable comp = hediff.TryGetComp<HediffComp_Harvestable>();

                Thing resource = ThingMaker.MakeThing(comp.Props.resource);
                resource.stackCount = comp.Props.count;

                hediff.Severity += comp.Props.severityOffset;
                GenPlace.TryPlaceThing(resource, billDoer.Position, billDoer.Map, ThingPlaceMode.Direct);

                pawn.health.AddHediff(HediffDefOf.RK_HarvestShock);

                comp.ResetCooldown();
                
            }
        }
    }
}
