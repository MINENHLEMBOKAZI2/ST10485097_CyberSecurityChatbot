using CyberSecurityChatbot;

AudioPlayer audio = new AudioPlayer();
User person = new User();
Chatbot bot = new Chatbot();

audio.PlayIntro();
person.AskName();
bot.StartConversation();