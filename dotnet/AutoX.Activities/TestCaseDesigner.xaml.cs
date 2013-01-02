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
    // Interaction logic for TestCaseDesigner.xaml
    public partial class TestCaseDesigner
    {
        public TestCaseDesigner()
        {
            InitializeComponent();
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            DragEnterTestCase(null, e);

            base.OnDragEnter(e);
        }

        private void DragEnterTestCase(object sender, DragEventArgs e)
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
                if (Utilities.CheckValidDrop(data, "Script", "Datum"))
                {
                    var activity = Utilities.GetActivityFromXElement(data);
                    if (activity != null)
                    {
                        var mi = Context.Services.GetService<ModelTreeManager>().CreateModelItem(ModelItem,
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
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            DragOverTestCase(null, e);
            base.OnDragOver(e);
        }

        private void DropOverTestCase(object sender, DragEventArgs e)
        {
            e.Handled = true;
            var data = e.Data.GetData("DataFormat") as XElement;
            if (data != null)
            {
                Utilities.DropXElementToDesigner(data, "UserData", ModelItem);
                DragDropHelper.SetDragDropCompletedEffects(e, DragDropEffects.Move);
            }
            else
            {
                //TODO not a graceful implementation, think about change it.
                var droppedItem = DragDropHelper.GetDroppedObject(this, e, Context);
                var canvasActivity = ModelItem;
                canvasActivity.Properties["children"].Collection.Add(droppedItem);

                DragDropHelper.SetDragDropCompletedEffects(e, DragDropEffects.Move);
            }
        }


        protected override void OnDrop(DragEventArgs e)
        {
            DropOverTestCase(null, e);
            base.OnDrop(e);
        }

        private void DragOverTestCase(object sender, DragEventArgs e)
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
                if (Utilities.CheckValidDrop(data, "Script", "Datum"))
                {
                    e.Effects = (DragDropEffects.Move & e.AllowedEffects);
                    e.Handled = true;
                }
            }
        }
    }
}