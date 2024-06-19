using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using UnityEditor;

public class AnimatedTileGenerator : MonoBehaviour
{
    [FolderPath]
    public string spriteFolderPath;

    [FolderPath]
    public string saveFolderPath;
#if UNITY_EDITOR
    [Button]
    public void GenerateAnimatedTiles()
    {
        // 스프라이트 폴더에서 모든 스프라이트 에셋의 GUID를 가져옵니다.
        string[] spriteGUIDs = AssetDatabase.FindAssets("t:Sprite", new[] { spriteFolderPath });

        List<Sprite> sprites = new List<Sprite>();
        // 각 스프라이트 에셋에 대해 AnimatedTile을 생성합니다.
        foreach (string guid in spriteGUIDs)
        {
            // GUID를 사용하여 스프라이트 에셋의 경로를 가져옵니다.
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            sprites.Add(sprite);
        }

        AnimatedTile animatedTile = ScriptableObject.CreateInstance<AnimatedTile>();
        animatedTile.m_AnimatedSprites = sprites.ToArray();
        animatedTile.m_MinSpeed = 1f;
        animatedTile.m_MaxSpeed = 1f;

        // 생성된 AnimatedTile을 에셋으로 저장합니다.
        AssetDatabase.CreateAsset(animatedTile, $"{saveFolderPath}/{sprites[0].name}.asset");
        
        // 에셋 데이터베이스를 저장합니다.
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("AnimatedTile 생성 완료!");
    }
#endif
}