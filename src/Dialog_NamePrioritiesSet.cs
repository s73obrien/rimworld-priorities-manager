using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PrioritiesManager
{
    class Dialog_NamePrioritiesSet : Dialog_Rename
    {
        FactionPriorities prioritiesSet;
        public Dialog_NamePrioritiesSet(FactionPriorities prioritiesSet): base()
        {
            this.prioritiesSet = prioritiesSet;
            curName = prioritiesSet.name;
        }
        protected override void SetName(string name)
        {
            prioritiesSet.name = name;
        }
    }
}
