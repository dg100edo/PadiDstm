using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

using PADI_DSTM_Lib;
using System.Net.Sockets;
using System.IO;

using padi_dstm_exceptions;
using System.Threading;

namespace PADI_DSTM
{

    public class UrlUpdator : MarshalByRefObject, IClient
    {

        private Dictionary<int, string[]> buffer;

        public UrlUpdator()
        {
            buffer = new Dictionary<int, string[]>();
        }

        public string[] checkUpdates(int id)
        {
            try
            {
                return buffer[id];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public void UpdateUrls(int id, string[] storageServers)
        {
            buffer[id] = storageServers;
        }
    }


    public class PadiDstm
    {

        private static TcpChannel channel;
        public static ICentralServer centralServer;
        private static ITransactionCoordinator transactionCoordinator;
        private static long tid;
        private static string url;

        private static int MIN_PORT = 1024;
        private static int MAX_PORT = 65535;

        public static UrlUpdator urlUpdator;

        public static bool Init()
        {
            Console.WriteLine("Init");
            urlUpdator = new UrlUpdator();
            initAsServer();
            centralServer = (ICentralServer)Activator.GetObject(typeof(ICentralServer), CENTRAL_SERVER.URL);
            transactionCoordinator = (ITransactionCoordinator)Activator.GetObject(typeof(ITransactionCoordinator), CENTRAL_SERVER.URL);
            if (centralServer == null || transactionCoordinator == null)
            {
                Console.WriteLine("Init: Impossivel ligar ao central server");
                return false;
            }
            return true;
        }


        private static void initAsServer()
        {
            int port = registerAndReturnPort();
            RemotingServices.Marshal(urlUpdator, "Client", typeof(UrlUpdator));
            url = "tcp://localhost:" + port + "/Client";
        }

        private static int registerAndReturnPort()
        {
            Random rand = new Random();
            int port = rand.Next(MIN_PORT, MAX_PORT);
            try
            {
                channel = new TcpChannel(port);
                ChannelServices.RegisterChannel(channel, true);
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.AddressAlreadyInUse)
                    registerAndReturnPort();
                else
                    throw e;
            }
            return port;
        }


        public static PadInt CreatePadInt(int id)
        {
            Console.WriteLine("Create padint # " + id);
            string[] serversUrl = centralServer.ObjectAssignServers(url, id);
            if (serversUrl == null)
                return null;
            else
                return new PadInt(id, serversUrl);
        }

        public static PadInt AccessPadInt(int id)
        {
            Console.WriteLine("Access padint # " + id);
            string[] serversUrl = centralServer.ObjectServers(url, id);
            if (serversUrl == null)
                return null;
            else
                return new PadInt(id, serversUrl);
        }

        public static bool TxBegin()
        {
            Console.WriteLine("TxBegin");
            tid = transactionCoordinator.TxBegin();
            return true;
        }

        public static bool TxCommit()
        {
            Console.WriteLine("TxCommit");
            return transactionCoordinator.TxCommit(tid);
        }

        public static bool TxAbort()
        {
            Console.WriteLine("TxAbort");
            return transactionCoordinator.TxAbort(tid);
        }

        public static bool Status()
        {
            centralServer.Status();
            return true;
        }

        public static bool Fail(string url)
        {
            IFailServer server = (IFailServer)Activator.GetObject(typeof(IFailServer), url);
            server.Fail();
            return true;
        }

        public static bool Freeze(string url)
        {
            IFailServer server = (IFailServer)Activator.GetObject(typeof(IFailServer), url);
            server.Freeze();
            return true;
        }

        public static bool Recover(string url)
        {
            IFailServer server = (IFailServer)Activator.GetObject(typeof(IFailServer), url);
            server.Recover();
            return true;
        }

        public static long TID
        {
            get
            {
                return tid;
            }
            set
            {
                tid = value;
            }
        }

        public void UpdateUrls(int id, string[] storageServers)
        {
            throw new NotImplementedException();
        }
    }

    public class PadInt
    {
        private static int WAIT_TIME = 750;

        private int id;
        private string[] serversUrl;

        public PadInt(int id, string[] serversUrl)
        {
            this.id = id;
            this.serversUrl = serversUrl;
        }

        public int Read()
        {
            IStorageServer storageServer;
            Object result = -1;

            string[] urls = PadiDstm.urlUpdator.checkUpdates(id);
            if (urls != null)
            {
                serversUrl = urls;
            }
            foreach (String server in serversUrl)
            {
                try
                {
                    storageServer = (IStorageServer)Activator.GetObject(typeof(IStorageServer), server);
                    while (true)
                    {
                        try
                        {
                            result = storageServer.Load(PadiDstm.TID, id);
                            break;
                        }
                        catch (TxMaintenanceException)
                        {
                            Thread.Sleep(WAIT_TIME);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is SocketException || ex is IOException)
                    {
                    }
                    else throw;
                }
            }

            return Int32.Parse(result.ToString());
        }

        public void Write(int value)
        {
            IStorageServer storageServer;
              
            string[] urls = PadiDstm.urlUpdator.checkUpdates(id);
            if (urls != null)
            {
                serversUrl = urls;
            }

            foreach (String server in serversUrl)
            {
                try
                {
                    storageServer = (IStorageServer)Activator.GetObject(typeof(IStorageServer), server);                    
                    while (true)
                    {
                        try
                        {
                            storageServer.Store(PadiDstm.TID, id, value);
                            break;
                        }
                        catch (TxMaintenanceException)
                        {
                            Thread.Sleep(WAIT_TIME);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    if (ex is SocketException || ex is IOException)
                    {
                    }
                    else throw;
                }
            }
   
        }

    }

}