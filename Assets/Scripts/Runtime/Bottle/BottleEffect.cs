using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using Runtime.Carbonation;

namespace Runtime.Bottle
{
    /// <summary>
    /// ボトルのエフェクトを管理するクラス
    /// </summary>
    public class BottleEffect : MonoBehaviour
    {
        [SerializeField] private GameObject _bottleEffectPrefab;
        [SerializeField] private Material _materialMedium;
        [SerializeField] private Material _materialHigh;
        [SerializeField] private Material _materialMax;

        private BottleCarbonationStatus _bottleCarbonationStatus;
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            TryGetComponent(out _bottleCarbonationStatus);
            _bottleEffectPrefab.TryGetComponent(out _meshRenderer);
            Subscribe();
        }

        private void Subscribe()
        {
            _bottleCarbonationStatus.CurrentCarbonationEnum
                .Subscribe( value =>
                {
                    switch (value)
                    {
                        case BottleCarbonationEnum.CarbonationEnum.Medium:
                            _bottleEffectPrefab.SetActive(true);
                            _meshRenderer.material = _materialMedium;
                            break;
                        case BottleCarbonationEnum.CarbonationEnum.High:
                            _meshRenderer.material = _materialHigh;
                            break;
                        case BottleCarbonationEnum.CarbonationEnum.Max:
                            _meshRenderer.material = _materialMax;
                            break;
                    }
                }).AddTo(this);
        }

        private void OnDisable()
        {
            _bottleEffectPrefab.SetActive(false);
        }
    }
}