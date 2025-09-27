using Raylib_cs;
using System.Collections.Generic;

public class AudioHandler
{
    private Music bgMusic;
    private Dictionary<string, Sound> soundEffects = new Dictionary<string, Sound>();

    public void LoadAudio()
    {
        Raylib.InitAudioDevice();

        // Load background music
        bgMusic = Raylib.LoadMusicStream("audio/Flask77-BG.mp3");
        Raylib.PlayMusicStream(bgMusic);

        // Load sound effects
        soundEffects["click"] = Raylib.LoadSound("audio/click.wav");
        soundEffects["buyFish"] = Raylib.LoadSound("audio/buyFish.wav");
        soundEffects["coin1"] = Raylib.LoadSound("audio/coin1.wav");
        soundEffects["coin2"] = Raylib.LoadSound("audio/coin2.wav");
        soundEffects["poop"] = Raylib.LoadSound("audio/poop.wav");
        soundEffects["drop"] = Raylib.LoadSound("audio/drop.wav");
        soundEffects["die"] = Raylib.LoadSound("audio/fishDeath.wav");
        soundEffects["eat"] = Raylib.LoadSound("audio/munch.wav");
        Raylib.SetSoundVolume(soundEffects["eat"], 0.3f); // Set munch sound to 30% volume
    }

    public void Update()
    {
        Raylib.UpdateMusicStream(bgMusic);
    }

    public void PlaySound(string name)
    {
        if (soundEffects.ContainsKey(name))
        {
            Raylib.PlaySound(soundEffects[name]);
        }
        else
        {
            System.Console.WriteLine($"Sound '{name}' not found.");
        }
    }

    public void UnloadAudio()
    {
        Raylib.StopMusicStream(bgMusic);
        Raylib.UnloadMusicStream(bgMusic);

        foreach (var sound in soundEffects.Values)
        {
            Raylib.UnloadSound(sound);
        }

        Raylib.CloseAudioDevice();
    }
}
public static class PlaySingle
{
    public static void PlaySound(string soundName)
    {
        Raylib.PlaySound(Raylib.LoadSound("res/" + soundName + ".wav"));
    }
}