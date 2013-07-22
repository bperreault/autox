#region

using System.Xml.Linq;

#endregion

namespace AutoX.Client.Core
{
    internal class Enter : AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            if (UIObject.Count == 0)
            {
                sr.Error("Expected UI Object is not found!");
            }
            else
            {
                var text = UIObject[0];
                text.Clear();
                if (!string.IsNullOrEmpty(Data))
                {
                    text.SendKeys(Data);
                }
            }
            return sr.GetResult();
        }
    }
}