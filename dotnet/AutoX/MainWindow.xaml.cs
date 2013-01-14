// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.Comm;
using IDataObject = AutoX.Basic.Model.IDataObject;

#endregion

namespace AutoX
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IHost
    {
        public MainWindow()
        {
            InitializeComponent();
//            HostManager.GetInstance().Register(this);
            StartProgressBar();
            //set datagrid itemssource
            ClientTable.ItemsSource = _clientSource.Get();
            InstanceTable.ItemsSource = _instanceSource.Get();
//            TestCaseResultTable.ItemsSource = _testCaseResultSource.Get();
//            TestStepsResultTable.ItemsSource = _testStepSource.Get();
//            TranslationTable.ItemsSource = _translationSource.Get();

            LoadToolBox();
            RegisterMetadata();
            //AddTestDesigner("TestSuite");
//            InitScreen();
            //InitializeProject();
            //LoadProject();

            StopProgressBar();
        }

        #region IHost Members

        public void SetCommand( XElement steps)
        {
            throw new NotImplementedException("This method should never be called.");
        }

        public XElement GetResult( string guid)
        {
            throw new NotImplementedException("This method should never be called.");
        }

        #endregion

        public void InitScreen()
        {
            //load the project tree
            ReloadOnProjectTree(null, null);
            //load data tree
            ReloadOnDataTree(null, null);
            //load ui pool
            ReloadOnUITree(null, null);
            //load result tree
            ReloadOnResultTree(null, null);
            //load suite tree
            ReloadOnSuiteTree(null, null);
            //load computer table
            RefreshClientTable(null, null);
        }

        #region Progress bar

        private void StartProgressBar()
        {
            Progressing.IsIndeterminate = true;
            Progressing.Visibility = Visibility.Visible;
            var duration = new Duration(TimeSpan.FromSeconds(1));
            var doubleanimation = new DoubleAnimation(10.0, duration);
            Dispatcher.BeginInvoke(new Action(() => Progressing.BeginAnimation(RangeBase.ValueProperty, doubleanimation)));
        }

        private void StopProgressBar()
        {
            Dispatcher.BeginInvoke(new Action(() => Progressing.BeginAnimation(RangeBase.ValueProperty, null)));
            Progressing.Visibility = Visibility.Collapsed;
        }

        #endregion Progress bar
    }
}