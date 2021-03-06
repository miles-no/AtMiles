﻿using System.ServiceProcess;
using no.miles.at.Backend.Infrastructure;

namespace WorkerService
{
    public partial class Service : ServiceBase
    {
        private readonly ILog _logger;
        private readonly WorkerProcess _process;

        public Service()
        {
            InitializeComponent();

            _logger = new EventLogger("MilesSource", "AtMilesLog");
            _process = new WorkerProcess(_logger);
        }

        protected override void OnStart(string[] args)
        {
            _process.Start();
            _logger.Info("Running");
        }

        protected override void OnStop()
        {
            _process.Stop();
            _logger.Info("Stopped");
        }
    }
}
