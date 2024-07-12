using System;
using System.Collections.Generic;
using UnityEngine;

namespace CombatScene.System
{
    public class AttackFocusPool : MonoBehaviour
    {
        private List<AttackFocus> attackFocuses;
        private Dictionary<Vector2, PlacedAttackFocus> attackFocusMap;
        private int idx;
        [SerializeField]
        private Vector3 poolPosition;

        [SerializeField]
        private int numOfDefaultFocus;
        [SerializeField] 
        private AttackFocus attackFocusPrefab;

        private void Start()
        {
            attackFocusMap = new Dictionary<Vector2, PlacedAttackFocus>();
            attackFocuses = new List<AttackFocus>();
            for (int i = 0; i < numOfDefaultFocus; i++)
            {
                SpawnFocus(true);
            }
        }

        public AttackFocus GetAttackFocus()
        {
            AttackFocus attackFocus = attackFocuses[idx++];
            idx %= attackFocuses.Count;

            if (attackFocus.gameObject.activeInHierarchy)
            {
                attackFocus = SpawnFocus(false);
            }

            return attackFocus;
        }

        public void PlaceAttackFocus(Vector2 position)
        {
            if (attackFocusMap.ContainsKey(position))
            {
                attackFocusMap[position].count++;
                return;
            }
            
            AttackFocus attackFocus = attackFocuses[idx++];
            idx %= attackFocuses.Count;

            if (attackFocus.gameObject.activeInHierarchy)
            {
                attackFocus = SpawnFocus(false);
            }

            attackFocus.transform.position = new Vector3(position.x + ConstVariables.tileSizeX/2, position.y + ConstVariables.tileSizeY /2);
            attackFocus.gameObject.SetActive(true);
            PlacedAttackFocus placedAttackFocus = new PlacedAttackFocus();
            placedAttackFocus.count = 1;
            placedAttackFocus.attackFocus = attackFocus;
            
            attackFocusMap.Add(position, placedAttackFocus);
        }
        
        public void ReturnAttackFocus(Vector2 position)
        {
            if (!attackFocusMap.TryGetValue(position, out var placedAttackFocus))
            {
                Debug.LogError("Attack Focus가 위치 " + position + " 에 존재하지 않습니다.");
                return;
            }

            if (--placedAttackFocus.count <= 0)
            {
                attackFocusMap.Remove(position);
                ReturnToPool(placedAttackFocus.attackFocus);
            }
        }
        
        private AttackFocus SpawnFocus(bool save)
        {
            AttackFocus attackFocus = Instantiate(attackFocusPrefab, this.gameObject.transform);
            attackFocus.isSave = save;
            if (save)
            {
                attackFocuses.Add(attackFocus);
                attackFocus.gameObject.SetActive(false);
            }
            
            return attackFocus;
        }

        public void ReturnToPool(AttackFocus attackFocus)
        {
            if (attackFocus.isSave)
            {
                attackFocus.gameObject.SetActive(false);
                attackFocus.transform.position = poolPosition;
            }
            else
            {
                Destroy(attackFocus.gameObject);
            }
        }

        private class PlacedAttackFocus
        {
            public int count;
            public AttackFocus attackFocus;
        }
    }
}