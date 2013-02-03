// Hapa Project, CC
// Created @2012 09 18 15:18
// Last Updated  by Huang, Jien @2012 09 18 15:18

namespace AutoX.Activities.AutoActivities
{
    internal interface IPassData
    {
        void PassData(string instanceId, string outerData);

        bool GetResult();
    }
}