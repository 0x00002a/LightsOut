﻿//************************************************
// Enable speakers in the middle of a party
//************************************************

using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using LightsOut.Common;

namespace LightsOut.Patches.ModCompatibility.Ideology
{
    public class PatchTick : ICompatibilityPatchComponent<RitualOutcomeComp_NumActiveLoudspeakers>
    {
        public override string ComponentName => "Patch Tick for RitualOutcomeComp_NumActiveLoudspeakers";

        public override IEnumerable<PatchInfo> GetPatches(Type type)
        {
            PatchInfo prefix = new PatchInfo();
            prefix.method = GetMethod<RitualOutcomeComp_NumActiveLoudspeakers>(nameof(RitualOutcomeComp_NumActiveLoudspeakers.Tick));
            prefix.patch = GetMethod<PatchTick>(nameof(Prefix));
            prefix.patchType = PatchType.Prefix;

            return new List<PatchInfo>() { prefix };
        }

        private static void Prefix(RitualOutcomeComp_NumActiveLoudspeakers __instance, LordJob_Ritual ritual)
        {
            if (_tick++ != 30)
                return;

            _tick = 0;
            TargetInfo selectedTarget = ritual.selectedTarget;
            if (selectedTarget.ThingDestroyed || !selectedTarget.HasThing)
                return;

            foreach (Thing thing in selectedTarget.Map.listerBuldingOfDefInProximity.GetForCell(
                selectedTarget.Cell, (float)__instance.maxDistance, ThingDefOf.Loudspeaker))
                Tables.EnableTable(thing as Building);
        }

        private static int _tick = 0;
    }
}