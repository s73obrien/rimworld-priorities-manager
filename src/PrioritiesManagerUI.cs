using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace PrioritiesManager
{
    static partial class HarmonyPatches
    {
        public static void HarmonyPatches_PrioritiesManagerUI(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method(
                    typeof(MainTabWindow_Work),
                    "DoWindowContents"
                ),
                new HarmonyMethod(
                    typeof(HarmonyPatches),
                    nameof(DoWindowContents_Postfix)
                )
            );

            harmony.Patch(
                AccessTools.Method(
                    typeof(Pawn_WorkSettings),
                    "SetPriority",
                    new Type[] {
                        typeof(WorkTypeDef),
                        typeof(int)
                    }
                ),
                new HarmonyMethod(
                    typeof(HarmonyPatches),
                    nameof(SetPriority_Postfix)
                )
            );

        }

        static IEnumerable<Widgets.DropdownMenuElement<FactionPriorities>> GetPrioritiesSetsMenu(int _i)
        {
            PrioritiesManager manager = Find.World.GetComponent<PrioritiesManager>();

            return manager.factionPrioritiesList.Select((priorities, index) =>
            {
                var newItem = default(Widgets.DropdownMenuElement<FactionPriorities>);
                newItem.payload = priorities;
                newItem.option = new FloatMenuOption(priorities.name, () =>
                {
                    manager.CurrentSet = priorities;
                    manager.CurrentSet.ApplyToCurrent();
                });
                return newItem;
            }).Concat(new Widgets.DropdownMenuElement<FactionPriorities>(){
                payload = null,
                option = new FloatMenuOption("Add new set...", ShowAddNewSetDialog)
            });
        }

        static void ShowAddNewSetDialog()
        {
            PrioritiesManager manager = Find.World.GetComponent<PrioritiesManager>();
            manager.CurrentSet = FactionPriorities.BuildFromCurrent(manager.GetUniqueNewName());
            Find.WindowStack.Add(new Dialog_NamePrioritiesSet(manager.CurrentSet));
        }

        static string GetCurrentPrioritiesSetName()
        {
            PrioritiesManager manager = Find.World.GetComponent<PrioritiesManager>();
            return manager.CurrentSet.name;
        }
 
        static void DoWindowContents_Postfix()
        {
            PrioritiesManager manager = Find.World.GetComponent<PrioritiesManager>();
            Widgets.Dropdown(new Rect(160f, 5f, 140f, 30f), 0, (i) => manager.factionPrioritiesList[0], GetPrioritiesSetsMenu, GetCurrentPrioritiesSetName());
        }

        static void SetPriority_Postfix(Pawn ___pawn, WorkTypeDef w, int priority)
        {
            PrioritiesManager manager = Find.World.GetComponent<PrioritiesManager>();
            if (___pawn.Faction == Faction.OfPlayer)
            {
                manager.SetCurrentPriority(___pawn, w, priority);
            }
        }
    }
}
