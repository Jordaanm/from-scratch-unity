using System;
using FromScratch.Inventory;
using UnityEngine;

namespace FromScratch.Character.Modes
{
    public class PlacementMode: CharacterMode
    {
        private static readonly Vector3 AboveOffset = new Vector3(0, 100, 0);
        private static readonly Vector3 Down = new Vector3(0, -1, 0); 
        private Item item;
        public GameObject reticulePrefab;
        
        private GameObject reticule;
     
        public override MovementType MovementType => MovementType.Overview;

        public void SetItemToPlace(Item item)
        {
            this.item = item;
        }

        private void OnEnable()
        {
            SpawnReticule();
        }

        private void OnDisable()
        {
            DestroyReticule();
        }

        private void DestroyReticule()
        {
            if (reticule != null)
            {
                Destroy(reticule);
            }
        }

        private void SpawnReticule()
        {
            var tx = character.transform;
            reticule = Instantiate(reticulePrefab, tx.position, tx.rotation);
            MoveReticule(tx.position);
        }

        public void MoveReticule(Vector3 position)
        {
            if (reticule == null)
            {
                return;
            }

            Vector3 top = position + AboveOffset;
            int terrainLayer = LayerMask.NameToLayer("Terrain");
            int floorLayer = LayerMask.NameToLayer("Floor");
            LayerMask layersToHit = terrainLayer | floorLayer;
            RaycastHit hitInfo;
            
            if (Physics.Raycast(top, Down, out hitInfo, Mathf.Infinity, layersToHit))
            {
                reticule.transform.position = hitInfo.point;
            }
        }
    }
}