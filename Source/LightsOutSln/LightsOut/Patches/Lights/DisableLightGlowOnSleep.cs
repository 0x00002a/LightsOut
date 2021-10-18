﻿//************************************************
// Disable the light glow in a room any time a
// pawn goes to sleep
//************************************************

using System;
using System.Collections.Generic;
using HarmonyLib;
using LightsOut.Utility;
using RimWorld;
using Verse;
using Verse.AI;
using ModSettings = LightsOut.Boilerplate.ModSettings;

namespace LightsOut.Patches.Lights
{
    [HarmonyPatch(typeof(Toils_LayDown))]
    [HarmonyPatch(nameof(Toils_LayDown.LayDown))]
    public class DisableLightGlowOnSleep
    {
        public static void Postfix(Toil __result, bool canSleep)
        {
            if (!ModSettings.NightLights && canSleep)
            {
                __result.AddPreInitAction(() =>
                {
                    Pawn pawn = __result.actor;
                    Room room = pawn?.GetRoom();

                    if (!(room is null) && ModResources.AllPawnsSleeping(room, pawn) 
                        && pawn.jobs.curDriver.asleep)
                        ModResources.DisableAllLights(room);

                    __result.AddFinishAction(() => 
                    {
                        // imagine forgetting this and making it so that
                        // the lights never went out the second time a pawn
                        // went to sleep haha... :(
                        Sleepers.Remove(pawn);
                        ModResources.EnableAllLights(pawn.GetRoom()); 
                    });
                });

                Action tickAction = __result.tickAction;

                __result.tickAction = () =>
                {
                    Pawn pawn = __result.actor;
                    bool? asleep = null;
                    if (!Sleepers.ContainsKey(pawn))
                    {
                        asleep = pawn.jobs.curDriver?.asleep;
                    }
                    
                    tickAction();

                    if(!Sleepers.ContainsKey(pawn))
                    {
                        Sleepers.Add(pawn, true);
                        
                        if (asleep != pawn.jobs.curDriver?.asleep)
                        {
                            // if the pawn is waking up
                            if (asleep == true)
                            {
                                // turn the lights back on
                                ModResources.EnableAllLights(pawn.GetRoom());
                            }
                            // otherwise the pawn is going to bed
                            else if (asleep == false)
                            {
                                // turn the lights off
                                ModResources.DisableAllLights(pawn.GetRoom());
                            }
                        }
                    }
                };
            }
        }

        private static Dictionary<Pawn, bool> Sleepers = new Dictionary<Pawn, bool>();
    }
}