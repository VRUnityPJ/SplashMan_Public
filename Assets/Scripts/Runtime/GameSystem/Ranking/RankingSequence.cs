using System.Threading;
using Cysharp.Threading.Tasks;

namespace Runtime.GameSystem.Ranking;

/// <summary>
/// ランキングのシーケンスを管理するクラス
/// </summary>
public class RankingSequence
{
    private readonly RankingPlayFab _rankingPlayFab;
    private readonly RankingPresenter _rankingPresenter;

    public RankingSequence(RankingPlayFab rankingPlayFab, RankingPresenter rankingPresenter)
    {
        _rankingPlayFab = rankingPlayFab;
        _rankingPresenter = rankingPresenter;
    }
    /// <summary>
    /// ランキングのシーケンスを開始する
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async UniTask StartSequenceAsync(CancellationToken cancellationToken)
    {
        var rankingList = await _rankingPlayFab.SubmitAndGetAsync(cancellationToken);
        await _rankingPresenter.ShowRankingAsync(rankingList, cancellationToken);
    }
}