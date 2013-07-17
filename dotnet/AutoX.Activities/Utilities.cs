#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.XamlIntegration;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AutoX.Activities.AutoActivities;
using AutoX.Basic;

#endregion

#endregion

namespace AutoX.Activities
{
    public enum OnError
    {
        [Description("Always Return Success, Ignore All Errors")] AlwaysReturnTrue,

        [Description("Only Show Warning in Result, Even Error")] JustShowWarning,

        [Description("Mark Error in Result, but Continue Next Step")] Continue,

        [Description("Stop Current Script, Mark it Error")] StopCurrentScript,

        [Description("Terminate this Test Instance")] Terminate
    }

    public static class Utilities
    {
        public const string Filter = "Name;Type;Description;Type;_id;";
        public const string ReservedList = "_id|_type|_parentId|Created|Updated|";

        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[]) fi.GetCustomAttributes(
                    typeof (DescriptionAttribute),
                    false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static void DropXElementToDesigner(XElement data, string dropData, ModelItem navtiveModelItem)
        {
            var guid = data.GetAttributeValue(Constants._ID);
            var tag = data.Name.ToString();
            var modelProperty = navtiveModelItem.Properties[dropData];
            if (modelProperty == null) return;
            if (modelProperty.Value == null) return;
            var userData = modelProperty.Value.GetCurrentValue() as string ?? "";

            if (tag.Equals(Constants.DATUM))
            {
                if (userData.Contains(guid))
                {
                    userData = userData.Replace(guid, "");
                }
                userData += guid + ";";
            }
            if (tag.Equals(Constants.UI_OBJECT))
            {
                var xSteps = XElement.Parse(userData);

                var xStep = new XElement(Constants.STEP);
                var name = data.GetAttributeValue(Constants.NAME);
                xStep.SetAttributeValue(Constants._ID, Guid.NewGuid().ToString());
                xStep.SetAttributeValue(Constants.UI_ID, data.GetAttributeValue(Constants._ID));
                xStep.SetAttributeValue(Constants.UI_OBJECT, name);
                xStep.SetAttributeValue(Constants.ENABLE, "False");
                xStep.SetAttributeValue(Constants.DATA, "");
                xStep.SetAttributeValue(Constants.DEFAULT_DATA, "");
                xStep.SetAttributeValue(Constants.ACTION, "");
                xSteps.Add(xStep);
                userData = xSteps.ToString();
                AddVariable(navtiveModelItem, name);
            }
            modelProperty.SetValue(userData);
        }

        public static void AddVariable(ModelItem navtiveModelItem, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var variablesProperty = navtiveModelItem.Properties["Variables"];
                if (variablesProperty != null)
                {
                    var existed = false;
                    foreach (ModelItem v in variablesProperty.Collection)
                    {
                        if (v.Properties["Name"].Value.ToString().Equals(name))
                        {
                            existed = true;
                            break;
                        }
                    }
                    if (!existed)
                    {
                        var variable = new Variable<string>(name);
                        variablesProperty.Collection.Add(variable);
                    }
                }
            }
        }

        public static bool CheckValidDrop(XElement data, params string[] types)
        {
            if (data == null)
                return false;
            var type = data.Name.ToString();
            return !String.IsNullOrEmpty(type) && types.Any(s => s.Equals(type));
        }

        public static Activity GetActivityFromContentString(string content)
        {
            return ActivityXamlServices.Load(new StringReader(content));
        }

        public static Activity GetActivityFromXElement(XElement data)
        {
            var scriptType = data.GetAttributeValue(Constants.SCRIPT_TYPE);
            if (!String.IsNullOrEmpty(scriptType))
            {
                var host = HostManager.GetInstance().GetHost();
                if (scriptType.Equals("TestCase"))
                {
                    var activity = new CallTestCaseActivity
                    {
                        TestCaseId = data.GetAttributeValue(Constants._ID),
                        TestCaseName = data.GetAttributeValue(Constants.NAME),
                        DisplayName = "Call Test Case: " + data.GetAttributeValue(Constants.NAME)
                    };

                    activity.SetHost(host);
                    return activity;
                }
                if (scriptType.Equals("TestScreen"))
                {
                    var activity = new CallTestScreenActivity
                    {
                        TestSreenId = data.GetAttributeValue(Constants._ID),
                        TestSreenName = data.GetAttributeValue(Constants.NAME),
                        DisplayName = "Call Test Screen: " + data.GetAttributeValue(Constants.NAME),
                        Steps = XElement.Parse(data.GetAttributeValue(Constants.CONTENT)).GetAttributeValue("Steps")
                    };
                    activity.SetHost(host);
                    return activity;
                }
                if (scriptType.Equals("TestSuite"))
                {
                    var activity = new CallTestSuiteActivity
                    {
                        TestSuiteId = data.GetAttributeValue(Constants._ID),
                        TestSuiteName = data.GetAttributeValue(Constants.NAME),
                        TestSuiteDescription = data.GetAttributeValue("Description"),
                        DisplayName = "Call Test Suite: " + data.GetAttributeValue(Constants.NAME)
                    };
                    activity.SetHost(host);
                    return activity;
                }
            }
            return null;
        }

        public static List<UserData> GetUserData(string rawData, IHost host)
        {
            var dic = GetRawUserData(rawData, host);
            return dic.Values.ToList();
        }

