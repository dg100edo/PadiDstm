using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PADI_DSTM_Lib;
using System.Net.Sockets;
using System.IO;
using System.Collections;

namespace CentralServer
{

    class SlavesManager
    {
        private string centralServerUrl;
        private Dictionary<string, StorageServer> storageServers;
       
        private Dictionary<int, string> objectServer;
        private Dictionary<int, ArrayList> objectClients;
        //
        private int nextObjectServer;
        private int leastNumberOfObjectsStoredInAServer;

        private StorageServer masterStorageServer;
        private StorageServer orphanStorageServer;
        private CentralServer central;

        public SlavesManager(string centralServerUrl, CentralServer central)
        {
            this.centralServerUrl = centralServerUrl;
            storageServers = new Dictionary<string, StorageServer>();
            objectServer = new Dictionary<int, string>();
            nextObjectServer = 0; leastNumberOfObjectsStoredInAServer = -1;
            masterStorageServer = null;
            orphanStorageServer = null;
            this.central = central;

            objectClients = new Dictionary<int, ArrayList>();
        }

        public void AddStorageServer(string url)
        {
            lock (this)
            {
                checkEveryStorageServerAliveness();
                StorageServer newStorageServer = new StorageServer(url);
                StorageServer newStorageServerReplica;
                string urlUpdateToClient = null;

                setSystemMaintenanceState(true);
                Console.WriteLine("### ### INIT ADDING STORAGE SERVER PROCESS");
                if (orphanStorageServer != null)
                { // A server without replica exists (2 tolerance degree)

                    Console.WriteLine("### A server without replica exists - ORPHAN");
                    newStorageServerReplica = orphanStorageServer;
                    updateObjectUrls(orphanStorageServer.Replica.URL, url);
                    central.replaceUrlOfTransactions(orphanStorageServer.Replica.URL, url);
                    urlUpdateToClient = url;
                    orphanStorageServer = null;
                    
                }
                else if (masterStorageServer != null)
                { // Master is a storage server (new server occupy its position)
                    Console.WriteLine("### Master is a Storage Server - New Server will occupy its Position");
                    newStorageServerReplica = masterStorageServer.Replica;
                    storageServers.Remove(masterStorageServer.URL);
                    updateObjectUrls(masterStorageServer.URL, url);
                    urlUpdateToClient = url;                   
                    central.replaceUrlOfTransactions(masterStorageServer.URL, url);
                    masterStorageServer = null;
                    central.setTransactionPartipanct(null);
                }
                else
                { // New server does not have pair (Master will be a storage server)
                    Console.WriteLine("### New server does not have pair - Master will be a storage server");
                    masterStorageServer = new StorageServer(centralServerUrl);
                    newStorageServerReplica = masterStorageServer;
                    central.SetStorageServerContext(new TransactionParticipant(CENTRAL_SERVER.URL, CENTRAL_SERVER.URL));
                }
                newStorageServer.Replica = newStorageServerReplica;
                newStorageServerReplica.Replica = newStorageServer;

                storageServers[newStorageServer.URL] = newStorageServer;
                storageServers[newStorageServerReplica.URL] = newStorageServerReplica;

                //Copy context of of replica to new storage server
                IStorageServer storageServer = (IStorageServer)Activator.GetObject(typeof(IStorageServer), newStorageServer.URL);
                IStorageServer replica = (IStorageServer)Activator.GetObject(typeof(IStorageServer), newStorageServerReplica.URL);
                if (storageServer == null || replica == null)
                {
                    throw new ApplicationException("AddStorageServer: Impossivel ligar-se ao servidor ou a replica");
                }
                TransactionParticipant replicaContext = replica.GetStorageServerContext();
                storageServer.SetStorageServerContext(replicaContext);

                if (urlUpdateToClient != null) {
                    sendUpdatedUrlsToClients(urlUpdateToClient);            
                }
                Console.WriteLine("### updates to clients were sent");
                setSystemMaintenanceState(false);
                Console.WriteLine("### ### END ADDING STORAGE SERVER PROCESS");
            }
        }


        private void setSystemMaintenanceState(bool state){
            
            foreach(string url in storageServers.Keys){

                IMaintenance server = (IMaintenance)Activator.GetObject(typeof(IMaintenance), url);
                server.setMaintenanceStatus(state);
 
            }
         
        }

        internal void reportFailure(string url)
        {
            Console.WriteLine("####Failure of: " + url + " was reported.");
            try
            {
                orphanStorageServer = storageServers[url].Replica;
                storageServers.Remove(url);
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("## This failure has already been reported.");
            }
        }


        struct ClientToRemove {
            public int id;
            public string url;

            public ClientToRemove(int id, string url) {
                this.id = id;
                this.url = url;
            }      
        }

