using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Design;

namespace ViewSonic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public static int ActionOption = 0; //0 for draw, 1 for erase, 2 for select
        private int ShapeOption = 0; //0 for rect, 1 for ellipses, 2 triangles.
        private Point mouseStartPosition;
 
        private List<CanvasShape> canvasShapes = new List<CanvasShape>();
        private CanvasShape? selectedShape;


        public MainWindow()
        {
            InitializeComponent();
            
            DataContext = new ViewModel();
        }

        /*
        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            RectanglesButton.IsEnabled = true;
            EllipsesButton.IsEnabled = true;
            TrianglesButton.IsEnabled = true;

            ActionOption = 0;
        }
        */
        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            RectanglesButton.IsEnabled = false;
            EllipsesButton.IsEnabled = false;
            TrianglesButton.IsEnabled = false;
            */
            ActionOption = 1;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            RectanglesButton.IsEnabled = false;
            EllipsesButton.IsEnabled = false;
            TrianglesButton.IsEnabled = false;
            */
            ActionOption = 2;
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            DrawAreaCanvas.Children.Clear();
            canvasShapes.Clear();
        }

        private void RectanglesButton_Click(object sender, RoutedEventArgs e)
        {
            ShapeOption = 0;
        }

        private void EllipsesButton_Click(object sender, RoutedEventArgs e)
        {
            ShapeOption = 1;
        }

        private void TrianglesButton_Click(object sender, RoutedEventArgs e)
        {
            ShapeOption = 2;
        }

        private void DrawArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseStartPosition = e.GetPosition(DrawAreaCanvas);

            int x = (int)mouseStartPosition.X;
            int y = (int)mouseStartPosition.Y;

            if (2 == ActionOption)
            {
                foreach (var item in canvasShapes)
                {
                    if (item.Shape.IsMouseOver) {
                        selectedShape = item;
                        break;
                    }
                }
            }

            Coordinate.Content = $"x: {x} y: {y}";
        }

        private void DrawArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point mouseEndPosition = e.GetPosition(DrawAreaCanvas);
            CanvasShape c;
            Shape s;

            Coordinate.Content = $"x: {(int)mouseEndPosition.X} y: {(int)mouseEndPosition.Y}";

            //Could be cheanged by delegate function
            if (0 == ActionOption)
            {
                if (ShapeOption == 0)
                {
                    s = new Rectangle()
                    {
                        Stroke = ViewModel.BrushColor,
                        StrokeThickness = ViewModel.Thickness,
                        Fill = Brushes.White,
                        Width = Math.Abs(mouseStartPosition.X - mouseEndPosition.X),
                        Height = Math.Abs(mouseStartPosition.Y - mouseEndPosition.Y),
                    };

                    Canvas.SetLeft(s, Math.Min(mouseStartPosition.X, mouseEndPosition.X));
                    Canvas.SetTop(s, Math.Min(mouseStartPosition.Y, mouseEndPosition.Y));

                    c = new CanvasShape(s);
                    canvasShapes.Add(c);    //Save to shape list

                    DrawAreaCanvas.Children.Add(c.Shape);
                }
                else if (ShapeOption == 1)
                {
                    s = new Ellipse()
                    {
                        Stroke = ViewModel.BrushColor,
                        StrokeThickness = ViewModel.Thickness,
                        Fill = Brushes.White,
                        Width = Math.Abs(mouseStartPosition.X - mouseEndPosition.X),
                        Height = Math.Abs(mouseStartPosition.Y - mouseEndPosition.Y),
                    };

                    Canvas.SetLeft(s, Math.Min(mouseStartPosition.X, mouseEndPosition.X));
                    Canvas.SetTop(s, Math.Min(mouseStartPosition.Y, mouseEndPosition.Y));

                    c = new CanvasShape(s);
                    canvasShapes.Add(c);

                    DrawAreaCanvas.Children.Add(c.Shape);
                }
                else if (ShapeOption == 2)
                {
                    Polygon p = new Polygon
                    {
                        Stroke = ViewModel.BrushColor,
                        StrokeThickness = ViewModel.Thickness,
                        Fill = Brushes.White
                    };

                    PointCollection points = new PointCollection
                    {
                        new Point(mouseStartPosition.X, mouseStartPosition.Y),
                        new Point(mouseEndPosition.X, mouseStartPosition.Y),
                        new Point(mouseStartPosition.X , mouseEndPosition.Y)
                    };

                    p.Points = points;
                    s = p;

                    c = new CanvasShape(s);
                    canvasShapes.Add(c);

                    DrawAreaCanvas.Children.Add(c.Shape);
                }
                else
                {
                    //For future use
                }
            }
            else if (1 == ActionOption)
            {
                foreach (var shape in canvasShapes)
                {
                    if (shape.Shape.IsMouseOver)
                    {
                        DrawAreaCanvas.Children.Remove(shape.Shape);
                        canvasShapes.Remove(shape);
                        break;
                    }
                }
            }
            else if (2 == ActionOption)
            {
                if (selectedShape == null)
                {
                    MessageBox.Show("Shape selected is null");
                    return;
                }

                double deltaX = mouseEndPosition.X - mouseStartPosition.X;
                double deltaY = mouseEndPosition.Y - mouseStartPosition.Y;

                if (selectedShape.Shape.GetType() == typeof(Polygon))
                {
                    c = new CanvasShape(selectedShape.Shape);

                    PointCollection collections = new PointCollection();

                    foreach (Point point in ((Polygon)(selectedShape.Shape)).Points)
                    {
                        collections.Add(new Point(point.X + deltaX, point.Y + deltaY));
                    }

                    ((Polygon)(c.Shape)).Points = collections;
                }
                else
                {
                    c = new CanvasShape(selectedShape.Shape);
                    Canvas.SetLeft(c.Shape, Canvas.GetLeft(c.Shape) + deltaX);
                    Canvas.SetTop(c.Shape, Canvas.GetTop(c.Shape) + deltaY);
                }

                canvasShapes.Add(c);
                canvasShapes.Remove(selectedShape);

                DrawAreaCanvas.Children.Remove(selectedShape.Shape);
                DrawAreaCanvas.Children.Add(c.Shape);
            }
            else
            {
                //For use in future
            }
        }

        /*
        private void ThicknessBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text;

            if (newText == null)
            {
                MessageBox.Show("Can not be zero");
                return;
            }

            if (int.TryParse(newText, out int intValue))
            {
                if ((intValue <= 0) || (intValue > 10))
                {
                    MessageBox.Show("Invalid value : should more than 0 and less than 10");

                    return;
                }

                BrushProperty.Thickness = intValue;
            }
            else
            {
                MessageBox.Show("Invalid value");
                return;
            }
        }

        private void ColorBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)sender;

            if (1 == a.SelectedIndex) {
                BrushProperty.BrushColor = Brushes.Black;
            }
            else if (2 == a.SelectedIndex) {
                BrushProperty.BrushColor = Brushes.Red;
            }
            else if (3 == a.SelectedIndex) {
                BrushProperty.BrushColor = Brushes.Green;
            }
        }
        */
        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Files (*.png)|*.png",
                Title = "Save Image"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string saveImagePath = saveFileDialog.FileName;

                try
                {
                    // Create a RenderTargetBitmap with the canvas size
                    RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                        (int)DrawAreaCanvas.RenderSize.Width, (int)DrawAreaCanvas.RenderSize.Height,
                        96d, 96d, PixelFormats.Pbgra32);

                    // Render the Canvas onto the bitmap
                    renderBitmap.Render(DrawAreaCanvas);

                    // Create a PngBitmapEncoder and save the bitmap as a PNG image
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (var stream = new FileStream(saveImagePath, FileMode.Create))
                    {
                        encoder.Save(stream);
                    }

                    MessageBox.Show("Image saved successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving the image: {ex.Message}");
                }
            }
        }

        private void MenuItem_Load_Click(object sender, RoutedEventArgs e)
        {
            // Create a new instance of BitmapImage for each load
            BitmapImage bitmapImage = new BitmapImage();

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PNG Files (*.png)|*.png",
                Title = "Select a PNG Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;

                // Check if the selected file is a PNG image
                if (System.IO.Path.GetExtension(selectedImagePath).Equals(".png", StringComparison.OrdinalIgnoreCase))
                {
                    // Load the PNG image from the file
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(selectedImagePath);
                    bitmapImage.EndInit();

                    // Create an Image element to display the loaded image
                    Image image = new Image
                    {
                        Source = bitmapImage,
                        Width = bitmapImage.PixelWidth,
                        Height = bitmapImage.PixelHeight
                    };

                    // Clear existing content and add the image to the Canvas
                    DrawAreaCanvas.Children.Clear();
                    canvasShapes.Clear();
                    DrawAreaCanvas.Children.Add(image);
                }
                else
                {
                    MessageBox.Show("Please select a PNG image file.");
                }
            }
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
