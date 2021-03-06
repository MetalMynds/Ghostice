﻿using System;
using System.Windows.Forms;
using System.IO;

using Ghostice.Core.Server;
using Ghostice.Core.Server.Services;
using Ghostice.Core.Utilities;
using Ghostice.ApplicationKit.Utilities;

namespace Ghostice.ApplicationKit
{
    public partial class FormAppKit : Form
    {

        private const String APPKIT_APPLICATION_DOMAIN_PREFIX = "AppKit_TestDomain_";

        private delegate void ThreadSafeLogMessage(String Message, String Result);

        private delegate void ThreadSafeDisplaySummary(String executable, String arguments);

        private static ulong _rpcRequestIndex;

        private GhosticeServer _server;

        public FormAppKit()
        {
            InitializeComponent();

            this.Text = "Ghostice Application Kit Server - v" + ReflectionHelper.ApplicationVersion;

            trevewDiagnostics.ExpandAll();

            Helpers.PositionBottomRightDesktop(this);

        }

        void Status_SystemUnderTestStarted(object sender, StartupEventArgs e)
        {
            DisplaySummary(e.Path, e.Arguments);
            LogMessage(String.Format("Started: {0} Arguments: {1}", e.Path, String.IsNullOrWhiteSpace(e.Arguments) ? "None" : e.Arguments), String.Empty);
        }

        void Status_SystemUnderTestShutdown(object sender, ShutdownEventArgs e)
        {
            throw new NotImplementedException();
        }

        void Status_ActionPerformed(object sender, ActionEventArgs e)
        {
            LogMessage(String.Format("Target: {0} {1}: {2} Value: {3}", e.Request.Target != null ? e.Request.Target.ToString() : "None", e.Request.Operation.ToString(), e.Request.Name, e.Result.ReturnValue), e.Result.Status.ToString());
        }

        private void HandleAppKitFormClosed(object sender, FormClosedEventArgs e)
        {
            _server.Shutdown();
            Ghostice.ApplicationKit.Properties.Settings.Default.Save();            
        }

        private void HandleAppKitFormLoad(object sender, EventArgs e)
        {

            try
            {

                var executablePath = Path.GetDirectoryName(Application.ExecutablePath);

                var extensions = Path.Combine(executablePath, "Extensions");

                _server = new GhosticeServer(extensions);

                _server.StatusListener.ActionPerformed += Status_ActionPerformed;

                _server.StatusListener.SystemUnderTestShutdown += Status_SystemUnderTestShutdown;

                _server.StatusListener.SystemUnderTestStarted += Status_SystemUnderTestStarted;

                _server.Start(new Uri(Ghostice.ApplicationKit.Properties.Settings.Default.AppKitRpcEndpointAddress));

                txtRpcAddress.Text = _server.EndPoint;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,String.Format("Start Application Kit Server Failed!\r\n{0}\r\n{1}",ex.Message, ex.StackTrace), "Ghostice Application Kit Server");
                Close();
            }

        }

        protected void LogMessage(String Message, String Result)
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new ThreadSafeLogMessage(this.LogMessage), new Object[] { Message, Result });
            }
            else
            {

                _rpcRequestIndex++;

                var timestamp = String.Format("{0} {1}", DateTime.Now.ToString("HH:mm.ss.fff"), DateTime.Now.ToShortDateString());

                var newItem = lstvewLog.Items.Insert(0, _rpcRequestIndex.ToString());

                newItem.SubItems.Add(timestamp);

                newItem.SubItems.Add(Message);

                newItem.SubItems.Add(Result);

                lstvewLog.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                lstvewLog.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                lstvewLog.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                lstvewLog.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

            }

        }

        protected void DisplaySummary(String executable, String arguments)
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new ThreadSafeDisplaySummary(this.DisplaySummary), new Object[] { executable, arguments });
            }
            else
            {
                txtTarget.Text = executable;
                txtArguments.Text = arguments;
            }

        }

    }
}
