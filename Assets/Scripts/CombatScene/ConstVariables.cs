using Sirenix.OdinInspector;
using UnityEngine;

namespace CombatScene
{
    public static class ConstVariables
    {
        public const float tileSizeX = 1f;
        public const float tileSizeY = 1f;
        public const int mapHeight = 100;
        public const int mapWidth = 100;
        public const float CharacterHeight = 0.5f;

        public const int maxDetactRange = 5;
        public static readonly int[] dX = { 0, 1, 0, -1 };
        public static readonly int[] dY = { 1, 0, -1, 0 };
    }
}