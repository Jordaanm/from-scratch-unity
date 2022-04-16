using System;
using System.Collections.Generic;
using System.Linq;
using FromScratch.Interaction;
using UnityEngine;

namespace FromScratch.Exploration.Climbing
{
    public class ClimbAdjacency
    {
        public ClimbingHold hold;
        public float distance;
        public Vector3 delta;

        public ClimbAdjacency(ClimbingHold hold, Vector3 delta)
        {
            this.hold = hold;
            distance = delta.magnitude;
            this.delta = delta;
        }

        public ClimbAdjacency(ClimbingHold baseHold, ClimbingHold hold)
        {
            this.hold = hold;
            delta = baseHold.transform.position - hold.transform.position;
            distance = delta.magnitude;
        }
    }
    public class ClimbingHold : Interactable
    {
        public const float MaxAdjacencyDistance = 10f;
        public List<ClimbAdjacency> adjacencies;

        public override InteractionType GetInteractionType()
        {
            return InteractionType.Climb;
        }

        private void Update()
        {
            var pos = transform.position;
            if (adjacencies != null && adjacencies.Count > 0)
            {
                for (var i = 0; i < adjacencies.Count; i++)
                {
                    Debug.DrawLine(pos, adjacencies[i].hold.transform.position);
                }
            }
        }

        public void BuildAdjacencyInfo()
        {
            var adjacentHolds = GetAdjacentHolds();
            Debug.LogFormat("{0} adjacent climbing holds found", adjacentHolds.Count);
            adjacencies = adjacentHolds
                .Select(x => new ClimbAdjacency(this, x))
                .OrderBy(x => x.distance)
                .ToList();
        }

        public List<ClimbingHold> GetAdjacentHolds()
        {
            Collider[] hitColliders = new Collider[10];
            //TODO: Figure how to filter to just climbable objects using physics layers;
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, MaxAdjacencyDistance, hitColliders, Physics.AllLayers,
                QueryTriggerInteraction.Collide);

            List<ClimbingHold> holds = new();
        
            for (int i = 0; i < numColliders; ++i)
            {
                var hold = hitColliders[i].gameObject.GetComponent<ClimbingHold>();
                if (hold != null && hold != this)
                {
                    holds.Add(hold);        
                }
            }

            return holds;
        }
    }
}
