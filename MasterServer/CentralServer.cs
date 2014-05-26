using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

using System.Collections;

using PADI_DSTM_Lib;
using System.Runtime.Serialization.Formatters;
using System.Net.Sockets;

using padi_dstm_exceptions;

namespace CentralServer{

    class CentralServer : MarshalByRefObject, ICentralServer, IStorageServer, ITransactionCoordinator, ITransactionParticipant, IMaintenance
    {

        private SlavesManager slavesManager;
        private TransactionCoordinator transactionCoordinator;
        private TransactionParticipant transactionParticipant;

        private bool maintenance;

        
        public CentralServer() {
            slavesManager = new SlavesManager(CENTRAL_SERVER.URL, this);
            transactionCoordinator = new TransactionCoordinator();
            maintenance = false;
        }

        // ICentralServer implementation
        public void AddStorageServer(string url) {
            Console.WriteLine("New server: " + url);
            slavesManager.AddStorageServer(url);
        }

        public string [] ObjectAssignServers(string clientUrl, int id) {
            return slavesManager.ObjectAssignServers(clientUrl, id);
        }

        public string[] ObjectServers(string clientUrl, int id) {
            return slavesManager.ObjectServers(clientUrl, id);
        }

        public void Status() {
            //checkFreezeOrFail();
           lock (this)
            {
                foreach (String server in slavesManager.GetStorageServersUrl())
                {
                    IStorageServer storageServer = (IStorageServer)Activator.GetObject(typeof(IStorageServer), server);
                    storageServer.DumpStatus();
                }
            }
        }

        public void DumpStatus()
        {
       
                Console.WriteLine("----------------------------------------\r\n");
                Console.WriteLine("\n# of Storage Servers: " + slavesManager.GetStorageServersUrl().Count);
                transactionCoordinator.DumpStatus();
                transactionParticipant.DumpStatus();
 
        }


        public void setTransactionPartipanct(TransactionParticipant transactionParticipant){
            this.transactionParticipant = transactionParticipant;
        }


        // ITransactionCoordinator implementation
        public long TxBegin() {
            Console.WriteLine("TxBegin (coordinator)");
            return transactionCoordinator.TxBegin();
        }

        public bool TxCommit(long tid) {
            Console.WriteLine("TxCommit tid: {0} (coordinator)", tid);
            return transactionCoordinator.TxCommit(tid);
        }

        public bool TxAbort(long tid) {
            Console.WriteLine("TxAbort tid: {0} (coordinator)", tid);
            return transactionCoordinator.TxAbort(tid);
        }

        public void JoinTransaction(long tid, string serverUrl) {
            Console.WriteLine("JoinTransaction tid: {0} Server: {1}", tid, serverUrl);
            transactionCoordinator.JoinTransaction(tid, serverUrl);
        }

        public bool getDecision(long tid){
            return transactionCoordinator.getDecision(tid);
        }

        public bool haveCommited(long tid, string serverUrl){
            return transactionCoordinator.haveCommited(tid, serverUrl);
        }


        // IStorageServer implementation
        public void Store(long tid, int id, Object value) {
            if (maintenance)
            {
                throw new TxMaintenanceException("Server in maintenance");
            }
            Console.WriteLine("Store tid={0} id={1} ", tid, id);
            transactionParticipant.Store(tid, id, value);
  
        }

        public Object Load(long tid, int id) {
            if (maintenance)
            {
                throw new TxMaintenanceException("Server in maintenance");
            }
            Console.WriteLine("Load tid={0} id={1} ", tid, id);
            return transactionParticipant.Load(tid, id);
 
        }

        //used by SlavesManager
        public TransactionParticipant GetStorageServerContext() {
                return transactionParticipant;
        }

        
        public void SetStorageServerContext(TransactionParticipant context) {
                transactionParticipant = context;
        }

        //

        // ITransactionPartipant implementation
        public bool TxCommit_participant(long tid)
        {
            Console.WriteLine("TxCommit tid={0} (participant)", tid);
            return transactionParticipant.TxCommit_participant(tid);
        }

        public bool TxAbort_participant(long tid)
        {
            Console.WriteLine("TxAbort tid={0} (participant)", tid);
            return transactionParticipant.TxAbort_participant(tid);
        }

        public bool TxPrepare(long tid)
        {
            Console.WriteLine("TxPrepare tid={0} (participant)", tid);
            return transactionParticipant.TxPrepare(tid);
        }

        public bool canCommit(long tid)
        {
            Console.WriteLine("canCommit tid={0} (participant)", tid);
            return transactionParticipant.canCommit(tid);
        }


        internal void replaceUrlOfTransactions(string failedServerUrl, string newServerUrl){
            transactionCoordinator.replaceUrlOfTransactions(failedServerUrl, newServerUrl);
        }


        public bool isAlive()
        {
            if (transactionParticipant != null)
                return true;
            else
                return false;
        }

        //IMaintenance Implm
        public void setMaintenanceStatus(bool state)
        {
            Console.WriteLine("Maintenance status was set to: " + state);
             maintenance = state;
            
        }
    }

    class Program {
        static void Main(string[] args){

            BinaryClientFormatterSinkProvider clientProvider = null;
            BinaryServerFormatterSinkProvider serverProvider =
                 new BinaryServerFormatterSinkProvider();

            serverProvider.TypeFilterLevel =
                    System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            IDictionary props = new Hashtable();
            props["port"] = CENTRAL_SERVER.PORT;
            props["typeFilterLevel"] = TypeFilterLevel.Full;
            TcpChannel channel = new TcpChannel(props, clientProvider, serverProvider);

            ChannelServices.RegisterChannel(channel, true);
            CentralServer centralServer = new CentralServer();
            RemotingServices.Marshal(centralServer, CENTRAL_SERVER.SERVICE_NAME, typeof(CentralServer));
            Console.WriteLine("<enter> para sair do central server...");
            Console.ReadKey();
        }
    }
}
