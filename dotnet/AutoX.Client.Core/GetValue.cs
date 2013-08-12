using System;
using System.Xml.Linq;

namespace AutoX.Client.Core
{
    internal class GetValue : AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            if (UIObject == null || UIObject.Count == 0)
            {
                sr.Error("Expected UI Object is not found!");
            }
            else
            {
                var target = UIObject[0];
                //Data should look like text=>variable
                if (!string.IsNullOrEmpty(Data))
                {
                    sr.Error("Please define variable and attribute.");
                    return sr.GetResult();
                }
                if (!Data.Contains("=>"))
                {
                    sr.Error("The correct format for this action is 'attribute=>variable', e.g.: value=>currentValue");
                    return sr.GetResult();
                }
                int pos = Data.IndexOf("=>", System.StringComparison.Ordinal);
                try
                {
                    var attr = Data.Substring(0, pos);
                    var variable = Data.Substring(pos + 2);
                    var value = target.GetAttribute(attr);
                    sr.GetResult().SetAttributeValue(variable,value);
                }
                catch (Exception ex)
                {
                    sr.Error("GetValue Failed:"+ex.Message);
                }
                
            }
            return sr.GetResult();
        }
    }
}
