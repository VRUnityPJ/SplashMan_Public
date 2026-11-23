using UnityEngine;

namespace Runtime.Bottle
{
    /// <summary>
    /// ボトルのデータを管理するクラス
    /// </summary>
    [CreateAssetMenu]
    public class BottleData : ScriptableObject
    {
        [SerializeField]
        private float _maxCarbonation;

        [SerializeField]
        private float _minCarbonation;

        public Carbonation.Carbonation MaxCarbonation
            => new Carbonation.Carbonation(_maxCarbonation);

        public Carbonation.Carbonation MinCarbonation
            => new Carbonation.Carbonation(_minCarbonation);
    }
}