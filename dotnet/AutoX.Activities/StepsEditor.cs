// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Activities.Presentation.Converters;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace AutoX.Activities
{
    internal class StepsEditor : DialogPropertyValueEditor
    {
        public StepsEditor()
        {
            InlineEditorTemplate = new DataTemplate();

            var stack = new FrameworkElementFactory(typeof (StackPanel));
            stack.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            //var textbox = new FrameworkElementFactory(typeof (TextBox));
            //var textBinding = new Binding("Value");
            //textbox.SetValue(TextBox.TextProperty, textBinding);
            //textbox.SetValue(FrameworkElement.MaxWidthProperty, 64.0);
            //textbox.SetValue(TextBox.TextWrappingProperty,TextWrapping.NoWrap);
            //textbox.SetValue(TextBoxBase.IsReadOnlyProperty,true);
            //textbox.SetValue(TextBoxBase.AcceptsReturnProperty,false);
            //textbox.SetValue(TextBoxBase.VerticalScrollBarVisibilityProperty,false);
            //stack.AppendChild(textbox);

            var editModeSwitch = new FrameworkElementFactory(typeof (EditModeSwitchButton));

            editModeSwitch.SetValue(EditModeSwitchButton.TargetEditModeProperty, PropertyContainerEditMode.Dialog);

            stack.AppendChild(editModeSwitch);

            InlineEditorTemplate.VisualTree = stack;
        }

        public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
        {
            var converter = new ModelPropertyEntryToOwnerActivityConverter();
            var modelItem = (ModelItem) converter.Convert(propertyValue.ParentProperty, typeof (ModelItem), false, null);
            if (modelItem == null) return;
            //var property = modelItem.Properties["UserData"];
            //if (property == null) return;
            //var userData = "";
            //if (property.Value != null) userData = property.Value.ToString();
            var stepId = modelItem.Properties["TestSreenId"];
            
            var stepsDialog = new StepsDialog();
            if (stepId != null)
                stepsDialog.Set(stepId.Value.ToString());
            stepsDialog.Set(propertyValue.Value.ToString(), "");
            //if (!string.IsNullOrEmpty(steps))
            //    stepsDialog.Set(steps);
            stepsDialog.ShowDialog();

            if (stepsDialog.DialogResult == true)
            {
                propertyValue.Value = stepsDialog.Get();
            }
        }
    }
}