using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace PrioritiesManager
{

    class FactionPriorities : IExposable
    {
        public string name;
        public List<PawnPriorities> pawnPrioritiesList = new List<PawnPriorities>();
        public readonly Guid guid;

        public FactionPriorities()
        {
            guid = Guid.NewGuid();
        }
 
        public static FactionPriorities BuildFromCurrent(string name = "Current")
        {
            var newFactionPriorities = new FactionPriorities { name = name };
            foreach (Pawn pawn in PawnsFinder.AllMapsAndWorld_Alive)
            {
                if (pawn.Faction == Faction.OfPlayer && pawn.workSettings != null)
                {
                    newFactionPriorities.pawnPrioritiesList.Add(PawnPriorities.BuildFromCurrent(pawn));
                }
            }
            return newFactionPriorities;
        }

        public void SetPriority(Pawn pawn, WorkTypeDef workTypeDef, int priority)
        {
            var index = pawnPrioritiesList.FindIndex(p => p.pawn == pawn);
            if (index == -1)
            {
                var newPawnPriorities = PawnPriorities.BuildFromCurrent(pawn);
                newPawnPriorities.SetPriority(workTypeDef, priority);
                pawnPrioritiesList.Add(newPawnPriorities);

            } else
            {
                pawnPrioritiesList[index].SetPriority(workTypeDef, priority);
            }
        }


        public void ApplyToCurrent()
        {
            foreach (var pawnPriorities in pawnPrioritiesList)
            {
                pawnPriorities.ApplyToCurrent();
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref name, "name", "Unknown");

            Scribe_Collections.Look(ref pawnPrioritiesList, "pawnPrioritesList", LookMode.Deep);
            if (pawnPrioritiesList == null && Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                pawnPrioritiesList = new List<PawnPriorities>();
            }
        }
    }
}
