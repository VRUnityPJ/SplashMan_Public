using R3;
using Runtime.PlayerAction;
using UnityEngine;

namespace Runtime.Bottle
{
    public class ChangeBottle : MonoBehaviour
    {
        [SerializeField] private Vector3 _leftSpawnPosition;
        [SerializeField] private Vector3 _rightSpawnPosition;
        [SerializeField] private Quaternion _spawnRotation;
        [SerializeField] private CatchThrowBottle _leftCatchThrowBottle;
        [SerializeField] private CatchThrowBottle _rightCatchThrowBottle;
        [SerializeField] private GameObject[] _bottlePrefabs;
        private RandomBottleSpawner _randomBottleSpawner;
        private GameObject _leftBottle;
        private GameObject _rightBottle;

        private void Awake()
        {
            _randomBottleSpawner = new RandomBottleSpawner(_bottlePrefabs);
            _leftBottle = _randomBottleSpawner.RentRandomBottle(_leftSpawnPosition, _spawnRotation);
            _rightBottle = _randomBottleSpawner.RentRandomBottle(_rightSpawnPosition, _spawnRotation);
            Subscribe();
        }

        private void Subscribe()
        {
            _leftCatchThrowBottle.OnTriggerEnterBottle
                .Subscribe(this, static (other, self) =>
                {
                    self.ChangeNextBottle(other);
                }).AddTo(this);

            _rightCatchThrowBottle.OnTriggerEnterBottle
                .Subscribe(this, static (other, self) =>
                {
                    self.ChangeNextBottle(other);
                }).AddTo(this);

            BottleCollision.BottleCollisionToFactory
                .Subscribe(this, static (bottleObj, self) =>
                {
                    self._randomBottleSpawner.ReturnIndicatedBottle(bottleObj);
                }).AddTo(this);
        }

        private void ChangeNextBottle(Collider other)
        {
            if (other.gameObject == _leftBottle)
            {
                _leftBottle = null;
                _leftBottle = _randomBottleSpawner.RentRandomBottle(_leftSpawnPosition, _spawnRotation);
            }
            else if (other.gameObject == _rightBottle)
            {
                _rightBottle = null;
                _rightBottle = _randomBottleSpawner.RentRandomBottle(_rightSpawnPosition, _spawnRotation);
            }
        }
    }
}