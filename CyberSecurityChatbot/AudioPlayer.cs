using System.Media;
//using System.Media;

namespace CyberSecurityChatbot
{
    public class AudioPlayer
    {
        public void PlayIntro()
        {
            try
            {
                SoundPlayer player = new SoundPlayer(CyberSecurityChatbot.Properties.Resources.Voice_wav);
                player.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"(Audio not found: {ex.Message})");
            }
        }
    }
}