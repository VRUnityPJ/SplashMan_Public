using Runtime.Bonus.Core;
using TMPro;
using UnityEngine;

namespace Runtime.Bonus.UI
{
    /// <summary>
    /// ボーナスのランクごとの文字色をまとめた設定。
    /// ゲーム中の表示とリザルトの表示で同じアセットを参照することで、色を1か所で管理する
    /// </summary>
    [CreateAssetMenu(menuName = "BonusRankColors")]
    public class BonusRankColors : ScriptableObject
    {
        [SerializeField, Tooltip("レアなボーナスの文字色")]
        private Color _rareColor = new(1f, 0.84f, 0f);

        /// <summary>
        /// ランクに応じた文字色をテキストに反映する。
        /// 通常のランクは色を上書きせず、テキストの元の色のままにする
        /// </summary>
        public void ApplyTo(TMP_Text text, BonusRank rank)
        {
            switch (rank)
            {
                case BonusRank.Rare:
                    text.color = _rareColor;
                    break;
            }
        }
    }
}
