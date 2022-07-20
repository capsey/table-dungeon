using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace TableDungeon.Maze
{
    public class DiceScript : MonoBehaviour
    {
        private static readonly Dictionary<int, Quaternion> DiceRotations = new Dictionary<int, Quaternion>
        {
            { 1, Quaternion.Euler(180, 0, 90) },
            { 2, Quaternion.Euler(90, 0, 90) },
            { 3, Quaternion.identity },
            { 4, Quaternion.Euler(0, 0, 180) },
            { 5, Quaternion.Euler(270, 0, 90) },
            { 6, Quaternion.Euler(0, 0, 90) }
        };

        public Transform modelTransform;
        
        private Random _random;

        private void Awake()
        {
            _random = new Random();
            Next();
        }

        public int Next()
        {
            // Randomly move the dice
            transform.localRotation = Quaternion.Euler(0, _random.NextRange(0, 360), 0);
            var offset = new Vector3(_random.NextRange(-0.25F, 0.25F), 0, _random.NextRange(-0.05F, 0.05F));
            modelTransform.localPosition = offset;

            // Calculating result
            var result = _random.Next(1, 6 + 1);
            modelTransform.localRotation = DiceRotations[result];
            return result;
        }
    }
}