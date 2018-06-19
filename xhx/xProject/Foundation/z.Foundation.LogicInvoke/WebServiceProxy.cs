using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace z.Foundation.LogicInvoke
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "WebServiceServerSoap", Namespace = "http://tempuri.org/")]
    internal partial class WebServiceServer : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback ExecuteOperationCompleted;

        /// <remarks/>
        public WebServiceServer()
        {
            this.Url = "http://localhost:7327/WebServiceServer.asmx";
        }

        /// <remarks/>
        public event ExecuteCompletedEventHandler ExecuteCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Execute", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Execute(string serializeString)
        {
            object[] results = this.Invoke("Execute", new object[] {
                    serializeString});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginExecute(string serializeString, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Execute", new object[] {
                    serializeString}, callback, asyncState);
        }

        /// <remarks/>
        public string EndExecute(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void ExecuteAsync(string serializeString)
        {
            this.ExecuteAsync(serializeString, null);
        }

        /// <remarks/>
        public void ExecuteAsync(string serializeString, object userState)
        {
            if ((this.ExecuteOperationCompleted == null))
            {
                this.ExecuteOperationCompleted = new System.Threading.SendOrPostCallback(this.OnExecuteOperationCompleted);
            }
            this.InvokeAsync("Execute", new object[] {
                    serializeString}, this.ExecuteOperationCompleted, userState);
        }

        private void OnExecuteOperationCompleted(object arg)
        {
            if ((this.ExecuteCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ExecuteCompleted(this, new ExecuteCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    internal delegate void ExecuteCompletedEventHandler(object sender, ExecuteCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    internal partial class ExecuteCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal ExecuteCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}


