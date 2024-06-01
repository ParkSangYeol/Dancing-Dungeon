using Sirenix.OdinInspector;
using UnityEngine;

namespace CombatScene.Enemy
{
    [CreateAssetMenu(fileName = "EnemyCharacterScriptableObject", menuName = "Scriptable Objects/EnemyCharacterScriptableObject")]
    public class EnemyCharacterScriptableObject : ScriptableObject
    {
        [AssetsOnly]
        [PreviewField(70, ObjectFieldAlignment.Right)]
        public Sprite thumbnail;

        [Title("캐릭터 정보")] 
        public string name;
        [Range(10, 20)]
        public int hp;
        [Range(1, 3)]
        public float defaultPower;
        [Range(0, 2)]
        public float shield;
    }
}