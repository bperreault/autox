using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoX.WF.Core
{
    public class StatusTracker : TrackingParticipant
    {
        private readonly TrackingProfile trackingProfile = new TrackingProfile();

        public StatusTracker()
        {
            trackingProfile.Queries.Add(new ActivityStateQuery
            {
                ActivityName = "*",
                States = { "*" },
                Variables = { "*" },
                Arguments = { "*" }
            });
            trackingProfile.Queries.Add(new WorkflowInstanceQuery
            {
                States = { "*" },
            });

            TrackingProfile = trackingProfile;
        }

        public string Status { get; private set; }

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            var instanceRecord = record as WorkflowInstanceRecord;
            if (instanceRecord != null)
            {
                Status = instanceRecord.State;
            }
        }
    }
}