        public static Dictionary<string, UserData> GetRawUserData(string rawData, IHost host)
        {
            var dic = new Dictionary<string, UserData>();
            if (!String.IsNullOrEmpty(rawData))
            {
                var dataStrings = rawData.Split(';');
                foreach (string dataString in dataStrings)
                {
                    if (String.IsNullOrEmpty(dataString))
                        continue;
                    var sData = host.GetDataObject(dataString);

                    //sometimes, some data has been deleted.
                    if (sData == null)
                        continue;
                    var dataSetName = sData.GetAttributeValue(Constants.NAME);

                    foreach (XAttribute xAttribute in sData.Attributes())
                    {
                        var name = xAttribute.Name.ToString();
                        if (Filter.Contains(name)) continue;
                        var dataValue = xAttribute.Value;
                        var data = new UserData
                        {
                            DataSet = dataSetName,
                            Name = name,
                            Value = dataValue,
                            DataSetId = dataString
                        };

                        //remove the duplicate value
                        if (dic.ContainsKey(name))
                            dic[name] = data;
                        else
                        {
                            dic.Add(name, data);
                        }
                    }
                }
            }
            return dic;
        }

        public static Dictionary<string, string> GetActualUserData(string rawData, IHost host)
        {
            var dic = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(rawData))
            {
                var dataStrings = rawData.Split(';');
                foreach (string dataString in dataStrings)
                {
                    if (String.IsNullOrEmpty(dataString))
                        continue;
                    var sData = host.GetDataObject(dataString).ToString();
                    if (String.IsNullOrEmpty(sData)) continue;
                    var xData = XElement.Parse(sData);

                    foreach (XAttribute xAttribute in xData.Attributes())
                    {
                        var name = xAttribute.Name.ToString();
                        if (Filter.Contains(name)) continue;
                        var dataValue = xAttribute.Value;
                        var data = dataValue;

                        //remove the duplicate value
                        if (dic.ContainsKey(name))
                            dic[name] = data;
                        else
                        {
                            dic.Add(name, data);
                        }
                    }
                }
            }
            return dic;
        }

        public static void PrintDictionary(Dictionary<string, string> dict)
        {
            var pS = dict.Aggregate("\n",
                (current, variable) => current + (variable.Key + "=" + variable.Value + "\n"));
            Log.Debug(pS);
        }

        public static ArrayList GetStepsList(string textValue, ArrayList possibleAction, IHost host)
        {
            var ret = new ArrayList();
            if (textValue != null)
            {
                var xSteps = XElement.Parse(textValue);
                foreach (XElement element in xSteps.Descendants(Constants.STEP))
                {
                    var uiId = element.GetAttributeValue(Constants.UI_ID);
                    if (String.IsNullOrEmpty(uiId)) continue;

                    var sData = host.GetDataObject(uiId);
                    if (sData == null) continue;

                    //var xData = sData.GetXElementFromDataObject();
                    var uiObject = element.GetAttributeValue(Constants.UI_OBJECT);
                    if (String.IsNullOrEmpty(uiObject)) continue;
                    var enable = Boolean.Parse(element.GetAttributeValue(Constants.ENABLE));
                    var defaultDataValue = element.GetAttributeValue(Constants.DEFAULT_DATA);
                    var dataName = element.GetAttributeValue(Constants.DATA);
                    var stepId = element.GetAttributeValue(Constants._ID);
                    var action = element.GetAttributeValue(Constants.ACTION) ?? "";
                    var step = new Step
                    {
                        _id = stepId,
                        Action = action,
                        UIId = uiId,
                        UIObject = uiObject,
                        Enable = enable,
                        DefaultData = defaultDataValue,
                        Data = dataName,
                        PossibleAction = possibleAction
                    };
                    ret.Add(step);
                }
            }
            return ret;
        }

        public static string PassData(string outerData, string userData, bool ownDataFirst)
        {
            string finalRet;
            if (!ownDataFirst)
                finalRet = userData + ";" + outerData;
            else
            {
                finalRet = outerData + ";" + userData;
            }
            return finalRet;
        }

        public static WorkflowApplication GetWorkflowApplication(AutomationActivity activity)
        {
            var workflowApplication = new WorkflowApplication(activity)
            {
                Completed = delegate(WorkflowApplicationCompletedEventArgs e)
                {
                    switch (e.CompletionState)
                    {
                        case ActivityInstanceState.Faulted:

                            //Logger.GetInstance().Log().Error("workflow " +
                            //                                 scriptGuid +
                            //                                 " stopped! Error Message:\n"
                            //                                 +
                            //                                 e.TerminationException.
                            //                                     GetType().FullName +
                            //                                 "\n"
                            //                                 +
                            //                                 e.TerminationException.
                            //                                     Message);
                            //Status = "Terminated";
                            break;

                        case ActivityInstanceState.Canceled:

                            //Logger.GetInstance().Log().Warn("workflow " + scriptGuid +
                            //                                " Cancel.");
                            //Status = "Canceled";
                            break;

                        default:

                            //Logger.GetInstance().Log().Info("workflow " + scriptGuid +
                            //                                " Completed.");
                            //Status = "Completed";
                            break;
                    }
                },
                Aborted = delegate
                {
                    //Logger.GetInstance().Log().Error("workflow " +
                    //                                 scriptGuid
                    //                                 + " aborted! Error Message:\n"
                    //                                 + e.Reason.GetType().FullName + "\n" +
                    //                                 e.Reason.Message);
                    //Status = "Aborted";
                }
            };
            return workflowApplication;
        }
    }
}