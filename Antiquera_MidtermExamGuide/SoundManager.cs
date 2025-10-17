using Raylib_cs;
using System.Collections.Generic;

namespace MemoryGame
{
    public class SoundManager
    {
        private Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
        private Music music = new Music();

        public void LoadSounds()
        {
            try { sounds["correct"] = Raylib.LoadSound("audio/Correct.wav"); } catch { }
            try { sounds["wrong"] = Raylib.LoadSound("audio/Wrong.wav"); } catch { }
            try { sounds["hit"] = Raylib.LoadSound("audio/Hit.wav"); } catch { }
            try { sounds["flip"] = Raylib.LoadSound("audio/Flip.mp3"); } catch { }
            try { music = Raylib.LoadMusicStream("img/Shadow Circuit.mp3"); } catch { }
        }

        public void UnloadSounds() { foreach (var sound in sounds.Values) Raylib.UnloadSound(sound); }
        public void PlaySound(string name) { if (sounds.ContainsKey(name)) Raylib.PlaySound(sounds[name]); }
        public void UpdateMusic() { try { if (music.FrameCount > 0) Raylib.UpdateMusicStream(music); } catch { } }
    }
}