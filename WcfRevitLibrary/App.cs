/*
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 *                                                                                 *
 * Copyright (c) 2019 TYDI_BIM_Technology_Center                                   *
 *                                                                                 *
 * Author xu.lanhui                                                               *
 *                                                                                 *
 * Time 2021/1/19 14:23:43                                                                     *
 *                                                                                 *
 * Describe $Used to do something$                                                 *
 * TODO:                                                                           *
 *                                                                                 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 */

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfRevitLibrary
{
    class App : Autodesk.Revit.UI.IExternalApplication
    {
        private const string servceUri = "net.pipe://localhost/";
        private ServiceHost serviceHost;
        public Result OnStartup(UIControlledApplication application)
        {
            application.Idling += OnIdling;
            Uri uri = new Uri(servceUri);

            serviceHost = new ServiceHost(typeof(RevitExternalService), uri);

            try
            {
                serviceHost.AddServiceEndpoint(typeof(IRevitExternalService), new NetNamedPipeBinding(), "RevitExternalService");

                serviceHost.Open();
            }
            catch(Exception ex)
            {
                application.ControlledApplication.WriteJournalComment(
                    string.Format("{0}.\r\n{1}", "not start wcf", ex.ToString()), true);

            }
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            application.Idling -= OnIdling;

            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            return Result.Succeeded;
        }

        private void OnIdling(object sender,IdlingEventArgs e)
        {
            var uiApp = sender as UIApplication;
            Debug.Print("OnIdling: {0}", DateTime.Now.ToString("HH:mm:ss.fff"));

            e.SetRaiseWithoutDelay();

            if (!TaskContainer.Instance.HasTaskToPerform)
                return;

            try
            {
                Debug.Print("{0}:{1}", "Revit binding ... ", DateTime.Now.ToString("HH:mm:ss.fff"));

                var task = TaskContainer.Instance.DequeueTask();
                task(uiApp);
               // TaskDialog.Show("Reivt", string.Format("{0}:{1}", "Revit binding ... ", DateTime.Now.ToString("HH:mm:ss.fff")));
                Debug.Print("{0}:{1}", "Revit binding", DateTime.Now.ToString("HH:mm:ss.fff"));
            }
            catch(Exception ex)
            {
                uiApp.Application.WriteJournalComment(
                    string.Format("RevitExternalService. {0}\r\n{1}", "Error on Idling", ex.ToString()), true);
                TaskDialog.Show("Revit",string.Format("RevitExternalService. {0}\r\n{1}", "Error on Idling", ex.ToString()));
                Debug.WriteLine(ex);
            }
        }
    }
}
