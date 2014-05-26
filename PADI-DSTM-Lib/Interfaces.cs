using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Threading;

namespace PADI_DSTM_Lib
{
    public class CENTRAL_SERVER{
        public static int PORT = 8086;
        public static string SERVICE_NAME = "Server";
        public static string URL = "tcp://localhost:" + PORT + "/" + SERVICE_NAME;
    }
    
    public interface ICentralServer {
        string[] ObjectAssignServers(string clientUrl, int id);
        string[] ObjectServers(string clientUrl, int id);
        void AddStorageServer(string url);
        void Status();
    }

    public interface IStorageServer {
        void Store(long tid, int id, Object value);
        Object Load(long tid, int id);
        void DumpStatus();
        TransactionParticipant GetStorageServerContext();
        void SetStorageServerContext(TransactionParticipant context);
        bool isAlive();
    }

    public interface IMaintenance {
        void setMaintenanceStatus(bool state);
    }

    public interface IFailServer {
        void Fail();
        void Freeze();
        void Recover();
    }

    public interface ITransactionCoordinator {
        long TxBegin();
        bool TxCommit(long tid);
        bool TxAbort(long tid);
        void JoinTransaction(long tid, string serverUrl);
        bool haveCommited(long tid, string serverUrl);
        bool getDecision(long tid);

    }

    public interface ITransactionParticipant {
        bool TxCommit_participant(long tid);
        bool TxAbort_participant(long tid);
        bool canCommit(long tid);
    }

    public interface IClient {
        void UpdateUrls(int id, string[] storageServers);
    }

    public enum TransCoordStates { Begin, Commit, Abort, End}

    public enum TransPartiStates { Begin, Ready, Commit, Abort }
}
