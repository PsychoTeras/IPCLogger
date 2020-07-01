using IPCLogger.Common;
using IPCLogger.Loggers.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace IPCLogger.Loggers.LFactory
{
    public sealed class LFactory : BaseLogger<LFactorySettings>
    {

#region Private static fields

        private static readonly List<Type> _registeredLoggers;

#endregion

#region Private fields

        private LightLock _lockObj;
        private BaseLoggerInt[] _loggers;

        private string _configurationFile;
        private FileSystemWatcher _configurationFileWatcher;

        private bool _enabled;
        private bool _shouldLock;

        private volatile bool _initialized;
        private volatile bool _suspended;

#endregion

#region Static properties

        public static LFactory Instance { get; }

#endregion

#region Ctor

        static LFactory()
        {
            Instance = new LFactory();
            _registeredLoggers = GetLoggers();
        }

        private LFactory() : base(true)
        {
            _lockObj = new LightLock();
            PreInitialize = OnceInitializedCheck;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

#endregion

#region Static methods

        internal static List<Type> GetLoggers()
        {
            List<Type> types = new List<Type>();
            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.
                GetAssemblies().
                Where(a => !a.FullName.StartsWith("System."));
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    types.AddRange(assembly.
                        GetTypes().
                        Where(t => Helpers.IsAssignableTo(t, typeof(BaseLogger<>)) && 
                                   !t.IsAbstract && t != typeof(LFactory)));
                }
                catch { }
            }
            return types;
        }

#endregion

#region ILogger

        protected internal override void Write(Type callerType, Enum eventType, string eventName,
            byte[] data, string text, bool writeLine, bool immediateFlush)
        {
            if (!_initialized || _suspended) return;

            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!_initialized) return;

                foreach (BaseLoggerInt logger in _loggers)
                {
                    if (logger.CheckApplicableEvent(eventName))
                    {
                        logger.Write(callerType, eventType, eventName, data, text, writeLine, immediateFlush);
                    }
                }
            }
            finally
            {
                _lockObj.Set(_shouldLock);
            }
        }

        public override void Initialize()
        {
            throw new Exception("Use Initialize(string configurationFile) method");
        }

        public override void Deinitialize()
        {
            DeinitializeInt(false);
        }

        public override bool Suspend()
        {
            if (!_enabled) return false;

            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (_initialized && !_suspended)
                {
                    foreach (BaseLoggerInt logger in _loggers)
                    {
                        logger.Suspend();
                    }
                    _suspended = true;
                }
            }
            finally
            {
                _lockObj.Set(_shouldLock);
            }
            return _suspended;
        }

        public override bool Resume()
        {
            if (!_enabled) return false;

            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (_initialized && _suspended)
                {
                    foreach (BaseLoggerInt logger in _loggers)
                    {
                        logger.Resume();
                    }
                    _suspended = false;
                }
            }
            finally
            {
                _lockObj.Set(_shouldLock);
            }
            return !_suspended;
        }

        public override void Flush()
        {
            if (!_enabled) return;

            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!_initialized) return;

                foreach (BaseLoggerInt logger in _loggers)
                {
                    logger.Flush();
                }
            }
            finally
            {
                _lockObj.Set(_shouldLock);
            }
        }

#endregion

