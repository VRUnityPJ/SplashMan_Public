using TMPro;
using UnityEngine;

namespace Runtime.GameSystem.Timer
{
    public class TimeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;

        /// <summary>
        /// タイマーテキストを受け取った文字列で書き換える
        /// </summary>
        /// <param name="formattedTime">整形済みの文字列</param>
        public void SetTimeText(string formattedTime)
        {
            _timerText.text = formattedTime;
        }
    }
}