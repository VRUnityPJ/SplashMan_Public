using System;
using Cysharp.Threading.Tasks;
using Runtime.Utility.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Enemy
{
    public class EnemyHpBarView : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;

        private Camera _camera;

        private void Start() => _camera = Camera.main;

        private void LateUpdate()
        {
            var forward = _camera.transform.forward;
            forward.y = 0;
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        //最大HPを受け取ってスライダーの上限値決めるやつ
        public void SetMaxHp(int maxHp)
        {
            _slider.maxValue = maxHp;
            _slider.value = maxHp;
        }

        //現在のHPを受け取ってスライダーを更新するやつ
        public void UpdateHp(int currentHp)
        {
            GaugeAnimation.PlayAsync(_slider, currentHp, 0.2f, destroyCancellationToken).Forget();
        }
    }
}