public class ExtraDrawRelic : Relic
{
    public int extraDraws = 1;

    public override void OnGameStart()
    {
        RelicName = "Ancient Tome";
        Description = "Draw 1 extra card each turn";
    }

    public override void OnTurnStart()
    {
        if (CardsController.Instance != null)
        {
            CardsController.Instance.StartCoroutine(CardsController.Instance.Draw(extraDraws));
        }
    }
}