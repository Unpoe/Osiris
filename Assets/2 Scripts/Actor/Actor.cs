using System;
using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class Actor : MonoBehaviour
    {
        [SerializeField] private ActorAnimationConfig animationConfig = default;
        [SerializeField] private Animator unityAnimator = default;

        private ActorAnimator animator;

        private bool ally;
        private int hp;
        private int range;
        private float speed;

        private Func<bool, List<Actor>> getActors;
        public delegate bool FindPathDelegate(HexCell fromCell , HexCell toCell, Actor actor, ref List<HexCell> path);
        private FindPathDelegate findPath;

        private Actor target;

        private bool moving;

        private HexCell currentCell, nextCell;
        private HexDirection currentDir, targetDir;
        private float progress = 0f;
        private List<HexCell> currentPath = new List<HexCell>();

        public void GameAwake() {
            animator = new ActorAnimator();
            animator.Configure(unityAnimator, animationConfig);
        }

        private void OnDestroy() {
            animator.Destroy();
        }

        public void Initialize(bool isAlly, int maxHP, HexCell startingCell, HexDirection startingDir, Func<bool, List<Actor>> getActors, FindPathDelegate findPath) {
            ally = isAlly;
            hp = maxHP;
            range = 1;
            speed = 1f;

            this.getActors = getActors;
            this.findPath = findPath;

            moving = false;

            currentCell = startingCell;
            currentCell.actor = this;
            transform.position = currentCell.worldPosition;

            SetDirection(startingDir);

            progress = 0f;

            animator.Play(speed);
        }

        public void Clear() {
            animator.Stop();
        }

        public virtual bool GameUpdate(float dt) {
            animator.GameUpdate(dt);

            // Check for death
            if(hp <= 0) {
                return false;
            }

            // Update targeting
            if(target == null) {
                List<Actor> potentialTargets = getActors(!ally);
                if(potentialTargets.Count > 0) {
                    Actor newTarget = potentialTargets[CustomRandom.Range(0, potentialTargets.Count)];
                    SetTarget(newTarget);
                }

                if (target == null) {
                    // We do not have a target, just idle
                    Debug.Log("Idle");
                    return true;
                }
            }

            // At this point, we can assume we have a valid target
            bool isTargetInRange = currentCell.coordinates.DistanceTo(target.currentCell.coordinates) <= range;
            if (isTargetInRange) {
                // Update attack
                Debug.Log("Attacking");
            } else {
                // Update movement
                if (moving) {
                    progress += speed * dt;
                    while (progress >= 1f) {
                        currentCell.actor = null;
                        currentCell = nextCell;
                        currentCell.actor = this;
                        if (currentCell.coordinates.DistanceTo(target.currentCell.coordinates) <= range) {
                            moving = false;
                            return true;
                        }

                        nextCell = GetNextCell();
                        if (nextCell == null) {
                            moving = false;
                            return true;
                        }
                        SetDirection(HexCell.GetDirection(currentCell, nextCell));

                        progress -= 1f;
                    }

                    transform.position = Vector3.LerpUnclamped(currentCell.worldPosition, nextCell.worldPosition, progress);
                } else {
                    nextCell = GetNextCell();
                    SetDirection(HexCell.GetDirection(currentCell, nextCell));
                    moving = nextCell != null;
                }
            }

            return true;
        }

        private void SetTarget(Actor actor) {
            target = actor;
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
            return currentPath[1];
        }
    }
}