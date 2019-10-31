/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.    

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using PCLStorage;
using MobileGUCTechTool.Classes;
using System.Data;

namespace MobileGUCTechTool
{
    public partial class MainPage : TabbedPage
    {
        public DataSets.Classroom.ControllerDBDataTable _Controllers = new DataSets.Classroom.ControllerDBDataTable();
        public DataSets.Classroom.ProjectorDBDataTable _Projectors = new DataSets.Classroom.ProjectorDBDataTable();        

        public Pages.AliveTest.AliveTestFrame aliveTestFrame;
        public Pages.Controls.ControlsFrame controlFrame;
        public Pages.EditDB.EditDBFrame dbFrame;

        public NavigationPage aliveTestNavPage;        
	    public NavigationPage controlNavPage;
        public NavigationPage editDBNavPage;

        private IFolder folder;

        public UDPServer udpServer = new UDPServer();

        public MainPage()
        {
            InitializeComponent();
            InitializeTabs();
            udpServer.OnReceivedMessage += UdpServer_ReceivedMessage;
            udpServer.StartServer();
            Task.Run(() =>
            {
                AssembleDBs();
            });
        }

        private void InitializeTabs()
        {
            aliveTestFrame = new Pages.AliveTest.AliveTestFrame();
            controlFrame = new Pages.Controls.ControlsFrame();
            dbFrame = new Pages.EditDB.EditDBFrame();

            aliveTestNavPage = new NavigationPage(aliveTestFrame);
            aliveTestNavPage.Title = "Alive test";
            //aliveTestNavPage.SetHasNavigationBar(aliveTestNavPage, false);
            aliveTestNavPage.SetValue(NavigationPage.HasNavigationBarProperty, false);
            controlNavPage = new NavigationPage(controlFrame);
            controlNavPage.Title = "Classroom Controls";
            controlNavPage.SetValue(NavigationPage.HasNavigationBarProperty, false);
            editDBNavPage = new NavigationPage(dbFrame);
            editDBNavPage.Title = "Edit Database";
            editDBNavPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            Children.Add(aliveTestNavPage);
            Children.Add(controlNavPage);
            Children.Add(editDBNavPage);
        }

        private async void AssembleDBs()
        {
            await Task.Run(() =>
            {
                CreateDBDir();
            });
        }

        private async void CreateDBDir()    // creates the data directory for the app
        {
            /// Test code here
            /// Deletes data directory and then creates it again
            /// 
            
            folder = FileSystem.Current.LocalStorage;
            ExistenceCheckResult folderexist = await folder.CheckExistsAsync("data");
            // already run at least once, don't overwrite what's there  
            if (folderexist != ExistenceCheckResult.FolderExists)
            {
                // if the directory already exists
                // update the databases with current version from server
                // now check if file already exists

                await folder.CreateFolderAsync("data", CreationCollisionOption.ReplaceExisting);
                IFolder dataFolder = await folder.GetFolderAsync("data");

                ExistenceCheckResult fileExist = await folder.CheckExistsAsync("ControllerDB");
                if (fileExist != ExistenceCheckResult.FileExists)   // if the file doesn't exist, create it
                {
                    /// this will eventually request the file from the server but for now it will load the xml file from an embedded resource for testing purposes
                    await Task.Run(() =>
                    {
                        CreateControllerDB(dataFolder);
                    });
                }
                fileExist = await folder.CheckExistsAsync("ProjectorDB");
                if (fileExist != ExistenceCheckResult.FileExists)   // if the file doesn't exist, create it
                {
                    /// this will eventually request the file from the server but for now it will load the xml file from an embedded resource for testing purposes
                    await Task.Run(() =>
                    {
                        CreateProjectorDB(dataFolder);
                    });
                }
                fileExist = await folder.CheckExistsAsync("Scheduler");
                if (fileExist != ExistenceCheckResult.FileExists)   // if the file doesn't exist, create it
                {
                    /// this will eventually request the file from the server but for now it will load the xml file from an embedded resource for testing purposes
                    await Task.Run(() =>
                    {
                        CreateSchedulerDB(dataFolder);
                    });
                }
            }
            else
            {
                /// for now create a sample file, server communication pending
                // if the directory does not exist
                // create the necessary data
                IFolder dataFolder = await folder.GetFolderAsync("data");
                await dataFolder.DeleteAsync();
                await Task.Run(() =>
                {
                    CreateDBDir();
                });

            }

        }

