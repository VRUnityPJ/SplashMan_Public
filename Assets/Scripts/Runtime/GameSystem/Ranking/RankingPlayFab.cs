using Memoria;
using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Scores;
using UnityEngine;

namespace Runtime.GameSystem.Ranking;

public class RankingPlayFab
{
    private readonly RankingClient _rankingClient;
    private readonly PlayerScore _playerScore;

    public RankingPlayFab(PlayerScore playerScore)
    {
        _playerScore = playerScore;
        _rankingClient = new RankingClient("");
        Debug.Log("RankingPlayFab が生成されました");
    }

    private async UniTask SendScoreAsync(CancellationToken cancellationToken)
    {
        await _rankingClient.LoginAsync(cancellationToken);
        var playerName = PlayerNameStorage.GetPlayerName();
        if (playerName.Length < 3)
        {
            var randomName = Random.Range(1000, 9999).ToString();
            playerName += randomName;
        }
        var score = _playerScore.TotalScore.CurrentValue.Value;
        Debug.Log($"送信スコア: {score}");
        await _rankingClient.SendAsync(playerName, _playerScore.TotalScore.CurrentValue.Value, "HighScore", cancellationToken);
        Debug.Log($"スコア送信完了: {score}");
    }

    private async UniTask<(string, int)[]> GetRankingAsync(CancellationToken cancellationToken)
    {
        var rankingList = await _rankingClient.LoadAsync("HighScore", 10, cancellationToken);
        return rankingList;
    }

    public async UniTask<(string, int)[]> SubmitAndGetAsync(CancellationToken cancellationToken)
    {
        await SendScoreAsync(cancellationToken);
        await UniTask.Delay(System.TimeSpan.FromSeconds(1.0f), cancellationToken: cancellationToken);
        return await GetRankingAsync(cancellationToken);
    }
}