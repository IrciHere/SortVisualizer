using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace SortVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SortBlocks sortBlocks;
        private int _blockWidth, _sortSpeed;
        private int _positionUsed1 = -1, _positionUsed2 = -1, _positionUsed3 = -1;
        private List<Canvas> canvasList;
        private List<int> valuesList;
        private bool isResetButtonOn = false;

        public MainWindow()
        {
            InitializeComponent();
        }


        #region frontend components

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SpeedSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int speedValue = Convert.ToInt32(speedSlider.Value);
            speedLabel.Content = speedValue.ToString();
        }

        //Generates a list of random numbers, and creates blocks with height based on their value
        private void GenerateButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (objectsAmount.Text == "")
                return;
            int howMany = Convert.ToInt32(objectsAmount.Text);
            if (howMany < 10 || howMany > 100)
            {
                MessageBox.Show("You have to put a number between 10 and 100");
                return;
            }
            generateButton.IsEnabled = false;
            objectsAmount.IsEnabled = false;
            startButton.IsEnabled = true;

            canvasList = new List<Canvas>(new Canvas[howMany]);
            _blockWidth = (1000 / howMany) - 5;
            sortBlocks = new SortBlocks(howMany, this);
            valuesList = sortBlocks.blocks;
        }



        //Starts sorting the list, with a visualisation
        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (isResetButtonOn)
            {
                isResetButtonOn = false;
                startButton.Content = "START";
                speedSlider.IsEnabled = true;
                sortSelection.IsEnabled = true;

                generateButton.IsEnabled = true;
                objectsAmount.IsEnabled = true;
                startButton.IsEnabled = false;

                canvasList.Clear();
                valuesList.Clear();
                drawingSpace.Children.Clear();
            }
            else
            {
                speedSlider.IsEnabled = false;
                sortSelection.IsEnabled = false;
                _sortSpeed = 100 - (10 * Convert.ToInt32(speedSlider.Value));
                _sortSpeed = (_sortSpeed > 0) ? _sortSpeed : 1;

                switch (sortSelection.Text)
                {
                    case "Selection Sort":
                        new Thread(SelectionSort).Start();
                        break;
                    case "Bubble Sort":
                        new Thread(BubbleSort).Start();
                        break;
                    case "Insertion Sort":
                        new Thread(InsertionSort).Start();
                        break;
                    case "Quick Sort":
                        new Thread(QuickSort).Start();
                        break;
                    case "Merge Sort":
                        new Thread(MergeSort).Start();
                        break;
                    default:
                        MessageBox.Show("Pick a sort algorithm");
                        startButton.IsEnabled = true;
                        speedSlider.IsEnabled = true;
                        sortSelection.IsEnabled = true;
                        break;
                }
                isResetButtonOn = true;
                startButton.IsEnabled = false;
            }     
        }

        #endregion


        #region drawing

        //rendering
        public void DrawBlocks()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                drawingSpace.Children.Clear();
                foreach (Canvas block in canvasList)
                {
                    drawingSpace.Children.Add(block);
                }
            });
        }

        //creating all the blocks to draw
        private void SetCanvasList(List<int> blocks)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                for (int i = 0; i < blocks.Count; i++)
                {
                    Canvas tempCanvas = new Canvas
                    {
                        Height = blocks[i] * 6,
                        Width = _blockWidth,
                        Background = Brushes.Orange
                    };
                    Canvas.SetTop(tempCanvas, 610 - tempCanvas.Height);
                    if (i == 0)
                        Canvas.SetLeft(tempCanvas, (5));
                    else
                        Canvas.SetLeft(tempCanvas, 5 + (i * (_blockWidth + 5)));

                    canvasList[i] = tempCanvas;
                }

                if (_positionUsed1 != -1)
                    canvasList[_positionUsed1].Background = Brushes.DarkCyan;
                if (_positionUsed2 != -1)
                    canvasList[_positionUsed2].Background = Brushes.DarkCyan;
                if (_positionUsed3 != -1)
                    canvasList[_positionUsed3].Background = Brushes.Red;
            });
        }

        /// <summary>
        /// Creates a temporary list of blocks to draw, based on list of integers, and renders them
        /// </summary>
        /// <param name="blocks">List of values to visualize</param>
        public void Visualize(List<int> blocks)
        {
            Thread.Sleep(_sortSpeed);
            SetCanvasList(blocks);
            DrawBlocks();
        }

        #endregion



        #region sorting algorithms

        public void BubbleSort()
        {
            for (int i = 0; i < (valuesList.Count - 1); i++)
            {
                for (int j = 0; j < (valuesList.Count - i - 1); j++)
                {
                    _positionUsed1 = j;
                    _positionUsed2 = j + 1;



                    if (valuesList[j] > valuesList[j + 1])
                    {
                        int tmp = valuesList[j];
                        valuesList[j] = valuesList[j + 1];
                        valuesList[j + 1] = tmp;
                    }

                    Visualize(valuesList);
                }             
            }

            _positionUsed1 = -1;
            _positionUsed2 = -1;
            Visualize(valuesList);
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                startButton.Content = "RESET";
                startButton.IsEnabled = true;
            });
            
        }


        public void SelectionSort()
        {
            for (int i = 0; i < (valuesList.Count - 1); i++)
            {
                int minIndex = i, minValue = valuesList[i];
                for (int j = i + 1; j < valuesList.Count; j++)
                {
                    _positionUsed1 = minIndex;
                    _positionUsed2 = j;
                    if (valuesList[j] < valuesList[minIndex])
                    {
                        minValue = valuesList[j];
                        minIndex = j;
                    }
                    Visualize(valuesList);
                }
                int tmp = valuesList[minIndex];
                valuesList[minIndex] = valuesList[i];
                valuesList[i] = tmp;
            }
            _positionUsed1 = -1;
            _positionUsed2 = -1;
            Visualize(valuesList);
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                startButton.Content = "RESET";
                startButton.IsEnabled = true;
            });
        }


        //MERGE SORT CONTAINS 3 METHODS
        public void MergeSort()
        {
            MergeSortHelper(0, valuesList.Count - 1);
            Visualize(valuesList);
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                startButton.Content = "RESET";
                startButton.IsEnabled = true;
            });
        }

        private void MergeSortHelper(int left, int right)
        {
            if (left < right)
            {
                int middle = (left + right) / 2;

                MergeSortHelper(left, middle);
                MergeSortHelper(middle + 1, right);

                MergeSortMerge(left, middle, right);
            }
        }

        private void MergeSortMerge(int left, int middle, int right)
        {
            int[] leftArray = new int[middle - left + 1];
            int[] rightArray = new int[right - middle];

            int i = 0;
            int j = 0;
            for (i = left; i <= middle; i++)
            {
                leftArray[j] = valuesList[i];
                j++;
            }

            i = 0;
            j = 0;
            for (i = middle + 1; i <= right; i++)
            {
                rightArray[j] = valuesList[i];
                j++;
            }

            i = 0;
            j = 0;
            for (int k = left; k < right + 1; k++)
            {
                _positionUsed1 = k;
                if (i == leftArray.Length)
                {
                    valuesList[k] = rightArray[j];
                    j++;
                }
                else if (j == rightArray.Length)
                {
                    valuesList[k] = leftArray[i];
                    i++;
                }
                else if (leftArray[i] <= rightArray[j])
                {
                    valuesList[k] = leftArray[i];
                    i++;
                }
                else
                {
                    valuesList[k] = rightArray[j];
                    j++;
                }
                Visualize(valuesList);
            }
            Visualize(valuesList);
            _positionUsed1 = -1;
        }
        //MERGE SORT ENDS HERE

        
        //QUICK SORT CONTAINS 3 METHODS
        public void QuickSort()
        {
            QuickSortHelper(0, valuesList.Count - 1);
            _positionUsed1 = -1;
            _positionUsed2 = -1;
            _positionUsed3 = -1;
            Visualize(valuesList);
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                startButton.Content = "RESET";
                startButton.IsEnabled = true;
            });
        }

        private void QuickSortHelper(int low, int high)
        {
            if (low < high)
            {
                int partitioningIndex = Partition(low, high);

                QuickSortHelper(low, partitioningIndex - 1);
                QuickSortHelper(partitioningIndex + 1, high);
            }
        }

        private int Partition(int low, int high)
        {
            Visualize(valuesList);
            int pivot = valuesList[high];
            _positionUsed3 = high;

            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                _positionUsed1 = j;
                _positionUsed2 = i;
                Visualize(valuesList);
                if (valuesList[j] < pivot)
                {
                    i++;

                    int temp = valuesList[i];
                    valuesList[i] = valuesList[j];
                    valuesList[j] = temp;
                }
            }

            int temp1 = valuesList[i + 1];
            valuesList[i + 1] = valuesList[high];
            valuesList[high] = temp1;

            return i + 1;
        }
        //QUICK SORT ENDS HERE


        public void InsertionSort()
        {
            for (int i = 1; i < valuesList.Count; i++)
            {
                
                int temp = valuesList[i];
                int j = i - 1;
                while (j >= 0 && temp < valuesList[j])
                {
                    _positionUsed1 = j;
                    Visualize(valuesList);
                    valuesList[j + 1] = valuesList[j];
                    j--;
                }

                valuesList[j + 1] = temp;
            }

            _positionUsed1 = -1;
            Visualize(valuesList);
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                startButton.Content = "RESET";
                startButton.IsEnabled = true;
            });
        }
        #endregion
    }
}
