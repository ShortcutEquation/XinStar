using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.LogicInvoke
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal interface ILongWCFServiceChannel : ILongWCFService, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal partial class LongWCFServiceClient : System.ServiceModel.ClientBase<ILongWCFService>, ILongWCFService
    {

        public LongWCFServiceClient(System.ServiceModel.InstanceContext callbackInstance)
            :base(callbackInstance)
        {
        }

        public LongWCFServiceClient(System.ServiceModel.InstanceContext callbackInstance,string endpointConfigurationName) :
            base(callbackInstance,endpointConfigurationName)
        {
        }

        public LongWCFServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) :
            base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public LongWCFServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public LongWCFServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(callbackInstance, binding, remoteAddress)
        {
        }

        public void Execute(string serializeString)
        {
            base.Channel.Execute(serializeString);
        }

        public string ExecuteShort(string serializeString)
        {
            return base.Channel.ExecuteShort(serializeString);
        }

        public void UpdateAlive(int maxThreadCount)
        {
             base.Channel.UpdateAlive(maxThreadCount);
        }

        public void FinishTask()
        {
            base.Channel.FinishTask();
        }
    }
}
