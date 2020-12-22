using System.Collections.Generic;
using System.Linq;
using Verse;

namespace PrioritiesManager
{
    class Priority : IExposable
    {
        public WorkTypeDef workTypeDef;
        public int value;

        public bool Equals(Priority other)
        {
            return workTypeDef == other.workTypeDef;
        }

        public Priority(WorkTypeDef workTypeDef, int value)
        {
            this.workTypeDef = workTypeDef;
            this.value = value;
        }

        public Priority()
        { }

        public void ApplyToPawn(Pawn pawn)
        {
            if (
                !pawn.GetDisabledWorkTypes(false).Contains(workTypeDef)
                && pawn.workSettings != null
            )
            {
                pawn.workSettings.SetPriority(workTypeDef, value);
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref value, "priorityValue", 0);
            Scribe_Defs.Look(ref workTypeDef, "workTypeDef");
        }
    }

    class PawnPriorities : IExposable
    {
        public List<Priority> priorityList = null;
        public Pawn pawn = null;

        public bool Equals(PawnPriorities other)
        {
            return pawn == other.pawn;
        }

        public PawnPriorities()
        {}

        public static PawnPriorities BuildFromCurrent(Pawn pawn)
        {
            var newPriorities = new PawnPriorities();
            newPriorities.pawn = pawn;
            newPriorities.priorityList = new List<Priority>();
            if (pawn.workSettings != null)
            {
                var disabledWorkTypes = pawn.GetDisabledWorkTypes();
                foreach (WorkTypeDef workDef in DefDatabase<WorkTypeDef>.AllDefs)
                {
                    if (!disabledWorkTypes.Contains(workDef))
                    {
                        var priority = pawn.workSettings.GetPriority(workDef);
                        newPriorities.priorityList.Add(new Priority(workDef, priority));
                    }
                }
            }
            return newPriorities;
        }

        public void ApplyToCurrent()
        {
            foreach (var priority in priorityList)
            {
                priority.ApplyToPawn(pawn);
            }
        }

        public void SetPriority(WorkTypeDef workTypeDef, int priority)
        {
            var index = priorityList.FindIndex(p => p.workTypeDef == workTypeDef);
            if (index == -1)
            {
                priorityList.Add(new Priority(workTypeDef, priority));
            } else
            {
                priorityList[index].value = priority;
            }
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Collections.Look(ref priorityList, "priorityList", LookMode.Deep);
            if (priorityList == null && Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                priorityList = new List<Priority>();
            }

        }
    }
}
