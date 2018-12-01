using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DotNetBrowser;
using LeanBrowser.Controls;

namespace LeanBrowser
{
    public class BrowserContextMenuHandler : ContextMenuHandler
    {
        FrameworkElement view;
        MainWindow mainWindow;
        bool IsShow;
        Browser browser;

        public BrowserContextMenuHandler(FrameworkElement view, bool IsShow)
        {
            this.view = view;
            this.IsShow = IsShow;
            mainWindow = view.FindParent<MainWindow>();
        }

        public void ShowContextMenu(ContextMenuParams parameters)
        {
            view.Dispatcher.BeginInvoke(new Action(() =>
            {
                System.Windows.Controls.ContextMenu popupMenu = new System.Windows.Controls.ContextMenu();
                browser = parameters.Browser;

                // Regular context menu
                if (!browser.IsCommandEnabled(EditorCommand.PASTE))
                {
                    if (!string.IsNullOrEmpty(parameters.LinkURL))
                    {
                        string[] linkArgs = new string[] { parameters.LinkURL };

                        // Open in new tab
                        popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Open link in new tab", true, Visibility.Visible, delegate
                        {
                            CustomCommands.New.Execute(new OpenTabCommandParameters(parameters.LinkURL, "New tab"), mainWindow);
                        }));

                        // Open in new window
                        popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Open link in new window", true, Visibility.Visible, delegate
                        {
                            ApplicationCommands.New.Execute(new OpenWindowCommandParameters(linkArgs), mainWindow);
                        }));

                        // Open in new incognito window
                        popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Open link in incognito window", true, Visibility.Visible, delegate
                        {
                            CustomCommands.NewIncognito.Execute(new OpenWindowCommandParameters(linkArgs), mainWindow);
                        }));
                    }
                    else
                    {
                        // Back
                        popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Back", browser.CanGoBack(), Visibility.Visible, delegate
                        {
                            browser.GoBack();
                        }, "Alt+Left Arrow"));

                        // Forward
                        popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Forward", browser.CanGoForward(), Visibility.Visible, delegate
                        {
                            browser.GoForward();
                        }, "Alt+Right Arrow"));

                        // Reload
                        popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Reload", true, Visibility.Visible, delegate
                        {
                            browser.Reload();
                        }, "Ctrl+R"));
                    }

                    // Separator
                    popupMenu.Items.Add(new Separator());

                    if (!string.IsNullOrEmpty(parameters.SelectionText))
                    {
                        // Copy
                        popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Copy", true, Visibility.Visible, delegate
                        {
                            browser.ExecuteCommand(EditorCommand.COPY);
                        }, "Ctrl+C"));
                    }

                    if (!string.IsNullOrEmpty(parameters.LinkText))
                    {
                        // Copy link address
                        popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Copy link address", true, Visibility.Visible, delegate
                        {
                            Clipboard.SetText(parameters.LinkURL);
                        }));
                    }

                    if (string.IsNullOrEmpty(parameters.SelectionText))
                    {
                        // Select all
                        popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Select all", true, Visibility.Visible, delegate
                        {
                            browser.ExecuteCommand(EditorCommand.SELECT_ALL);
                        }, "Ctrl+A"));
                    }

                    // Print
                    popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Print", true, Visibility.Visible, delegate
                    {
                        browser.Print();
                    }, "Ctrl+P"));

                    // Exit full screen
                    if (mainWindow.IsFullScreen)
                    {
                        popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Exit full-screen", true, Visibility.Visible, delegate
                        {
                            mainWindow.IsFullScreen = false;
                        }, "Esc"));
                    }
                } else // Editor context menu
                {
                    // Undo
                    popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Undo", browser.IsCommandEnabled(EditorCommand.UNDO), Visibility.Visible, delegate
                    {
                        browser.ExecuteCommand(EditorCommand.UNDO);
                    }, "Ctrl+Z"));

                    // Redo
                    popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Redo", browser.IsCommandEnabled(EditorCommand.REDO), Visibility.Visible, delegate
                    {
                        browser.ExecuteCommand(EditorCommand.REDO);
                    }, "Ctrl+Shift+Z"));

                    // Separator
                    popupMenu.Items.Add(new Separator());

                    // Cut
                    popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Cut", browser.IsCommandEnabled(EditorCommand.CUT), Visibility.Visible, delegate
                    {
                        browser.ExecuteCommand(EditorCommand.CUT);
                    }, "Ctrl+X"));

                    // Copy
                    popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Copy", browser.IsCommandEnabled(EditorCommand.COPY), Visibility.Visible, delegate
                    {
                        browser.ExecuteCommand(EditorCommand.COPY);
                    }, "Ctrl+C"));

                    // Paste
                    IDataObject ClipboardData = Clipboard.GetDataObject();
                    bool canPaste = ClipboardData.GetDataPresent(DataFormats.UnicodeText) 
                                || ClipboardData.GetDataPresent(DataFormats.Text) 
                                || ClipboardData.GetDataPresent(DataFormats.Html);

                    popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Paste", canPaste, Visibility.Visible, delegate
                    {
                        browser.ExecuteCommand(EditorCommand.PASTE);
                    }, "Ctrl+V"));

                    popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Paste as plain text", canPaste, Visibility.Visible, delegate
                    {
                        browser.ExecuteCommand(EditorCommand.INSERT_TEXT, (string)Clipboard.GetData("Text"));
                    }, "Ctrl+Shift+V"));

                    // Select All
                    popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Select all", browser.IsCommandEnabled(EditorCommand.SELECT_ALL), Visibility.Visible, delegate
                    {
                        browser.ExecuteCommand(EditorCommand.SELECT_ALL);
                    }, "Ctrl+A"));
                }

                // Separator
                popupMenu.Items.Add(new Separator());

                popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("View page source", !browser.URL.StartsWith("view-source:"), Visibility.Visible, delegate
                {
                    CustomCommands.ViewSource.Execute(null, mainWindow);
                }, "Ctrl+U"));

                popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Inspect element", true, Visibility.Visible, delegate
                {
                    MessageBox.Show("inspect element invoked");
                }, "Ctrl+Shift+I"));

                popupMenu.IsOpen = true;
            }));
        }
    }
}
