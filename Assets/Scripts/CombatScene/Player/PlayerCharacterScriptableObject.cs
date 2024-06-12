using Sirenix.OdinInspector;
using UnityEngine;

namespace CombatScene.Player
{
    [CreateAssetMenu(fileName = "PlayerCharacterScriptableObject", menuName = "Scriptable Objects/PlayerCharacterScriptableObject")]
    public class PlayerCharacterScriptableObject : ScriptableObject
    {
        [AssetsOnly]
        [PreviewField(70, ObjectFieldAlignment.Right)]
        public Sprite thumbnail;

        [Title("캐릭터 정보")] 
        public string name;
        [Range(10, 1000)]
        public int hp;
        [Range(1, 3)]
        public float defaultPower;
        [Range(0, 2)]
        public float shield;
        [AssetsOnly]
        public AudioClip moveToBlockSFX;
    }
}