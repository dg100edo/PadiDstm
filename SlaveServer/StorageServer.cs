using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using System.Threading;

using PADI_DSTM_Lib;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Runtime.Serialization.Formatters;
using System.Net.Sockets;

using padi_dstm_exceptions;

namespace StorageServer
{
    class StorageServer : MarshalByRefObject, IStorageServer, IFailServer, ITransactionParticipant, IMaintenance
    {
        public static string SERVICE_NAME = "Server";

        private int port;
        private string url;

        private bool freeze;
        private bool fail;
        private bool registered;

        private bool maintenance;

        private ICentralServer centralServer;
        private TransactionParticipant transactionParticipant;

        public StorageServer(int port)
        {
            this.port = port;
            this.url = "tcp://localhost:" + port + "/" + SERVICE_NAME;
            transactionParticipant = new TransactionParticipant(url, CENTRAL_SERVER.URL);
            centralServer = (ICentralServer)Activator.GetObject(typeof(ICentralServer), CENTRAL_SERVER.URL);
            if (centralServer == null)
            {
                throw new ApplicationException("StorageServer: Impossivel ligar-se ao central server");
            }
            freeze = false;
            registered = false;
            maintenance = false;
        }

        public void RegisterServer()
        {
            centralServer.AddStorageServer(url);
            registered = true;
        }

        // IStorageServer implementation
        public void Store(long tid, int id, Object value)
        {
            if (maintenance) 
            {
                throw new TxMaintenanceException("Server in maintenance");
            }
            checkFreezeOrFail();
            Console.WriteLine("Store tid={0} id={1} ", tid, id);
            transactionParticipant.Store(tid, id, value);
        }

        public Object Load(long tid, int id)
        {
            if (maintenance)
            {
                throw new TxMaintenanceException("Server in maintenance");
            }
            checkFreezeOrFail();
            Console.WriteLine("Load tid={0} id={1} ", tid, id);
            return transactionParticipant.Load(tid, id);
        }

        public void DumpStatus()
        {
            lock (this)
            {
                transactionParticipant.DumpStatus();
            }
        }

        public TransactionParticipant GetStorageServerContext()
        {
                return transactionParticipant;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter
= true)]
        public void SetStorageServerContext(TransactionParticipant context)
        {
                transactionParticipant = context;
                transactionParticipant.setUrl(url);
        }

        // ITransactionPartipant implementation
        public bool TxCommit_participant(long tid)
        {
            checkFreezeOrFail();
            Console.WriteLine("TxCommit tid={0}", tid);
            return transactionParticipant.TxCommit_participant(tid);
        }

        public bool TxAbort_participant(long tid)
        {
            checkFreezeOrFail();
            Console.WriteLine("TxAbort tid={0}", tid);
            return transactionParticipant.TxAbort_participant(tid);
        }

        public bool TxPrepare(long tid)
        {
            checkFreezeOrFail();
            Console.WriteLine("TxPrepare tid={0}", tid);
            return transactionParticipant.TxPrepare(tid);
        }

        public bool canCommit(long tid)
        {
            checkFreezeOrFail();
            Console.WriteLine("canCommit tid={0}", tid);
            return transactionParticipant.canCommit(tid);
        }

        // IFailServer implementation
        public void Fail()
        {
            lock (this)
            {
                fail = true;
                transactionParticipant = null;
                registered = false;
                Monitor.PulseAll(this);
            }
        }

        public void Freeze()
        {
            lock (this)
            {
                freeze = true;
                Monitor.PulseAll(this);
            }
        }

        public void Recover()
        {
            lock (this)
            {
                if (freeze)
                {
                    freeze = false;
                }
                else if (fail)
                {
                    transactionParticipant = new TransactionParticipant(url, CENTRAL_SERVER.URL);
                    RegisterServer();
                    fail = false;
                }

                Monitor.PulseAll(this);
            }
        }

        void checkFreezeOrFail()
        {
            lock (this)
            {
                while (freeze)
                {
                    Monitor.Wait(this);
                }
                Monitor.PulseAll(this);
                if (fail)
                {
                    throw new SocketException();
                }
            }

        }


        public bool isAlive()
        {
            if (registered)
                return true;
            else
                return false;
        }

        public void setMaintenanceStatus(bool state)
        {
            Console.WriteLine("Maintenance status was set to: " + state);
            maintenance = state;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Insira o port para o novo servidor: ");
            int port = Int32.Parse(Console.ReadLine());

            BinaryClientFormatterSinkProvider clientProvider = null;
            BinaryServerFormatterSinkProvider serverProvider =
                 new BinaryServerFormatterSinkProvider();

            serverProvider.TypeFilterLevel =
                    System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            IDictionary props = new Hashtable();
            props["port"] = port;
            props["typeFilterLevel"] = TypeFilterLevel.Full;
            TcpChannel channel = new TcpChannel(props, clientProvider, serverProvider);

            ChannelServices.RegisterChannel(channel, true);
            StorageServer storageServer = new StorageServer(port);

            RemotingServices.Marshal(storageServer, StorageServer.SERVICE_NAME, typeof(StorageServer));
            storageServer.RegisterServer();
            System.Console.WriteLine("<enter> para sair do storage server ...");
            System.Console.ReadLine();
            ChannelServices.UnregisterChannel(channel);
        }
    }
}
