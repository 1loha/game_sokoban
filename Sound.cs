using System.Media;

namespace SokobanKR
{
    class Sound
    {
        private static SoundPlayer moveSound = 
           new SoundPlayer(Properties.Resources.MoveSound);
        private static SoundPlayer victorySound = 
           new SoundPlayer(Properties.Resources.VictorySound);

        private static bool flagMoveSound = true;

        public static void MoveSoundPlay()
        {
            if(flagMoveSound)
                moveSound.Play();
        }

        internal static void MoveSoundPlayON()
        {
            flagMoveSound = true;
            
        }
        internal static void MoveSoundPlayOFF()
        {
            flagMoveSound = false;
        }


        public static void VictorySoundPlay()
        {
            if (flagMoveSound)
                victorySound.Play();
        }
        public static void VictorySoundStop()
        {
            victorySound.Stop();
        }
        internal static void VictorySoundPlayON()
        {
            flagMoveSound = true;
        }
        internal static void VictorySoundPlayOFF()
        {
            flagMoveSound = false;
        }
    }
}
