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
using System.Windows.Input;
using System.Collections.Generic;

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
        private MonteCarlo ma;
        private Range prevRange, currRange;
        private Boundaries boundaries;

        public MainWindow()
        {
            InitializeComponent();

            // initialize structure updates
            dispatcher = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 1)
            };
            dispatcher.Tick += Dispatcher_Tick;

            ca = new CelularAutomata();
            ma = new MonteCarlo();
            currRange = new Range();

            Substructures.SubstrListPoints = new List<System.Drawing.Point>();
            Substructures.SubstrGrainList = new List<Grain>();

            SetProperties();
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
                MCS = Converters.StringToInt(MCSTextBox.Text),
                Inclusions = new InitInclusions()
                {
                    CreationTime = ChooseCreationTime(),
                    InclusionsType = ChooseInclusionsType(),
                    IsEnable = (bool)InclusionsCheckBox.IsChecked,
                    Number = Converters.StringToInt(NumOfInclusionsTextBox.Text),
                    Size = Converters.StringToInt(SizeOfInclusionsTextBox.Text),
                },
                SubstructuresType = ChooseSubstructuresType()
            };
            MonteCarlo.MCS_local = properties.MCS;
            boundaries = new Boundaries(properties);
        }

        private void Dispatcher_Tick(object sender, EventArgs e)
        {
            if(MonteCarloRadioButton.IsChecked == true)
                currRange = ma.Grow(prevRange, properties);
            else
                currRange = ca.Grow(properties.NeighbourhoodType, prevRange, properties.GrowthProbability);

            prevRange = currRange;
            Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);

            if (currRange.IsFull)
            {
                if (AfterInclusionRadioButton.IsChecked == true && InclusionsCheckBox.IsChecked == true)
                    AddInclusionsButton_Click(new object(), new RoutedEventArgs());

                SubstrAndBoundCheckBoxs_Checked();
                dispatcher.Stop();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetProperties();
            CelularAutomata.UpdateGrainsArray(currRange);

            if (MonteCarloRadioButton.IsEnabled)
                currRange = ma.Grow(prevRange, properties);

            CelularAutomata.UpdateBitmap(currRange);

            prevRange = currRange;
            Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);
        }

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            RectangleCanvas.Visibility = Visibility.Hidden;

            if (Substructures.SubstrListPoints.Count > 0)
            {
                SetProperties();
                

                if(MonteCarloRadioButton.IsChecked == true)
                    prevRange = Substructures.UpdateSubstructuresMC(currRange, properties);
                else
                {
                    prevRange = Substructures.UpdateSubstructures(currRange, properties);
                }

                dispatcher.Start();
            }
            else
            {
                SetProperties();

                if (MonteCarloRadioButton.IsChecked == true)
                    prevRange = InitStructure.InitMonteCarlo(properties);
                else
                {
                    prevRange = InitStructure.InitializeStructure(properties);

                    if (properties.Inclusions.CreationTime == InclusionsCreationTime.Begin && InclusionsCheckBox.IsChecked == true)
                        prevRange = properties.Inclusions.AddInclusionsAtTheBegining(prevRange);
                }
                dispatcher.Start();
            }
        }

        private void Generate_Boundaries_Click(object sender, RoutedEventArgs e)
        {
            if(BoundariesRadioButtonAll.IsChecked == true)
            {
                for (int i = 1; i < currRange.Width - 1; i++)
                {
                    for (int j = 1; j < currRange.Height - 1; j++)
                    {
                        if (Boundaries.IsOnGrainBoundaries(currRange, new System.Drawing.Point(i, j)))
                        {
                            boundaries.BoundariesWithBackground.GrainsArray[i, j].Color = Color.Black;
                            boundaries.BoundariesWithBackground.GrainsArray[i, j].Id = -1;
                            boundaries.ClearBoundaries.GrainsArray[i, j] = boundaries.BoundariesWithBackground.GrainsArray[i, j];
                        }
                        else
                        {
                            boundaries.BoundariesWithBackground.GrainsArray[i, j].Color = currRange.GrainsArray[i, j].Color;
                            boundaries.BoundariesWithBackground.GrainsArray[i, j].Id = currRange.GrainsArray[i, j].Id;
                        }
                    }
                }
                CelularAutomata.UpdateBitmap(boundaries.BoundariesWithBackground);
                CelularAutomata.UpdateGrainsArray(boundaries.BoundariesWithBackground);
                currRange = boundaries.BoundariesWithBackground;
               
                Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);
                Clear_Selected_Grains_Click(sender, e);
            }
            else
            {
                boundaries.BoundariesAllSelected = currRange;
                foreach(var point in Substructures.SubstrListPoints)
                {
                    var color = currRange.StructureBitmap.GetPixel(point.X, point.Y);

                    DrawSingleSelect(color);

                    for (int i = 1; i < boundaries.BoundariesSingleSelect.Width - 1; i++)
                    {
                        for (int j = 1; j < boundaries.BoundariesSingleSelect.Height - 1; j++)
                        {
                            if (boundaries.BoundariesSingleSelect.GrainsArray[i, j].Color == Color.Black)
                            {
                                boundaries.BoundariesAllSelected.GrainsArray[i, j] = boundaries.BoundariesSingleSelect.GrainsArray[i, j];
                                boundaries.ClearBoundaries.GrainsArray[i,j] = boundaries.BoundariesSingleSelect.GrainsArray[i, j];
                            }
                        }
                    }
                }
                CelularAutomata.UpdateBitmap(boundaries.BoundariesAllSelected);
                CelularAutomata.UpdateGrainsArray(boundaries.BoundariesAllSelected);

                currRange = boundaries.BoundariesAllSelected;

                Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);
            }
        }

        private void Clear_Content_Click(object sender, RoutedEventArgs e)
        {
            CelularAutomata.UpdateBitmap(boundaries.ClearBoundaries);
            CelularAutomata.UpdateGrainsArray(boundaries.ClearBoundaries);

            currRange = boundaries.ClearBoundaries;
            CelularAutomata.UpdateBitmap(currRange);
            CelularAutomata.UpdateGrainsArray(currRange);

            Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);
            Clear_Selected_Grains_Click(sender, e);
        }

        private void Image_Click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point point = e.GetPosition(Image);
            Substructures.SubstrListPoints.Add(new System.Drawing.Point { X = (int)point.X, Y = (int)point.Y });

            var color = currRange.StructureBitmap.GetPixel((int)point.X, (int)point.Y);

            DrawSingleSelect(color);
            Image.Source = Converters.BitmapToImageSource(boundaries.BoundariesSingleSelect.StructureBitmap);

            numberOfSelectingGrains.Content = Substructures.SubstrListPoints.Count;
        }

        private void DrawSingleSelect(Color color)
        {
            for (int i = 1; i < currRange.Width - 1; i++)
            {
                for (int j = 1; j < currRange.Height - 1; j++)
                {
                    if (boundaries.IsOnGrainBoundariesColor(currRange, new System.Drawing.Point(i, j), color) &&
                        currRange.GrainsArray[i, j].Color != color)
                    {
                        boundaries.BoundariesSingleSelect.GrainsArray[i, j].Color = Color.Black;
                        boundaries.BoundariesSingleSelect.GrainsArray[i, j].Id = -1;
                    }
                    else
                    {
                        boundaries.BoundariesSingleSelect.GrainsArray[i, j].Color = currRange.GrainsArray[i, j].Color;
                        boundaries.BoundariesSingleSelect.GrainsArray[i, j].Id = currRange.GrainsArray[i, j].Id;
                    }
                }
            }
            CelularAutomata.UpdateBitmap(boundaries.BoundariesSingleSelect);
        }

        private void ImportBitmap_Click(object sender, RoutedEventArgs e)
        {
            RectangleCanvas.Visibility = Visibility.Hidden;
            currRange = new Range();
            currRange = InitStructure.InitializeStructure(properties);
            SetProperties();
            OpenFileDialog openfiledialog = new OpenFileDialog();

            openfiledialog.Title = "Open Image";
            openfiledialog.Filter = "Image File|*.bmp; *.gif; *.jpg; *.jpeg; *.png;";

            if (openfiledialog.ShowDialog() == true)
            {
                Image.Source = Converters.BitmapToImageSource(new Bitmap(openfiledialog.FileName));
                currRange.StructureBitmap = new Bitmap(openfiledialog.FileName);
                
                CelularAutomata.UpdateGrainsArray(currRange);
                CelularAutomata.UpdateBitmap(currRange);
            }

            dispatcher.Stop();
            Clear_Selected_Grains_Click(sender, e);
            SubstrAndBoundCheckBoxs_Checked();
        }

        private void ImportTXT_Click(object sender, RoutedEventArgs e)
        {
            RectangleCanvas.Visibility = Visibility.Hidden;
            currRange = new Range();
            currRange = InitStructure.InitializeStructure(properties);
            SetProperties();
            OpenFileDialog openfiledialog = new OpenFileDialog();

            openfiledialog.Title = "Open Image";
            openfiledialog.Filter = "Image File|*.txt";

            if (openfiledialog.ShowDialog() == true)
            {
                Image.Source = Converters.BitmapToImageSource(new Bitmap(openfiledialog.FileName));
                currRange.StructureBitmap = new Bitmap(openfiledialog.FileName);

                CelularAutomata.UpdateGrainsArray(currRange);
                CelularAutomata.UpdateBitmap(currRange);
            }

            dispatcher.Stop();
            Clear_Selected_Grains_Click(sender, e);
            SubstrAndBoundCheckBoxs_Checked();
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
            Application.Current.Shutdown();
        }

        private void AddInclusionsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetProperties();
                currRange.IsFull = true;
                if (properties.Inclusions.IsEnable && (properties.Inclusions.CreationTime == InclusionsCreationTime.After))
                {
                    CelularAutomata.UpdateGrainsArray(currRange);
                    currRange =  properties.Inclusions.AddInclusionsAfterGrainGrowth(currRange);
                    CelularAutomata.UpdateBitmap(currRange);
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

        private void Clear_Selected_Grains_Click(object sender, RoutedEventArgs e)
        {
            Substructures.SubstrListPoints.Clear();
            numberOfSelectingGrains.Content = Substructures.SubstrListPoints.Count;
        }

        private void SubstrAndBoundCheckBoxs_Checked()
        {
            if (currRange != null)
            {
                BoundariesCheckBox.IsEnabled = true;
                SubstructuresCheckBox.IsEnabled = true;
            }
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

        private SubstructuresType ChooseSubstructuresType()
        {
            if (SubstrRadioButton1.IsChecked == true)
            {
                return SubstructuresType.Substructure;
            }
            else
            {
                return SubstructuresType.DualPhase;
            }
        }

    }
}