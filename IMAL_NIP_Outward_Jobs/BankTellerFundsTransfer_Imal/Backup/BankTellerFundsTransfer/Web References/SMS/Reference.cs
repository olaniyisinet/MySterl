﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5448
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 2.0.50727.5448.
// 
#pragma warning disable 1591

namespace BankTellerFundsTransfer.SMS {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.5420")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="smsSenderSoap", Namespace="http://tempuri.org/")]
    public partial class smsSender : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback sendMessageOperationCompleted;
        
        private System.Threading.SendOrPostCallback sendMessageLiveOldOperationCompleted;
        
        private System.Threading.SendOrPostCallback sendMessageLiveOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public smsSender() {
            this.Url = global::BankTellerFundsTransfer.Properties.Settings.Default.BankTellerFundsTransfer_SMS_smsSender;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event sendMessageCompletedEventHandler sendMessageCompleted;
        
        /// <remarks/>
        public event sendMessageLiveOldCompletedEventHandler sendMessageLiveOldCompleted;
        
        /// <remarks/>
        public event sendMessageLiveCompletedEventHandler sendMessageLiveCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/sendMessage", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string sendMessage(string msg) {
            object[] results = this.Invoke("sendMessage", new object[] {
                        msg});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void sendMessageAsync(string msg) {
            this.sendMessageAsync(msg, null);
        }
        
        /// <remarks/>
        public void sendMessageAsync(string msg, object userState) {
            if ((this.sendMessageOperationCompleted == null)) {
                this.sendMessageOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsendMessageOperationCompleted);
            }
            this.InvokeAsync("sendMessage", new object[] {
                        msg}, this.sendMessageOperationCompleted, userState);
        }
        
        private void OnsendMessageOperationCompleted(object arg) {
            if ((this.sendMessageCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.sendMessageCompleted(this, new sendMessageCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/sendMessageLiveOld", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string sendMessageLiveOld(string msg, string requester) {
            object[] results = this.Invoke("sendMessageLiveOld", new object[] {
                        msg,
                        requester});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void sendMessageLiveOldAsync(string msg, string requester) {
            this.sendMessageLiveOldAsync(msg, requester, null);
        }
        
        /// <remarks/>
        public void sendMessageLiveOldAsync(string msg, string requester, object userState) {
            if ((this.sendMessageLiveOldOperationCompleted == null)) {
                this.sendMessageLiveOldOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsendMessageLiveOldOperationCompleted);
            }
            this.InvokeAsync("sendMessageLiveOld", new object[] {
                        msg,
                        requester}, this.sendMessageLiveOldOperationCompleted, userState);
        }
        
        private void OnsendMessageLiveOldOperationCompleted(object arg) {
            if ((this.sendMessageLiveOldCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.sendMessageLiveOldCompleted(this, new sendMessageLiveOldCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/sendMessageLive", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string sendMessageLive(string msg, string requester) {
            object[] results = this.Invoke("sendMessageLive", new object[] {
                        msg,
                        requester});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void sendMessageLiveAsync(string msg, string requester) {
            this.sendMessageLiveAsync(msg, requester, null);
        }
        
        /// <remarks/>
        public void sendMessageLiveAsync(string msg, string requester, object userState) {
            if ((this.sendMessageLiveOperationCompleted == null)) {
                this.sendMessageLiveOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsendMessageLiveOperationCompleted);
            }
            this.InvokeAsync("sendMessageLive", new object[] {
                        msg,
                        requester}, this.sendMessageLiveOperationCompleted, userState);
        }
        
        private void OnsendMessageLiveOperationCompleted(object arg) {
            if ((this.sendMessageLiveCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.sendMessageLiveCompleted(this, new sendMessageLiveCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.5420")]
    public delegate void sendMessageCompletedEventHandler(object sender, sendMessageCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.5420")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class sendMessageCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal sendMessageCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.5420")]
    public delegate void sendMessageLiveOldCompletedEventHandler(object sender, sendMessageLiveOldCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.5420")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class sendMessageLiveOldCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal sendMessageLiveOldCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.5420")]
    public delegate void sendMessageLiveCompletedEventHandler(object sender, sendMessageLiveCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.5420")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class sendMessageLiveCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal sendMessageLiveCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591