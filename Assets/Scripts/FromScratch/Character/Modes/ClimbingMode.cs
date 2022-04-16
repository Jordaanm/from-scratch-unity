using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FromScratch.Exploration.Climbing;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace FromScratch.Character.Modes
{
    enum LeftOrRight
    {
        Left,
        Right
    }
    public class ClimbingMode: CharacterMode
    {
        private const int ClimbingLayer = 10;
        public Transform chest;

        private Dictionary<AvatarIKGoal, ClimbingHold> currentHolds = new Dictionary<AvatarIKGoal, ClimbingHold>();
        
        public override MovementType MovementType => MovementType.Climbing;
        public override bool HandlesIK => true;
        
        Vector3 intendedDirection = Vector3.up;
        private bool isAdvancing;

        protected override void Awake()
        {
            base.Awake();
            
            currentHolds.Add(AvatarIKGoal.LeftFoot, null);
            currentHolds.Add(AvatarIKGoal.RightFoot, null);
            currentHolds.Add(AvatarIKGoal.LeftHand, null);
            currentHolds.Add(AvatarIKGoal.RightHand, null);
        }
        
        public void StartingHold(ClimbingHold startingHold)
        {
            if (character == null)
            {
                Debug.LogError("ClimbingMode::StartingHold No Character Found");
                return;
            }
            
            //Determine which side it's on to determine hand
            LeftOrRight side = DetermineSide(startingHold);

            if (side == LeftOrRight.Left)
            {
                SetHold(AvatarIKGoal.LeftHand, startingHold);
                Debug.Log("It's on the left");
            }
            else
            {
                SetHold(AvatarIKGoal.RightHand, startingHold);
                Debug.Log("It's on the right");
            }
        }

        private void SetHold(AvatarIKGoal goal, ClimbingHold climbingHold)
        {
            currentHolds[goal] = climbingHold;
            //Build Adjacency info
            climbingHold.BuildAdjacencyInfo();
        }

        private void OnDisable()
        {
            currentHolds[AvatarIKGoal.LeftFoot] = null;
            currentHolds[AvatarIKGoal.RightFoot] = null;
            currentHolds[AvatarIKGoal.LeftHand] = null;
            currentHolds[AvatarIKGoal.RightHand] = null;
        }

        private LeftOrRight DetermineSide(ClimbingHold startingHold)
        {
            var charTx = character.transform;

            Vector3 delta = startingHold.transform.position - charTx.position;
            var cross = Vector3.Cross(charTx.forward, delta);
            
            return cross.y < 0 ? LeftOrRight.Left : LeftOrRight.Right;
        }

        private void Update()
        {
            if (character == null)
            {
                return; 
            }

            if (character.wantsToJump && !isAdvancing)
            {
                Advance();
            }

            intendedDirection = Vector3.Lerp(character.intendedDirection, intendedDirection, 0.2f);
            
            Debug.DrawRay(chest.position, intendedDirection, Color.yellow, 0.1f);

            DebugNextHandhold(AvatarIKGoal.LeftHand, Color.magenta);
            DebugNextHandhold(AvatarIKGoal.RightHand, Color.cyan);
        }

        private void Advance()
        {
            var nextHandSide = DetermineNextHand();
            var nextHand = nextHandSide == LeftOrRight.Left ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;

            ClimbingHold anchor = nextHandSide == LeftOrRight.Left
                ? currentHolds[AvatarIKGoal.RightHand]
                : currentHolds[AvatarIKGoal.LeftFoot];
            
            var closest = AdjacencyClosestToDir(intendedDirection);
            if (closest == null)
            {
                return; 
            }

            isAdvancing = true;
            Vector3 anchorPos = anchor == null ? chest.position : anchor.transform.position;
            var avgPosition = (anchorPos + closest.hold.transform.position) / 2;
            var delta = avgPosition - chest.position;
            character.characterController.Move(delta);
            SetHold(nextHand, closest.hold);
            isAdvancing = false;
        }

        private LeftOrRight DetermineNextHand()
        {
            var leftHold = currentHolds[AvatarIKGoal.LeftHand];
            var rightHold = currentHolds[AvatarIKGoal.RightHand];
            
            if (leftHold == null)
            {
                return LeftOrRight.Left;
            } 
            if (rightHold == null)
            {
                return LeftOrRight.Right;
            }
            
            //Climbing Up
            if (intendedDirection.y > 0)
            {
                return leftHold.transform.position.y > rightHold.transform.position.y
                    ? LeftOrRight.Right
                    : LeftOrRight.Left;
            }
            
            //Climbing Down
            return leftHold.transform.position.y < rightHold.transform.position.y
                ? LeftOrRight.Right
                : LeftOrRight.Left;
        }

        private void DebugNextHandhold(AvatarIKGoal goal, Color color)
        {
            var hold = currentHolds[goal];
            if (hold == null)
            {
                return;
            }

            ClimbAdjacency adjacency = AdjacencyClosestToDir(hold, intendedDirection);
            Debug.DrawLine(chest.position, adjacency.hold.transform.position, color, 0.2f);
        }

        private ClimbAdjacency AdjacencyClosestToDir(Vector3 direction)
        {
            var leftHold = currentHolds[AvatarIKGoal.LeftHand];
            var rightHold = currentHolds[AvatarIKGoal.RightHand];

            var leftAdj = AdjacencyClosestToDir(leftHold, direction);
            var rightAdj = AdjacencyClosestToDir(rightHold, direction);


            if (leftAdj == null)
            {
                return rightAdj;
            }

            if (rightAdj == null)
            {
                return leftAdj;
            }

            return leftAdj.distance > rightAdj.distance ? rightAdj : leftAdj;

        }
        
        private ClimbAdjacency AdjacencyClosestToDir(ClimbingHold hold, Vector3 direction)
        {
            if (hold == null)
            {
                return null;
            }

            if(hold.adjacencies == null) { hold.BuildAdjacencyInfo(); }

            if (hold.adjacencies.Count == 0)
            {
                return null;
            }

            return hold.adjacencies
                .OrderBy(x => Mathf.Abs(Vector3.Angle(x.delta, direction)))
                .Last();
        }

        private void OnDrawGizmos()
        {
            
        }

        // private float DistanceFromLineSegment(Vector3 from, Vector3 to, Vector3 point)
        // {
        //     var delta = to - from;
        //     var d2 = delta.sqrMagnitude;
        //
        //     if (d2 == 0)
        //     {
        //         return Vector3.Distance(from, point);
        //     }
        //
        //     float t = Mathf.Clamp01(Vector3.Dot(point - from, delta));
        //     Vector3 projection = from + t * delta;
        //     return Vector3.Distance(point, projection);
        // }

        public override void PerformIK(Animator animator)
        {
            foreach (AvatarIKGoal goal in currentHolds.Keys)
            {
                PositionLimb(goal, animator, currentHolds[goal]);
            }
        }

        private void PositionLimb(AvatarIKGoal goal, Animator animator, ClimbingHold climbingHold)
        {
            if (climbingHold == null)
            {
                animator.SetIKPositionWeight(goal, 0f);
                animator.SetIKRotationWeight(goal, 0f);
                return;
            }
            
            animator.SetIKPositionWeight(goal, 1f);
            animator.SetIKRotationWeight(goal, 1f);
            animator.SetIKPosition(goal, climbingHold.transform.position);
        }
    }
}