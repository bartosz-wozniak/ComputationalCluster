using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Autofac;
using CCluster.Common;
using CCluster.Common.Communication;
using CCluster.Common.Configuration;
using CCluster.Messages;
using DvrpTaskSolverCommon;
using DvrpTaskSolverCommon.DataConversion;
using DvrpTaskSolverCommon.ProblemData;
using log4net;

namespace CCluster.Client
{
    class Program
    {

        private string problemDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName
            + "\\problems\\testproblem.txt";
        private ILog logger = LogProvider.GetCurrentClassLogger();

        private ClientConfiguration configuration;

        private IContainer container;
        private SimpleMediator mediator;
        private IServerClient client;
        private FileLoader fileLoader;
        private ISerializer serializer;

        static void Main(string[] args)
        {
            new Program().Run();
        }

        public Program()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
        }

        public void Run()
        {
            logger.Info($"Preparing.");
            using (container = PrepareContainer())
            {
                configuration = container.Resolve<Loader>().Load<ClientConfiguration>(GetAddressFromConfig, GetPortFromConfig);

                mediator = container.Resolve<SimpleMediator>();
                client = container.Resolve<IServerClient>();
                fileLoader = container.Resolve<FileLoader>();
                serializer = container.Resolve<ISerializer>();

                if (configuration.ProblemId.HasValue)
                {
                    this.AskForResponse();
                }
                else
                {
                    this.SendProblemRequest();
                }
            }
            logger.Info("System stopped.");
        }

        private void AskForResponse()
        {
            var problem = configuration.ProblemId.Value;
            logger.Info($"Asking for solution to the problem {problem}.");
            try
            {
                client.Send(new SolutionRequest
                {
                    Id = problem
                });
                var soultionResult = fileLoader.LoadSolution(problemDir);
                var response = mediator.GetLast<Solutions>();
                var solution = serializer.DeserializeSolution(response.SolutionsList[0].Data);
                logger.Info($"Solution:: {solution.Result}. Shoudl be:: {soultionResult}");
                var statuses = string.Join(",", response.SolutionsList.Select(s => s.Type.ToString()));
                logger.Info($"Response received, task statuses: {statuses}.");
            }
            catch (Exception ex)
            {
                logger.Fatal("Asking for solution failed.", ex);
            }
        }

        private void SendProblemRequest()
        {
            logger.Info($"Adding new problem.");

            try
            {
                var problem = fileLoader.LoadProblem(problemDir);
                client.Send(new SolveRequest
                {
                    ProblemType = Constants.ProblemName,
                    SolvingTimeout = null,
                    Data = serializer.SerializeProblem(problem)
                });
                var response = mediator.GetLast<SolveRequestResponse>();
                logger.Info($"Task acquired by CS. Assigned id: {response.Id}.");
            }
            catch (Exception ex)
            {
                logger.Fatal("Cannot send the problem.", ex);
            }
        }

        private string GetAddressFromConfig(ClientConfiguration cfg)
        {
            return cfg.CsAddress;
        }

        private int GetPortFromConfig(ClientConfiguration cfg)
        {
            return cfg.CsPort;
        }

        private IContainer PrepareContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CommonModule>();
            builder.RegisterModule<ConfigurationModule>();
            builder.RegisterModule<CommunicationModule>();
            builder.RegisterType<SimpleMediator>().AsSelf().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<FileLoader>().AsSelf();
            builder.RegisterType<Serializer>().AsImplementedInterfaces().SingleInstance();
            return builder.Build();
        }

        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            logger.Fatal("Unhandled exception occured", (Exception)args.ExceptionObject);
        }
     
    }
}
