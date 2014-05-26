using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Threading;

using System.Runtime.Serialization;

using padi_dstm_exceptions;
using System.Security.Permissions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PADI_DSTM_Lib
{
    [Serializable]
    public class TransactionParticipant : ITransactionParticipant, ISerializable
    {
        [Serializable]
        public class Transaction_info : ISerializable
        {
            public TransPartiStates state;
            public List<int> transactionObjects; // list of objects ids

            public Transaction_info()
            {
                transactionObjects = new List<int>();
                state = TransPartiStates.Ready;
            }

            // SERIALIZATION BEGIN
            [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter
= true)]
            protected Transaction_info(SerializationInfo info, StreamingContext context)
            {

                state = (TransPartiStates)info.GetValue("state", typeof(TransPartiStates));
                transactionObjects = (List<int>)info.GetValue("transactionObjects", typeof(List<int>));

            }

            [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter
      = true)]
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("state", state);
                info.AddValue("transactionObjects", transactionObjects);
            }
            // SERIALIZATION END
        }

        private string url;
        private ITransactionCoordinator coordinator;

        private Dictionary<int, ObjectManager> objects; // object id - Object manager
        private Dictionary<long, Transaction_info> transactions; //key: tid

        public TransactionParticipant(string url, string coordinatorUrl)
        {
            this.url = url;
            coordinator = (ITransactionCoordinator)Activator.GetObject(typeof(ITransactionCoordinator), coordinatorUrl);
            objects = new Dictionary<int, ObjectManager>();
            transactions = new Dictionary<long, Transaction_info>();
        }

        // SERIALIZATION BEGIN
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter
= true)]
        protected TransactionParticipant(SerializationInfo info, StreamingContext context)
        {
            url = (string)info.GetValue("url", typeof(string));
            coordinator = (ITransactionCoordinator)info.GetValue("coordinator", typeof(ITransactionCoordinator));
            objects = (Dictionary<int, ObjectManager>)info.GetValue("objects", typeof(Dictionary<int, ObjectManager>));
            transactions = (Dictionary<long, Transaction_info>)info.GetValue("transactions", typeof(Dictionary<long, Transaction_info>));
        }
       
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter
        = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("url", url);
            info.AddValue("coordinator", coordinator);
            info.AddValue("objects", objects);
            info.AddValue("transactions", transactions);
        }
        // SERIALIZATION END


        public void setUrl(String url)
        {
            this.url = url;
        }


        public void DumpStatus()
        {

                String result = "----------------------------------------\r\n";
                result += "\r\n\tTransaction Participant \r\n\r\n# Saved Objects: " + objects.Keys.Count + "\r\n" + "Saved Objects IDs: {";
                foreach (int tid in objects.Keys)
                {
                    result += " " + tid;
                }
                result += "}" + "\r\n";
                result += "# Transactions: " + transactions.Keys.Count + "\r\n";
                foreach (long trans_key in transactions.Keys)
                {
                    result += "TID: " + trans_key;
                    result += " State: " + transactions[trans_key].state + "\r\n";
                }
                result += "----------------------------------------\r\n";
                Console.WriteLine(result);

        }

        public bool canCommit(long tid)
        {
            return transactions[tid].state == TransPartiStates.Ready;
        }

        public bool TxCommit_participant(long tid)
        {
            foreach (int i in transactions[tid].transactionObjects)
                objects[i].commit(tid);
            transactions[tid].state = TransPartiStates.Commit;
            return true;
        }

        public bool TxAbort_participant(long tid)
        {
            foreach (int i in transactions[tid].transactionObjects)
                objects[i].abort(tid);
            transactions[tid].state = TransPartiStates.Abort;
            return true;
        }

        public bool TxPrepare(long tid)
        {
            return true;
        }

        public Object Load(long tid, int id)
        {
            ObjectManager objectManager;
            List<int> transactionObjects;
            lock (this)
            {
                try
                {
                    objectManager = objects[id];
                }
                catch (KeyNotFoundException)
                {
                    objectManager = new ObjectManager();
                    objects[id] = objectManager;
                }
                try
                {
                    transactionObjects = transactions[tid].transactionObjects;
                }
                catch (KeyNotFoundException)
                {
                    coordinator.JoinTransaction(tid, url);
                    Transaction_info transInfo = new Transaction_info();
                    transactions[tid] = transInfo;
                }

                if (!transactions[tid].transactionObjects.Contains(id))
                    transactions[tid].transactionObjects.Add(id);
            }
            return objectManager.read(tid);
        }

        public void Store(long tid, int id, Object value)
        {

            ObjectManager objectManager;
            List<int> transactionObjects;
            lock (this)
            {
                try
                {
                    objectManager = objects[id];
                }
                catch (KeyNotFoundException)
                {
                    objectManager = new ObjectManager();
                    objects[id] = objectManager;
                }
                try
                {
                    transactionObjects = transactions[tid].transactionObjects;
                }
                catch (KeyNotFoundException)
                {
                    coordinator.JoinTransaction(tid, url);
                    Transaction_info transInfo = new Transaction_info();
                    transactions[tid] = transInfo; ;
                }
                if (!transactions[tid].transactionObjects.Contains(id))
                    transactions[tid].transactionObjects.Add(id);

                try
                {
                    objectManager.write(tid, Int32.Parse(value.ToString()));
                }
                catch (TxException)
                {
                    transactions[tid].state = TransPartiStates.Abort;
                }
                Monitor.PulseAll(this);
            }
        }
    }

    [Serializable]
    public class ObjectManager : ISerializable
    {

        [Serializable]
        struct PreWrite : ISerializable
        {
            public Interval interval;
            public int value;

            // SERIALIZATION BEGIN
            public PreWrite(SerializationInfo info, StreamingContext context) {
                interval = (Interval)info.GetValue("interval", typeof(Interval));
                value = (int) info.GetValue("value", typeof(int));          
            }

            [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter
= true)]
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("interval", interval);
                info.AddValue("value", value);
            }
            // SERIALIZATION END
        }

        [Serializable]
        struct Interval : ISerializable
        {
            public long min;
            public long max;
 
            // SERIALIZATION BEGIN
            public Interval(SerializationInfo info, StreamingContext context) {
                min = (long)info.GetValue("min", typeof(long));
                max = (long) info.GetValue("max", typeof(long));          
            }

            [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter
= true)]
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("min", min);
                info.AddValue("max", max);
            }
            // SERIALIZATION END
      

        }

        ArrayList read_timestamps;
        SortedDictionary<long, int> write_timestamps; //key: timestamp
        Dictionary<long, PreWrite> pre_writes;

        public ObjectManager()
        {
            read_timestamps = new ArrayList();
            write_timestamps = new SortedDictionary<long, int>();
            pre_writes = new Dictionary<long, PreWrite>();
        }

        // SERIALIZATION BEGIN
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter
= true)]
        protected ObjectManager(SerializationInfo info, StreamingContext context)
        {
            read_timestamps = (ArrayList)info.GetValue("read_timestamps", typeof(ArrayList));
            write_timestamps = (SortedDictionary<long, int>)info.GetValue("write_timestamps", typeof(SortedDictionary<long, int>));
            pre_writes = (Dictionary<long, PreWrite>)info.GetValue("pre_writes", typeof(Dictionary<long, PreWrite>));
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter
 = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("read_timestamps", read_timestamps);
            info.AddValue("write_timestamps", write_timestamps);
            info.AddValue("pre_writes", pre_writes);
        }
        // SERIALIZATION END

        public int read(long timestamp)
        {
            int return_value;
            lock (this)
            {
                read_timestamps.Add(timestamp);
                while (timestamp_in_pre_writes_intervals(timestamp))
                {
                    Monitor.Wait(this); //goes to buffer, blocks itself
                }
                if (pre_writes.ContainsKey(timestamp))
                {
                    return_value = pre_writes[timestamp].value;
                }
                else
                {
                    return_value = write_timestamps.Last(ts => ts.Key < timestamp).Value; //returns the value com maior ts < timestamp
                }
                Monitor.PulseAll(this);
            }
            return return_value;        
        }

        public void write(long timestamp, int value)
        {
            lock (this)
            {
                Interval interval = calculate_interval(timestamp);
                if (exists_read_timestamp_in(interval))
                {
                    throw new TxException("pre_write abort");
                    //Reject, manda excepçao, faz abort a transaccao
                }
                //adiciona o seu intervalo à estrutura dos intervalos do prewrites que estão bloqueados
                PreWrite prewrite;
                prewrite.interval = interval;
                prewrite.value = value;
                pre_writes[timestamp] = prewrite;
            }
        }

        public void commit(long timestamp)
        {
            lock (this)
            {
                //release  o seu pre-write, removendo o seu intervalo da estrutura dos intervalos do prewrites que estão bloqueados
                if (pre_writes.ContainsKey(timestamp))
                {
                    int value = pre_writes[timestamp].value;
                    pre_writes.Remove(timestamp);
                    write_timestamps[timestamp] = value;
                }
                Monitor.PulseAll(this);//fazer com que os reads, testem outra vez a sua condiçao
            }
        }

        public void abort(long timestamp)
        {
            lock (this)
            {
                pre_writes.Remove(timestamp);
                Monitor.PulseAll(this);
            }
        }

        private bool timestamp_in_pre_writes_intervals(long timestamp)
        {
            foreach (PreWrite pre in pre_writes.Values)
            {
                if (pre.interval.min > timestamp && timestamp < pre.interval.max)
                    return true;
            }
            return false;
        }

        private Interval calculate_interval(long timestamp)
        {
            Interval interval;
            interval.min = timestamp;
            try
            {
                interval.max = write_timestamps.First(ts => ts.Key > timestamp).Value; //smallest W-ts(x) > ts(P)
            }
            catch (InvalidOperationException)
            {
                interval.max = Int64.MaxValue;
            }

            return interval;
        }

        private bool exists_read_timestamp_in(Interval interval)
        {
            foreach (long read_ts in read_timestamps)
            {
                if (read_ts > interval.min && read_ts < interval.max)
                    return true;
            }
            return false;
        }
    }
}
