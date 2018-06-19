using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
namespace z.Foundation.LogicInvoke
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal interface IWCFServiceChannel : IWCFService, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal partial class WCFServiceClient : System.ServiceModel.ClientBase<IWCFService>, IWCFService
    {

        public WCFServiceClient()
        {
        }

        public WCFServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public WCFServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public WCFServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public WCFServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public string Execute(string serializeString)
        {
            return base.Channel.Execute(serializeString);
        }

        public System.Threading.Tasks.Task<string> ExecuteAsync(string serializeString)
        {
            return base.Channel.ExecuteAsync(serializeString);
        }
    }
}