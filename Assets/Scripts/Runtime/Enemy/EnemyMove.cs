using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Runtime.Audio;
using UnityEngine;
using VContainer;
using AudioType = Runtime.Audio.AudioType;

namespace Runtime.Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyMove : MonoBehaviour
    {
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private float _destroyDistance;

        /// <summary>
        /// プレイヤーの元に到達したときに再生するエフェクトのPrefab. 未設定なら再生しない.
        /// </summary>
        [SerializeField] private ParticleSystem _reachEffect;

        /// <summary>
        /// 移動速度. EnemyCoreからSetSpeedで設定される.
        /// </summary>
        private float _moveSpeed;

        private readonly Subject<Unit> _reachedSubject = new Subject<Unit>();

        /// <summary>
        /// プレイヤーの近くまで来たときに流れる.
        /// </summary>
        public Observable<Unit> OnReached => _reachedSubject;

        private AudioManager _audioManager;

        /// <summary>
        /// シーンが破棄されるまで有効なトークン.
        /// 敵自身は到達後に破棄されるので、到達時の効果音はこちらのトークンで鳴らしきる.
        /// </summary>
        private CancellationToken _sceneCancellationToken;

        [Inject]
        public void Construct(AudioManager audioManager, CancellationToken sceneCancellationToken)
        {
            _audioManager = audioManager;
            _sceneCancellationToken = sceneCancellationToken;
        }

        public void SetSpeed(float speed) => _moveSpeed = speed;

        private void Awake()
        {
            _reachedSubject.AddTo(this);
        }

        private void Start()
        {
            //Playerタグのオブジェクトを探してセットする
            if (_playerTransform == null)
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    _playerTransform = playerObj.transform;
                }
            }
        }

        private void Update()
        {
            Move(_moveSpeed);
            transform.LookAt(_playerTransform);
        }

        private void Move(float moveSpeed)
        {
            transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, moveSpeed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Fence"))
            {
                enabled = false; // 到達したら自分で止まる
                PlayReachEffect(collision.GetContact(0).point);
                PlayReachSound();
                _reachedSubject.OnNext(Unit.Default);
            }
        }

        /// <summary>
        /// 到達時の効果音を再生する.
        /// </summary>
        private void PlayReachSound()
        {
            // スポナーを経由せずシーンに直接置いた敵は注入されないので、その場合は鳴らさない
            if (_audioManager == null) return;

            _audioManager.PlaySeAsync(AudioType.Enemy.ReachSound, _sceneCancellationToken).Forget();
        }

        /// <summary>
        /// 到達エフェクトを再生する.
        /// 敵自身は到達後に破棄されるので、エフェクトは敵の子にせず単独で生成し、再生が終わったら破棄する.
        /// </summary>
        private void PlayReachEffect(Vector3 position)
        {
            if (_reachEffect == null) return;

            var effect = Instantiate(_reachEffect, position, Quaternion.identity);
            effect.Play();

            var main = effect.main;
            Destroy(effect.gameObject, main.duration + main.startLifetime.constantMax);
        }
    }
}
