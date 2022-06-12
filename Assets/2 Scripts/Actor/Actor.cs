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

        private int movesToReachTargetCount = 0;

        public ActorId Id => actorDefinition.Id;
        public HexCoordinates Coordinates => currentCell.coordinates;

        private static List<Actor> POTENTIAL_TARGETS_BUFFER = new List<Actor>();
        private static List<HexCell> PATH_TO_TARGET_BUFFER = new List<HexCell>();

        private const int MAX_MOVES_TO_REACH_TARGET = 2;

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
            movesToReachTargetCount = 0;

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
                    target = GetBestTarget(potentialTargets);
                }

                if (target == null) {
                    // We do not have a target, just idle
                    if (animator.CurrentClip != ActorAnimator.Clip.Idle) {
                        animator.PlayIdle();
                    }
                    return true;
                } else {
                    // Being here means that we started without a target and now we have one, so this is the place to initialize things regarding the target
                    movesToReachTargetCount = 0;
                }
            }

            if (moving) {
                // Update movement
                progress += speed * dt;
                while (progress >= 1f) {
                    // When we reach here, it means we have completed one movement (from one cell to another)
                    progress -= 1f;
                    currentCell = nextCell;

                    // Check if we reached our target
                    if (currentCell.coordinates.DistanceTo(target.currentCell.coordinates) <= range) {
                        moving = false;
                        progress = 0f; // When we stop, we set our progress to 0
                        transform.position = currentCell.worldPosition;
                        return true;
                    } else {
                        // If we haven't reached our target yet, we need to check if it is better to just change our target
                        movesToReachTargetCount++;
                        if(movesToReachTargetCount >= MAX_MOVES_TO_REACH_TARGET) {
                            target = null;
                        }
                    }

                    nextCell = GetNextCell();
                    if (nextCell == null) {
                        moving = false;
                        progress = 0f; // When we stop, we set our progress to 0
                        transform.position = currentCell.worldPosition;
                        return true;
                    } else {
                        // Advance to the next cell
                        currentCell.Actor = null;
                        nextCell.Actor = this;
                    }

                    SetDirection(HexCell.GetDirection(currentCell, nextCell));
                }

                transform.position = Vector3.LerpUnclamped(currentCell.worldPosition, nextCell.worldPosition, progress);
            } else {
                // At this point, we can assume we have a valid target (be careful that GetNextCell can delete our target)
                int distanceToTarget = currentCell.coordinates.DistanceTo(target.currentCell.coordinates);
                bool isTargetInRange = distanceToTarget <= range;
                if (isTargetInRange) {
                    // Update attack
                    // BUG: here we do not take into account that the target can be moving around us
                    // for now this only means that we do not rotate, which is easy to fix, but maybe there are other implications
                    SetDirection(HexCell.GetDirection(currentCell, target.currentCell));
                    if (animator.CurrentClip != ActorAnimator.Clip.Attack) {
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

            if(!findPath.Invoke(currentCell, target.currentCell, this, ref currentPath)) {
                // If the path to our target is null, we also delete our target
                // so later GetBestTarget function will handle a new target that is accesible
                target = null;
                return null;
            }
            
            return currentPath[1];
        }

        private Actor GetBestTarget(List<Actor> enemies) {
            POTENTIAL_TARGETS_BUFFER.Clear();
            int nearDistance = int.MaxValue;

            for(int i = 0; i < enemies.Count; i++) {
                Actor enemy = enemies[i];
                // We only allow targets that are reachable
                if(findPath.Invoke(currentCell, enemy.currentCell, this, ref PATH_TO_TARGET_BUFFER)) {
                    // The potential targets are the ones that are closer
                    int distance = currentCell.coordinates.DistanceTo(enemy.currentCell.coordinates);
                    if (PATH_TO_TARGET_BUFFER.Count < nearDistance) {
                        // If we found a new nearDistance, we clean the previous targets because they are too far
                        POTENTIAL_TARGETS_BUFFER.Clear();
                        POTENTIAL_TARGETS_BUFFER.Add(enemy);

                        nearDistance = PATH_TO_TARGET_BUFFER.Count;
                    } else if (PATH_TO_TARGET_BUFFER.Count == nearDistance) {
                        POTENTIAL_TARGETS_BUFFER.Add(enemy);
                    }
                }
            }

            if(POTENTIAL_TARGETS_BUFFER.Count == 0) {
                return null;
            }

            return POTENTIAL_TARGETS_BUFFER[CustomRandom.Range(0, POTENTIAL_TARGETS_BUFFER.Count)];
        }
    }
}