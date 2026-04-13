namespace CyberSecurityChatbot
{
    public class User
    {
        public string Name { get; set; }

        public void AskName()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("CYBERSECURITY AWARENESS BOT");
            Console.WriteLine("_________");
            Console.Write("What is your name?: ");
            Name = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Hello Welcome to the cybersecurity awareness bot");
        }
    }
}