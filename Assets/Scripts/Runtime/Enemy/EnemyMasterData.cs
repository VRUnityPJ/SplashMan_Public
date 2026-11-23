using UnityEngine;

namespace Runtime.Enemy
{
    [CreateAssetMenu(menuName = "EnemyData")]
    public class EnemyMasterData : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;
        public GameObject Prefab => _prefab;
        [SerializeField] private string _enemyName;
        public string EnemyName => _enemyName;
        [SerializeField] private int _hp;
        public int Hp => _hp;
        [SerializeField] private int _attackPower;
        public int AttackPower => _attackPower; //プレイヤーに到達したとき、この値だけスコアを減らす.
        [SerializeField] private int _defensePower;
        public int DefensePower => _defensePower;
        [SerializeField] private float _moveSpeed;
        public float MoveSpeed => _moveSpeed;
        [SerializeField] private int _score;
        public int Score => _score;
    }
}