using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Hanoi
{
    public partial class MainWindow : Window
    {
        private List<Canvas> poles = new List<Canvas>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            int numDisks = 5;

            // Retrieve the canvases from the XAML
            poles.Add((Canvas)FindName("first"));
            poles.Add((Canvas)FindName("second"));
            poles.Add((Canvas)FindName("third"));

            // Clear all the disks from the poles
            foreach (Canvas pole in poles)
            {
                pole.Children.Clear();
            }

            // Create the disks and add them to the first pole
            List<Disk> disks = new List<Disk>();
            for (int i = numDisks; i >= 1; i--)
            {
                Disk disk = new Disk(i);
                disks.Add(disk);
                poles[0].Children.Add(disk);
                Canvas.SetBottom(disk, (numDisks - i) * disk.Height);
            }

            // Run the Hanoi algorithm
            await MoveDisks(numDisks, poles, 0, 2, 1);
        }


        private async Task MoveDisks(int numDisks, List<Canvas> poles, int fromPole, int toPole, int tempPole)
        {
            if (numDisks == 0)
            {
                return;
            }

            // Move the top numDisks-1 disks from the fromPole to the tempPole
            await MoveDisks(numDisks - 1, poles, fromPole, tempPole, toPole);

            // Move the bottom disk from the fromPole to the toPole
            Disk disk = GetTopDisk(poles[fromPole]);
            poles[fromPole].Children.Remove(disk);
            poles[toPole].Children.Add(disk);
            Canvas.SetBottom(disk, GetTopDiskHeight(poles[toPole])-20);

            // Delay to show the movement of the disk
            await Task.Delay(1000);

            // Move the top numDisks-1 disks from the tempPole to the toPole
            await MoveDisks(numDisks - 1, poles, tempPole, toPole, fromPole);
        }

        private Disk GetTopDisk(Canvas pole)
        {
            if (pole.Children.Count > 0)
            {
                return (Disk)pole.Children[pole.Children.Count - 1];
            }
            return null;
        }

        private double GetTopDiskHeight(Canvas pole)
        {
            Disk topDisk = GetTopDisk(pole);
            if (topDisk != null)
            {
                return Canvas.GetBottom(topDisk) + topDisk.Height;
            }
            return 0;
        }
    }
    public class Disk : System.Windows.Shapes.Shape
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                // Return a rectangle geometry to define the shape of the disk
                return new RectangleGeometry(new Rect(0, 0, Width, Height));
            }
        }

        protected override System.Windows.Media.Geometry GetLayoutClip(Size layoutSlotSize)
        {
            // Return a rectangle geometry to define the layout clip of the disk
            return new RectangleGeometry(new Rect(0, 0, Width, Height));
        }

        public Disk(int value)
        {
            // Set the width and height based on the disk's value
            Width = value * 10; // Assumes a width of 50 per value
            Height = 20; // Assumes a height of 20

            int numSteps = 10; // Define the number of steps for the gradient

            // Map the disk values to a range between 0 and numSteps
            int stepValue = (int)Math.Round((double)(value - 1) / 2 * numSteps);

            // Map the step values to color values using a sine function
            byte colorValue = (byte)(Math.Sin(stepValue / (double)numSteps * Math.PI) * 255);

            Fill = new SolidColorBrush(Color.FromRgb(colorValue, 44 , 23));
        }
    }
}
