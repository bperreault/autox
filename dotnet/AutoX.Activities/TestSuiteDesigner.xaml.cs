#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

using AutoX.Activities.AutoActivities;

#region

using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Windows;
using System.Xml.Linq;
using AutoX.Basic;

#endregion

#endregion

namespace AutoX.Activities
{
    // Interaction logic for TestSuiteDesigner.xaml
    public partial class TestSuiteDesigner
    {
        public TestSuiteDesigner()
        {
            InitializeComponent();
        }

        public void Connect(int connectionId, object target)
        {
            throw new NotImplementedException();
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (DragDropHelper.AllowDrop(
                e.Data,
                Context,
                typeof (Activity)))
            {
                e.Effects = (DragDropEffects.Move & e.AllowedEffects);
                e.Handled = true;
            }
            else
            {
                var data = e.Data.GetData(Constants.DATA_FORMAT) as XElement;
                if (Utilities.CheckValidDrop(data, Constants.SCRIPT))
                {
                    e.Effects = (DragDropEffects.Move & e.AllowedEffects);
                    e.Handled = true;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
                }
            }
            base.OnDragEnter(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            if (DragDropHelper.AllowDrop(
                e.Data,
                Context,
                typeof (Activity)))
            {
                e.Effects = (DragDropEffects.Move & e.AllowedEffects);
                e.Handled = true;
            }
            else
            {
                var data = e.Data.GetData(Constants.DATA_FORMAT) as XElement;
                if (Utilities.CheckValidDrop(data, Constants.SCRIPT))
                {
                    e.Effects = (DragDropEffects.Move & e.AllowedEffects);
                    e.Handled = true;
                }
            }
            base.OnDragOver(e);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            e.Handled = true;
            var canvasActivity = ModelItem;
            var data = e.Data.GetData(Constants.DATA_FORMAT) as XElement;
            if (Utilities.CheckValidDrop(data, Constants.SCRIPT))
            {
                var activity = (AutomationActivity) Utilities.GetActivityFromXElement(data);
                if (activity != null)
                {
                    canvasActivity.Properties["children"].Collection.Add(activity);
                }
            }

            //var droppedItems = DragDropHelper.GetDroppedObjects(this, e, Context);
            //var droppedItem = DragDropHelper.GetDroppedObject(this, e, Context);
            //foreach (var droppedItem in droppedItems)
            //{
            //    canvasActivity.Properties["children"].Collection.Add(droppedItem);
            //}
            //DragDropHelper.SetDragDropCompletedEffects(e, DragDropEffects.Move);
            base.OnDrop(e);
        }
    }
}