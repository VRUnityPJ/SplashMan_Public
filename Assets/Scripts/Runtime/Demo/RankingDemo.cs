using System;
using Memoria;
using UnityEngine;

namespace Runtime.Demo
{
    public class RankingDemo : MonoBehaviour
    {
        private RankingClient _rankingClient;

        private async void Start()
        {
            try
            {
                _rankingClient = new RankingClient("1F95F8");

                // ログイン
                await _rankingClient.LoginAsync(destroyCancellationToken);

                // スコア送信
                await _rankingClient.SendAsync(
                    "TestUser_0303",
                    1000,
                    "HighScore",
                    destroyCancellationToken);

                await _rankingClient.SendAsync(
                    "TestUser_0303",
                    2000,
                    "HighScore",
                    destroyCancellationToken);

                // ランキング取得
                var leaderboard = await _rankingClient.LoadAsync(
                    "HighScore",
                    10,
                    destroyCancellationToken);

                foreach (var entry in leaderboard)
                {
                    Debug.Log($"{entry.playerName}: {entry.score}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error: {ex.Message}");
            }
        }
    }
}