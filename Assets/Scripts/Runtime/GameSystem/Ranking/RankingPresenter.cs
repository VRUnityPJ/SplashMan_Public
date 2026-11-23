using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Audio;
using UnityEngine;
using VContainer;
using AudioType = Runtime.Audio.AudioType;

namespace Runtime.GameSystem.Ranking
{
    /// <summary>
    /// ランキング画面の表示を管理するクラス
    /// ランキングシステムにアタッチする予定
    /// </summary>
    public class RankingPresenter : MonoBehaviour
    {
        [SerializeField] RankingAnimation _rankingAnimation;
        [SerializeField] GameObject _rankingPrefab;
        [SerializeField] RectTransform _rankingContentParent;
        private AudioManager _audioManager;
        private RankingPlayFab _rankingPlayFab;

        [Inject]
        private void Construct(AudioManager audioManager) => _audioManager = audioManager;

        public async UniTask ShowRankingAsync((string playerName, int score)[] rankingList, CancellationToken cancellationToken)
        {
            _audioManager.PlaySeAsync(AudioType.Ranking.RankingPanelPopup, cancellationToken).Forget();
            await _rankingAnimation.FadeInPanelAsync(cancellationToken);
            for (var i = rankingList.Length - 1; i >= 0; i--)
            {
                var (playerName, score) = rankingList[i];
                var rankingObject = Instantiate(_rankingPrefab, _rankingContentParent);
                var position = CalculateResultPosition.GetPosition(i + 1);
                rankingObject.transform.localPosition = position;
                rankingObject.TryGetComponent(out SetRankingSlot setRankingSlot);
                setRankingSlot.SetData(i+1, playerName, score);
                _audioManager.PlaySeAsync(AudioType.Ranking.RankingPopup, cancellationToken).Forget();
                await _rankingAnimation.ShowRankingAsync(rankingObject, cancellationToken);

            }
            await UniTask.Delay(System.TimeSpan.FromSeconds(10), cancellationToken: cancellationToken);
            await _rankingAnimation.FadeOutPanelAsync(cancellationToken);
        }

#if UNITY_EDITOR
        [Button("ダミーランキング表示")]
        private void DebugShowRanking()
        {
            var dummyRankingList = new (string playerName, int score)[]
            {
                ("Player1", 1000),
                ("Player2", 900),
                ("Player3", 800),
                ("Player4", 700),
                ("Player5", 600),
                ("Player6", 500),
                ("Player7", 400)
            };
            ShowRankingAsync(dummyRankingList, CancellationToken.None).Forget();
        }

#endif
    }
}