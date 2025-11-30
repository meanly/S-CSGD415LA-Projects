using Raylib_cs;
using System.Collections.Generic;

namespace Antiquera_LabActivity1_Finals;

public class Animation
{
    private Texture2D spriteSheet;
    private Rectangle[] frameRects;
    private int frameCount;
    private float frameTime;
    private float currentTime;
    private int currentFrame;
    private bool looping;
    private bool isPlaying;
    private int frameWidth;
    private int frameHeight;

    // Constructor for sprite sheet (single image with multiple frames)
    public Animation(string spriteSheetPath, int numFrames, float frameTime, bool looping = true)
    {
        if (System.IO.File.Exists(spriteSheetPath))
        {
            spriteSheet = Raylib.LoadTexture(spriteSheetPath);
            frameCount = numFrames;
            
            // Calculate frame dimensions (assuming frames are arranged horizontally)
            frameWidth = spriteSheet.Width / numFrames;
            frameHeight = spriteSheet.Height;
            
            // Create rectangles for each frame
            frameRects = new Rectangle[numFrames];
            for (int i = 0; i < numFrames; i++)
            {
                frameRects[i] = new Rectangle(i * frameWidth, 0, frameWidth, frameHeight);
            }
        }
        else
        {
            spriteSheet = new Texture2D();
            frameRects = new Rectangle[0];
            frameCount = 0;
        }
        
        this.frameTime = frameTime;
        this.looping = looping;
        this.currentTime = 0;
        this.currentFrame = 0;
        this.isPlaying = true;
    }

    // Legacy constructor for multiple separate frame files (kept for compatibility)
    public Animation(string[] framePaths, float frameTime, bool looping = true)
    {
        // This constructor is kept but not recommended - use sprite sheet constructor instead
        spriteSheet = new Texture2D();
        frameRects = new Rectangle[0];
        frameCount = 0;
        this.frameTime = frameTime;
        this.looping = looping;
        this.currentTime = 0;
        this.currentFrame = 0;
        this.isPlaying = true;
    }

    public void Update(float deltaTime)
    {
        if (!isPlaying) return;

        currentTime += deltaTime;
        if (currentTime >= frameTime)
        {
            currentTime = 0;
            currentFrame++;
            if (currentFrame >= frameCount)
            {
                if (looping)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame = frameCount - 1;
                    isPlaying = false;
                }
            }
        }
    }

    public Texture2D GetSpriteSheet()
    {
        return spriteSheet;
    }

    public Rectangle GetCurrentFrameRect()
    {
        if (frameRects.Length == 0 || currentFrame >= frameRects.Length)
        {
            return new Rectangle(0, 0, 0, 0);
        }
        return frameRects[currentFrame];
    }

    public int GetFrameWidth()
    {
        return frameWidth;
    }

    public int GetFrameHeight()
    {
        return frameHeight;
    }

    public void Reset()
    {
        currentFrame = 0;
        currentTime = 0;
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
    }

    public void Play()
    {
        isPlaying = true;
    }

    public bool IsFinished()
    {
        return !looping && !isPlaying;
    }

    public void Unload()
    {
        if (spriteSheet.Id != 0)
        {
            Raylib.UnloadTexture(spriteSheet);
        }
    }
}


