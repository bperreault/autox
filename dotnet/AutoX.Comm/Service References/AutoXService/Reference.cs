﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutoX.Comm.AutoXService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AutoXService.ServiceSoap")]
    public interface ServiceSoap {
        
        // CODEGEN: Generating message contract since element name input from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Hello", ReplyAction="*")]
        AutoX.Comm.AutoXService.HelloResponse Hello(AutoX.Comm.AutoXService.HelloRequest request);
        
        // CODEGEN: Generating message contract since element name xmlFormatCommand from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Command", ReplyAction="*")]
        AutoX.Comm.AutoXService.CommandResponse Command(AutoX.Comm.AutoXService.CommandRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class HelloRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Hello", Namespace="http://tempuri.org/", Order=0)]
        public AutoX.Comm.AutoXService.HelloRequestBody Body;
        
        public HelloRequest() {
        }
        
        public HelloRequest(AutoX.Comm.AutoXService.HelloRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class HelloRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string input;
        
        public HelloRequestBody() {
        }
        
        public HelloRequestBody(string input) {
            this.input = input;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class HelloResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="HelloResponse", Namespace="http://tempuri.org/", Order=0)]
        public AutoX.Comm.AutoXService.HelloResponseBody Body;
        
        public HelloResponse() {
        }
        
        public HelloResponse(AutoX.Comm.AutoXService.HelloResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class HelloResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string HelloResult;
        
        public HelloResponseBody() {
        }
        
        public HelloResponseBody(string HelloResult) {
            this.HelloResult = HelloResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class CommandRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Command", Namespace="http://tempuri.org/", Order=0)]
        public AutoX.Comm.AutoXService.CommandRequestBody Body;
        
        public CommandRequest() {
        }
        
        public CommandRequest(AutoX.Comm.AutoXService.CommandRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class CommandRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string xmlFormatCommand;
        
        public CommandRequestBody() {
        }
        
        public CommandRequestBody(string xmlFormatCommand) {
            this.xmlFormatCommand = xmlFormatCommand;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class CommandResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="CommandResponse", Namespace="http://tempuri.org/", Order=0)]
        public AutoX.Comm.AutoXService.CommandResponseBody Body;
        
        public CommandResponse() {
        }
        
        public CommandResponse(AutoX.Comm.AutoXService.CommandResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class CommandResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string CommandResult;
        
        public CommandResponseBody() {
        }
        
        public CommandResponseBody(string CommandResult) {
            this.CommandResult = CommandResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ServiceSoapChannel : AutoX.Comm.AutoXService.ServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceSoapClient : System.ServiceModel.ClientBase<AutoX.Comm.AutoXService.ServiceSoap>, AutoX.Comm.AutoXService.ServiceSoap {
        
        public ServiceSoapClient() {
        }
        
        public ServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        AutoX.Comm.AutoXService.HelloResponse AutoX.Comm.AutoXService.ServiceSoap.Hello(AutoX.Comm.AutoXService.HelloRequest request) {
            return base.Channel.Hello(request);
        }
        
        public string Hello(string input) {
            AutoX.Comm.AutoXService.HelloRequest inValue = new AutoX.Comm.AutoXService.HelloRequest();
            inValue.Body = new AutoX.Comm.AutoXService.HelloRequestBody();
            inValue.Body.input = input;
            AutoX.Comm.AutoXService.HelloResponse retVal = ((AutoX.Comm.AutoXService.ServiceSoap)(this)).Hello(inValue);
            return retVal.Body.HelloResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        AutoX.Comm.AutoXService.CommandResponse AutoX.Comm.AutoXService.ServiceSoap.Command(AutoX.Comm.AutoXService.CommandRequest request) {
            return base.Channel.Command(request);
        }
        
        public string Command(string xmlFormatCommand) {
            AutoX.Comm.AutoXService.CommandRequest inValue = new AutoX.Comm.AutoXService.CommandRequest();
            inValue.Body = new AutoX.Comm.AutoXService.CommandRequestBody();
            inValue.Body.xmlFormatCommand = xmlFormatCommand;
            AutoX.Comm.AutoXService.CommandResponse retVal = ((AutoX.Comm.AutoXService.ServiceSoap)(this)).Command(inValue);
            return retVal.Body.CommandResult;
        }
    }
}
