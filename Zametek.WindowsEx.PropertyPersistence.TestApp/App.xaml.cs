using System;
using System.IO;
using System.Windows;
using Zametek.WindowsEx.PropertyPersistence.Xml;

namespace Zametek.WindowsEx.PropertyPersistence.TestApp
{
    public partial class App
    {
        private readonly StateResourceAccess m_StateResourceAccess;

        public App()
        {
            string xmlFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "persistence.xml");
            m_StateResourceAccess = new StateResourceAccess(xmlFileName);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
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
