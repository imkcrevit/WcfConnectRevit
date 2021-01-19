using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace WcfRevitLibrary
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    public class RevitExternalService : IRevitExternalService
    {
        private string currentDocumentPath;
        private static readonly object _locker = new object();

        private const int WALL_TIMEOUT = 10000;


        public string GetCurrentDocumentPath()
        {
            Debug.Print("{0}:{1}", "start", DateTime.Now.ToString("HH:mm:ss.fff"));
            lock (_locker)
            {
                TaskContainer.Instance.EnqueueTask(GetDocumentPath);
                Monitor.Wait(WALL_TIMEOUT);
            }
            Debug.Print("{0}:{1}", "End", DateTime.Now.ToString("HH:mm:ss.fff"));
            return currentDocumentPath;
        }

        private void GetDocumentPath(Autodesk.Revit.UI.UIApplication uiapp)
        {
            try
            {
                currentDocumentPath = uiapp.ActiveUIDocument.Document.PathName;
            }
            finally
            {
                lock (_locker)
                {
                    Monitor.Pulse(_locker);
                }
            }
        }

        public bool LoadFamily(string path)
        {
            Autodesk.Revit.DB.Family family = null;
            lock (_locker)
            {
                TaskContainer.Instance.EnqueueTask(uiapp =>
                {
                    try
                    {
                        var doc = uiapp.ActiveUIDocument.Document;



                        using (Autodesk.Revit.DB.Transaction trans = new Autodesk.Revit.DB.Transaction(doc, "loadFamily"))
                        {
                            trans.Start();

                            var isLoad = doc.LoadFamily(path, out family);
                            if (isLoad)
                            {
                                Debug.Print("{0}:{1}", "load is success", family.Name);
                            }
                            else
                            {
                                Debug.Print("load is failed");
                            }


                            trans.Commit();
                        }
                    }
                    finally
                    {
                        lock (_locker)
                        {
                            Monitor.Pulse(_locker);
                        }
                    }
                });
                Monitor.Wait(_locker);
            }
            return family != null;
        }
    }
}
