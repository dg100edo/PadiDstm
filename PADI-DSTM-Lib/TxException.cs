using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace padi_dstm_exceptions
{
    [Serializable]
    public class TxException : ApplicationException
    {

        public TxException(string msg) : base(msg) { }

        public TxException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }


    [Serializable]
    public class TxMaintenanceException : ApplicationException
    {

        public TxMaintenanceException(string msg) : base(msg) { }

        public TxMaintenanceException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }
}