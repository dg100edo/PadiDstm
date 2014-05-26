using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM_Lib
{


    public class DiaryElement
    {
        public List<string> urls;
        public TransCoordStates state;

        public DiaryElement()
        {
            urls = new List<string>();
            state = TransCoordStates.Begin;
        }

    }

    public class TransactionCoordinator : ITransactionCoordinator
    {
        private long nextTid;
        private Dictionary<long, DiaryElement> diary;

        public TransactionCoordinator()
        {
            diary = new Dictionary<long, DiaryElement>();
            nextTid = 0;
        }

        public void replaceUrlOfTransactions(string failedServerUrl, string newServerUrl)
        {
            foreach (var diaryElements in diary.Values)
            {
                if (diaryElements.urls.Contains(failedServerUrl))
                {
                    diaryElements.urls.Remove(failedServerUrl);
                    diaryElements.urls.Add(newServerUrl);
                }
            }
        }

        public long NewTid()
        {
            return nextTid++;
        }

        public long TxBegin()
        {
            lock (this)
            {
                long tid = NewTid();
                diary[tid] = new DiaryElement();
                return tid;
            }
        }

        public void JoinTransaction(long tid, string serverUrl)
        {
            lock (this)
            {
                diary[tid].urls.Add(serverUrl);
            }
        }

        public bool TxCommit(long tid)
        {
            lock (this)
            {
                bool everyoneCanCommit = true;
                
                List<string> urlsList = new List<string>(diary[tid].urls);
                foreach (string serverUrl in diary[tid].urls)
                {
                    try
                    {
                        ITransactionParticipant participant = (ITransactionParticipant)Activator.GetObject(typeof(ITransactionParticipant), serverUrl);
                        everyoneCanCommit = everyoneCanCommit && participant.canCommit(tid);
                    }
                    catch (Exception ex)
                    {
                        if (ex is SocketException || ex is IOException)
                        {
                            urlsList.Remove(serverUrl);
                        }
                        else throw;
                    }
                }


                if (everyoneCanCommit)
                {
                    diary[tid].state = TransCoordStates.Commit;
                    foreach (string serverUrl in urlsList)
                    {
                        try
                        {
                            ITransactionParticipant participant = (ITransactionParticipant)Activator.GetObject(typeof(ITransactionParticipant), serverUrl);
                            participant.TxCommit_participant(tid);
                        }
                        catch (Exception ex)
                        {
                            if (ex is SocketException || ex is IOException)
                            {
                            }
                            else throw;
                        }
                    }
                    return true;
                }
                else
                {
                    diary[tid].state = TransCoordStates.Abort;
                    foreach (string serverUrl in urlsList)
                    {
                        try
                        {
                            ITransactionParticipant participant = (ITransactionParticipant)Activator.GetObject(typeof(ITransactionParticipant), serverUrl);
                            participant.TxAbort_participant(tid);
                        }
                        catch (Exception ex)
                        {
                            if (ex is SocketException || ex is IOException)
                            {
                            }
                            else throw;
                        }
                    }
                    return false;
                }
            }
        }

        public bool TxAbort(long tid)
        {
            lock (this)
            {
                diary[tid].state = TransCoordStates.Abort;
                foreach (string serverUrl in diary[tid].urls)
                {
                    try
                    {
                        ITransactionParticipant participant = (ITransactionParticipant)Activator.GetObject(typeof(ITransactionParticipant), serverUrl);
                        participant.TxAbort_participant(tid);
                    }
                    catch (Exception ex)
                    {
                        if (ex is SocketException || ex is IOException)
                        {
                        }
                        else throw;
                    }
                }

                return true;
            }
        }

        public bool haveCommited(long tid, string serverUrl)
        {
            throw new NotImplementedException();
        }

        public bool getDecision(long tid)
        {
            throw new NotImplementedException();
        }

        public void DumpStatus()
        {
            string result = "\r\n\tCoordinator " + "\r\n\r\n";
            result += "# of Transactions: " + diary.Keys.Count + "\r\n";
            foreach (long diary_key in diary.Keys)
            {
                result += "TID: " + diary_key + "\r\n" + "Servers' urls:" + "\r\n";
                foreach (string url in diary[diary_key].urls)
                {
                    result += "\t" + url + "\r\n";
                }
            }
            Console.WriteLine(result);
        }
    }
}
