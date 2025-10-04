namespace Antiquera_LabActivity3
{
    // ScoreManager class for handling scores and combos
    public class ScoreManager
    {
        public int Score { get; private set; }
        public int Combo { get; private set; }
        private DateTime lastComboTime;
        private const int COMBO_TIMEOUT = 10; // seconds

        public ScoreManager()
        {
            Score = 0;
            Combo = 0;
            lastComboTime = DateTime.Now;
        }

        public void AddScore(int points)
        {
            int multiplier = Combo > 0 ? Combo + 1 : 1;
            Score += points * multiplier;
        }

        public void SetCombo(int combo)
        {
            Combo = combo;
            lastComboTime = DateTime.Now;
        }

        public void ResetCombo()
        {
            Combo = 0;
        }

        public void Update()
        {
            // Reset combo after timeout
            if (Combo > 0 && DateTime.Now.Subtract(lastComboTime).TotalSeconds > COMBO_TIMEOUT)
            {
                ResetCombo();
            }
        }

        public int GetScore()
        {
            return Score;
        }
    }
}
