using Finals_SpaceFlap.GameObjects;

namespace Finals_SpaceFlap.Systems;

public class AsteroidManager
{
    private readonly List<Asteroid> asteroids;
    private readonly Random random;
    private float spawnTimer;
    private const float SpawnInterval = 2.0f;
    private const float MinGap = 150f;
    private const float MaxGap = 250f;
    private readonly int screenWidth;
    private readonly int screenHeight;

    public AsteroidManager(int screenWidth, int screenHeight)
    {
        asteroids = new List<Asteroid>();
        random = new Random();
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
    }

    public void Update(float deltaTime, float spaceshipX)
    {
        spawnTimer += deltaTime;

        // Spawn new asteroids
        if (spawnTimer >= SpawnInterval)
        {
            SpawnAsteroid();
            spawnTimer = 0f;
        }

        // Update existing asteroids
        foreach (var asteroid in asteroids)
        {
            asteroid.Update(deltaTime);
        }

        // Remove off-screen asteroids
        asteroids.RemoveAll(a => a.IsOffScreen(screenWidth));
    }

    private void SpawnAsteroid()
    {
        float gap = random.NextSingle() * (MaxGap - MinGap) + MinGap;
        float asteroidHeight = random.NextSingle() * (screenHeight - gap - 200) + 100;

        // Top asteroid
        asteroids.Add(new Asteroid(screenWidth, 0, 80, asteroidHeight));

        // Bottom asteroid
        float bottomAsteroidY = asteroidHeight + gap;
        float bottomAsteroidHeight = screenHeight - bottomAsteroidY;
        asteroids.Add(new Asteroid(screenWidth, bottomAsteroidY, 80, bottomAsteroidHeight));
    }

    public void Render()
    {
        foreach (var asteroid in asteroids)
        {
            asteroid.Render();
        }
    }

    public List<Asteroid> GetAsteroids()
    {
        return asteroids;
    }

    public void Clear()
    {
        asteroids.Clear();
        spawnTimer = 0f;
    }

    public int CheckScore(float spaceshipX)
    {
        int score = 0;
        foreach (var asteroid in asteroids)
        {
            if (!asteroid.HasBeenPassed && asteroid.RightEdge < spaceshipX)
            {
                asteroid.MarkAsPassed();
                score++;
            }
        }
        return score;
    }
}
