using Microsoft.Win32;
using PluginInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Threading;
using ColorPickerWPF;
using ColorPickerWPF.Code;

namespace Rysowanie
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Color _currentColor = Colors.Red;
        private int _currentThickness = 3;
        private IPlugin _currentActivePlugin;
        string filename;
        private string filemane;

        public MainWindow()
        {
            InitializeComponent();
            InitializePlugins();
            this.Closing += MainWindow_Closing;
        }

        private void InitializePlugins()
        {
            var assemblies = GetAssemblies("Plugins");

            List<MenuItem> menuItems = new List<MenuItem>();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass && type.IsPublic && type.GetInterface(typeof(IPlugin).FullName) != null)
                    {
                        var item = Activator.CreateInstance(type) as IPlugin;
                        var menuItem = item.GetMenuItem();
                        menuItem.Tag = item;
                        menuItem.Click += pluginMenuItem_Click;
                        menuItems.Add(menuItem);

                    }
                }
            }

            if (menuItems.Any())
            {
                var tools = new MenuItem();
                tools.Header = "Tools";
                menuItems.ForEach((i) => {
                    tools.Items.Add(i);

                    /* MenuItem z = new MenuItem();
                    // z = i;
                     paseczek.Items.Add(i); */
                });

                v_Menu.Items.Add(tools);
            }
        }


        private void pluginMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem == null)
                return;

            if (_currentActivePlugin != null)
                _currentActivePlugin.Dispose();

            IPlugin plugin = menuItem.Tag as IPlugin;
            if (plugin != null)
            {
                _currentActivePlugin = plugin;
                _currentActivePlugin.Initialize(v_Canvas, _currentColor, _currentThickness);
            }
        }

        private List<Assembly> GetAssemblies(string directory)
        {
            var assemblies = new List<Assembly>();
            if (Directory.Exists(directory))
            {
                foreach (var file in Directory.GetFiles(directory, "*.dll"))
                {
                    assemblies.Add(Assembly.LoadFrom(file));
                }
            }
            return assemblies;
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Canvas)sender).ReleaseMouseCapture();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_currentActivePlugin != null)
            {
                _currentActivePlugin.Dispose();
            }
        }

        private async void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            string filenameRes = await OknoAsync();
            SaveFile();
        }

        private async Task<string> OknoAsync()
        {
            return await Task.Run(() =>
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Shape"; // Default file name
                dlg.DefaultExt = ".text"; // Default file extension
                dlg.Filter = "Shape (.png)|*.png"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    filename = dlg.FileName;
                }
                return filemane;
            });
        }
        private void SaveFile()
        {
            if (filemane != null)
            {
                var renderTargetBitmap = new RenderTargetBitmap((int)v_Canvas.RenderSize.Width,
                 (int)v_Canvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
                renderTargetBitmap.Render(v_Canvas);

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    encoder.Save(fs);
                    fs.Close();
                }
            }
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "Just png graphics (.png)|*.png"; // PNG
            if (op.ShowDialog() == true)
            {
                v_Canvas.Children.Clear();
                Image brush = new Image();
                brush.Source = new BitmapImage(new Uri(op.FileName));
                v_Canvas.Children.Add(brush);
            }
            
        }
        
        private void New_Click(object sender, RoutedEventArgs e)
        {
            clear("Czy utworzyć nowy obraz. Nie zapisane zmiany zostaną utracone?", "Nowy");
        }

        private void clear(string komunikat, string tytul)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(komunikat, tytul, MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                v_Canvas.Children.Clear();
            }
        }

        private void Transformacja_Click(object sender, RoutedEventArgs e)
        {
            if (_currentActivePlugin != null)
                _currentActivePlugin.Dispose();

            RotateTransform rotate = new RotateTransform(130);
            v_Canvas.LayoutTransform = rotate;


        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
           // int topItem = v_Canvas.Str
            //v_Canvas.Children
        }
    
        private void Usowanie_Click(object sender, RoutedEventArgs e)
        {
            clear("Czy wyczyścić obraz", "Usówanie");
        }

        private void Kolor_Click(object sender, RoutedEventArgs e)
        {
            Color color;

            bool ok = ColorPickerWindow.ShowDialog(out color, ColorPickerDialogOptions.SimpleView);

            _currentColor = color;
        }

        private void line_ComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
      
            
            if (linecombo.SelectedIndex == 0)
            {
                _currentThickness = 3;
                _currentActivePlugin.Dispose();
                _currentActivePlugin.Initialize(v_Canvas, _currentColor, _currentThickness);
            }
            if (linecombo.SelectedIndex == 1)
            {
                _currentThickness = 6;
                _currentActivePlugin.Dispose();
                _currentActivePlugin.Initialize(v_Canvas, _currentColor, _currentThickness);
            }
            if (linecombo.SelectedIndex == 2)
            {
                _currentThickness = 9;
                _currentActivePlugin.Dispose();
                _currentActivePlugin.Initialize(v_Canvas, _currentColor, _currentThickness);
            }
        }
    }
}
