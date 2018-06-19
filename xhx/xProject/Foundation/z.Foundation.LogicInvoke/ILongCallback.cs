using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.LogicInvoke
{
    public interface ILongCallback
    {
        [OperationContract(IsOneWay = true)]
        void Callback(object obj);

        [OperationContract(IsOneWay = true)]
        void Completed(object obj);

        //[OperationContract(IsOneWay = true)]
        //void Heartbeat();
    }
}
