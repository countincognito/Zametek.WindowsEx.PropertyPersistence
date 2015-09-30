﻿using System;
using System.IO;
using System.Windows;
using Zametek.WindowsEx.PropertyPersistence.Xml;

namespace Zametek.WindowsEx.PropertyPersistence.TestApp
{
    public partial class App
    {
        private readonly string m_PropertyPersistenceFileName;
        private readonly StateResourceAccess m_StateResourceAccess;

        public App()
        {
            m_PropertyPersistenceFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "PropertyPersistence.xml");
            m_StateResourceAccess = new StateResourceAccess(m_PropertyPersistenceFileName);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            string message = string.Empty;
            if (File.Exists(m_PropertyPersistenceFileName))
            {
                message = "I will now attempt to load the following file:\n"
                    + m_PropertyPersistenceFileName + "\n\nThis file will be overwritten when the application closes.";
            }
            else
            {
                message = "I could not find the following file:\n"
                    + m_PropertyPersistenceFileName + "\n\nIt will be created when the application closes.";
            }
            MessageBox.Show(message, "Pay attention!", MessageBoxButton.OK);
            PropertyStateHelper.Load(m_StateResourceAccess);
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            PropertyStateHelper.Save(m_StateResourceAccess);
            base.OnExit(e);
        }
    }
}
