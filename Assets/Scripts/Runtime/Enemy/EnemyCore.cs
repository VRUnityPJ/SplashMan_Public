using System.Threading;
using UnityEngine.InputSystem;
using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using Runtime.GameSystem.Timer;
using Runtime.Scores;
using Runtime.Utility.UI;

namespace Runtime.Enemy
{
    [RequireComponent(typeof(BoxCollider), typeof(EnemyMove), typeof(EnemyScoreHandler))]
    public class EnemyCore : MonoBehaviour
    {
        [SerializeField]
        private EnemyMasterData _masterData;

        [SerializeField]
        private EnemyHpBarView _hpBarView;

        [SerializeField]
        private DamageTextPopup _damageTextPopupPrefab;

        [SerializeField]
        private Transform _damagePopupPoint;

        private EnemyStatus _status;

        private EnemyMove _move;

        private EnemyScoreHandler _scoreHandler;

        private BoxCollider _boxCollider;

        /// <summary>
        /// 到達アニメーション. 付いていなければnullで、到達時にすぐ破棄する.
        /// </summary>
        private IEnemyReachAnimation _reachAnimation;

        private readonly HitStop _hitStop = new HitStop();

        private readonly Subject<EnemyDefeatInfo> _defeatedSubject = new Subject<EnemyDefeatInfo>();

        /// <summary>
        /// 撃破またはプレイヤーへの到達によって、敵の結末が決まったかどうかのフラグ.
        /// 撃破と到達の処理を二重で行わないようにするためのフラグ.
        /// </summary>
        private bool _isDefeated;

        public Observable<EnemyDefeatInfo> OnDefeated => _defeatedSubject;

        private void Awake()
        {
            _status = new EnemyStatus(_masterData);
            _hpBarView.SetMaxHp(_masterData.Hp);
            _defeatedSubject.AddTo(this);
            TryGetComponent(out _reachAnimation);
            TryGetComponent(out _move);
            TryGetComponent(out _scoreHandler);
            TryGetComponent(out _boxCollider);
            _move.SetSpeed(_status.Speed);
            Subscribe();
        }

        /// <summary>
        /// プレイヤーに到達したときに呼ぶ. スコアを減らし、
        /// 到達アニメーションがあれば再生し終わってから、なければすぐに自身を破棄する.
        /// </summary>
        private void Reach()
        {
            if (_isDefeated) return;
            _isDefeated = true;
            _scoreHandler.Subtract(new Score(_status.Attack));
            _boxCollider.enabled = false; // 攻撃に当たらないようにする

            ReachAsync(destroyCancellationToken).Forget();
        }

        private async UniTaskVoid ReachAsync(CancellationToken ct)
        {
            if (_reachAnimation != null)
                await _reachAnimation.PlayReachAsync(ct);

            Destroy(gameObject);
        }

        private void Subscribe()
        {
            _move.OnReached
                .Subscribe(_ => Reach())
                .AddTo(this);

            _status.Hp
                .Where(currentHp => currentHp <= 0 && !_isDefeated)
                .Subscribe(_ =>
                {
                    _isDefeated = true;
                    _move.SetSpeed(0f); // 撃破されたら止まる
                    var defeatInfo = new EnemyDefeatInfo(_masterData, transform.position);
                    _scoreHandler.Add(defeatInfo.Score);
                    _defeatedSubject.OnNext(defeatInfo);
                })
                .AddTo(this);

            _status.Hp
                .Pairwise()
                .Where(pair => pair.Previous != pair.Current)
                .Subscribe(pair =>
                {
                    _hpBarView.UpdateHp(pair.Current);
                    SpawnDamagePopup(pair.Previous - pair.Current);
                })
                .AddTo(this);
        }

        public bool ApplyDamage(int damage)
        {
            // 撃破済み(撃破アニメーション中)の敵には当たらない. 同じ敵を二重に撃破扱いしないようにする
            if (_isDefeated) return false;

            _status?.TakeDamage(damage);
            _hitStop.HitStopAsync(
                duration: 0.01f,
                timeScale: 0.0f,
                externalToken: this.GetCancellationTokenOnDestroy()
            ).Forget();

            return _status?.Hp.CurrentValue <= 0;
        }

        public void Despawn() => Destroy(gameObject);

        private void SpawnDamagePopup(int damage)
        {
            if (!_damageTextPopupPrefab)
            {
                return;
            }

            Vector3 basePos = _damagePopupPoint ? _damagePopupPoint.position : transform.position + Vector3.up * 1.5f;

            Vector3 spawnPos = basePos + new Vector3(Random.Range(-0.2f, 0.2f), 0, 0);

            DamageTextPopup popup = Instantiate(_damageTextPopupPrefab, spawnPos, Quaternion.identity);
            popup.Show(damage);
        }

        private void Update()
        {
            // 新しい Input System でスペースキー入力を取得 ※デバック用
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                ApplyDamage(50);
            }
        }

        private void OnDestroy()
        {
            _status?.Dispose();
            _hitStop?.Dispose();
        }
    }
}