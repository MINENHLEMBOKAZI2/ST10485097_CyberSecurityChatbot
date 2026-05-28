using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CYBER_SECURITY_CHATBOT_PART2
{
    public delegate string TextProcessingHandler(string inputText);

    public partial class MainWindow : Window
    {
        private BotEngine _botBrain;
        private Func<string, string> _pipelineExecutor;

        public MainWindow()
        {
            InitializeComponent();
            _botBrain = new BotEngine();
            _pipelineExecutor = new Func<string, string>(_botBrain.ProcessInputPipeline);
            PlayVoiceGreeting();
        }

        private void PlayVoiceGreeting()
{
    try
    {
        // Simple local path now that the file lives inside your active project directory
        SoundPlayer player = new SoundPlayer("Voice.wav.wav");
        player.Load();
        player.Play();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine("Audio file load failed: " + ex.Message);
    }
}

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            string userInput = TxtUserInput.Text.Trim();

            if (string.IsNullOrEmpty(userInput) || userInput == "Type your message here...")
                return;

            AppendMessageBubble("User", userInput, isBot: false);
            TxtUserInput.Text = "";

            string botResponseText = _pipelineExecutor(userInput);
            AppendMessageBubble("Cybersecurity Chatbot", botResponseText, isBot: true);
        }

        private void AppendMessageBubble(string visualSender, string dynamicContent, bool isBot)
        {
            Grid frameLayout = new Grid { Margin = new Thickness(0, 5, 0, 10) };
            frameLayout.HorizontalAlignment = isBot ? HorizontalAlignment.Left : HorizontalAlignment.Right;

            ColumnDefinition iconColumn = new ColumnDefinition { Width = new GridLength(40) };
            ColumnDefinition messageColumn = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };

            if (isBot)
            {
                frameLayout.ColumnDefinitions.Add(iconColumn);
                frameLayout.ColumnDefinitions.Add(messageColumn);
            }
            else
            {
                frameLayout.ColumnDefinitions.Add(messageColumn);
                frameLayout.ColumnDefinitions.Add(iconColumn);
            }

            TextBlock visualIcon = new TextBlock
            {
                Text = isBot ? "🤖" : "👤",
                FontSize = 20,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 5, 0, 0)
            };
            Grid.SetColumn(visualIcon, isBot ? 0 : 1);
            frameLayout.Children.Add(visualIcon);

            Border designBorder = new Border
            {
                Background = isBot ? Brushes.White : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                CornerRadius = isBot ? new CornerRadius(0, 15, 15, 15) : new CornerRadius(15, 0, 15, 15),
                Padding = new Thickness(12, 10, 12, 10),
                MaxWidth = 300,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E8F0")),
                BorderThickness = isBot ? new Thickness(1) : new Thickness(0)
            };
            Grid.SetColumn(designBorder, isBot ? 1 : 0);

            StackPanel textAlignmentGroup = new StackPanel();
            TextBlock identityLabel = new TextBlock
            {
                Text = visualSender,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = isBot ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#93C5FD")),
                Margin = new Thickness(0, 0, 0, 4)
            };

            TextBlock textPayload = new TextBlock
            {
                Text = dynamicContent,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 13,
                Foreground = isBot ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#334155")) : Brushes.White
            };

            textAlignmentGroup.Children.Add(identityLabel);
            textAlignmentGroup.Children.Add(textPayload);
            designBorder.Child = textAlignmentGroup;
            frameLayout.Children.Add(designBorder);

            ChatHistoryPanel.Children.Add(frameLayout);

            DependencyObject traversalNode = VisualTreeHelper.GetParent(ChatHistoryPanel);
            while (traversalNode != null && !(traversalNode is ScrollViewer))
            {
                traversalNode = VisualTreeHelper.GetParent(traversalNode);
            }
            (traversalNode as ScrollViewer)?.ScrollToEnd();
        }

        private void TxtUserInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtUserInput.Text == "Type your message here...")
            {
                TxtUserInput.Text = "";
                TxtUserInput.Foreground = Brushes.Black;
            }
        }

        private void TxtUserInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtUserInput.Text))
            {
                TxtUserInput.Text = "Type your message here...";
                TxtUserInput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            }
        }
    }

    public class BotEngine
    {
        private Dictionary<string, List<string>> _keywordResponses;
        private Dictionary<string, string> _sentimentPrefixes;
        private List<string> _fallbackResponses;
        private Dictionary<string, int> _topicResponseTrackers;

        private string _userName = "";
        private string _currentTopic = "";
        private bool _isAwaitingName = true;

        public BotEngine()
        {
            InitializeDataStructures();
        }

        private void InitializeDataStructures()
        {
            _keywordResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "password", new List<string> {
                    "Make sure to use unique passwords with a clean mix of letters, numbers, and symbols.",
                    "Never reuse a single password across multiple sites. Try using a verified password manager application!",
                    "Try combining four random words into a long passphrase. It's easy for you to recall but hard to break."
                }},
                { "scam", new List<string> {
                    "Always look over link paths closely before clicking. If an online deal seems too amazing to be true, it's usually a trap.",
                    "Threat actors rely on artificial urgency. Pause and consider details calmly before sending personal info.",
                    "Be extremely cautious of callers claiming to be from bank fraud divisions asking for immediate OTP details."
                }},
                { "privacy", new List<string> {
                    "Routinely clean up your social media platform privacy preferences to block unknown data scrapers.",
                    "Do not sign into your bank profiles while on open public Wi-Fi networks without running an active VPN tunnel.",
                    "Your online digital trail is permanent. Be careful about posting metadata or location clues publicly."
                }},
                { "phishing", new List<string> {
                    "Phishing emails often mimic big companies or banks. Always double-check the sender's actual email address for typos!",
                    "Never click suspicious links or download unexpected attachments from strange emails; they can hide malware.",
                    "Phishers try to trick you into entering credentials on lookalike websites. Look closely at the URL address bar!"
                }}
            };

            _topicResponseTrackers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "password", 0 }, { "scam", 0 }, { "privacy", 0 }, { "phishing", 0 }
            };

            _sentimentPrefixes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "worried", "It is completely normal to feel anxious about these technical threats. Let's look closely at standard precautions: " },
                { "frustrated", "Security protocols can feel incredibly annoying. Let's break this down into simple steps: " },
                { "curious", "Excellent attitude! Staying curious is your most powerful shield online. Check this out: " }
            };

            _fallbackResponses = new List<string>
            {
                "I didn't quite catch that. Could you ask me explicitly about 'phishing', 'passwords', 'scams', or 'privacy'?",
                "Try using terms like 'phishing' or 'password safety' so I can fetch the right records.",
                "I am tracking specific cybersecurity risks. Try asking a targeted question about phishing or scam prevention directly."
            };
        }

        public string ProcessInputPipeline(string input)
        {
            if (_isAwaitingName)
            {
                _userName = input;
                _isAwaitingName = false;
                return $"Awesome to meet you, {_userName}! I've initialized your profile for this session. What safety topics are we looking into today? (e.g., Phishing, Passwords, Scams)";
            }

            if (input.Contains("tell me more", StringComparison.OrdinalIgnoreCase) ||
                input.Contains("explain more", StringComparison.OrdinalIgnoreCase) ||
                input.Contains("give me another tip", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(_currentTopic) && _keywordResponses.ContainsKey(_currentTopic))
                {
                    string ongoingTip = GetNextSequentialResponse(_currentTopic);
                    return $"Following up on your interest in *{_currentTopic}*: {ongoingTip}";
                }
                return "We haven't zeroed in on a concrete security category yet. Mention a term like 'phishing' or 'password' to begin!";
            }

            string sentimentPrefix = "";
            foreach (var pattern in _sentimentPrefixes.Keys)
            {
                if (input.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    sentimentPrefix = _sentimentPrefixes[pattern];
                    break;
                }
            }

            foreach (var subject in _keywordResponses.Keys)
            {
                if (input.Contains(subject, StringComparison.OrdinalIgnoreCase))
                {
                    _currentTopic = subject;
                    string coreTip = GetNextSequentialResponse(subject);

                    if (!string.IsNullOrEmpty(sentimentPrefix))
                    {
                        return $"{sentimentPrefix} {coreTip}";
                    }
                    return coreTip;
                }
            }

            if (!string.IsNullOrEmpty(sentimentPrefix))
            {
                return $"{sentimentPrefix} Tell me what specific topic you are exploring ('phishing' or 'passwords') so I can give you explicit details.";
            }

            Random generator = new Random();
            return _fallbackResponses[generator.Next(_fallbackResponses.Count)];
        }

        private string GetNextSequentialResponse(string topic)
        {
            var responseList = _keywordResponses[topic];
            int currentIndex = _topicResponseTrackers[topic];
            string selectedResponse = responseList[currentIndex];
            _topicResponseTrackers[topic] = (currentIndex + 1) % responseList.Count;
            return selectedResponse;
        }
    }
}