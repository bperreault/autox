namespace AutoX.Activities.AutoActivities
{
    internal interface IPassData
    {
        void PassData(string instanceId, string outerData);

        bool GetResult();
    }
}