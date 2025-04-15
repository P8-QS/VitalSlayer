public class GameRoundCompleted: Unity.Services.Analytics.Event
{
	public GameRoundCompleted() : base("gameRoundCompleted")
	{
	}

	public bool GameWon { set { SetParameter("gameWon", value); } }
	public int TotalEnemiesKilled { set { SetParameter("totalEnemiesKilled", value); } }
	public int XpGained{ set { SetParameter("xpGained", value); } }
	public int LevelsGained { set { SetParameter("levelsGained", value); } }
	public string RoundDuration { set { SetParameter("roundDuration", value); } }
}