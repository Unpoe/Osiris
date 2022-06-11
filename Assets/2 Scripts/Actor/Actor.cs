using System;
using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class Actor : MonoBehaviour
    {
        [SerializeField] private ActorDefinition actorDefinition = default;
        [SerializeField] private Animator unityAnimator = default;

        private ActorAnimator animator;

        // This is public for debugging purposes
        public int battleId = -1;

        public bool ally { get; private set; }

        private float hp;
        private int range;
        private float speed;
        private float attackSpeed; // attacks per second

        private Func<bool, List<Actor>> getActors;
        public delegate bool FindPathDelegate(HexCell fromCell , HexCell toCell, Actor actor, ref List<HexCell> path);
        private FindPathDelegate findPath;

        private Actor target;

        private bool moving;

        private HexCell currentCell, nextCell;
        private HexDirection currentDir, targetDir;
        private float progress = 0f;
        private List<HexCell> currentPath = new List<HexCell>();

        public ActorId Id => actorDefinition.Id;
        public HexCoordinates Coordinates => currentCell.coordinates;

        public void GameAwake() {
            animator = new ActorAnimator();
            animator.Configure(unityAnimator, actorDefinition.AnimationConfig);
        }

        private void OnDestroy() {
            animator.Destroy();
        }

        public void Initialize(int battleId, bool isAlly, HexCell startingCell, HexDirection startingDir, Func<bool, List<Actor>> getActors, FindPathDelegate findPath) {
            this.battleId = battleId;

            ally = isAlly;
            hp = actorDefinition.Hp;
            range = actorDefinition.Range;
            speed = actorDefinition.Speed;
            attackSpeed = actorDefinition.AttackSpeed;

            this.getActors = getActors;
            this.findPath = findPath;

            target = null;
            moving = false;

            nextCell = null;
            currentCell = startingCell;
            currentCell.Actor = this;
            transform.position = currentCell.worldPosition;

            SetDirection(startingDir);

            progress = 0f;

            currentPath.Clear();

            animator.PlayIdle();
        }

        public void Clear() {
            animator.Stop();
        }

        public virtual bool GameUpdate(float dt) {
            //if (battleId == 3) {
            //    Debug.LogWarning("Debugging Actor...");
            //}

            animator.GameUpdate(dt);

            // Check for death
            if(hp <= 0) {
                return false;
            }

            // Update targeting
            if(target == null) {
                List<Actor> potentialTargets = getActors(!ally);
                if(potentialTargets.Count > 0) {
                    target = potentialTargets[CustomRandom.Range(0, potentialTargets.Count)];
                }

                if (target == null) {
                    // We do not have a target, just idle
                    if (animator.CurrentClip != ActorAnimator.Clip.Idle) {
                        animator.PlayIdle();
                    }
                    return true;
                }
            }

            // At this point, we can assume we have a valid target
            if (moving) {
                // Update movement
                progress += speed * dt;
                while (progress >= 1f) {
                    progress -= 1f;
                    currentCell = nextCell;
                    if (currentCell.coordinates.DistanceTo(target.currentCell.coordinates) <= range) {
                        moving = false;
                        return true; // BUG?: why there is a return here?
                    }

                    nextCell = GetNextCell();
                    if (nextCell == null) {
                        moving = false;
                        return true; // BUG?: why there is a return here?
                    } else {
                        currentCell.Actor = null;
                        nextCell.Actor = this;
                    }

                    SetDirection(HexCell.GetDirection(currentCell, nextCell));
                }

                transform.position = Vector3.LerpUnclamped(currentCell.worldPosition, nextCell.worldPosition, progress);
            } else {
                int distanceToTarget = currentCell.coordinates.DistanceTo(target.currentCell.coordinates);
                bool isTargetInRange = distanceToTarget <= range;
                if (isTargetInRange) {
                    // Update attack
                    if (animator.CurrentClip != ActorAnimator.Clip.Attack) {
                        SetDirection(HexCell.GetDirection(currentCell, target.currentCell));
                        animator.PlayAttack(attackSpeed);
                    }
                } else {
                    // Here we are not moving but we have a valid target
                    // So we just try to get a valid cell to move towards our target
                    nextCell = GetNextCell();
                    moving = nextCell != null;
                    if (moving) {
                        currentCell.Actor = null;
                        nextCell.Actor = this;
                        animator.PlayWalk(speed);
                        SetDirection(HexCell.GetDirection(currentCell, nextCell));
                    }
                }
            }

            return true;
        }

        private void SetDirection(HexDirection hexDirection) {
            currentDir = hexDirection;
            transform.rotation = Quaternion.Euler(0, currentDir.GetYAngle(), 0);
        }

        private HexCell GetNextCell() {
            if(target == null) {
                return null;
            }

            findPath.Invoke(currentCell, target.currentCell, this, ref currentPath);
            // This is very temporary, we should not assume that current path has this Length
            // Anyways, maybe in the future we should not calculate the hole path
            // Taking in to account that the battle is "deterministic" (there is no input) maybe another
            // approach for pathfinding could be had
            return currentPath[1];
        }
    }
}