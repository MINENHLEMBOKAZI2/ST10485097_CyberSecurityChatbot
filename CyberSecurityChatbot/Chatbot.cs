namespace CyberSecurityChatbot
{
    public class Chatbot
    {
        private string asciibot = @"
       ||==============================||
       || Cybersecurity  Knowledge     ||
       ||==============================||
";

        public void StartConversation()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(asciibot);
            Console.Write("How are you?: ");

            bool Validchoice = false;

            while (!Validchoice)
            {
                Console.WriteLine("======Menu========");
                Console.WriteLine("1. Password Safety");
                Console.WriteLine("2. Phishing");
                Console.WriteLine("3. Safe Browsing");
                Console.WriteLine("4. Exit");

                Console.Write("What is your purpose?: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.WriteLine("Password safety refers to the practice of creating, managing, and protecting log in credentials to prevent unauthorized access to digital accounts ");
                }
                else if (choice == "2")
                {
                    Console.WriteLine("Phishing is a cyberattack where scammers impersonate trusted entities(banks, companies, colleagues), to steal sensitive information like password, credit card numbers, or personal data);");
                }
                else if (choice == "3")
                {
                    Console.WriteLine("Safe browsing is a security service that protects users by warning them before they visit dangerous websites, download malicious softwares, or encounter phishing attempts");
                }
                else if (choice == "4")
                {
                    Console.WriteLine("Goodbye");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("I did not quite understand that, please rephrase");
                }
            }
        }
    }
}