using RimWorld.Planet;
using Verse;
using System.Collections.Generic;
using RimWorld;

namespace PrioritiesManager
{
    class PrioritiesManager : WorldComponent
    {
        public List<FactionPriorities> factionPrioritiesList = null;
        public int currentSet = 0;
        public PrioritiesManager(World world) : base(world)
        { }

        public FactionPriorities CurrentSet
        {
            get
            {
                if (factionPrioritiesList == null || currentSet >= factionPrioritiesList.Count)
                {
                    return null;
                } else
                {
                    return factionPrioritiesList[currentSet];
                }
            }

            set
            {
                currentSet = AddSet(value);
            }
        }

        public int AddSet(FactionPriorities set)
        {
            int newIndex = factionPrioritiesList.FindIndex(p => p.guid == set.guid);
            if (newIndex == -1)
            {
                newIndex = factionPrioritiesList.Count;
                factionPrioritiesList.Add(set);
            }

            return newIndex;
        }

        public string GetUniqueNewName()
        {
            int count = 1;
            string newName = "New Priority Set " + count;
            while (factionPrioritiesList.FindIndex(p => p.name == newName) != -1)
            {
                count++;
                newName = "New Priority Set " + count;
            }
            return newName;
        }

        public void SetCurrentPriority(Pawn pawn, WorkTypeDef workTypeDef, int priority)
        {
            CurrentSet?.SetPriority(pawn, workTypeDef, priority);
        }

        public void InitializePrioritiesSets()
        {
            if (factionPrioritiesList == null)
            {
                factionPrioritiesList = new List<FactionPriorities>();
                CurrentSet = FactionPriorities.BuildFromCurrent();
            }
        }

        public override void WorldComponentTick()
        {
            InitializePrioritiesSets();
            base.WorldComponentTick();
        }
        public override void ExposeData()
        {
            Scribe_Collections.Look(ref factionPrioritiesList, "factionPrioritiesList", LookMode.Deep);
            Scribe_Values.Look(ref currentSet, "currentSet", 0);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                InitializePrioritiesSets();
            }
            base.ExposeData();
        }

    }
}
