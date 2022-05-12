using System;

namespace Osiris
{
    public class Gold
    {
        public int spent;
        public int permanent;
        public int temporary;

        public const int MaxAmount = 10;

        public Gold(int initialGold) {
            permanent = initialGold;
        }

        public int GetUnlocked() {
            return Math.Min(permanent + temporary, MaxAmount);
        }

        public int GetAvailable() {
            return Math.Min(permanent + temporary - spent, MaxAmount);
        }
    }
}