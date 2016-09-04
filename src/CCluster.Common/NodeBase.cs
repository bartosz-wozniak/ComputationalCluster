using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Autofac;
using CCluster.Common.Configuration;
using CCluster.Common.EventBus;
using CCluster.Messages.Notifications;
using log4net;
using MediatR;

namespace CCluster.Common
{
    public abstract class NodeBase<TConfig, TType> : RequestHandler<ShutdownSystem>
        where TConfig : class, new()
    {
        protected string libDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName + "\\libraries";
        private readonly Assembly coreAssembly;
        private readonly ManualResetEvent shutdownEvent = new ManualResetEvent(false);

        protected readonly string NodeType;
        protected readonly ILog Logger = LogManager.GetLogger(typeof(TType));
        protected TConfig Configuration { get; private set; }

        public NodeBase(string nodeType)
        {
            this.coreAssembly = typeof(TType).Assembly;
            NodeType = nodeType;
        }

        public void Run()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            Console.CancelKeyPress += (s, e) =>
            {
                shutdownEvent.Set();
                e.Cancel = true;
            };

            Logger.Info($"Preparing {NodeType}.");
            using (var container = PrepareContainer())
            {
                Configuration = container.Resolve<Loader>().Load<TConfig>(GetAddressFromConfig, GetPortFromConfig);
                Inject(container);

                Logger.Info("Starting mediator.");
                var mediator = container.Resolve<SequentialMediator>();
                mediator.Start();

                Logger.Info("Starting system.");
                try
                {
                    StartSystem();
                    shutdownEvent.WaitOne();
                }
                catch (Exception ex)
                {
                    Logger.Fatal("Cannot start the system.", ex);
                }

                Logger.Info("Stopping system.");
                StopSystem();

                mediator.Stop();
            }
            Logger.Info("System stopped.");
        }

        protected virtual void LoadModules(ContainerBuilder builder)
        {
            builder.RegisterModule<CommunicationModule>();
            builder.RegisterModule<NodeBackupModule>();
            builder.RegisterModule<ConfigurationModule>();
        }

        protected abstract void Inject(ILifetimeScope scope);
        protected abstract void StartSystem();
        protected abstract void StopSystem();

        protected abstract string GetAddressFromConfig(TConfig cfg);
        protected abstract int GetPortFromConfig(TConfig cfg);

        protected override void HandleCore(ShutdownSystem message)
        {
            Logger.Fatal("Some component requested system shutdown, exiting.");
            shutdownEvent.Set();
        }

        private IContainer PrepareContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CommonModule>();
            builder.RegisterModule(new MediatRModule(coreAssembly));
            builder.Register(_ => this).AsImplementedInterfaces().SingleInstance();
            LoadModules(builder);
            return builder.Build();
        }

        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Logger.Fatal("Unhandled exception occured", (Exception)args.ExceptionObject);
        }
    }
}
