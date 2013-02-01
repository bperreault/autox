// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Activities.Presentation;
using System.Windows;
using System.Xml.Linq;

#endregion

namespace AutoX.Activities
{
    // Interaction logic for TestScreenDesigner.xaml
    public partial class TestScreenDesigner
    {
        public TestScreenDesigner()
        {
            InitializeComponent();
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            var data = e.Data.GetData("DataFormat") as XElement;
            if (Utilities.CheckValidDrop(data, "UIObject", "Datum"))
            {
                e.Effects = (DragDropEffects.Move & e.AllowedEffects);
                e.Handled = true;
            }
            base.OnDragEnter(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            var data = e.Data.GetData("DataFormat") as XElement;
            if (Utilities.CheckValidDrop(data, "UIObject"))
            {
                e.Effects = (DragDropEffects.Move & e.AllowedEffects);
                e.Handled = true;
            }
            base.OnDragOver(e);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            e.Handled = true;
            var data = e.Data.GetData("DataFormat") as XElement;
            if (data != null)
            {
                var tag = data.Name.ToString();
                if (tag.Equals("Datum"))
                    Utilities.DropXElementToDesigner(data, "UserData", ModelItem);
                if (tag.Equals("UIObject"))
                    Utilities.DropXElementToDesigner(data, "Steps", ModelItem);
                DragDropHelper.SetDragDropCompletedEffects(e, DragDropEffects.Move);
            }
            base.OnDrop(e);
        }
    }
}