#region Class methods

        public bool Initialize(string configurationFile)
        {
            try
            {
                if (_initialized) return false;

                Settings.Setup(_configurationFile = configurationFile);
                Patterns.Setup(_configurationFile);
                MapSettingsParamsToLocal();

                if (_enabled)
                {
                    List<DeclaredLogger> declaredLoggers = LFactorySettings.GetDeclaredLoggers(configurationFile);
                    InstantiateLoggers(declaredLoggers);
                    foreach (BaseLoggerInt logger in _loggers)
                    {
                        logger.Initialize();
                    }
                    _initialized = true;
                }

                SetupAutoReloadWatcher(configurationFile);
                return true;
            }
            catch (Exception ex)
            {
                CatchLoggerException("Failed to initialize LFactory", ex);
                return false;
            }
        }

        private void MapSettingsParamsToLocal()
        {
            _enabled = Settings.Enabled;
            _shouldLock = Settings.ShouldLock;
        }

        private void OnceInitializedCheck()
        {
            if (!_initialized)
            {
                Initialize();
            }
            PreInitialize = null;
        }

        private void SetupAutoReloadWatcher(string configurationFile)
        {
            if (Settings.AutoReload)
            {
                string cfgPath = Path.GetDirectoryName(configurationFile);
                string cfgFile = Path.GetFileName(configurationFile);
                _configurationFileWatcher = new FileSystemWatcher(cfgPath, cfgFile);
                _configurationFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                _configurationFileWatcher.Changed += ConfigurationFileChanged;
                _configurationFileWatcher.EnableRaisingEvents = true;
            }
        }

        private void DeinitializeInt(bool configurationFileChanged)
        {
            if (!configurationFileChanged)
            {
                _lockObj.WaitOne(_shouldLock);
            }
            try
            {
                if (!_initialized || !_enabled) return;

                foreach (BaseLoggerInt logger in _loggers)
                {
                    logger.Deinitialize();
                    logger.Dispose();
                }
                _loggers = null;

                if (!configurationFileChanged && _configurationFileWatcher != null)
                {
                    _configurationFileWatcher.EnableRaisingEvents = false;
                    _configurationFileWatcher.Dispose();
                    _configurationFileWatcher = null;
                }

                _suspended = false;
                _initialized = false;
            }
            finally
            {
                if (!configurationFileChanged)
                {
                    _lockObj.Set(_shouldLock);
                }
            }
        }

        private void ConfigurationFileChanged(object sender, FileSystemEventArgs e)
        {
            _lockObj.WaitOne(_shouldLock);

            try
            {
                _configurationFileWatcher.EnableRaisingEvents = false;
                Thread.Sleep(10);

                Settings.Setup(_configurationFile);
                Patterns.Setup(_configurationFile);
                MapSettingsParamsToLocal();

                Settings.AutoReload = true;

                if (_enabled)
                {
                    List<DeclaredLogger> declaredLoggers = LFactorySettings.GetDeclaredLoggers(_configurationFile);
                    IEnumerable<BaseLoggerInt> intLoggers = _loggers ?? new BaseLoggerInt[0];

                    IEnumerable<BaseLoggerInt> toRemove = intLoggers.
                        Where(l => !declaredLoggers.Exists(dl => dl.UniqueId == l.UniqueId));
                    foreach (BaseLoggerInt logger in toRemove)
                    {
                        logger.Deinitialize();
                        logger.Dispose();
                    }

                    List<BaseLoggerInt> loggers = new List<BaseLoggerInt>();
                    List<BaseLoggerInt> newLoggers = new List<BaseLoggerInt>();
                    if (declaredLoggers.Any())
                    {
                        Dictionary<string, BaseLoggerInt> existings = intLoggers.
                            Where(l => !toRemove.Contains(l)).
                            ToDictionary(l => l.UniqueId, l => l);
                        foreach (DeclaredLogger declaredLogger in declaredLoggers)
                        {
                            if (existings.TryGetValue(declaredLogger.UniqueId, out var logger))
                            {
                                if (SetupInstantiatedLogger(declaredLogger, logger))
                                {
                                    loggers.Add(logger);
                                }
                            }
                            else
                            {
                                logger = InstantiateLoggerByDeclared(declaredLogger);
                                if (logger != null && SetupInstantiatedLogger(declaredLogger, logger))
                                {
                                    loggers.Add(logger);
                                    newLoggers.Add(logger);
                                }
                            }
                        }
                    }

                    foreach (BaseLoggerInt logger in newLoggers)
                    {
                        logger.Initialize();
                    }

                    _loggers = loggers.ToArray();
                    _initialized = true;
                }
                else
                {
                    DeinitializeInt(true);
                }

                if (_configurationFileWatcher != null)
                {
                    _configurationFileWatcher.EnableRaisingEvents = true;
                }
            }
            catch (Exception ex)
            {
                CatchLoggerException("Unable to autoreload LFactory", ex);
            }
            finally
            {
                _lockObj.Set(_shouldLock);
            }
        }

        private void InstantiateLoggers(List<DeclaredLogger> declaredLoggers)
        {
            List<BaseLoggerInt> loggers = new List<BaseLoggerInt>();
            foreach (DeclaredLogger declaredLogger in declaredLoggers)
            {
                BaseLoggerInt logger = InstantiateLoggerByDeclared(declaredLogger);
                if (logger != null && SetupInstantiatedLogger(declaredLogger, logger))
                {
                    loggers.Add(logger);
                }
            }
            _loggers = loggers.ToArray();
        }

        private bool SetupInstantiatedLogger(DeclaredLogger declaredLogger, BaseLoggerInt logger)
        {
            try
            {
                logger.Setup(declaredLogger.CfgNode);
                return true;
            }
            catch (Exception ex)
            {
                string msg = $"Failed to setup logger '{logger}'";
                CatchLoggerException(msg, ex);
            }
            return false;
        }

        private BaseLoggerInt InstantiateLoggerByDeclared(DeclaredLogger declaredLogger)
        {
            Type loggerType = _registeredLoggers.FirstOrDefault
                (
                    t => t.Name == declaredLogger.TypeName &&
                         (string.IsNullOrEmpty(declaredLogger.Namespace) || t.Namespace != null && t.Namespace.Equals(declaredLogger.Namespace))
                );

            BaseLoggerInt logger = loggerType != null
                ? Activator.CreateInstance(loggerType, new object[] {true}) as BaseLoggerInt
                : null;
            if (logger != null)
            {
                logger.SetPatternsFactory(Patterns);
                logger.SetUniqueId(declaredLogger.UniqueId);
                return logger;
            }

            string nameSpace = !string.IsNullOrEmpty(declaredLogger.Namespace)
                ? declaredLogger.Namespace + "."
                : string.Empty;
            string msg = $"Logger '{nameSpace}{declaredLogger.TypeName}' is not available";
            CatchLoggerException(msg, null);
            return null;
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            Deinitialize();
        }

#endregion

    }
}
