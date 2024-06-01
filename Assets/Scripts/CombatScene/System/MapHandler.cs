using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CombatScene
{
    public class MapHandler : MonoBehaviour
    {
        [InfoBox("startPosition은 0,0이 맵의 왼쪽 아래를 지정하도록 위치를 조정")]
        public Vector2 startPosition = Vector2.zero;
        public LayerMask collisionLayer;
        
        [InfoBox("맵 정보 파일이 존재하더라도 강제로 업데이트 시켜줄지 여부를 결정")]
        public bool forceUpdateMap = false;
        
        
        [InfoBox("맵 경계 Collider2D를 추가해주세요!", InfoMessageType.Error, "IsColliderNotSet")]
        public TilemapCollider2D mapCollider2D;
        
        private ObjectType[,] mapData;
        private int height = ConstVariables.mapHeight;
        private int width = ConstVariables.mapWidth;
        
        private void Awake()
        {
            startPosition.x += ConstVariables.tileSizeX / 2;
            startPosition.y += ConstVariables.tileSizeY / 2;
            
            if (forceUpdateMap)
            {
                Debug.Log(GetType().Name + " Force Update Detected. Generating Map");
                GenerateMapData();
                SaveMapData();
                return;
            }
            
            if (PlayerPrefs.HasKey("MapData"))
            {
                Debug.Log(GetType().Name + " Find Key! Loading Map");
                LoadMapData();
            }
            else
            {
                Debug.Log(GetType().Name + " There is no Key. Generating Map");
                GenerateMapData();
                SaveMapData();
            }
        }
        
        private void GenerateMapData()
        {
            mapData = new ObjectType[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 currentPosition = startPosition + new Vector2(x * ConstVariables.tileSizeX, y * ConstVariables.tileSizeY);

                    if (IsCollisionArea(currentPosition))
                    {
                        mapData[x, y] = ObjectType.Block;
                    }
                    else
                    {
                        mapData[x, y] = ObjectType.Load;
                    }
                }
            }
            Debug.Log(GetType().Name + "Generating Map Done!");
        }

        private bool IsCollisionArea(Vector2 worldPosition)
        {
            return mapCollider2D.OverlapPoint(worldPosition);
        }

        private void SaveMapData()
        {
            string json = JsonUtility.ToJson(new MapDataWrapper(mapData));
            PlayerPrefs.SetString("MapData", json);
            PlayerPrefs.Save();
            Debug.Log(GetType().Name + "Saving Map Done!");
            Debug.Log( GetType().Name + "Json File size is " + json.Length);
        }

        private void LoadMapData()
        {
            string json = PlayerPrefs.GetString("MapData");
            mapData = JsonUtility.FromJson<MapDataWrapper>(json).GetMapData();
            Debug.Log(GetType().Name + "Loading Map Done!");
            Debug.Log( GetType().Name + "Json File size is " + json.Length);
        }
        
        [Serializable]
        private class MapDataWrapper
        {
            public ObjectType[] mapData;
            public int rows;
            public int columns;

            public MapDataWrapper(ObjectType[,] mapData)
            {
                rows = mapData.GetLength(0);
                columns = mapData.GetLength(1);
                this.mapData = new ObjectType[rows * columns];

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        this.mapData[i * columns + j] = mapData[i, j];
                    }
                }
            }

            public ObjectType[,] GetMapData()
            {
                ObjectType[,] result = new ObjectType[rows, columns];

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        result[i, j] = mapData[i * columns + j];
                    }
                }

                return result;
            }
        }
        
        /// <summary>
        /// worldPosition x, y가 맵 내부에 있는지 확인하는 함수
        /// </summary>
        /// <param name="x">world position x</param>
        /// <param name="y">world position y</param>
        /// <returns>true if x,y inside map</returns>
        public bool IsInsideMap(int x, int y)
        {
            int idxX = (int)(((x - startPosition.x + ConstVariables.tileSizeX/2)) / ConstVariables.tileSizeX);
            int idxY = (int)(((y - startPosition.y + ConstVariables.tileSizeY/2)) / ConstVariables.tileSizeY);

            return idxX is >= 0 and < ConstVariables.mapWidth && idxY is >= 0 and < ConstVariables.mapHeight;
        }
        
        /// <summary>
        /// x, y의 맵 정보를 가져오는 함수
        /// </summary>
        /// <param name="x">가로 축 값</param>
        /// <param name="y">세로 축 값</param>
        /// <returns>해당하는 타일의 ObjectType</returns>
        public ObjectType GetPoint(int x, int y)
        {
            int idxX = (int)(((x - startPosition.x + ConstVariables.tileSizeX/2)) / ConstVariables.tileSizeX);
            int idxY = (int)(((y - startPosition.y + ConstVariables.tileSizeY/2)) / ConstVariables.tileSizeY);
            return mapData[idxX , idxY];
        }
        
        /// <summary>
        /// Vector2의 맵 정보를 가져오는 함수
        /// </summary>
        /// <param name="point"> 가져올 맵 정보의 World Position 좌표</param>
        /// <returns>해당하는 타일의 ObjectType. 영역 벗어날 시 Block return</returns>
        public ObjectType GetPoint(Vector2 point)
        {
            Vector2 relativePoint = point - startPosition;
            int idxX = (int)((relativePoint.x + ConstVariables.tileSizeX/2) / ConstVariables.tileSizeX);
            int idxY = (int)((relativePoint.y + ConstVariables.tileSizeY/2) / ConstVariables.tileSizeY);
            if (idxX >= ConstVariables.mapWidth || idxY >= ConstVariables.mapHeight)
                return ObjectType.Block;
            return mapData[idxX , idxY];
        }
        
        /// <summary>
        /// Block 타일을 제외한 타일의 오브젝트 값을 지정한 type으로 변경하는 함수
        /// </summary>
        /// <param name="point">변경할 타일의 WorldPosition</param>
        /// <param name="type">설정할 타일의 오브젝트 값</param>
        /// <returns> 성공 여부 반환</returns>
        public bool SetMapObject(Vector2 point, ObjectType type)
        {
            if (type.Equals(ObjectType.Block))
            {
                // Block 타일은 변경 불가
                return false;
            }
            Vector2 relativePoint = point - startPosition;
            int idxX = (int)((relativePoint.x + ConstVariables.tileSizeX/2) / ConstVariables.tileSizeX);
            int idxY = (int)((relativePoint.y + ConstVariables.tileSizeY/2) / ConstVariables.tileSizeY);

            if (mapData[idxX, idxY].Equals(type))
            {
                return false;
            }
            
            mapData[idxX, idxY] = type;

            return true;
        }

        #region Odin

        private bool IsColliderNotSet()
        {
            return mapCollider2D == null;
        }

        #endregion
    }

    [Serializable]
    public enum ObjectType
    {
        Block,
        Player,
        Enemy,
        Load
    }
}