        private async void CreateControllerDB(IFolder data)     /// This is a test function. LoadControllerDB will be it's replacement pending server communication
        {
            try
            {
                IFile controllerDBFile = await data.CreateFileAsync("ControllerDB.xml", CreationCollisionOption.ReplaceExisting);       // create the db file in passed directory

                // create a stream for the embedded resource
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
                Stream embeddedFileStream = assembly.GetManifestResourceStream("MobileGUCTechTool.DBs.ControllerDB.xml");

                // open pcl storage file stream and write from embedded to this
                StreamReader reader = new StreamReader(embeddedFileStream);
                string contents = reader.ReadToEnd();
                await controllerDBFile.WriteAllTextAsync(contents);
                LoadControllerDB();
            }
            catch (FileNotFoundException e)
            { }
            catch (PCLStorage.Exceptions.DirectoryNotFoundException e)
            {

            }
        }

        private async void CreateProjectorDB(IFolder data)      /// This is a test function. LoadProjectorrDB will be it's replacement pending server communication
        {
            try
            {
                IFile projectorDBFile = await data.CreateFileAsync("ProjectorDB.xml", CreationCollisionOption.ReplaceExisting);     // create the db file in passed directory

                // create a stream for the embedded resource
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
                Stream embeddedFileStream = assembly.GetManifestResourceStream("MobileGUCTechTool.DBs.ProjectorDB.xml");

                // open pcl storage file stream and write from embedded to this
                StreamReader reader = new StreamReader(embeddedFileStream);
                string contents = reader.ReadToEnd();
                await projectorDBFile.WriteAllTextAsync(contents);
                LoadProjectorDB();
            }
            catch (FileNotFoundException e)
            { }
            catch (PCLStorage.Exceptions.DirectoryNotFoundException e)
            {

            }
        }

        private async void CreateSchedulerDB(IFolder data)
        {
            try
            {
                IFile projectorDBFile = await data.CreateFileAsync("Scheduler.xml", CreationCollisionOption.ReplaceExisting);     // create the db file in passed directory

                // create a stream for the embedded resource
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
                Stream embeddedFileStream = assembly.GetManifestResourceStream("MobileGUCTechTool.DBs.Scheduler.xml");

                // open pcl storage file stream and write from embedded to this
                StreamReader reader = new StreamReader(embeddedFileStream);
                string contents = reader.ReadToEnd();
                await projectorDBFile.WriteAllTextAsync(contents);
                LoadScheduler();
            }
            catch (FileNotFoundException e)
            { }
            catch (PCLStorage.Exceptions.DirectoryNotFoundException e)
            {

            }
        }

        public async void LoadControllerDB()
        {
            /// should provide a method for updating xml files from server before loading the existing file into memory
            /// /// should provide a method for updating xml files from server before loading the existing file into memory
            /// This will be a separate method.
            /// LoadControllerDB() will now read xml data from a file on the local filesystem, presumably after the file has been updated from the server request

            _Controllers = new DataSets.Classroom.ControllerDBDataTable();

            try
            {
                IFolder dataFolder = await folder.GetFolderAsync("data");
                IFile controllerDBFile = await dataFolder.GetFileAsync("ControllerDB.xml");
                Stream pclStorageFileStream = await controllerDBFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite);

                _Controllers.ReadXml(pclStorageFileStream);
                pclStorageFileStream.Close();
            }
            catch (FileNotFoundException e)
            { }
            catch (PCLStorage.Exceptions.DirectoryNotFoundException e)
            {

            }
        }

