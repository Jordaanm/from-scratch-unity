using System;
using FromScratch.Inventory;
using Sirenix.Utilities;
using UnityEngine;

namespace FromScratch.Character.Modes
{
    public class PlacementMode: CharacterMode
    {
        private static readonly Vector3 AboveOffset = new Vector3(0, 100, 0);
        private static readonly Vector3 Down = new Vector3(0, -1, 0); 
        private Item item;
        public Material ghostMaterial;
        
        public GameObject reticulePrefab;
        
        private GameObject reticule;
        private GameObject ghost;

        public override MovementType MovementType => MovementType.Overview;

        private void OnEnable()
        {
            SpawnReticule();
        }

        private void OnDisable()
        {
            DestroyReticule();
            KillGhost();
        }

        public void SetItemToPlace(Item item)
        {
            this.item = item;
            SpawnGhost();
        }

        private void SpawnGhost()
        {
            //Instantiate and parent to reticule
            var instance = Instantiate(item.itemData.prefab, reticule.transform, false);
            instance.transform.localPosition = Vector3.zero;
            
            Util.GameObjectUtils.TraverseHierarchy(instance, SetGhostMaterial);
            Util.GameObjectUtils.TraverseHierarchy(instance, DisableColliders);

            ghost = instance;
        }

        private void DisableColliders(GameObject target)
        {
            var colliders = target.GetComponents<Collider>();
            foreach (var coll in colliders)
            {
                coll.enabled = false;
            }
        }

        private void SetGhostMaterial(GameObject target)
        {
            var targetRenderer = target.GetComponent<Renderer>();
            if (targetRenderer == null)
            {
                return; 
            }

            var newMaterials = targetRenderer.materials;
            for (int x = 0; x < targetRenderer.materials.Length; ++x)
            {
                newMaterials[x] = ghostMaterial;
            }
            targetRenderer.materials = newMaterials;
        }
        
        private void KillGhost()
        {
            if (ghost != null)
            {
                DestroyImmediate(ghost);
            }
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
            var position = tx.position;
            reticule = Instantiate(reticulePrefab, position, tx.rotation);
            MoveReticule(position);
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

        public void ConfirmItemPlacement()
        {
            Item itemToPlace = item;
            item = null;

            Vector3 position = ghost.transform.position;
            Quaternion rotation = ghost.transform.rotation;
            KillGhost();

            character.characterInventory.CompleteItemPlacement(itemToPlace, position, rotation);
        }


        public void CancelItemPlacement()
        {
            item = null;
            KillGhost();
            
            character.characterInventory.CancelItemPlacement();
        }
    }
}