        private void sendUpdatedUrlsToClients(string newServerUrl) {

            Console.WriteLine("entrou sendUpdatedUrlsToClients");

            ArrayList clientsToRemove = new ArrayList();

            foreach (var obj in objectServer)
            {
                if (obj.Value.Equals(newServerUrl) || storageServers[obj.Value].Replica.URL.Equals(newServerUrl))
                {
                    foreach(string url in objectClients[obj.Key]){
                        try
                        {
                            IClient client = (IClient)Activator.GetObject(typeof(IClient), url);
                            client.UpdateUrls(obj.Key, ObjectServers(obj.Key));
                            Console.WriteLine("sendUpdatedUrlsToClients : " + obj.Key + "url1: " + ObjectServers(obj.Key)[0] + "url2: " + ObjectServers(obj.Key)[1]);
                        }
                        catch (Exception ex)
                        {
                            if (ex is SocketException || ex is IOException)
                            {
                                Console.WriteLine("Cliente remeovido");
                                clientsToRemove.Add(new ClientToRemove(obj.Key, url));
                            }
                            else throw;
                        }
                    }
                }
            }

            //remove dead clients from struct
            foreach(ClientToRemove client in clientsToRemove){
                objectClients[client.id].Remove(client.url);    
            }

        }



        private void checkEveryStorageServerAliveness()
        {
            List<string> serversList = storageServers.Keys.ToList();
            foreach (string serverUrl in serversList)
            {
                IStorageServer serverProxy = (IStorageServer)Activator.GetObject(typeof(IStorageServer), serverUrl);
                try
                {
                    if(!serverProxy.isAlive())
                        reportFailure(serverUrl);
                }
                catch (Exception ex)
                {
                    if (ex is SocketException || ex is IOException)
                    {
                        reportFailure(serverUrl);
                    }
                    else throw;
                }
            }
        }

        //DISCLAIMER: you cannot modify objects in c# collection while iterating through it
        void updateObjectUrls(string oldUrl, string newUrl)
        {
            Dictionary<int, string> copy = CloneDictionaryCloningValues(objectServer);

            foreach (var obj in objectServer)
            {
                if (obj.Value.Equals(oldUrl))
                {
                    copy[obj.Key] = newUrl;
                }
            }
            objectServer = copy;
        }

        /*************************************************/
        public static Dictionary<TKey, TValue> CloneDictionaryCloningValues<TKey, TValue>
   (Dictionary<TKey, TValue> original) where TValue : ICloneable
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count,
                                                                    original.Comparer);
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                ret.Add(entry.Key, (TValue)entry.Value.Clone());
            }
            return ret;
        }
        /*************************************************/

        public string[] ObjectAssignServers(string clientUrl, int id)
        {
            lock (this)
            {
                try
                {
                    string serverURL = objectServer[id];
                    return null;
                }
                catch (KeyNotFoundException)
                {
                    string serverURL = NextObjectServerUrl();
                    StorageServer server = storageServers[serverURL];
                    StorageServer replica = server.Replica;
                    server.NumObjects++;
                    replica.NumObjects++;
                    objectServer[id] = serverURL;
                    //
                    objectClients[id] = new ArrayList();
                    objectClients[id].Add(clientUrl);
                    //
                    return new string[] { server.URL, replica.URL };
                }
            }
        }

        public string[] ObjectServers(int id)
        {
                try
                {
                    string serverURL = objectServer[id];                  
                    StorageServer server = storageServers[serverURL];
                    StorageServer replica = server.Replica;
                    return new string[] { server.URL, replica.URL };
                }
                catch (KeyNotFoundException e)
                {
                    //DEBUG
                    Console.WriteLine("#### DEBUG, SlavesManager, ObjectServers())\n");
                    Console.WriteLine(e.StackTrace);
                    return null;
                }
        }

        public string[] ObjectServers(string clientUrl, int id)
        {
            lock (this)
            {
                    objectClients[id].Add(clientUrl);
                    return ObjectServers(id);               
            }
        }

        private string NextObjectServerUrl()
        {
            int numberOfObjectsOfCurrentServer = storageServers.ElementAt(nextObjectServer).Value.NumObjects;
            if (numberOfObjectsOfCurrentServer >= leastNumberOfObjectsStoredInAServer){
                leastNumberOfObjectsStoredInAServer = numberOfObjectsOfCurrentServer;
                nextObjectServer = (nextObjectServer + 1) % storageServers.Count;
            }
            return storageServers.ElementAt(nextObjectServer).Value.URL;
        }

        public List<string> GetStorageServersUrl()
        {
            return storageServers.Keys.ToList();
        }
    }

    class StorageServer
    {
        private string url;
        private int numObjects;
        StorageServer replica;

        public StorageServer(string url)
        {
            this.url = url;
            numObjects = 0;
        }

        public string URL
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
            }
        }

        public int NumObjects
        {
            get
            {
                return numObjects;
            }
            set
            {
                numObjects = value;
            }
        }

        public StorageServer Replica
        {
            get
            {
                return replica;
            }
            set
            {
                replica = value;
            }
        }
    }
}
