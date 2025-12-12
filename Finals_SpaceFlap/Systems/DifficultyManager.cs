namespace Finals_SpaceFlap.Systems;

public class DifficultyManager
{
    public float AsteroidSpeed { get; private set; }
    public float MinGap { get; private set; }
    public float MaxGap { get; private set; }
    public float BackgroundScrollSpeed { get; private set; }

    // Base values
    private const float BaseSpeed = 50f;
    private const float BaseMinGap = 120f; // Increased gap so spaceship can fit
    private const float BaseMaxGap = 160f; // Increased gap so spaceship can fit
    private const float BaseBackgroundSpeed = 50f;

    public DifficultyManager()
    {
        UpdateDifficulty(0);
    }

    public void UpdateDifficulty(int score)
    {
        // Simple gradual speed increase with score
        // Speed increases slightly as you score more
        float speedMultiplier = 1.0f + (score * 0.02f); // 2% speed increase per point

        AsteroidSpeed = BaseSpeed * speedMultiplier;
        MinGap = BaseMinGap;
        MaxGap = BaseMaxGap;
        BackgroundScrollSpeed = BaseBackgroundSpeed * speedMultiplier;
    }
}
