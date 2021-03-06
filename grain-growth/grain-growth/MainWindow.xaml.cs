﻿using grain_growth.MainMethods;
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
using FastBitmapLib;

namespace grain_growth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainProperties properties;
        private DispatcherTimer dispatcher;
        private Range prevRange, currRange;
        private CellularAutomata ca;
        private MonteCarlo mc;
        private InitInclusions inclusions;
        private InitSubstructures substructures;
        private InitBoundaries boundaries;
        private InitNucleons nucleons;
        private int tempIteration;

        public MainWindow()
        {
            InitializeComponent();

            // initialize structure updates
            dispatcher = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 1)
            };
            dispatcher.Tick += Dispatcher_Tick;

            substructures = new InitSubstructures();
            ca = new CellularAutomata();
            mc = new MonteCarlo();
            currRange = new Range();
            prevRange = new Range();

            Substructures.SubStrucrtuePointsList = new List<System.Drawing.Point>();
            SetProperties();
        }
    
        private void SetProperties()
        {
            properties = new MainProperties()
            {
                RangeWidth = (int)Image.Width,
                RangeHeight = (int)Image.Height,
                AmountOfGrains = Converters.StringToInt(NumOfGrainsTextBox.Text),
                NeighbourhoodType = ChooseNeighbourhoodType(),
                GrowthProbability = Converters.StringToInt(GrowthProbabilityTextBox.Text),
                MCS = Converters.StringToInt(MCSTextBox.Text),
                SubstructuresType = ChooseSubstructuresType(),
                MethodType = ChooseMethodType()
            };
            inclusions = new InitInclusions()
            {
                CreationTime = ChooseInlcusionCreationTime(),
                InclusionsType = ChooseInclusionsType(),
                IsEnable = (bool)InclusionsCheckBox.IsChecked,
                AmountOfInclusions = Converters.StringToInt(NumOfInclusionsTextBox.Text),
                Size = Converters.StringToInt(SizeOfInclusionsTextBox.Text)
            };
            boundaries = new InitBoundaries(properties);
            nucleons = new InitNucleons()
            {
                IsEnable = (bool)SRXCheckBox.IsChecked,
                AmountOfNucleons = Converters.StringToInt(NumOfNucleonsTextBox.Text),
                NucleonsStates = new Color[Converters.StringToInt(NumOfStatesTextBox_SRX.Text)],
                TypeOfcreation = ChooseTypeOfNucleonsCreation(),
                EnergyDistribution = ChooseEnegryDistribution(),
                EnergyInside = Converters.StringToInt(EnergyInsideTextBox.Text),
                EnergyOnEdges = Converters.StringToInt(EnergyOnEdgesTextBox.Text),
                PositionDistribiution = ChoosePositionDistribution(),
                EnergyRange = new Range()
                
            };
            tempIteration = Converters.StringToInt(MCSTextBox.Text);
            
        }

        private void Dispatcher_Tick(object sender, EventArgs e)
        {
            if (MonteCarloRadioButton.IsChecked == true)
            {
                currRange = mc.Grow(prevRange, nucleons);
                properties.MCS--;
                if (properties.MCS <= 0)
                {
                    if (AfterInclusionRadioButton.IsChecked == true && InclusionsCheckBox.IsChecked == true)
                        AddInclusionsButton_Click(new object(), new RoutedEventArgs());

                    SetEnableSubStrAndBoundCheckBoxs();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Mouse.OverrideCursor = null;
                    });
                    currRange.IsFull = true;
                    dispatcher.Stop();
                }
                if (nucleons.TypeOfcreation == TypeOfNucleonsCreation.Constant)
                {
                    prevRange = nucleons.InitializeNucleons(currRange, nucleons);
                }
                else if (nucleons.TypeOfcreation == TypeOfNucleonsCreation.Increasing)
                {
                    nucleons.AmountOfNucleons += tempIteration;
                    prevRange = nucleons.InitializeNucleons(currRange, nucleons);
                }
            }
            else
            {
                currRange = ca.Grow(prevRange, properties);

                if (currRange.IsFull)
                {
                    if (AfterInclusionRadioButton.IsChecked == true && InclusionsCheckBox.IsChecked == true)
                        AddInclusionsButton_Click(new object(), new RoutedEventArgs());

                    SetEnableSubStrAndBoundCheckBoxs();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Mouse.OverrideCursor = null;
                    });
                    dispatcher.Stop();
                }
            }

            prevRange = currRange;
            Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);
        }

        private void SRX_Add_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                });

                SetProperties();
                CellularAutomata.UpdateGrainsArray(currRange);
                prevRange = nucleons.InitializeNucleons(currRange, nucleons);
                prevRange = nucleons.EnergyDistributor(currRange, nucleons);
                Image.Source = Converters.BitmapToImageSource(prevRange.StructureBitmap);
                dispatcher.Start();
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = null;
                });
            }
        }

        private void SRX_New_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                });

                SetProperties();
                SRXCheckBox.IsChecked = false;
                prevRange = substructures.UpdateSubstructuresSRX(currRange, properties);
                dispatcher.Start();
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = null;
                });
            }
        }

        private void Energy_Vizualization_Button_Click(object sender, RoutedEventArgs e)
        {
            currRange = new Range();
            currRange = InitStructures.InitCellularAutomata(properties);

            CellularAutomata.UpdateBitmap(prevRange);

            currRange.StructureBitmap = prevRange.StructureBitmap;
            CellularAutomata.UpdateGrainsArray(currRange);
            CellularAutomata.UpdateBitmap(currRange);
            nucleons.EnergyDistributor(prevRange, nucleons);
            nucleons.EnergyVisualization(ref prevRange, nucleons);
            CellularAutomata.UpdateBitmap(prevRange);
            Image.Source = Converters.BitmapToImageSource(prevRange.StructureBitmap);
        }

        private void Previous_Structure_Button_Click(object sender, RoutedEventArgs e)
        {
            prevRange = new Range();
            prevRange = InitStructures.InitCellularAutomata(properties);

            CellularAutomata.UpdateBitmap(currRange);

            prevRange.StructureBitmap = currRange.StructureBitmap;
            CellularAutomata.UpdateGrainsArray(prevRange);
            CellularAutomata.UpdateBitmap(prevRange);
            prevRange = nucleons.EnergyDistributor(currRange, nucleons);

            CellularAutomata.UpdateBitmap(currRange);
            Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);
        }

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });
            RectangleCanvas.Visibility = Visibility.Hidden;

            if (Substructures.SubStrucrtuePointsList.Count > 0)
            {
                SetProperties();

                if(MonteCarloRadioButton.IsChecked == true)
                    prevRange = substructures.UpdateSubstructuresMC(currRange, properties);
                else
                    prevRange = substructures.UpdateSubstructuresCA(currRange, properties);
            }
            else
            {
                SetProperties();

                if (MonteCarloRadioButton.IsChecked == true)
                {
                    prevRange = InitStructures.InitMonteCarlo(properties);
                    SRXCheckBox.IsChecked = false;
                    nucleons.IsEnable = false;
                }
                else
                {
                    prevRange = InitStructures.InitCellularAutomata(properties);

                    if (inclusions.CreationTime == InclusionsCreationTime.Begin && InclusionsCheckBox.IsChecked == true)
                        prevRange = inclusions.AddInclusionsAtTheBegining(prevRange);
                }
            }
            dispatcher.Start();
        }

        private void MCS_Growth_Button_Click(object sender, RoutedEventArgs e)
        {
            if(prevRange != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                });

                SetProperties();
                dispatcher.Start();
            }
        }

        private void Generate_Boundaries_Click(object sender, RoutedEventArgs e)
        {
            if(BoundariesRadioButtonAll.IsChecked == true)
            {
                boundaries.GenerateBoundariesAll(currRange);
                currRange = boundaries.BoundariesAll;
                Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);
                Clear_Selected_Grains_Click(sender, e);
            }
            else
            {
                boundaries.GenerateBoundariesSelected(currRange);
                currRange = boundaries.BoundariesSelected;
                Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);
            }
        }

        private void Clear_Content_Click(object sender, RoutedEventArgs e)
        {
            CellularAutomata.UpdateBitmap(boundaries.ClearBoundaries);
            currRange = boundaries.ClearBoundaries;
            Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);
            Clear_Selected_Grains_Click(sender, e);
        }

        private void Image_Click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point point = e.GetPosition(Image);
            Substructures.SubStrucrtuePointsList.Add(new System.Drawing.Point { X = (int)point.X, Y = (int)point.Y });

            var color = currRange.StructureBitmap.GetPixel((int)point.X, (int)point.Y);

            boundaries.DrawSingleSelect(currRange, color);
            Image.Source = Converters.BitmapToImageSource(boundaries.BoundariesSingleSelect.StructureBitmap);

            numberOfSelectingGrains.Content = Substructures.SubStrucrtuePointsList.Count;
        }

        private void ImportBitmap_Click(object sender, RoutedEventArgs e)
        {
            RectangleCanvas.Visibility = Visibility.Hidden;
            currRange = new Range();
            currRange = InitStructures.InitCellularAutomata(properties);
            SetProperties();
            OpenFileDialog openfiledialog = new OpenFileDialog();

            openfiledialog.Title = "Open Image";
            openfiledialog.Filter = "Image File|*.bmp; *.gif; *.jpg; *.jpeg; *.png;";

            if (openfiledialog.ShowDialog() == true)
            {
                Image.Source = Converters.BitmapToImageSource(new Bitmap(openfiledialog.FileName));
                currRange.StructureBitmap = new Bitmap(openfiledialog.FileName);
                
                CellularAutomata.UpdateGrainsArray(currRange);
                for (int i = 0; i < currRange.Width; i++)
                    for (int j = 0; j < currRange.Height; j++)
                        currRange.StructureBitmap.SetPixel(i, j, currRange.GrainsArray[i, j].Color);
            }

            dispatcher.Stop();
            Clear_Selected_Grains_Click(sender, e);
            SetEnableSubStrAndBoundCheckBoxs();
        }

        private void ImportTXT_Click(object sender, RoutedEventArgs e)
        {
            RectangleCanvas.Visibility = Visibility.Hidden;
            currRange = new Range();
            currRange = InitStructures.InitCellularAutomata(properties);
            SetProperties();
            OpenFileDialog openfiledialog = new OpenFileDialog();

            openfiledialog.Title = "Open Image";
            openfiledialog.Filter = "Image File|*.txt";

            if (openfiledialog.ShowDialog() == true)
            {
                Image.Source = Converters.BitmapToImageSource(new Bitmap(openfiledialog.FileName));
                currRange.StructureBitmap = new Bitmap(openfiledialog.FileName);

                CellularAutomata.UpdateGrainsArray(currRange);
                CellularAutomata.UpdateBitmap(currRange);
            }

            dispatcher.Stop();
            Clear_Selected_Grains_Click(sender, e);
            SetEnableSubStrAndBoundCheckBoxs();
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
                if (inclusions.IsEnable && (inclusions.CreationTime == InclusionsCreationTime.After))
                {
                    CellularAutomata.UpdateGrainsArray(currRange);
                    currRange =  inclusions.AddInclusionsAfter(currRange);
                    CellularAutomata.UpdateBitmap(currRange);
                    prevRange = currRange;
                    Image.Source = Converters.BitmapToImageSource(currRange.StructureBitmap);
                }
            }
            catch (Exception) { }
        }

        private void Clear_Selected_Grains_Click(object sender, RoutedEventArgs e)
        {
            Substructures.SubStrucrtuePointsList.Clear();
            numberOfSelectingGrains.Content = Substructures.SubStrucrtuePointsList.Count;
        }

        private void SetEnableSubStrAndBoundCheckBoxs()
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

        private InclusionsCreationTime ChooseInlcusionCreationTime()
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

        private TypeOfNucleonsCreation ChooseTypeOfNucleonsCreation()
        {
            if (ConstantRadioButton_SRX.IsChecked == true)
            {
                return TypeOfNucleonsCreation.Constant;
            }
            else if (IncreasingRadioButton_SRX.IsChecked == true)
            {
                return TypeOfNucleonsCreation.Increasing;
            }
            else
            {
                return TypeOfNucleonsCreation.AtTheBeginning;
            }
        }

        private EnergyDistribution ChooseEnegryDistribution()
        {
            if (HomogenousRadioButton_SRX.IsChecked == true)
            {
                return EnergyDistribution.Homogenous;
            }
            else
            {
                return EnergyDistribution.Heterogenous;
            }
        }

        private PositionDistribiution ChoosePositionDistribution()
        {
            if (AnywhereRadioButton_SRX.IsChecked == true)
            {
                return PositionDistribiution.Anywhere;
            }
            else
            {
                return PositionDistribiution.OnGrainBoundaries;
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

        private MethodType ChooseMethodType()
        {
            if (CellularAutomataRadioButton.IsChecked == true)
            {
                return MethodType.CellularAutomata;
            }
            else
            {
                return MethodType.MonteCarlo;
            }
        }
    }
}