        public async void LoadProjectorDB()
        {
            /// should provide a method for updating xml files from server before loading the existing file into memory
            /// This will be a separate method.
            /// LoadProjectorDB() will now read xml data from a file on the local filesystem, presumably after the file has been updated from the server request

            _Projectors = new DataSets.Classroom.ProjectorDBDataTable();

            try
            {
                IFolder dataFolder = await folder.GetFolderAsync("data");
                IFile projectorDBFile = await dataFolder.GetFileAsync("ProjectorDB.xml");
                Stream pclStorageFileStream = await projectorDBFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite);

                _Projectors.ReadXml(pclStorageFileStream);
                pclStorageFileStream.Close();
            }
            catch (FileNotFoundException e)
            { }
            catch (PCLStorage.Exceptions.DirectoryNotFoundException e)
            {

            }
        }

        public async void LoadScheduler()
        {
            udpServer._Scheduler = new DataSets.SchedulerAddress.SchedulerIPDataTable();

            try
            {
                IFolder dataFolder = await folder.GetFolderAsync("data");
                IFile projectorDBFile = await dataFolder.GetFileAsync("Scheduler.xml");
                Stream pclStorageFileStream = await projectorDBFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite);

                udpServer._Scheduler.ReadXml(pclStorageFileStream);
                pclStorageFileStream.Close();
            }
            catch (FileNotFoundException e)
            { }
            catch (PCLStorage.Exceptions.DirectoryNotFoundException e)
            {

            }
        }

        public async void SaveController()
        {
            /// should provide a method for updating server-side xml files

            try
            {
                IFolder dataFolder = await folder.GetFolderAsync("data");
                IFile controllerDBFile = await dataFolder.GetFileAsync("ControllerDB.xml");
                Stream pclStorageFileStream = await controllerDBFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite);

                _Controllers.WriteXml(pclStorageFileStream);
                pclStorageFileStream.Close();
            }
            catch (FileNotFoundException e)
            { }

        }

        public async void SaveProjector()
        {
            /// should provide a method for updating server-side xml files

            try
            {
                IFolder dataFolder = await folder.GetFolderAsync("data");
                IFile projectorDBFile = await dataFolder.GetFileAsync("ProjectorDB.xml");
                Stream pclStorageFileStream = await projectorDBFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite);

                _Projectors.WriteXml(pclStorageFileStream);
                pclStorageFileStream.Close();
            }
            catch (FileNotFoundException e)
            { }
        }

        public async void SaveScheduler()
        {
            /// should provide a method for updating server-side xml files

            try
            {
                IFolder dataFolder = await folder.GetFolderAsync("data");
                IFile projectorDBFile = await dataFolder.GetFileAsync("Scheduler.xml");
                Stream pclStorageFileStream = await projectorDBFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite);

                udpServer._Scheduler.WriteXml(pclStorageFileStream);
                pclStorageFileStream.Close();
            }
            catch (FileNotFoundException e)
            { }
        }

        private void UdpServer_ReceivedMessage(object sender, UDPServer.OnReceivedMessageEventArgs e)
        {
            string ipAddress = e.mIPAddress.ToString();

            if (e.mMessage.Contains("Proccessing Command"))
            {
                return;
            }

            DataRowCollection drc = _Controllers.Rows;

            if (drc.Contains(ipAddress))
            {
                DataRow dataRow = drc.Find(ipAddress);
                // controlMenu.Log(dataRow["Building"].ToString() + " " + dataRow["Room"].ToString() + " response: " + e.mMessage);
            }

            if (e.mMessage.Contains("Alive") || e.mMessage.Contains("alive") || e.mMessage.Contains("Controler Test"))
            {
                aliveTestFrame.UDPAlive(ipAddress, true);
                // write to log the ip and message 
            }
            else if (e.mMessage.Contains("Run Test"))
            {
                aliveTestFrame.RunAliveTest();
            }
            else if (e.mMessage.Contains("!GETDEVLABELS=") || e.mMessage.Contains("!GETIP=") || e.mMessage.Contains("!GETUSER=") || e.mMessage.Contains("!GETPW="))  // Not sure these are the right responses for auth config 
            {

            }
            else
            {

            }
        }
    }
}
