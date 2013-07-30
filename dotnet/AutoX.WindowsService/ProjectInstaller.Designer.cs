namespace AutoX.WindowsService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AutoXServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.AutoXServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // AutoXServiceProcessInstaller
            // 
            this.AutoXServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.AutoXServiceProcessInstaller.Password = null;
            this.AutoXServiceProcessInstaller.Username = null;
            // 
            // AutoXServiceInstaller
            // 
            this.AutoXServiceInstaller.DelayedAutoStart = true;
            this.AutoXServiceInstaller.Description = "Host a mini web server and automation instances.";
            this.AutoXServiceInstaller.DisplayName = "AutoX Windows Service";
            this.AutoXServiceInstaller.ServiceName = "AutoX";
            this.AutoXServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.AutoXServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.AutoXServiceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.AutoXServiceProcessInstaller,
            this.AutoXServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller AutoXServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller AutoXServiceInstaller;
    }
}