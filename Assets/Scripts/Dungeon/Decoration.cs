using System;
using UnityEngine;
using Random = System.Random;

namespace TableDungeon.Dungeon
{
    public class Decoration : MonoBehaviour
    {
        [SerializeField] private bool dontHide;
        [SerializeField] private bool dontOffset;
        [Space]
        [SerializeField] private Vector2 minDeviation;
        [SerializeField] private Vector2 maxDeviation;
        
        private Vector3 _initialPosition;

        private void Awake()
        {
            _initialPosition = transform.position;
        }
        
        public void Randomize(Random random, float chance)
        {
            if (!dontHide)
            {
                gameObject.SetActive(random.NextDouble() < chance);
            }
            
            if (!dontOffset)
            {
                var min = _initialPosition + (Vector3) minDeviation;
                var max = _initialPosition + (Vector3) maxDeviation;
                transform.position = new Vector3(
                    (float) random.NextRange(min.x, max.x),
                    (float) random.NextRange(min.y, max.y),
                    (float) random.NextRange(min.z, max.z)
                );
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) return;

            var size = maxDeviation - minDeviation;
            var center = transform.position + (Vector3) (maxDeviation + minDeviation) / 2;
            
            Gizmos.DrawWireCube(center, size);
        }
    }
}