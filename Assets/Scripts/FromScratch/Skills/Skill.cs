using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Unlockable;
using Util;

namespace FromScratch.Skills
{
    [CreateAssetMenu(fileName = "NewSkill", menuName = "From Scratch/Skill")]
    public class Skill: ScriptableData, IUnlock
    {
        [SerializeField]
        private string id;
        [SerializeField, TableList]
        private List<Prerequisite> prerequisites = new List<Prerequisite>();
        [SerializeField, EnumToggleButtons]
        private Significance significance;

        #region IUnlock
        public string ID => id;
        public List<Prerequisite> Prerequisites => prerequisites;
        public Significance Significance => significance;
#endregion
#region ScriptableData
        public override string GetID()
        {
            return ID;
        }
#endregion


        public static Skill FindFromPrereq(Prerequisite prereq)
        {
            var DB = GameDatabases.Instance.skills.GetDatabase();
            if (DB.ContainsKey(prereq.id))
            {
                return DB[prereq.id];
            }
            return null;
        }
    }
}