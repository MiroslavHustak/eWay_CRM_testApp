using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace eWay_CRM_testApp
{
    public partial class MainWindowNonOpt : Window
    {
        public MainWindowNonOpt() => InitializeComponent();

        //Add EventTrigger KeyDown="OnKeyDownHandler" in MainWindow.xaml   
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
              => this.WindowState = (e.Key == Key.Escape) ? WindowState.Minimized : WindowState.Normal;
      
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            // https://stackoverflow.com/questions/1050953/wpf-toolbar-how-to-remove-grip-and-overflow

            ToolBar toolBar = sender as ToolBar;

            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;

            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;

            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
       
        private void E_Way_Loaded(object sender, RoutedEventArgs e)
        {
            //Shall be here TODO: find out why
        }        
    }
}
