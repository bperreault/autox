using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime.Tracking;
using AutoX.Basic;
using AutoX.WF.Core.Properties;

namespace AutoX.WF.Core
{
    public class LogTrackingChannel : TrackingChannel
    {
        public static Mutex mutex = new Mutex();
        private readonly Dictionary<string, ActivityStatusInfo> list;

        protected LogTrackingChannel()
        {
        }

        public LogTrackingChannel(Dictionary<string, ActivityStatusInfo> l)
        {
            list = l;
        }

        // Send() is called by Tracking runtime to send various tracking records
        protected override void Send(TrackingRecord record)
        {
            // filter on record type
            if (record is WorkflowTrackingRecord)
            {
                WriteWorkflowTrackingRecord((WorkflowTrackingRecord)record);
            }
            if (record is ActivityTrackingRecord)
            {
                WriteActivityTrackingRecord((ActivityTrackingRecord)record);
                var ar = (ActivityTrackingRecord)record;
                mutex.WaitOne();


                if (list.ContainsKey(ar.QualifiedName))
                {
                    var asi = new ActivityStatusInfo(ar.QualifiedName, WorkflowMonitorDesignerGlyphProvider.EXECUTING);
                    list.TryGetValue(ar.QualifiedName, out asi);
                    if (!asi.Status.Equals(WorkflowMonitorDesignerGlyphProvider.FAILED) &&
                        !asi.Status.Equals(WorkflowMonitorDesignerGlyphProvider.WARNING))
                    {
                        list.Remove(ar.QualifiedName);
                        list.Add(ar.QualifiedName,
                                 new ActivityStatusInfo(ar.QualifiedName, ar.ExecutionStatus.ToString()));
                    }
                }
                else
                {
                    list.Add(ar.QualifiedName,
                             new ActivityStatusInfo(ar.QualifiedName, ar.ExecutionStatus.ToString()));
                }

                mutex.ReleaseMutex();
            }
            if (record is UserTrackingRecord)
            {
                WriteUserTrackingRecord((UserTrackingRecord)record);
                var r = (UserTrackingRecord)record;
                mutex.WaitOne();
                if (list.ContainsKey(r.QualifiedName))
                    list.Remove(r.QualifiedName);
                list.Add(r.QualifiedName, new ActivityStatusInfo(r.QualifiedName, r.UserData.ToString()));
                mutex.ReleaseMutex();
            }
        }

        // InstanceCompletedOrTerminated() is called by Tracking runtime to indicate that the Workflow instance finished running
        protected override void InstanceCompletedOrTerminated()
        {
            WriteTitle("Workflow Instance Completed or Terminated");
        }

        private static void WriteTitle(string title)
        {
            Log.Info("\n***********************************************************");
            Log.Info("\t" + title);
            Log.Info("***********************************************************");
        }

        private static void WriteWorkflowTrackingRecord(WorkflowTrackingRecord workflowTrackingRecord)
        {
            WriteTitle("Workflow Tracking Record");
            Log.Info("EventDateTime: " + workflowTrackingRecord.EventDateTime);

            Log.Info("Status: " + workflowTrackingRecord.TrackingWorkflowEvent);
        }

        private static void WriteActivityTrackingRecord(ActivityTrackingRecord activityTrackingRecord)
        {
            WriteTitle("Activity Tracking Record");
            Log.Info("EventDateTime: " + activityTrackingRecord.EventDateTime);
            Log.Info("QualifiedName: " + activityTrackingRecord.QualifiedName);
            Log.Info("Type: " + activityTrackingRecord.ActivityType);
            Log.Info("Status: " + activityTrackingRecord.ExecutionStatus);
        }

        private static void WriteUserTrackingRecord(UserTrackingRecord userTrackingRecord)
        {
            WriteTitle("User Activity Record");
            Log.Info("EventDateTime: " + userTrackingRecord.EventDateTime);
            Log.Info("QualifiedName: " + userTrackingRecord.QualifiedName);
            Log.Info("ActivityType: " + userTrackingRecord.ActivityType.FullName);
            Log.Info("Args: " + userTrackingRecord.UserData);
        }
    }

