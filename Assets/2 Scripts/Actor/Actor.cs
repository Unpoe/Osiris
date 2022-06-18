using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class Actor : MonoBehaviour
    {
        [SerializeField] private Animator unityAnimator = default;

        private bool hasBeenInitializedBefore;

        private ActorAnimator animator;

        // This is public for debugging purposes
        public int battleId = -1;

        public bool ally { get; private set; }

        private float hp;
        private int range;
        private float speed;
        private float rotationSpeed; // This variable is purely visual
        private float attackSpeed; // attacks per second

        private Battle battle;
        private ActorDefinition actorDefinition;

        private Actor target;

        private bool moving;
        private bool rotating;

        private HexCell currentCell, nextCell;
        private float movementProgress, rotationProgress = 0f;
        private List<HexCell> currentPath = new List<HexCell>();
        private HexDirection currentDir, targetDir;
        private float angleFrom, angleTo;

        private int movesToReachTargetCount = 0;

        private float rotationProgressSpeed = 1f;

        public ActorId Id => actorDefinition.Id;
        public HexCoordinates Coordinates => currentCell.coordinates;

        private static List<Actor> POTENTIAL_TARGETS_BUFFER = new List<Actor>();
        private static List<HexCell> PATH_TO_TARGET_BUFFER = new List<HexCell>();

        private const int MAX_MOVES_TO_REACH_TARGET = 2;

        public void GameAwake() {
            hasBeenInitializedBefore = false;
        }

        private void OnDestroy() {
            animator.Destroy();
        }

        public void Initialize(int battleId, ActorDefinition actorDefinition, bool isAlly, HexCell startingCell, HexDirection startingDir, Battle battle) {
            this.battleId = battleId;

            ally = isAlly;
            hp = actorDefinition.Hp;
            range = actorDefinition.Range;
            speed = actorDefinition.Speed;
            rotationSpeed = 7f;
            attackSpeed = actorDefinition.AttackSpeed;

            this.battle = battle;
            this.actorDefinition = actorDefinition;

            if (!hasBeenInitializedBefore) {
                // BUG: if the Enemy is recycled and it grabs an old animator, maybe there is parameters such as
                // the progression of the animation that are not 0. Could lead to some visual bugs
                hasBeenInitializedBefore = true;
                animator = new ActorAnimator();
                animator.Configure(unityAnimator, actorDefinition.AnimationConfig);
            }

            target = null;
            moving = false;
            rotating = false;

            nextCell = null;
            currentCell = startingCell;
            currentCell.Actor = this;
            transform.position = currentCell.worldPosition;

            SetDirection(startingDir, true);

            movementProgress = 0f;
            movesToReachTargetCount = 0;

            rotationProgress = 0f;

            currentPath.Clear();

            animator.PlayIdle();
        }

        public void Clear() {
            animator.Stop();
        }

        public virtual bool GameUpdate(float dt) {
            //if (battleId == 4) {
            //    Debug.LogWarning("Debugging Actor...");
            //}

            animator.GameUpdate(dt);

            // Check for death
            if(hp <= 0) {
                return false;
            }

            // Update rotation
            if (rotating) {
                rotationProgress += rotationProgressSpeed * dt;
                float yAngle = 0f;
                if(rotationProgress >= 1f) {
                    rotating = false;
                    currentDir = targetDir;
                    yAngle = currentDir.GetYAngle();
                } else {
                    yAngle = Mathf.Lerp(angleFrom, angleTo, rotationProgress);
                }

                transform.rotation = Quaternion.Euler(0f, yAngle, 0f);
            }

            // Update targeting
            if(target == null) {
                List<Actor> potentialTargets = battle.GetActorList(!ally);
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
                movementProgress += speed * dt;
                while (movementProgress >= 1f) {
                    // When we reach here, it means we have completed one movement (from one cell to another)
                    movementProgress -= 1f;
                    currentCell = nextCell;

                    // Check if we reached our target
                    // If our target is moving, we need to check the cell that its going to
                    HexCell targetCellToCheck = target.moving ? target.nextCell : target.currentCell;
                    int distanceToTarget = currentCell.coordinates.DistanceTo(targetCellToCheck.coordinates);
                    if (distanceToTarget <= range) {
                        moving = false;
                        movementProgress = 0f; // When we stop, we set our progress to 0
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
                        movementProgress = 0f; // When we stop, we set our progress to 0
                        transform.position = currentCell.worldPosition;
                        return true;
                    } else {
                        // Advance to the next cell
                        currentCell.Actor = null;
                        nextCell.Actor = this;
                    }

                    SetDirection(HexCell.GetDirection(currentCell, nextCell), false);
                }

                transform.position = Vector3.LerpUnclamped(currentCell.worldPosition, nextCell.worldPosition, movementProgress);
            } else {
                // At this point, we can assume we have a valid target (be careful that GetNextCell can delete our target)
                int distanceToTarget = currentCell.coordinates.DistanceTo(target.currentCell.coordinates);
                bool isTargetInRange = distanceToTarget <= range;
                if (isTargetInRange) {
                    // Update attack
                    // The target can be moving around us and it will still be in range
                    // but SetDirection will do nothing if we are pointing at the same direction
                    SetDirection(HexCell.GetDirection(currentCell, target.currentCell), false);
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
                        SetDirection(HexCell.GetDirection(currentCell, nextCell), false);
                    }
                }
            }

            return true;
        }

        private void SetDirection(HexDirection hexDirection, bool snap) {
            if (snap) {
                currentDir = targetDir = hexDirection;
                angleFrom = angleTo = currentDir.GetYAngle();
                transform.rotation = Quaternion.Euler(0, angleFrom, 0);
                return;
            }

            // If we are or we are going to the desired direction, skip this function
            if(currentDir == hexDirection || targetDir == hexDirection) {
                return;
            }

            targetDir = hexDirection;
            int steps = currentDir.GetRotationStepsTo(targetDir);
            angleFrom = angleTo;
            // Angle to is calculated this way instead of using targetDir.GetAngleY() to handle the
            // lerp better. Instead of lerping between 30 and 270 we want to lerp between 30 and -30
            angleTo = angleFrom + (HexMetrics.innerAngle * steps);
            rotating = true;
            rotationProgress = 0f;
            rotationProgressSpeed = rotationSpeed * Mathf.Abs(steps);
        }

        private HexCell GetNextCell() {
            if(target == null) {
                return null;
            }

            if(!battle.grid.FindPath(currentCell, target.currentCell, this, ref currentPath)) {
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
                if(battle.grid.FindPath(currentCell, enemy.currentCell, this, ref PATH_TO_TARGET_BUFFER)) {
                    // The potential targets are the ones that are closer
                    int distance = currentCell.coordinates.DistanceTo(enemy.currentCell.coordinates);
                    if (distance < nearDistance) {
                        // If we found a new nearDistance, we clean the previous targets because they are too far
                        POTENTIAL_TARGETS_BUFFER.Clear();
                        POTENTIAL_TARGETS_BUFFER.Add(enemy);

                        nearDistance = PATH_TO_TARGET_BUFFER.Count;
                    } else if (distance == nearDistance) {
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