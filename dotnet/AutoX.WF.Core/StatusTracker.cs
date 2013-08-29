using System;
using System.Activities.Tracking;

namespace AutoX.WF.Core
{
    public sealed class StatusTracker : TrackingParticipant
    {
        private readonly TrackingProfile _trackingProfile = new TrackingProfile();

        public StatusTracker()
        {
            _trackingProfile.Queries.Add(new ActivityStateQuery
            {
                ActivityName = "*",
                States = { "*" },
                Variables = { "*" },
                Arguments = { "*" }
            });
            _trackingProfile.Queries.Add(new WorkflowInstanceQuery
            {
                States = { "*" },
            });

            TrackingProfile = _trackingProfile;
        }

        public string Status { get; private set; }

        public ITracker Tracking { get; set; }

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            if (Tracking != null)
                Tracking.Update(record);
            var instanceRecord = record as WorkflowInstanceRecord;
            if (instanceRecord != null)
            {
                Status = instanceRecord.State;
            }

        }
    }

    public interface ITracker
    {
        void Update(TrackingRecord record);
    }
}
