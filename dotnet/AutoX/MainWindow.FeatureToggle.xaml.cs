#region

using System.ComponentModel;
using AutoX.FeatureToggles;

#endregion

namespace AutoX
{
    public partial class MainWindow
    {
        private EncryptFeature _encryptFeature;
        private SaucelabFeature _saucelabFeature;
        private TranslationFeature _translationFeature;
        private AmazonEC2Feature _amazonEc2Feature;

        public AmazonEC2Feature AmazonEC2Feature
        {
            get { return _amazonEc2Feature; }
            set
            {
                _amazonEc2Feature = value;
                Notify("AmazonEC2Feature");
            }
        }

        public EncryptFeature EncryptFeature
        {
            get { return _encryptFeature; }
            set
            {
                _encryptFeature = value;
                Notify("EncryptFeature");
            }
        }

        public TranslationFeature TranslationFeature
        {
            get { return _translationFeature; }
            set
            {
                _translationFeature = value;
                Notify("TranslationFeature");
            }
        }

        public SaucelabFeature SaucelabFeature
        {
            get { return _saucelabFeature; }
            set
            {
                _saucelabFeature = value;
                Notify("SaucelabFeature");
            }
        }

        public void InitFeatureToggle()
        {
            TranslationFeature = new TranslationFeature();
            SaucelabFeature = new SaucelabFeature();
            EncryptFeature = new EncryptFeature();
            AmazonEC2Feature = new AmazonEC2Feature();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}