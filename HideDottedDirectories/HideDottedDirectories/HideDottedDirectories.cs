using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace HideDottedDirectories
{
    public partial class HideDottedDirectories : ServiceBase
    {
        public HideDottedDirectories()
        {
            InitializeComponent();
            InitializeFields();
            SetupPoller();
        }

        private void InitializeFields()
        {
            timer = new Timer();
        }

        protected override void OnStart(string[] args)
        {
            
        }

        protected override void OnStop()
        {
        }

        private void SetupPoller()
        {
            timer.Interval = 60000;
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            
            RenameFolders();
        }

        private void RenameFolders()
        {
            var directories = EnumerateFolders();
            foreach (var dir in directories)
            {
                File.SetAttributes(dir, FileAttributes.Hidden);
            }

        }

        private IEnumerable<string> EnumerateFolders()
        {
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            var directories = Directory.GetDirectories(Directory.GetCurrentDirectory(), "*", SearchOption.TopDirectoryOnly).ToList();
            for (int i = 0; i < directories.Count; i++)
            {
                directories[i] = Path.GetFileName(directories[i]);
            }
            var dottedDirectories = (
                    from directory 
                    in directories
                    where Regex.IsMatch(directory, @"^\..*")
                    select directory
                                    ).ToList();

            return dottedDirectories;

        }

        private Timer timer;
    }
}
