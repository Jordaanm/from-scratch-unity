using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Unlockable;
using Util;

namespace FromScratch.Technology
{
    public enum TechType
    {
        Concept,
        Invention
    }
    
    [CreateAssetMenu(fileName = "tech", menuName = "From Scratch/Technology")]
    public class Technology: ScriptableData, IUnlock
    {
        [SerializeField]
        private string id;

        [SerializeField]
        private string techName;

        [SerializeField, TableList]
        private List<Prerequisite> prerequisites = new List<Prerequisite>();
        [SerializeField, EnumToggleButtons]
        private Significance significance;

        [EnumToggleButtons] public TechType techType;
        #region IUnlock
        public string ID => id;
        public List<Prerequisite> Prerequisites => prerequisites;
        public Significance Significance => significance;
        #endregion
        
        public override string GetID()
        {
            return ID;
        }
        
        public string Name => techName;

        public static Technology FindFromPrereq(Prerequisite prereq)
        {
            return GameDatabases.Instance.tech.GetDatabase()[prereq.id];
        }
    }
}