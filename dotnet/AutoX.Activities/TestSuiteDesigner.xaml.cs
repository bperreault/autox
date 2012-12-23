// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

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
                typeof (Activity)))
            {
                e.Effects = (DragDropEffects.Move & e.AllowedEffects);
                e.Handled = true;
            }
            else
            {
                var data = e.Data.GetData("DataFormat") as XElement;
                if (Utilities.CheckValidDrop(data, "Script"))
                {
                    Activity activity = Utilities.GetActivityFromXElement(data);
                    if (activity != null)
                    {
                        ModelItem mi = Context.Services.GetService<ModelTreeManager>().CreateModelItem(ModelItem,
                                                                                                       activity);
                        var dO = new DataObject(DragDropHelper.ModelItemDataFormat, mi);
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
                typeof (Activity)))
            {
                e.Effects = (DragDropEffects.Move & e.AllowedEffects);
                e.Handled = true;
            }
            else
            {
                var data = e.Data.GetData("DataFormat") as XElement;
                if (Utilities.CheckValidDrop(data, "Script"))
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
            object droppedItem = DragDropHelper.GetDroppedObject(this, e, Context);
            ModelItem canvasActivity = ModelItem;
            canvasActivity.Properties["children"].Collection.Add(droppedItem);

            DragDropHelper.SetDragDropCompletedEffects(e, DragDropEffects.Move);

            base.OnDrop(e);
        }
    }
}