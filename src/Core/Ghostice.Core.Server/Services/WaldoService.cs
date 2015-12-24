﻿using AustinHarris.JsonRpc;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.IO;
using Anotar.NLog;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting;
using Ghostice.Core.Server.Utilities;

namespace Ghostice.Core.Server.Services
{
    internal class WaldoService : JsonRpcService
    {

        private const String APPKIT_APPLICATION_DOMAIN_PREFIX = "AppKit_TestDomain_";

        public const int DEFAULT_SUT_STARTUP_TIMEOUT_SECONDS = 60;

        private ApplicationInfo _sutInformation;

        private ApplicationManagerSponsor _appManagerSponsor;

        private ApplicationManager _appManager;

        private ILease _appManagerLease;

        private Thread _pwnedUIThread;

        private int _sutStartupTimeout = DEFAULT_SUT_STARTUP_TIMEOUT_SECONDS;

        private IWaldoStatus _status;

        private String _extensionsPath;

        public WaldoService(IWaldoStatus Status, String ExtensionsPath)
        {
            
            _status = Status;

            _extensionsPath = ExtensionsPath;

            _appManagerSponsor = new ApplicationManagerSponsor(new TimeSpan(0, 5, 0));
        }

        [JsonRpcMethod]
        private String About()
        {
            return String.Format("Ghostice AppKit Server v{0}", Assembly.GetEntryAssembly().GetName().Version.ToString());
        }

        [JsonRpcMethod]
        private ApplicationInfo Start(String executablePath, String Arguments)
        {

            if (String.IsNullOrWhiteSpace(executablePath) || !Path.IsPathRooted(executablePath))
            {
                throw new ArgumentException(String.Format("Path is not Valid! A fully qualified path is required.\r\nExecutablePath: [{0}]", executablePath), "ExecutablePath");
            }

            var args = String.IsNullOrWhiteSpace(Arguments) ? String.Empty : Arguments;

            var appDomainBasePath = Path.GetDirectoryName(executablePath);

            LogTo.Info(String.Format("Starting: {0} Arguments: {1}", executablePath, String.IsNullOrWhiteSpace(Arguments) ? "None" : Arguments), String.Empty);

            try
            {

                _appManager = AppDomainFactory.Create<ApplicationManager>(appDomainBasePath, APPKIT_APPLICATION_DOMAIN_PREFIX, new Object[] { _extensionsPath }, false);

                _appManagerLease = (ILease)RemotingServices.GetLifetimeService(_appManager);

                _appManagerLease.Register(_appManagerSponsor);

                _sutInformation = _appManager.Start(executablePath, args, _sutStartupTimeout);

            }
            catch (Exception ex)
            {
                LogTo.ErrorException("Start System Under Test Failed!", ex);
            }

            LogTo.Info(String.Format("Started: {0}", executablePath), String.Empty);

            _status.OnStarted(executablePath, Arguments);

            return _sutInformation;
        }

        [JsonRpcMethod]
        private void Shutdown(ApplicationInfo application)
        {

        }

        [JsonRpcMethod]
        private ActionResult Perform(ActionRequest request)
        {
            ActionResult actionResult = null;

            // Casts any parameters to Type of 'ValueType' Property
            request.FixParameterTypes();

            switch (request.Operation)
            {
                case ActionRequest.OperationType.Get:

                    var getResult = _appManager.Perform(request);

                    actionResult = getResult;

                    LogTo.Debug("Target: {0} Get: {1}", request.Location.ToString(), request.Name);

                    _status.OnPerformed(request, actionResult);

                    return actionResult;

                case ActionRequest.OperationType.Set:

                    var setResult = _appManager.Perform(request);

                    actionResult = setResult;

                    LogTo.Debug("Target: {0} Set: {1} Value: {2}", request.Location.ToString(), request.Name, actionResult.ReturnValue, actionResult.ReturnValue);

                    _status.OnPerformed(request, actionResult);

                    return actionResult;

                case ActionRequest.OperationType.Execute:
                    
                    var executeDisplayArgs = request.HasParameters ? String.Join(", ", from parameter in request.Parameters select parameter.Value.ToString()) : "None";

                    var executeResult = _appManager.Perform(request);

                    actionResult = executeResult;

                    LogTo.Debug("Target: {0} Execute: {1} Arguments: {2}\r\nResult: {3}", request.Location.ToString(), request.Name, executeDisplayArgs, actionResult.ToString());

                    _status.OnPerformed(request, actionResult);

                    return actionResult;

                case ActionRequest.OperationType.Map:

                    var mapDisplayArgs = request.HasParameters ? String.Join(", ", from parameter in request.Parameters select parameter.Value.ToString()) : "None";

                    var mapResult = _appManager.Perform(request);

                    actionResult = mapResult;

                    LogTo.Debug("Target: {0} Map Arguments: {1}\r\nValue: {2}", request.Location.ToString(), request.Name, mapDisplayArgs, actionResult.ReturnValue.ToString());

                    _status.OnPerformed(request, actionResult);

                    return actionResult;

                case ActionRequest.OperationType.Ready:

                    var readyResult = _appManager.Perform(request);

                    actionResult = readyResult;

                    LogTo.Debug("Target: {0} Ready: Result: {1}", request.Location.ToString(), actionResult.ToString());

                    _status.OnPerformed(request, actionResult);

                    return actionResult;

                case ActionRequest.OperationType.List:

                    var listResult = _appManager.Perform(request);

                    actionResult = listResult;

                    LogTo.Debug("Target: Windows List:\r\nResult: {0}", actionResult.ToString());

                    _status.OnPerformed(request, actionResult);

                    return actionResult;

                case ActionRequest.OperationType.Unknown:
                default:

                    var message = String.Format("Ghostice Server Received Unrecognised/Invalid Operation!\r\nOperation: {0}", request.Operation.ToString());

                    LogTo.Error(message);

                    return ActionResult.Failed(request.Location != null ? request.Location.ToString() : "None", message);
            }
        }

        public int SutStartupTimeoutSeconds
        {
            get
            {
                return _sutStartupTimeout;
            }
            set
            {
                _sutStartupTimeout = value;
            }

        }
    }

    public class WaldoServiceException : Exception
    {

        public WaldoServiceException(String message)
            : base(message)
        {

        }

        public WaldoServiceException(String message, Exception innerException)
            : base(message, innerException)
        {

        }
    }

    public class WaldoStartupFailedException : WaldoServiceException
    {

        public WaldoStartupFailedException(String Path, String Error)
            : base(String.Format("Start System Under Test Failed!\r\nError: {0}\r\nPath: {1}", Error, Path))
        {

        }
    }

    public class WaldoShutdownFailedException : WaldoServiceException
    {


        public WaldoShutdownFailedException(ApplicationInfo application, String rrror)
            : base(String.Format("Shutdown System Under Test Failed!\r\nError: {0}\r\nPath: {1}", rrror, application.ApplicationPath))
        {
            this.Application = application;
        }

        public ApplicationInfo Application { get; protected set; }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }


}