    public class LogTrackingService : TrackingService
    {
        private readonly Dictionary<string, ActivityStatusInfo> l;

        public LogTrackingService(IServiceProvider designSurface)
        {
            Dictionary<string, ActivityStatusInfo> activityStatusList = setDesignSurface(designSurface);
            l = activityStatusList;
        }

        public LogTrackingService(Dictionary<string, ActivityStatusInfo> list)
        {
            l = list;
        }

        public static Dictionary<string, ActivityStatusInfo> setDesignSurface(IServiceProvider designSurface)
        {
            var activityStatusList = new Dictionary<string, ActivityStatusInfo>();
            activityStatusList.Clear();
            var glyphService =
                designSurface.GetService(typeof(IDesignerGlyphProviderService)) as IDesignerGlyphProviderService;
            var glyphProvider = new WorkflowMonitorDesignerGlyphProvider(activityStatusList);
            if (glyphService != null)
                glyphService.AddGlyphProvider(glyphProvider);
            return activityStatusList;
        }

        protected override bool TryGetProfile(Type workflowType, out System.Workflow.Runtime.Tracking.TrackingProfile profile)
        {
            //Depending on the workflowType, service can return different tracking profiles
            //In this Class we're returning the same profile for all running types
            profile = GetProfile();
            return true;
        }

        protected override System.Workflow.Runtime.Tracking.TrackingProfile GetProfile(Guid workflowInstanceId)
        {
            // Does not support reloading/instance profiles
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        protected override System.Workflow.Runtime.Tracking.TrackingProfile GetProfile(Type workflowType, Version profileVersionId)
        {
            // Return the version of the tracking profile that runtime requests (profileVersionId)
            return GetProfile();
        }

        protected override bool TryReloadProfile(Type workflowType, Guid workflowInstanceId, out TrackingProfile profile)
        {
            // Returning false to indicate there is no new profiles
            profile = null;
            return false;
        }

        protected override TrackingChannel GetTrackingChannel(TrackingParameters parameters)
        {
            //return a tracking channel to receive runtime events
            return new LogTrackingChannel(l);
        }

        // Profile creation
        private static System.Workflow.Runtime.Tracking.TrackingProfile GetProfile()
        {
            // Create a Tracking Profile
            var profile = new System.Workflow.Runtime.Tracking.TrackingProfile();
            

            // Add a TrackPoint to cover all activity status events
            var activityTrackPoint = new ActivityTrackPoint();
            var activityLocation = new ActivityTrackingLocation(typeof(System.Activities.Activity));
            activityLocation.MatchDerivedTypes = true;

            var statuses = Enum.GetValues(typeof(ActivityExecutionStatus)) as IEnumerable<ActivityExecutionStatus>;
            foreach (ActivityExecutionStatus status in statuses)
            {
                activityLocation.ExecutionStatusEvents.Add(status);
            }

            activityTrackPoint.MatchingLocations.Add(activityLocation);
            profile.ActivityTrackPoints.Add(activityTrackPoint);

            // Add a TrackPoint to cover all workflow status events
            var workflowTrackPoint = new WorkflowTrackPoint();
            workflowTrackPoint.MatchingLocation = new WorkflowTrackingLocation();
            foreach (TrackingWorkflowEvent workflowEvent in Enum.GetValues(typeof(TrackingWorkflowEvent)))
            {
                workflowTrackPoint.MatchingLocation.Events.Add(workflowEvent);
            }
            profile.WorkflowTrackPoints.Add(workflowTrackPoint);

            // Add a TrackPoint to cover all user track points
            var userTrackPoint = new UserTrackPoint();
            var userLocation = new UserTrackingLocation();
            userLocation.ActivityType = typeof(System.Activities.Activity);
            userLocation.MatchDerivedActivityTypes = true;
            userLocation.ArgumentType = typeof(object);
            userLocation.MatchDerivedArgumentTypes = true;
            userTrackPoint.MatchingLocations.Add(userLocation);
            profile.UserTrackPoints.Add(userTrackPoint);

            return profile;
        }
    }

