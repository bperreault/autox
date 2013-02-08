// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using AutoX.Basic;
using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Windows;
using System.Xml.Linq;

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

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (DragDropHelper.AllowDrop(
                e.Data,
                Context,
                typeof(Activity)))
            {
                e.Effects = (DragDropEffects.Move & e.AllowedEffects);
                e.Handled = true;
            }
            else
            {
                var data = e.Data.GetData(Constants.DATA_FORMAT) as XElement;
                if (Utilities.CheckValidDrop(data, Constants.SCRIPT))
                {
                    var activity = Utilities.GetActivityFromXElement(data);
                    if (activity != null)
                    {
                        var mi = Context.Services.GetService<ModelTreeManager>().CreateModelItem(ModelItem,
                                                                                                 activity);
                        var dO = new DataObject(DragDropHelper.ModelItemDataFormat, mi);
                        Utilities.AddVariable(mi, data.GetAttributeValue(Constants.NAME).Replace(" ", "_"));
                        try
                        {
                            DragDrop.DoDragDrop(this, dO, DragDropEffects.Move);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    e.Effects = (DragDropEffects.Move & e.AllowedEffects);
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
                typeof(Activity)))
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

            //TODO not a graceful implementation, think about change it.
            var canvasActivity = ModelItem;
            var droppedItems =DragDropHelper.GetDroppedObjects(this, e, Context);
            //var droppedItem = DragDropHelper.GetDroppedObject(this, e, Context);
            foreach (var droppedItem in droppedItems)
            {
                canvasActivity.Properties["children"].Collection.Add(droppedItem);
            }
           
            
            
            //DragDropHelper.SetDragDropCompletedEffects(e, DragDropEffects.Move);

            base.OnDrop(e);
        }
    }
}