#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;

#endregion

#endregion

namespace AutoX.Activities
{
    internal class UserDataEditor : DialogPropertyValueEditor
    {
        public UserDataEditor()
        {
            InlineEditorTemplate = new DataTemplate();

            var stack = new FrameworkElementFactory(typeof (StackPanel));
            stack.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            var editModeSwitch = new FrameworkElementFactory(typeof (EditModeSwitchButton));
            editModeSwitch.SetValue(EditModeSwitchButton.TargetEditModeProperty, PropertyContainerEditMode.Dialog);
            stack.AppendChild(editModeSwitch);
            InlineEditorTemplate.VisualTree = stack;
        }

        public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
        {
            var userDataDialog = new UserDataDialog();
            userDataDialog.Set(propertyValue.Value.ToString());
            userDataDialog.ShowDialog();
            if (userDataDialog.DialogResult == true)
            {
                propertyValue.Value = userDataDialog.Get();
            }
        }
    }
}