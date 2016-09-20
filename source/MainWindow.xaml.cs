using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using WindowsDesktop;

namespace AppOpener
{
    partial class MainWindow
    {
        private List<AppToOpen> appsWithParameters;

        public MainWindow()
        {
            InitializeWindows();
            LoadProperties();
            InitializeApps();
            this.Close();
        }

        private void InitializeWindows()
        {
            this.InitializeComponent();
            this.lblStatus.Content = "Starting apps...";
        }

        private void LoadProperties()
        {
            appsWithParameters = new List<AppToOpen>();
            var config = ConfigurationManager.GetSection("executables") as AppsSection;
            foreach (var e in config.Apps)
            {
                appsWithParameters.Add(e as AppToOpen);
            }
        }

        private void InitializeApps()
        {
            VirtualDesktop actual = VirtualDesktop.Current;
            foreach (var app in appsWithParameters)
            {
                var desktopToWork = CreateDesktopNumber(app.Desktop);
                desktopToWork.Switch();
                var proc = StartAppWithParameters(app);
                var window = proc.MainWindowHandle;
                proc.WaitForInputIdle();
            }
            Thread.Sleep(200);
            actual.Switch();
        }

        private Process StartAppWithParameters(AppToOpen app)
        {
            string fullPath = app.Path;
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Path.GetFileName(fullPath);
            psi.WorkingDirectory = Path.GetDirectoryName(fullPath);
            psi.Arguments = app.Parameters;
            return Process.Start(psi);
        }

        private VirtualDesktop CreateDesktopNumber(int numberOfDesktop)
        {
            VirtualDesktop desktop;
            var actualDesktops = VirtualDesktop.GetDesktops();
            if (numberOfDesktop >= actualDesktops.Length)
            {
                do
                {
                    desktop = VirtualDesktop.Create();
                    actualDesktops = VirtualDesktop.GetDesktops();
                } while (actualDesktops.Length < numberOfDesktop);
            }
            else
            {
                desktop = actualDesktops[numberOfDesktop];
            }
            return desktop;
        }


        private void MoveWindowToDesktop(IntPtr window, VirtualDesktop desktop)
        {
            VirtualDesktopHelper.MoveToDesktop(window, desktop);
        }
    }

}
