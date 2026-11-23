using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Runtime.Audio
{
    /// <summary>
    /// BGM/SEの再生を担当するクラス。SystemLifetimeScopeでVContainerに登録され、コンストラクタインジェクションで配られる。
    /// 再生はUniTaskでawait可能。渡したCancellationTokenをキャンセルすると、その時点で再生を停止する。
    /// </summary>
    public class AudioManager
    {
        private readonly SeAudioDataBase _seDataBase;
        private readonly BgmAudioDataBase _bgmDataBase;
        private readonly AudioSourcePool _sePool;
        private readonly BgmAudioSource _bgmAudioSource;

        public AudioManager(SeAudioDataBase seDataBase, BgmAudioDataBase bgmDataBase, AudioSourcePool sePool, BgmAudioSource bgmAudioSource)
        {
            _seDataBase = seDataBase;
            _bgmDataBase = bgmDataBase;
            _sePool = sePool;
            _bgmAudioSource = bgmAudioSource;
        }

        /// <summary>
        /// 名前で指定したSEを再生する。再生が終わるまでawaitできる。
        /// cancellationTokenがキャンセルされた場合、即座に再生を停止したうえでOperationCanceledExceptionを送出する
        /// (EnemyWaveDirectorなど他のUniTask処理と同様、呼び出し側でForget()や例外ハンドラを通す想定)。
        /// </summary>
        public async UniTask PlaySeAsync(string name, CancellationToken cancellationToken)
        {
            var clip = _seDataBase.GetAudioClip(name);
            if (clip == null)
            {
                return;
            }

            var source = _sePool.Rent();
            source.clip = clip;
            source.Play();
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(clip.length), cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // キャンセルされた場合は例外を握りつぶす
            }
            finally
            {
                if (source != null)
                {
                    source.Stop();
                    _sePool.Return(source);
                }
            }
        }

        /// <summary>
        /// 名前で指定したSEをループ再生する。再生が終わるまでawaitできる。
        /// cancellationTokenがキャンセルされた場合、即座に再生を停止したうえでOperationCanceledExceptionを送出する
        /// (EnemyWaveDirectorなど他のUniTask処理と同様、呼び出し側でForget()や例外ハンドラを通す想定)。
        /// </summary>
        public async UniTask PlaySeLoopAsync(string name, CancellationToken cancellationToken)
        {
            var clip = _seDataBase.GetAudioClip(name);
            if (clip == null)
            {
                return;
            }

            var source = _sePool.Rent();
            source.clip = clip;
            source.loop = true;
            source.Play();
            try
            {
                await UniTask.WaitUntilCanceled(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // キャンセルされた場合は例外を握りつぶす
            }
            finally
            {
                if (source != null)
                {
                    source.Stop();
                    _sePool.Return(source);
                }
            }
        }

        /// <summary>
        /// 名前で指定したBGMを再生する。既に再生中のBGMがあれば即座に停止してから切り替える。
        /// cancellationTokenがキャンセルされるまで鳴り続け、キャンセルされたらBGMを停止する。
        /// </summary>
        public async UniTask PlayBgmAsync(string name, CancellationToken cancellationToken)
        {
            var clip = _bgmDataBase.GetAudioClip(name);
            if (clip == null)
            {
                return;
            }

            _bgmAudioSource.Play(clip);
            try
            {
                await UniTask.WaitUntilCanceled(cancellationToken);
            }
            finally
            {
                _bgmAudioSource.Stop();
            }
        }
    }
}
