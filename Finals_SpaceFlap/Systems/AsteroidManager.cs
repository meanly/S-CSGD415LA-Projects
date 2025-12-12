using Finals_SpaceFlap.GameObjects;

namespace Finals_SpaceFlap.Systems;

public class AsteroidPipe
{
    public List<Asteroid> TopAsteroids { get; set; } // Stacked asteroids forming top pipe
    public List<Asteroid> BottomAsteroids { get; set; } // Stacked asteroids forming bottom pipe
    public bool HasBeenPassed { get; set; }
    public float GapY { get; set; } // Y position of the gap center
    public float GapHeight { get; set; } // Height of the gap

    public AsteroidPipe(List<Asteroid> top, List<Asteroid> bottom, float gapY, float gapHeight)
    {
        TopAsteroids = top;
        BottomAsteroids = bottom;
        HasBeenPassed = false;
        GapY = gapY;
        GapHeight = gapHeight;
    }

    // Get the right edge X position of the pipe (where gap ends)
    public float GetRightEdge()
    {
        if (TopAsteroids.Count > 0)
        {
            return TopAsteroids[0].Position.X + TopAsteroids[0].GetBounds().Width;
        }
        return 0;
    }

    public void Update(float deltaTime)
    {
        foreach (var asteroid in TopAsteroids)
        {
            asteroid.Update(deltaTime);
        }
        foreach (var asteroid in BottomAsteroids)
        {
            asteroid.Update(deltaTime);
        }
    }

    public void Render()
    {
        foreach (var asteroid in TopAsteroids)
        {
            asteroid.Render();
        }
        foreach (var asteroid in BottomAsteroids)
        {
            asteroid.Render();
        }
    }

    public List<Asteroid> GetAllAsteroids()
    {
        List<Asteroid> all = new List<Asteroid>();
        all.AddRange(TopAsteroids);
        all.AddRange(BottomAsteroids);
        return all;
    }
}

public class AsteroidManager
{
    private readonly List<AsteroidPipe> asteroidPipes;
    private readonly Random random;
    private float spawnTimer;
    private const float SpawnInterval = 3.0f; // Longer interval for spacing
    private const float PipeSpacing = 400f; // Large gap between each pipe pair
    private const float AsteroidSegmentHeight = 60f; // Height of each asteroid segment
    private const float PipeWidth = 80f; // Width of the pipe
    private readonly int screenWidth;
    private readonly int screenHeight;
    private DifficultyManager? difficultyManager;

    public AsteroidManager(int screenWidth, int screenHeight, DifficultyManager difficultyManager)
    {
        asteroidPipes = new List<AsteroidPipe>();
        random = new Random();
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        this.difficultyManager = difficultyManager;
    }

    public void Update(float deltaTime, float spaceshipX)
    {
        spawnTimer += deltaTime;

        // Spawn new asteroid pipes with proper spacing
        bool shouldSpawn = false;

        if (asteroidPipes.Count == 0)
        {
            // Spawn first pipe immediately
            shouldSpawn = true;
        }
        else
        {
            // Check if last pipe has moved enough to create spacing
            float lastPipeRightEdge = asteroidPipes[asteroidPipes.Count - 1].GetRightEdge();
            float distanceFromScreenEdge = screenWidth - lastPipeRightEdge;

            // Spawn new pipe when there's enough space (gap between pairs)
            if (distanceFromScreenEdge >= PipeSpacing)
            {
                shouldSpawn = true;
            }
        }

        if (shouldSpawn)
        {
            SpawnAsteroidPipe();
            spawnTimer = 0f;
        }

        // Update existing pipes
        foreach (var pipe in asteroidPipes)
        {
            pipe.Update(deltaTime);
        }

        // Remove off-screen pipes
        asteroidPipes.RemoveAll(pipe =>
        {
            var allAsteroids = pipe.GetAllAsteroids();
            return allAsteroids.All(a => a.IsOffScreen(screenWidth));
        });
    }

    private void SpawnAsteroidPipe()
    {
        if (difficultyManager == null) return;

        float asteroidSpeed = difficultyManager.AsteroidSpeed;
        float minGap = difficultyManager.MinGap;
        float maxGap = difficultyManager.MaxGap;

        // Random gap height
        float gapHeight = random.NextSingle() * (maxGap - minGap) + minGap;

        // Random gap Y position (center of gap)
        float minGapY = gapHeight / 2 + 50; // Minimum gap center position
        float maxGapY = screenHeight - gapHeight / 2 - 50; // Maximum gap center position
        float gapY = random.NextSingle() * (maxGapY - minGapY) + minGapY;

        // Calculate top and bottom pipe heights
        float topPipeHeight = gapY - gapHeight / 2; // From top to gap
        float bottomPipeY = gapY + gapHeight / 2; // From gap to bottom
        float bottomPipeHeight = screenHeight - bottomPipeY;

        // Calculate spawn X position - always spawn at screen edge
        float spawnX = screenWidth;

        // Create stacked asteroids for top pipe (continuous, no gaps between segments)
        List<Asteroid> topAsteroids = new List<Asteroid>();
        float currentY = 0;
        while (currentY < topPipeHeight)
        {
            float segmentHeight = Math.Min(AsteroidSegmentHeight, topPipeHeight - currentY);
            topAsteroids.Add(new Asteroid(spawnX, currentY, PipeWidth, segmentHeight, asteroidSpeed));
            currentY += AsteroidSegmentHeight;
        }

        // Create stacked asteroids for bottom pipe (continuous, no gaps between segments)
        List<Asteroid> bottomAsteroids = new List<Asteroid>();
        currentY = bottomPipeY;
        while (currentY < screenHeight)
        {
            float segmentHeight = Math.Min(AsteroidSegmentHeight, screenHeight - currentY);
            bottomAsteroids.Add(new Asteroid(spawnX, currentY, PipeWidth, segmentHeight, asteroidSpeed));
            currentY += AsteroidSegmentHeight;
        }

        // Add as a pipe
        asteroidPipes.Add(new AsteroidPipe(topAsteroids, bottomAsteroids, gapY, gapHeight));
    }

    public void Render()
    {
        foreach (var pipe in asteroidPipes)
        {
            pipe.Render();
        }
    }

    public List<Asteroid> GetAsteroids()
    {
        List<Asteroid> allAsteroids = new List<Asteroid>();
        foreach (var pipe in asteroidPipes)
        {
            allAsteroids.AddRange(pipe.GetAllAsteroids());
        }
        return allAsteroids;
    }

    public void Clear()
    {
        asteroidPipes.Clear();
        spawnTimer = 0f;
    }

    public int CheckScore(float spaceshipRightEdge)
    {
        int newScore = 0;
        foreach (var pipe in asteroidPipes)
        {
            // Score 1 point when spaceship passes through the gap
            if (!pipe.HasBeenPassed)
            {
                float pipeRightEdge = pipe.GetRightEdge();
                if (spaceshipRightEdge > pipeRightEdge)
                {
                    pipe.HasBeenPassed = true;
                    newScore++; // 1 point per pipe passed
                }
            }
        }
        return newScore;
    }
}
