using LeanBrowser.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeanBrowser
{
    /// <summary>
    /// Interaction logic for TabBar.xaml
    /// </summary>
    public partial class TabBar : UserControl
    {
        private MainWindow mainWindow;
        private Tab tab;
        public List<Tab> TabCollection;
        private int TabCount;

        public TabBar()
        {
            InitializeComponent();
            TabCount = 0;
            TabCollection = new List<Tab>();

            Loaded += TabBar_Loaded;
        }

        private void TabBar_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Raise command to open a new tab
            CustomCommands.New.Execute(new OpenTabCommandParameters(string.Empty, "New tab"), this);
        }

        public void SelectTab(Tab tabSelect)
        {
            // Focus the main window otherwise, keyDown event won't be fired
            // because the sender object is now hidden (TabView)
            mainWindow.Focus();

            foreach (var tab in TabCollection)
            {
                if (tab == tabSelect)
                {
                    tab.IsSelected = true;
                    tab.form.Visibility = Visibility.Visible;

                    TabView tv = tab.form as TabView;
                    mainWindow.Menu.tabView = tv;
                    mainWindow.ToolBar.TabView = tv;
                    mainWindow.Menu.RefreshZoom();
                }
                else
                {
                    tab.IsSelected = false;
                    tab.form.Visibility = Visibility.Hidden;
                }
            }

            // Close the popup menu if open
            mainWindow.MenuPopup.IsOpen = false;
        }

        public int GetSelectedTabIndex()
        {
            for (int i = 0; i < TabCollection.Count; i++)
            {
                if (TabCollection[i].IsSelected)
                {
                    if (i + 1 < TabCollection.Count)
                    {
                        // Return next tab index
                        return i + 1;
                    }
                }
            }

            // Return first tab if current tab is the last one
            return 0;
        }

        public void RemoveTab(Tab tabToRemove)
        {
            TabCount = 0;

            TabCollection.Remove(tabToRemove);
            canvas.Children.Remove(tabToRemove);
            RefreshTabWidth();

            foreach (var ctrl in TabCollection)
            {
                TabCount += 1;
                tabToRemove.mainWindow.container.Children.Remove(tabToRemove.form);
                if (tabToRemove.form.GetType() == typeof(TabView))
                {
                    var tv = tabToRemove.form as TabView;
                    tv.Shutdown();
                }
                if (tabToRemove.IsSelected)
                {
                    // Select the last tab
                    SelectTab(TabCollection[TabCollection.Count - 1]);
                }
            }
            if (TabCount == 0)
            {
                //Application.Current.Shutdown();
                mainWindow.Close();
            }
        }

        public async void AddTab(OpenTabCommandParameters commandParams, MainWindow mw)
        {
            UserControl userControl;
            string Title;

            if (commandParams != null)
            {
                Title = commandParams.Title;
                if (commandParams.Control == null)
                {
                    userControl = new TabView(mw, commandParams.Url);
                }
                else
                {
                    userControl = commandParams.Control;
                }
            }
            else
            {
                userControl = new TabView(mw, "");
                Title = "New Tab";
            }

            await Dispatcher.BeginInvoke((Action)(() =>
            {
                mainWindow = mw;
                tab = new Tab(Title, mw, userControl);

                canvas.Children.Add(tab);

                TabCount += 1;
                TabCollection.Add(tab);

                RefreshTabWidth();

                // Scroll to the right end
                sv.ScrollToRightEnd();
            }));
        }


        public Tab getTabFromForm(UserControl form)
        {
            foreach (var ctrl in TabCollection)
            {
                if (ctrl.form != null)
                {
                    if (ctrl.form.Equals(form))
                    {
                        return ctrl;
                    }
                }
            }
            return TabCollection[0];
        }


        public void RefreshTabWidth()
        {
            // Calculate new width
            int count = TabCollection.Count;
            if (count == 0)
            {
                count = 1;
            }

            double newWidth = (this.ActualWidth - 116) / (count);

            // Set the new width
            foreach (var tab in TabCollection)
            {
                tab.Width = newWidth;
            }
        }


        /// <summary>
        /// Handle scrolling through tabs using the mouse wheel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
                scrollviewer.LineLeft();
            else
                scrollviewer.LineRight();
            e.Handled = true;
        }


    }
}
