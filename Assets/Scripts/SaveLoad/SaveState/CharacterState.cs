using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveLoad.SaveState
{
    [Serializable]
    public class CharacterState
    {
        public string guid;
        public string prefabID;
        public Vector3 location;
        public Quaternion rotation;
        public Dictionary<string, string> customState;
    }
}