using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Runtime.Utility;
using Runtime.Utility.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.GameSystem.Tutorial
{
    /// <summary>
    /// チュートリアル周りのPresenterクラス
    /// このコンポーネント自体は常にアクティブなオブジェクトに置き、説明ページの表示・非表示は_pageRootで切り替える
    /// </summary>
    public class TutorialPresenter : MonoBehaviour
    {
        [SerializeField] private ButtonBase _selectButton = null!;
        [SerializeField] private ButtonBase _cancelButton = null!;
        [SerializeField] private TextMeshProUGUI _tutorialText = null!;

        [Header("説明ページのUI")]
        [SerializeField, Tooltip("説明ページの親オブジェクト(表示・非表示を切り替える対象)")]
        private GameObject _pageRoot = null!;
        [SerializeField] private Image _pageImage = null!;
        [SerializeField] private TextMeshProUGUI _pageText = null!;
        [SerializeField, Tooltip("操作を伴わないページを読み終えたときに押すボタン")]
        private ButtonBase _nextButton = null!;

        [Header("説明ページの内容")]
        [SerializeField] private TutorialPage _catchPage
            = new("ボトルに手を近づけてつかもう！");
        [SerializeField] private TutorialPage _carbonationPage
            = new("トリガーを押しながらボトルを振ると、炭酸ゲージがたまる！\nゲージが高いほど、攻撃の威力と範囲がアップ！\nゲージを半分以上ためてみよう");
        [SerializeField] private TutorialPage _practicePage
            = new("敵の方を向いて、トリガーを離して腕を振るとボトルを投げる！\n敵を倒してみよう");
        [SerializeField] private TutorialPage _scorePage
            = new("制限時間内に敵をたくさん倒してスコアをかせごう！\n敵に近づかれるとスコアが減ってしまうぞ\n準備ができたらボタンを押してゲームスタート！");

        private void Awake() => HidePage();

        /// <summary>
        /// チュートリアルの初期化処理を行う
        /// </summary>
        public void Initialize() => ShowTutorial();

        /// <summary>
        /// チュートリアルを開始するかユーザーの入力を待機
        /// </summary>
        public async UniTask<bool> SelectStartingTutorialAsync(CancellationToken cancellationToken)
        {
            var yesButton = _selectButton.OnDown
                .Do(_ => CustomDebug.Log("YES OnDown received"))
                .FirstAsync(cancellationToken: cancellationToken)
                .AsUniTask();

            var noButton = _cancelButton.OnDown
                .Do(_ => CustomDebug.Log("No OnDown received"))
                .FirstAsync(cancellationToken: cancellationToken)
                .AsUniTask();

            var (winnerIndex, _, _) = await UniTask.WhenAny(yesButton, noButton);

            // チュートリアルのUIを非表示にする
            HideTutorial();

            //チュートリアル開始が選択された
            if (winnerIndex == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 指定したステップの説明ページを表示する
        /// </summary>
        public void ShowPage(TutorialStep step)
        {
            var page = GetPage(step);
            _pageText.text = page.Text;
            _pageImage.sprite = page.Image;
            _pageImage.gameObject.SetActive(page.Image != null);
            _nextButton.gameObject.SetActive(false);
            _pageRoot.SetActive(true);
        }

        /// <summary>
        /// 説明ページを非表示にする
        /// </summary>
        public void HidePage() => _pageRoot.SetActive(false);

        /// <summary>
        /// 「次へ」ボタンを表示して、押されるまで待機する
        /// </summary>
        public async UniTask WaitForNextAsync(CancellationToken cancellationToken)
        {
            _nextButton.gameObject.SetActive(true);
            await _nextButton.OnDown.FirstAsync(cancellationToken: cancellationToken);
            _nextButton.gameObject.SetActive(false);
        }

        private TutorialPage GetPage(TutorialStep step) => step switch
        {
            TutorialStep.Catch => _catchPage,
            TutorialStep.Carbonation => _carbonationPage,
            TutorialStep.Practice => _practicePage,
            TutorialStep.Score => _scorePage,
            _ => throw new ArgumentOutOfRangeException(nameof(step), step, null),
        };

        private void ShowTutorial()
        {
            _tutorialText.gameObject.SetActive(true);
            _selectButton.gameObject.SetActive(true);
            _cancelButton.gameObject.SetActive(true);
        }

        private void HideTutorial()
        {
            _tutorialText.gameObject.SetActive(false);
            _selectButton.gameObject.SetActive(false);
            _cancelButton.gameObject.SetActive(false);
        }
    }
}
