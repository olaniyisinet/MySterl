﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ussdjobsInterbank8.ibs {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ibs.IBSServicesSoap")]
    public interface IBSServicesSoap {
        
        // CODEGEN: Generating message contract since element name request from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBSBridge", ReplyAction="*")]
        ussdjobsInterbank8.ibs.IBSBridgeResponse IBSBridge(ussdjobsInterbank8.ibs.IBSBridgeRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBSBridge", ReplyAction="*")]
        System.Threading.Tasks.Task<ussdjobsInterbank8.ibs.IBSBridgeResponse> IBSBridgeAsync(ussdjobsInterbank8.ibs.IBSBridgeRequest request);
        
        // CODEGEN: Generating message contract since element name request from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBSBridgeNE", ReplyAction="*")]
        ussdjobsInterbank8.ibs.IBSBridgeNEResponse IBSBridgeNE(ussdjobsInterbank8.ibs.IBSBridgeNERequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBSBridgeNE", ReplyAction="*")]
        System.Threading.Tasks.Task<ussdjobsInterbank8.ibs.IBSBridgeNEResponse> IBSBridgeNEAsync(ussdjobsInterbank8.ibs.IBSBridgeNERequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class IBSBridgeRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="IBSBridge", Namespace="http://tempuri.org/", Order=0)]
        public ussdjobsInterbank8.ibs.IBSBridgeRequestBody Body;
        
        public IBSBridgeRequest() {
        }
        
        public IBSBridgeRequest(ussdjobsInterbank8.ibs.IBSBridgeRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class IBSBridgeRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string request;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public int Appid;
        
        public IBSBridgeRequestBody() {
        }
        
        public IBSBridgeRequestBody(string request, int Appid) {
            this.request = request;
            this.Appid = Appid;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class IBSBridgeResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="IBSBridgeResponse", Namespace="http://tempuri.org/", Order=0)]
        public ussdjobsInterbank8.ibs.IBSBridgeResponseBody Body;
        
        public IBSBridgeResponse() {
        }
        
        public IBSBridgeResponse(ussdjobsInterbank8.ibs.IBSBridgeResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class IBSBridgeResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string IBSBridgeResult;
        
        public IBSBridgeResponseBody() {
        }
        
        public IBSBridgeResponseBody(string IBSBridgeResult) {
            this.IBSBridgeResult = IBSBridgeResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class IBSBridgeNERequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="IBSBridgeNE", Namespace="http://tempuri.org/", Order=0)]
        public ussdjobsInterbank8.ibs.IBSBridgeNERequestBody Body;
        
        public IBSBridgeNERequest() {
        }
        
        public IBSBridgeNERequest(ussdjobsInterbank8.ibs.IBSBridgeNERequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class IBSBridgeNERequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string request;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public int Appid;
        
        public IBSBridgeNERequestBody() {
        }
        
        public IBSBridgeNERequestBody(string request, int Appid) {
            this.request = request;
            this.Appid = Appid;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class IBSBridgeNEResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="IBSBridgeNEResponse", Namespace="http://tempuri.org/", Order=0)]
        public ussdjobsInterbank8.ibs.IBSBridgeNEResponseBody Body;
        
        public IBSBridgeNEResponse() {
        }
        
        public IBSBridgeNEResponse(ussdjobsInterbank8.ibs.IBSBridgeNEResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class IBSBridgeNEResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string IBSBridgeNEResult;
        
        public IBSBridgeNEResponseBody() {
        }
        
        public IBSBridgeNEResponseBody(string IBSBridgeNEResult) {
            this.IBSBridgeNEResult = IBSBridgeNEResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBSServicesSoapChannel : ussdjobsInterbank8.ibs.IBSServicesSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BSServicesSoapClient : System.ServiceModel.ClientBase<ussdjobsInterbank8.ibs.IBSServicesSoap>, ussdjobsInterbank8.ibs.IBSServicesSoap {
        
        public BSServicesSoapClient() {
        }
        
        public BSServicesSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public BSServicesSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BSServicesSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BSServicesSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ussdjobsInterbank8.ibs.IBSBridgeResponse ussdjobsInterbank8.ibs.IBSServicesSoap.IBSBridge(ussdjobsInterbank8.ibs.IBSBridgeRequest request) {
            return base.Channel.IBSBridge(request);
        }
        
        public string IBSBridge(string request, int Appid) {
            ussdjobsInterbank8.ibs.IBSBridgeRequest inValue = new ussdjobsInterbank8.ibs.IBSBridgeRequest();
            inValue.Body = new ussdjobsInterbank8.ibs.IBSBridgeRequestBody();
            inValue.Body.request = request;
            inValue.Body.Appid = Appid;
            ussdjobsInterbank8.ibs.IBSBridgeResponse retVal = ((ussdjobsInterbank8.ibs.IBSServicesSoap)(this)).IBSBridge(inValue);
            return retVal.Body.IBSBridgeResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ussdjobsInterbank8.ibs.IBSBridgeResponse> ussdjobsInterbank8.ibs.IBSServicesSoap.IBSBridgeAsync(ussdjobsInterbank8.ibs.IBSBridgeRequest request) {
            return base.Channel.IBSBridgeAsync(request);
        }
        
        public System.Threading.Tasks.Task<ussdjobsInterbank8.ibs.IBSBridgeResponse> IBSBridgeAsync(string request, int Appid) {
            ussdjobsInterbank8.ibs.IBSBridgeRequest inValue = new ussdjobsInterbank8.ibs.IBSBridgeRequest();
            inValue.Body = new ussdjobsInterbank8.ibs.IBSBridgeRequestBody();
            inValue.Body.request = request;
            inValue.Body.Appid = Appid;
            return ((ussdjobsInterbank8.ibs.IBSServicesSoap)(this)).IBSBridgeAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ussdjobsInterbank8.ibs.IBSBridgeNEResponse ussdjobsInterbank8.ibs.IBSServicesSoap.IBSBridgeNE(ussdjobsInterbank8.ibs.IBSBridgeNERequest request) {
            return base.Channel.IBSBridgeNE(request);
        }
        
        public string IBSBridgeNE(string request, int Appid) {
            ussdjobsInterbank8.ibs.IBSBridgeNERequest inValue = new ussdjobsInterbank8.ibs.IBSBridgeNERequest();
            inValue.Body = new ussdjobsInterbank8.ibs.IBSBridgeNERequestBody();
            inValue.Body.request = request;
            inValue.Body.Appid = Appid;
            ussdjobsInterbank8.ibs.IBSBridgeNEResponse retVal = ((ussdjobsInterbank8.ibs.IBSServicesSoap)(this)).IBSBridgeNE(inValue);
            return retVal.Body.IBSBridgeNEResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ussdjobsInterbank8.ibs.IBSBridgeNEResponse> ussdjobsInterbank8.ibs.IBSServicesSoap.IBSBridgeNEAsync(ussdjobsInterbank8.ibs.IBSBridgeNERequest request) {
            return base.Channel.IBSBridgeNEAsync(request);
        }
        
        public System.Threading.Tasks.Task<ussdjobsInterbank8.ibs.IBSBridgeNEResponse> IBSBridgeNEAsync(string request, int Appid) {
            ussdjobsInterbank8.ibs.IBSBridgeNERequest inValue = new ussdjobsInterbank8.ibs.IBSBridgeNERequest();
            inValue.Body = new ussdjobsInterbank8.ibs.IBSBridgeNERequestBody();
            inValue.Body.request = request;
            inValue.Body.Appid = Appid;
            return ((ussdjobsInterbank8.ibs.IBSServicesSoap)(this)).IBSBridgeNEAsync(inValue);
        }
    }
}
