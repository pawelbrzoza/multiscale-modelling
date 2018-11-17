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
        private Range prevRange, currRange;
        private Boundaries boundaries;

        public MainWindow()
        {
            InitializeComponent();

            // initialize structure updates
            dispatcher = new DispatcherTimer();
            dispatcher.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dispatcher.Tick += dispatcher_Tick;

            ca = new CelularAutomata();
            Substructures.SubstrListPoints = new System.Collections.Generic.List<System.Drawing.Point>();
            Substructures.SubstrGrainList = new System.Collections.Generic.List<Grain>();
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
                SubstructuresType = ChooseSubstructuresType(),

                Inclusions = new InclusionsProperties()
                {
                    CreationTime = ChooseCreationTime(),
                    InclusionsType = ChooseInclusionsType(),
                    IsEnable = (bool)InclusionsCheckBox.IsChecked,
                    Number = Converters.StringToInt(NumOfInclusionsTextBox.Text),
                    Size = Converters.StringToInt(SizeOfInclusionsTextBox.Text),
                }
            };
            boundaries = new Boundaries(properties);
        }

        private void dispatcher_Tick(object sender, EventArgs e)
        {
            currRange = ca.Grow(properties.NeighbourhoodType, prevRange, properties.GrowthProbability);
            prevRange = currRange;
            Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);

            if (currRange.IsFull)
            {
                if (AfterInclusionRadioButton.IsChecked == true && InclusionsCheckBox.IsChecked == true)
                    AddInclusionsButton_Click(new object(), new RoutedEventArgs());

                dispatcher.Stop();
            }
        }

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            if(Substructures.SubstrListPoints.Count > 0)
            {
                SetProperties();
                prevRange = Substructures.UpdateSubstructures(currRange, properties);
                dispatcher.Start();
            }
            else
            {
                SetProperties();
                prevRange = InitStructure.InitializeStructure(properties);
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
                        if (boundaries.IsCoordinateOnGrainBoundaries(currRange, new System.Drawing.Point(i, j)))
                        {
                            boundaries.BoundariesWithBackground.GrainsArray[i, j].Color = Color.Black;
                            boundaries.BoundariesWithBackground.GrainsArray[i, j].Id = -1;
                        }
                        else
                        {
                            boundaries.BoundariesWithBackground.GrainsArray[i, j].Color = currRange.GrainsArray[i, j].Color;
                            boundaries.BoundariesWithBackground.GrainsArray[i, j].Id = currRange.GrainsArray[i, j].Id;
                        }
                    }
                }
                CelularAutomata.updateBitmap(boundaries.BoundariesWithBackground);
                Image.Source = Converters.BitmapToImageSource(boundaries.BoundariesWithBackground.StructureBitmap);
            }
            else
            {
                foreach(var point in Substructures.SubstrListPoints)
                {
                    var color = currRange.StructureBitmap.GetPixel(point.X, point.Y);
                    for (int i = 1; i < currRange.Width - 1; i++)
                    {
                        for (int j = 1; j < currRange.Height - 1; j++)
                        {
                            if (IsOnGrainBoundariesColor(point, color) &&
                                currRange.GrainsArray[i, j].Color != color)
                            {
                                boundaries.BoundariesSelected.GrainsArray[i, j].Color = Color.Black;
                                boundaries.BoundariesSelected.GrainsArray[i, j].Id = -1;
                            }
                            else
                            {
                                boundaries.BoundariesSelected.GrainsArray[i, j].Color = currRange.GrainsArray[i, j].Color;
                                boundaries.BoundariesSelected.GrainsArray[i, j].Id = currRange.GrainsArray[i, j].Id;
                            }
                        }
                    }
                }
                CelularAutomata.updateBitmap(boundaries.BoundariesSelected);
                Image.Source = Converters.BitmapToImageSource(boundaries.BoundariesSelected.StructureBitmap);

            }
        }

        private void Clear_Content_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i < currRange.Width - 1; i++)
            {
                for (int j = 1; j < currRange.Height - 1; j++)
                {
                    if (boundaries.IsCoordinateOnGrainBoundaries(currRange, new System.Drawing.Point(i, j)))
                    {
                        boundaries.ClearBoundaries.GrainsArray[i, j].Color = Color.Black;
                        boundaries.ClearBoundaries.GrainsArray[i, j].Id = -1;
                    }
                }
            }

            CelularAutomata.updateBitmap(boundaries.ClearBoundaries);
            Image.Source = Converters.BitmapToImageSource(boundaries.ClearBoundaries.StructureBitmap);
        }

        private void Image_Click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point point = e.GetPosition(Image);
            Substructures.SubstrListPoints.Add(new System.Drawing.Point { X = (int)point.X, Y = (int)point.Y });

            var color = currRange.StructureBitmap.GetPixel((int)point.X, (int)point.Y);

            for (int i = 1; i < currRange.Width - 1; i++)
            {
                for (int j = 1; j < currRange.Height - 1; j++)
                {
                    if(IsOnGrainBoundariesColor(new System.Drawing.Point(i, j), color) &&
                        currRange.GrainsArray[i,j].Color != color)
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
            CelularAutomata.updateBitmap(boundaries.BoundariesSingleSelect);
            Image.Source = Converters.BitmapToImageSource(boundaries.BoundariesSingleSelect.StructureBitmap);
        }

        private void ImportBitmap_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfiledialog = new OpenFileDialog();

            openfiledialog.Title = "Open Image";
            openfiledialog.Filter = "Image File|*.bmp; *.gif; *.jpg; *.jpeg; *.png;";

            if (openfiledialog.ShowDialog() == true)
            {
                Image.Source = Converters.BitmapToImageSource(new Bitmap(openfiledialog.FileName));
                currRange.StructureBitmap = new Bitmap(openfiledialog.FileName);
                
                CelularAutomata.updateGrainsArray(currRange);
                CelularAutomata.updateBitmap(currRange);
            }

            dispatcher.Stop();
        }

        private void ImportTXT_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfiledialog = new OpenFileDialog();

            openfiledialog.Title = "Open Image";
            openfiledialog.Filter = "Image File|*.txt";

            if (openfiledialog.ShowDialog() == true)
            {
                Image.Source = Converters.BitmapToImageSource(new Bitmap(openfiledialog.FileName));
                currRange.StructureBitmap = new Bitmap(openfiledialog.FileName);

                CelularAutomata.updateGrainsArray(currRange);
                CelularAutomata.updateBitmap(currRange);
            }

            dispatcher.Stop();
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

                if (properties.Inclusions.IsEnable && (properties.Inclusions.CreationTime == InclusionsCreationTime.After))
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

        private bool IsOnGrainBoundariesColor(System.Drawing.Point point, Color color)
        {
            var neighbours = new List<Grain>
            {
                currRange.GrainsArray[point.X - 1, point.Y],
                currRange.GrainsArray[point.X + 1, point.Y],
                currRange.GrainsArray[point.X, point.Y - 1],
                currRange.GrainsArray[point.X, point.Y + 1],
                currRange.GrainsArray[point.X - 1, point.Y - 1],
                currRange.GrainsArray[point.X - 1, point.Y + 1],
                currRange.GrainsArray[point.X + 1, point.Y - 1],
                currRange.GrainsArray[point.X + 1, point.Y + 1]
            };

            foreach (var neighbour in neighbours)
            {
                if (neighbour.Color == color && !InitStructure.IsIdSpecial(neighbour.Id))
                {
                    return true;
                }
            }
            return false;
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