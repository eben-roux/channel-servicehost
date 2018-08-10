using System;
using System.IO;
using System.Threading;
using Shuttle.Core.ServiceHost;

namespace ConsoleService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost.Run<Host>();
        }
    }

    internal class Host : IServiceHost
    {
        private Thread _thread;
        private volatile bool _active;
        private DateTime _nextWriteDate;

        public Host()
        {
            _thread = new Thread(worker);
        }

        private void worker()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "messages.log");

            while (_active)
            {
                if (DateTime.Now < _nextWriteDate)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    using (var sw = new StreamWriter(filePath, true))
                    {
                        sw.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
                    }

                    _nextWriteDate = DateTime.Now.AddSeconds(2);
                }
            }
        }

        public void Start()
        {
            _nextWriteDate = DateTime.Now;
            _active = true;
            _thread.Start();
        }

        public void Stop()
        {
            _active = false;
            _thread.Join();
        }
    }
}
