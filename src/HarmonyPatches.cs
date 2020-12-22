using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrioritiesManager
{
    [StaticConstructorOnStartup]
    static partial class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("rimworld.s73obrien.prioritiesmanager");

            HarmonyPatches_PrioritiesManagerUI(harmony);
        }
    }
}
