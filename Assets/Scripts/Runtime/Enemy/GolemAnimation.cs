using UnityEngine;
using UnityEngine.InputSystem;
using R3;

namespace Runtime.Enemy
{
    public class GolemAnimation : MonoBehaviour
    {
        private Animator _anim;

        [SerializeField] private float _waitTimeForDeath = 1.5f;//死ぬまでの猶予時間

        private EnemyCore _enemycore;

        private bool _isDefeatTriggered = false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Debug.Log("スタート");
            _enemycore = GetComponent<EnemyCore>();
            _anim = GetComponent<Animator>();//アニメーション取得用
            _enemycore.OnDefeated.Subscribe(_=> ToDeath());
        }

        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)//デバッグ
            {
                Debug.Log("【ログ1】Tabキーの入力を検知しました！");
                if (_enemycore != null)
                {
                    //_enemycore.Death = true;
                    //Debug.Log($"【ログ2】_enemycore.Death を { _enemycore.Death } に変更しました！");
                    ToDeath();
                }
            }
        }

        // Update is called once per frame
        void ToDeath()
        {
            
            if (_enemycore != null  && !_isDefeatTriggered)
            {
                //Debug.Log("【ログ4】Animatorに GolemDefeat (true) を送信します！");
                _isDefeatTriggered = true;//二度と再生されないようロック
                _anim.SetTrigger("OnDeath");
                Destroy(gameObject, _waitTimeForDeath);
            } 
        }
    }
}