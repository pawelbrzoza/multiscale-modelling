using grain_growth.Alghorithms;
using grain_growth.Models;
using grain_growth.Helpers;

using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Drawing;
using System.IO;

namespace grain_growth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainProperties properties;
        private DispatcherTimer dispatcher;
        private CelularAutomata ca;
        private Range prevRange, currRange;

        public MainWindow()
        {
            InitializeComponent();

            // initialize structure updates
            dispatcher = new System.Windows.Threading.DispatcherTimer();
            dispatcher.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dispatcher.Tick += dispatcher_Tick;

            this.ca = new CelularAutomata();
        }
    
        private void SetProperties()
        {
            properties = new MainProperties()
            {
                RangeWidth = (int)Image.Width,
                RangeHeight = (int)Image.Height,
                NumberOfGrains = Converters.StringToInt(NumOfGrainsTextBox.Text),
                NeighbourhoodType = ChooseNeighbourhoodType(),
                GrowthProbability = Converters.StringToInt(GrowthProbabilityTextBox.Text),

                Inclusions = new InclusionsProperties()
                {
                    CreationTime = ChooseCreationTime(),
                    InclusionsType = ChooseInclusionsType(),
                    AreEnable = (bool)InclusionsCheckBox.IsChecked,
                    Number = Converters.StringToInt(NumOfInclusionsTextBox.Text),
                    Size = Converters.StringToInt(SizeOfInclusionsTextBox.Text),
                }
            };
        }

        private void dispatcher_Tick(object sender, EventArgs e)
        {
            currRange = ca.Grow(properties.NeighbourhoodType, prevRange, properties.GrowthProbability);
            prevRange = currRange;
            Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);

            if (prevRange.IsFull)
            {
                if (AfterInclusionRadioButton.IsChecked == true && InclusionsCheckBox.IsChecked == true)
                    AddInclusionsButton_Click(new object(), new RoutedEventArgs());

                dispatcher.Stop();
            }
        }

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            SetProperties();
            prevRange = InitStructure.InitializeStructure(properties);
            dispatcher.Start();
        }

        private void ImportBitmap_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfiledialog = new OpenFileDialog();

            openfiledialog.Title = "Open Image";
            openfiledialog.Filter = "Image File|*.bmp; *.gif; *.jpg; *.jpeg; *.png;";

            if (openfiledialog.ShowDialog() == true)
            {
                Image.Source = Converters.BitmapToImageSource(new Bitmap(openfiledialog.FileName));
            }
        }

        private void ImportTXT_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfiledialog = new OpenFileDialog();

            openfiledialog.Title = "Open Image";
            openfiledialog.Filter = "Image File|*.txt";

            if (openfiledialog.ShowDialog() == true)
            {
                Image.Source = Converters.BitmapToImageSource(new Bitmap(openfiledialog.FileName));
            }
        }

        private void ExportBitmap_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Bitmap Image|*.bmp";
            save.Title = "Save an Image File";
            if (save.ShowDialog() == true)
            {
                var image = Image.Source;
                using (var fileStream = new FileStream(save.InitialDirectory + save.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image as BitmapImage));
                    encoder.Save(fileStream);
                }
            }
        }
    
        private void ExportTXT_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Bitmap Image|*.txt";
            save.Title = "Save an Image File";
            if (save.ShowDialog() == true)
            {
                var image = Image.Source;
                using (var fileStream = new FileStream(save.InitialDirectory + save.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image as BitmapImage));
                    encoder.Save(fileStream);
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void AddInclusionsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetProperties();

                if (properties.Inclusions.AreEnable && (properties.Inclusions.CreationTime == InclusionsCreationTime.After))
                {
                    var inclusions = new InitInclusions(properties.Inclusions);
                    CelularAutomata.updateGrainsArray(currRange);
                    currRange = inclusions.AddInclusionsAfterGrainGrowth(currRange);
                    CelularAutomata.updateBitmap(currRange);
                    currRange.IsFull = true;
                    prevRange = currRange;
                    Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);
                }
            }
            catch (Exception) { }
        }

        private void AfterInclusionRadioButton_Click(object sender, RoutedEventArgs e)
        {
            AddInclusionsButton.IsEnabled = (bool)AfterInclusionRadioButton.IsChecked;
        }

        private void BeginInclusionRadioButton_Click(object sender, RoutedEventArgs e)
        {
            AddInclusionsButton.IsEnabled = (bool)AfterInclusionRadioButton.IsChecked;
        }

        private NeighbourhoodType ChooseNeighbourhoodType()
        {
            if (MooreRadioButton.IsChecked == true)
            {
                return NeighbourhoodType.Moore;
            }
            else if (NeumannRadioButton.IsChecked == true)
            {
                return NeighbourhoodType.Neumann;
            }
            else
            {
                return NeighbourhoodType.Moore2;
            }
        }

        private InclusionsCreationTime ChooseCreationTime()
        {
            if (BeginInclusionRadioButton.IsChecked == true)
            {
                return InclusionsCreationTime.Begin;
            }
            else
            {
                return InclusionsCreationTime.After;
            }
        }

        private InclusionsType ChooseInclusionsType()
        {
            if (SquareRadioButton.IsChecked == true)
            {
                return InclusionsType.Square;
            }
            else
            {
                return InclusionsType.Circular;
            }
        }
    }
}