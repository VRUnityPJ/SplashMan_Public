using R3;
using UnityEngine;

namespace Runtime.Bottle
{
    /// <summary>
    /// 各ボトルの情報をまとめているクラス
    /// </summary>
    public class BottleCarbonationStatus : MonoBehaviour
    {
        [SerializeField] private BottleData _bottleData;
        private ReactiveProperty<BottleCarbonationEnum.CarbonationEnum> _currentCarbonationEnum = new ReactiveProperty<BottleCarbonationEnum.CarbonationEnum>(BottleCarbonationEnum.CarbonationEnum.Low);

        public Carbonation.Carbonation CurrentCarbonation { get; private set; }
        public Carbonation.Carbonation MaxCarbonation => _bottleData.MaxCarbonation;
        public Carbonation.Carbonation MinCarbonation => _bottleData.MinCarbonation;
        public ReadOnlyReactiveProperty<BottleCarbonationEnum.CarbonationEnum> CurrentCarbonationEnum => _currentCarbonationEnum;

        private void Awake()
        {
            _currentCarbonationEnum.AddTo(this);
        }

        private void Start()
        {
            CurrentCarbonation = MinCarbonation;
        }

        private void OnDisable()
        {
            CurrentCarbonation = MinCarbonation;
            _currentCarbonationEnum.Value = BottleCarbonationEnum.CarbonationEnum.Low;
        }

        /// <summary>
        /// 現在の炭酸量を設定する
        /// </summary>
        /// <param name="carbonation"></param>
        public void SetCurrentCarbonation(Carbonation.Carbonation carbonation)
        {
            CurrentCarbonation = carbonation;
            if (CurrentCarbonation >= MaxCarbonation/3 && CurrentCarbonation < MaxCarbonation*2/3)
            {
                _currentCarbonationEnum.Value = BottleCarbonationEnum.CarbonationEnum.Medium;
            }
            else if (CurrentCarbonation >= MaxCarbonation*2/3 && CurrentCarbonation < MaxCarbonation)
            {
                _currentCarbonationEnum.Value = BottleCarbonationEnum.CarbonationEnum.High;
            }
            else if (CurrentCarbonation >= MaxCarbonation)
            {
                _currentCarbonationEnum.Value = BottleCarbonationEnum.CarbonationEnum.Max;
            }
        }
    }
}