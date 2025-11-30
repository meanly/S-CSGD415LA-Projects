using Raylib_cs;
using System.Collections.Generic;

namespace Antiquera_LabActivity1_Finals;

public class Animation
{
    private Texture2D[] frames;
    private float frameTime;
    private float currentTime;
    private int currentFrame;
    private bool looping;
    private bool isPlaying;

    public Animation(string[] framePaths, float frameTime, bool looping = true)
    {
        frames = new Texture2D[framePaths.Length];
        for (int i = 0; i < framePaths.Length; i++)
        {
            if (System.IO.File.Exists(framePaths[i]))
            {
                frames[i] = Raylib.LoadTexture(framePaths[i]);
            }
        }
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
            if (currentFrame >= frames.Length)
            {
                if (looping)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame = frames.Length - 1;
                    isPlaying = false;
                }
            }
        }
    }

    public Texture2D GetCurrentFrame()
    {
        if (frames.Length == 0) return new Texture2D();
        return frames[currentFrame];
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
        foreach (var frame in frames)
        {
            if (frame.Id != 0)
            {
                Raylib.UnloadTexture(frame);
            }
        }
    }
}


