using Raylib_cs;

namespace Antiquera_LabActivity3
{
    // SoundManager class for handling audio
    public class SoundManager
    {
        private Dictionary<string, Sound> sounds;
        // private Music backgroundMusic; // TODO: Implement when audio files are added

        public SoundManager()
        {
            sounds = new Dictionary<string, Sound>();
            InitializeSounds();
        }

        private void InitializeSounds()
        {
            // Note: In a real implementation, you would load actual sound files
            // For now, we'll create placeholder sounds
        }

        public void PlaySound(string soundName)
        {
            if (sounds.ContainsKey(soundName))
            {
                Raylib.PlaySound(sounds[soundName]);
            }
        }

        public void PlayMusic()
        {
            // Raylib.PlayMusicStream(backgroundMusic);
        }

        public void StopMusic()
        {
            // Raylib.StopMusicStream(backgroundMusic);
        }
    }
}
