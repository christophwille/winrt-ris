using System.Diagnostics;
using Callisto.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetroLog;
using MetroLog.Targets;
using Ris.Data;
using Risotto.Common;
using Risotto.Models;
using Risotto.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Search;
using System.Threading.Tasks;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Risotto
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += OnResuming;

//#if DEBUG
//            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new FileStreamingTarget());
//#else
//            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Info, LogLevel.Fatal, new FileStreamingTarget());
//#endif

            // Set up the global crash handler
            GlobalCrashHandler.Configure();
        }

        private bool _commonHasBeenInitialized = false;
        private async Task CommonInitialize()
        {
            if (_commonHasBeenInitialized) return;

            // Register handler for CommandsRequested events from the settings pane
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;

            // Register handler for SuggestionsRequested events from the search pane
            SearchPane.GetForCurrentView().SuggestionsRequested += OnSuggestionsRequested;

            await RisDbContext.InitializeDatabaseAsync();

            _commonHasBeenInitialized = true;
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                OnProtocolActivated((ProtocolActivatedEventArgs)args);
            }
        }

        //
        // http://tozon.info/blog/post/2011/10/06/Windows-8-Metro-declarations-Protocol.aspx
        //
        private async void OnProtocolActivated(ProtocolActivatedEventArgs args)
        {
            // Format:  risdok://NOR12110323
            string dokumentNummer = args.Uri.Host;
            string title = "Dokument " + dokumentNummer;

            var action = DocumentDetailNavigationParameter
                .CreateNavigationParameter(title, NavigationAction.LoadFromService, dokumentNummer);

            await CommonInitialize();
            await EnsureRootFrameInitialization(args.PreviousExecutionState, Window.Current.Content as Frame);

            NavigationService.Navigate<DocumentDetailPage>(action);
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            await CommonInitialize();

            var rootFrame = Window.Current.Content as Frame;
            rootFrame = await EnsureRootFrameInitialization(args.PreviousExecutionState, rootFrame);

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        private async Task<Frame> EnsureRootFrameInitialization(ApplicationExecutionState previousExecutionState, Frame rootFrame)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                NavigationService.Initialize(rootFrame);

                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (previousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            return rootFrame;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        private async void OnResuming(object sender, object e)
        {
            await SuspensionManager.RestoreAsync();
        }

        void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var c = new Color()
            {
                R = 46,
                G = 46,
                B = 46
            };

            // Add an About command
            var about = new SettingsCommand("about",
                "Über",
                (handler) =>
                {
                    var aboutFlyout = new Callisto.Controls.SettingsFlyout();
                    aboutFlyout.Content = new AboutControl();

                    aboutFlyout.ContentBackgroundBrush = new SolidColorBrush(c);
                    aboutFlyout.HeaderText = "Über";

                    WebViewFlyoutFixes.ShowSettingsFlyout(aboutFlyout);
                });

            args.Request.ApplicationCommands.Add(about);

            // Add a Preferences command
            var preferences = new SettingsCommand("preferences", "Einstellungen", (handler) =>
            {
                var settings = new Callisto.Controls.SettingsFlyout();
                settings.Content = new PreferencesUserControl();

                settings.ContentBackgroundBrush = new SolidColorBrush(c);
                settings.HeaderText = "Einstellungen";

                WebViewFlyoutFixes.ShowSettingsFlyout(settings);
            });

            args.Request.ApplicationCommands.Add(preferences);
        }

        //
        // http://blog.falafel.com/Blogs/john-waters/2012/10/10/doing-an-asynchronous-search-in-windows-8
        //
        async void OnSuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs args)
        {
            string query = args.QueryText.ToLower();

            var deferral = args.Request.GetDeferral();

            try
            {
                var ctx = new RisDbContext();
                var matches = await ctx.GetHistoryEntriesStartingWith(query);

                args.Request.SearchSuggestionCollection.AppendQuerySuggestions(matches);
            }
            finally
            {
                deferral.Complete();
            }
        }

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected async override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            // Reinitialize the app if a new instance was launched for search
            if (args.PreviousExecutionState == ApplicationExecutionState.NotRunning ||
                args.PreviousExecutionState == ApplicationExecutionState.ClosedByUser ||
                args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                await CommonInitialize();
            }

            // TODO: Register the Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
            // event in OnWindowCreated to speed up searches once the application is already running

            await EnsureRootFrameInitialization(args.PreviousExecutionState, Window.Current.Content as Frame);

            NavigationService.Navigate<SearchResultsPage>(args.QueryText);
            Window.Current.Activate();
        }
    }
}
