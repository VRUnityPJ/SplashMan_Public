using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.GameSystem.Timer
{
    public class HitStop : IDisposable
    {
        private CancellationTokenSource _internalCts;

        /// <summary>
        /// ヒットストップを非同期で実行するメソッド。指定された時間だけ時間の速度を変更する。
        /// timescaleを1以上にすれば逆に加速も可能
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="timeScale"></param>
        /// <param name="externalToken"></param>
        public async UniTaskVoid HitStopAsync(float duration, float timeScale,CancellationToken externalToken = default)
        {
            // 既に実行中のヒットストップがあればキャンセル（上書き処理）
            CancelHitStop();
            // このメソッド呼び出し専用のローカルなCancellationTokenSourceを作成
            var localCts = new CancellationTokenSource();
            _internalCts = localCts;

            // 統合
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(localCts.Token, externalToken);

            try
            {
                Time.timeScale = timeScale;

                // 統合されたトークンを使って待機
                await UniTask.Delay(TimeSpan.FromSeconds(duration), ignoreTimeScale: true, cancellationToken: linkedCts.Token);

                // 待機が正常に終了し、かつ自分が最新のヒットストップ処理である場合のみ時間を戻す
                if (_internalCts == localCts)
                {
                    Time.timeScale = 1.0f;
                    _internalCts = null;
                }
            }
            catch (OperationCanceledException)
            {
                // 外部のほうのトークンでキャンセルされたならここの処理を行う
                if (externalToken.IsCancellationRequested && _internalCts == localCts)
                {
                    Time.timeScale = 1.0f;
                    _internalCts = null;
                }
            }
            finally
            {
                localCts.Dispose();
            }
        }
        /// <summary>
        /// ヒットストップをキャンセルするメソッド。実行中のヒットストップがあれば即座に通常の時間速度に戻す。
        /// </summary>
        public void CancelHitStop()
        {
            if (_internalCts != null)
            {
                _internalCts.Cancel();
                _internalCts = null;
                Time.timeScale = 1.0f;
            }
        }

        /// <summary>
        /// DIコンテナから破棄される際に呼ばれる（MonoBehaviourのOnDestroyの代わり）
        /// </summary>
        public void Dispose()
        {
            CancelHitStop();
        }
    }
}
