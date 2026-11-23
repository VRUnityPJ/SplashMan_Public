using System.Collections.Generic;
using UnityEngine;
using uPools;

namespace Runtime.Bottle
{
    public class RandomBottleSpawner
    {
        private readonly GameObject[] _bottlePrefabs;
        private readonly GameObjectPool[] _bottlePools;
        private readonly Dictionary<GameObject, GameObjectPool> _bottleObjectToPoolMap
            = new Dictionary<GameObject, GameObjectPool>();

        public RandomBottleSpawner(GameObject[] bottlePrefabs)
        {
            _bottlePrefabs = bottlePrefabs;
            _bottlePools = new GameObjectPool[bottlePrefabs.Length];
            for (var i = 0; i < bottlePrefabs.Length; i++)
            {
                _bottlePools[i] = new GameObjectPool(bottlePrefabs[i]);
            }
        }

        public GameObject RentRandomBottle(Vector3 spawnPosition, Quaternion spawnRotation)
        {
            var randomIndex = Random.Range(0, _bottlePrefabs.Length);
            var pool = _bottlePools[randomIndex];
            var bottle = pool.Rent(spawnPosition, spawnRotation);
            _bottleObjectToPoolMap[bottle] = pool;
            return bottle;
        }

        public void ReturnIndicatedBottle(GameObject bottleObject)
        {
            if (!_bottleObjectToPoolMap.TryGetValue(bottleObject, out var pool))
            {
                return;
            }
            pool.Return(bottleObject);
            _bottleObjectToPoolMap.Remove(bottleObject);
        }
    }
}