/*
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 *                                                                                 *
 * Copyright (c) 2019 TYDI_BIM_Technology_Center                                   *
 *                                                                                 *
 * Author xu.lanhui                                                               *
 *                                                                                 *
 * Time 2021/1/19 10:24:25                                                                     *
 *                                                                                 *
 * Describe $Used to do something$                                                 *
 * TODO:                                                                           *
 *                                                                                 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfRevitLibrary
{
    public class TaskContainer
    {
        private static readonly object LockObj = new object();
        private volatile static TaskContainer _instance;

        private readonly Queue<Action<Autodesk.Revit.UI.UIApplication>> _tasks;

        private TaskContainer()
        {
            _tasks = new Queue<Action<Autodesk.Revit.UI.UIApplication>>();
        }

        public static TaskContainer Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock (LockObj)
                    {
                        if(_instance == null)
                        {
                            _instance = new TaskContainer();
                        }
                    }
                }

                return _instance;
            }

        }

        public void EnqueueTask(Action<Autodesk.Revit.UI.UIApplication> task)
        {
            _tasks.Enqueue(task);
        }

        public bool HasTaskToPerform
        {
            get { return _tasks.Count > 0; }
        }

        public Action<Autodesk.Revit.UI.UIApplication> DequeueTask()
        {
            return _tasks.Dequeue();
        }
    }
}