    public class ActivityStatusInfo
    {
        private readonly string NameValue;
        private readonly string StatusValue;

        public ActivityStatusInfo(string name, string status)
        {
            NameValue = name;
            StatusValue = status;
        }

        public string Name
        {
            get { return NameValue; }
        }

        public string Status
        {
            get { return StatusValue; }
        }
    }

    public class WorkflowMonitorDesignerGlyphProvider : IDesignerGlyphProvider
    {
        public const string COMPLETE = "Complete";
        public const string EXECUTING = "Executing";
        public const string FAILED = "Failed";
        public const string FAULTING = "Faulting";
        public const string WARNING = "Warning";
        private readonly Dictionary<string, ActivityStatusInfo> activityStatusList;

        internal WorkflowMonitorDesignerGlyphProvider(Dictionary<string, ActivityStatusInfo> activityStatusList)
        {
            this.activityStatusList = activityStatusList;
        }

        #region IDesignerGlyphProvider Members

        public ActivityDesignerGlyphCollection GetGlyphs(ActivityDesigner activityDesigner)
        {
            var glyphList = new ActivityDesignerGlyphCollection();
            LogTrackingChannel.mutex.WaitOne();

            foreach (ActivityStatusInfo activityStatus in activityStatusList.Values)
                if (activityStatus.Name == activityDesigner.Activity.QualifiedName)
                {
                    switch (activityStatus.Status)
                    {
                        case COMPLETE:
                            glyphList.Add(new CompletedGlyph());
                            break;
                        case EXECUTING:
                            glyphList.Add(new ExecutingGlyph());
                            break;
                        case FAILED:
                            glyphList.Add(new FailedGlyph());
                            break;
                        case FAULTING:
                            glyphList.Add(new FailedGlyph());
                            break;
                        case WARNING:
                            glyphList.Add(new WarningGlyph());
                            break;

                        default: //default set it to complete
                            glyphList.Add(new CompletedGlyph());
                            break;
                    }
                }

            LogTrackingChannel.mutex.ReleaseMutex();
            return glyphList;
        }

        #endregion
    }

    internal class CustomizedGlyphBase : DesignerGlyph
    {
        private readonly Image image;

        internal CustomizedGlyphBase(Image customImage)
        {
            image = customImage;
        }

        public override Rectangle GetBounds(ActivityDesigner designer, bool activated)
        {
            Rectangle imageBounds = Rectangle.Empty;
            if (image != null)
            {
                var glyphSize = new Size(16, 16);
                if (WorkflowTheme.CurrentTheme.AmbientTheme != null)
                    glyphSize = WorkflowTheme.CurrentTheme.AmbientTheme.GlyphSize;
                imageBounds.Location = new Point(designer.Bounds.Right - glyphSize.Width / 2,
                                                 designer.Bounds.Top - glyphSize.Height / 2);
                imageBounds.Size = glyphSize;
            }
            return imageBounds;
        }

        protected override void OnPaint(Graphics graphics, bool activated, AmbientTheme ambientTheme,
                                        ActivityDesigner designer)
        {
            var bitmap = (Bitmap)image;
            bitmap.MakeTransparent(Color.FromArgb(0, 255, 255));
            graphics.DrawImage(bitmap, GetBounds(designer, activated), new Rectangle(Point.Empty, bitmap.Size),
                               GraphicsUnit.Pixel);
        }
    }

    internal sealed class ExecutingGlyph : CustomizedGlyphBase
    {
        internal ExecutingGlyph()
            : base(Resources.Executing)
        {
        }
    }

    internal sealed class CompletedGlyph : CustomizedGlyphBase
    {
        internal CompletedGlyph()
            : base(Resources.Complete)
        {
        }
    }

    internal sealed class WarningGlyph : CustomizedGlyphBase
    {
        internal WarningGlyph()
            : base(Resources.Warning)
        {
        }
    }

    internal sealed class FailedGlyph : CustomizedGlyphBase
    {
        internal FailedGlyph()
            : base(Resources.Failed)
        {
        }
    }
}
