using System;
using System.Threading;
using System.Threading.Tasks;

namespace Contact.Infrastructure
{
    public abstract class LongRunningProcess
    {
        private readonly ILog _log;
        private volatile bool _running;
        private bool _isInitialized;
        private Thread _workerThread;

        protected LongRunningProcess(ILog log)
        {
            _log = log;
            _isInitialized = false;
        }

        protected abstract Task Initialize();
        protected abstract void Run();

        public void Start()
        {
            if (!_isInitialized)
            {
                try
                {
                    Initialize();
                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    _log.Error("Error initializing process", ex);
                }

            }
            _running = true;
            _workerThread = new Thread(Run);
            _workerThread.Start();
        }

        public void Stop()
        {
            _running = false;
            Thread.Sleep(5000);
            try
            {
                _workerThread.Abort();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception) { }
        }

        public bool IsRunning
        {
            get { return _running; }
        }

        protected ILog Log
        {
            get { return _log; }
        }
    }
}
