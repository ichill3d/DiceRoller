using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace DiceRoller
{
    public partial class MainWindow : Window
    {
        private readonly Random _random = new();

        public MainWindow()
        {
            InitializeComponent();
            InitializeDice();
        }

        private async void OnDiceClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Grid clickedGrid && clickedGrid.Tag is int diceValue)
            {
                var diceImage = clickedGrid.Children.OfType<Image>().FirstOrDefault();

                if (diceImage != null)
                {
                    // Add a RotateTransform to the dice image
                    var rotateTransform = new RotateTransform(0);
                    diceImage.RenderTransform = rotateTransform;
                    diceImage.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

                    // Create a rotation animation
                    var animation = new Animation
                    {
                        Duration = TimeSpan.FromSeconds(0.3),
                        Children =
                        {
                            new KeyFrame
                            {
                                Cue = new Cue(0),
                                Setters =
                                {
                                    new Setter(RotateTransform.AngleProperty, 0.0)
                                }
                            },
                            new KeyFrame
                            {
                                Cue = new Cue(1),
                                Setters =
                                {
                                    new Setter(RotateTransform.AngleProperty, 360.0)
                                }
                            }
                        }
                    };

                    // Run the animation
                    await animation.RunAsync(diceImage, CancellationToken.None);
                }

                // Generate a random number between 1 and the dice value
                int roll = _random.Next(1, diceValue + 1);

                // Update the result text
                ResultText.Text = $"You rolled a d{diceValue} with result: {roll}";

                // Add the roll to the history
                var rollEntry = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 5,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                rollEntry.Children.Add(new TextBlock { Text = $"d{diceValue}", VerticalAlignment = VerticalAlignment.Center });
                rollEntry.Children.Add(new Image { Source = diceImage?.Source, Width = 20, Height = 20, VerticalAlignment = VerticalAlignment.Center });
                rollEntry.Children.Add(new TextBlock { Text = $" ⇝ ", FontSize = 30, VerticalAlignment = VerticalAlignment.Center, Foreground = Brushes.Gray });
                rollEntry.Children.Add(new TextBlock { Text = $"{roll}", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center });

                RollHistory.Children.Insert(0, rollEntry);

                // Scroll to the top of the history
                if (RollHistory.Parent is ScrollViewer scrollViewer)
                {
                    scrollViewer.Offset = new Vector(0, 0);
                }
            }
        }

        private void ClearHistory(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            RollHistory.Children.Clear();
        }

        private void AddHR(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
           
            var horRuler = new Image
            {
                Source = new Avalonia.Media.Imaging.Bitmap($"Assets/separator.png"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(10, 5, 10, 5),
                Height = 9
            };

            RollHistory.Children.Insert(0, horRuler);

            // Scroll to the top of the history
            if (RollHistory.Parent is ScrollViewer scrollViewer)
            {
                scrollViewer.Offset = new Vector(0, 0);
            }
            
        }

        private void InitializeDice()
        {
            int[] diceValues = { 20, 12, 10, 8, 6, 4 };

            foreach (var diceValue in diceValues)
            {
                // Create the grid container
                var diceGrid = new Grid
                {
                    Tag = diceValue
                };

                // Add pointer event to the grid
                diceGrid.PointerPressed += OnDiceClick;

                // Define styles for cursor
                diceGrid.Styles.Add(new Style(x => x.OfType<TextBlock>())
                {
                    Setters = { new Setter(TextBlock.CursorProperty, new Cursor(StandardCursorType.Hand)) }
                });
                diceGrid.Styles.Add(new Style(x => x.OfType<Image>())
                {
                    Setters = { new Setter(Image.CursorProperty, new Cursor(StandardCursorType.Hand)) }
                });

                // Add image to the grid
                var diceImage = new Image
                {
                    Source = new Avalonia.Media.Imaging.Bitmap($"Assets/d{diceValue}.png"),
                    Width = 60,
                    Height = 60
                };
                diceGrid.Children.Add(diceImage);

                // Add text block to the grid
                var diceText = new TextBlock
                {
                    Text = $"d{diceValue}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.White,
                    FontWeight = FontWeight.Bold,
                    FontSize = 16
                };
                diceGrid.Children.Add(diceText);

                // Add the grid to the StackPanel
                DiceContainer.Children.Add(diceGrid);
            }
        }
    }
}
