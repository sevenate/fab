﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.488
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 5.0.61118.0
// 
namespace Fab.Client.RegistrationServiceReference {
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UserDTO", Namespace="http://schemas.datacontract.org/2004/07/Fab.Server.Core.DTO")]
    public partial class UserDTO : object, System.ComponentModel.INotifyPropertyChanged {
        
        private System.Guid IdField;
        
        private System.DateTime RegisteredField;
        
        private string ServiceUrlField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Registered {
            get {
                return this.RegisteredField;
            }
            set {
                if ((this.RegisteredField.Equals(value) != true)) {
                    this.RegisteredField = value;
                    this.RaisePropertyChanged("Registered");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ServiceUrl {
            get {
                return this.ServiceUrlField;
            }
            set {
                if ((object.ReferenceEquals(this.ServiceUrlField, value) != true)) {
                    this.ServiceUrlField = value;
                    this.RaisePropertyChanged("ServiceUrl");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="RegistrationServiceReference.IRegistrationService")]
    public interface IRegistrationService {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IRegistrationService/GenerateUniqueLogin", ReplyAction="http://tempuri.org/IRegistrationService/GenerateUniqueLoginResponse")]
        System.IAsyncResult BeginGenerateUniqueLogin(System.AsyncCallback callback, object asyncState);
        
        string EndGenerateUniqueLogin(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IRegistrationService/IsLoginAvailable", ReplyAction="http://tempuri.org/IRegistrationService/IsLoginAvailableResponse")]
        System.IAsyncResult BeginIsLoginAvailable(string login, System.AsyncCallback callback, object asyncState);
        
        bool EndIsLoginAvailable(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IRegistrationService/Register", ReplyAction="http://tempuri.org/IRegistrationService/RegisterResponse")]
        System.IAsyncResult BeginRegister(string login, string password, System.AsyncCallback callback, object asyncState);
        
        Fab.Client.RegistrationServiceReference.UserDTO EndRegister(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IRegistrationService/ResetPassword", ReplyAction="http://tempuri.org/IRegistrationService/ResetPasswordResponse")]
        System.IAsyncResult BeginResetPassword(string login, string email, System.AsyncCallback callback, object asyncState);
        
        void EndResetPassword(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IRegistrationServiceChannel : Fab.Client.RegistrationServiceReference.IRegistrationService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GenerateUniqueLoginCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GenerateUniqueLoginCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public string Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class IsLoginAvailableCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public IsLoginAvailableCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RegisterCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public RegisterCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public Fab.Client.RegistrationServiceReference.UserDTO Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((Fab.Client.RegistrationServiceReference.UserDTO)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RegistrationServiceClient : System.ServiceModel.ClientBase<Fab.Client.RegistrationServiceReference.IRegistrationService>, Fab.Client.RegistrationServiceReference.IRegistrationService {
        
        private BeginOperationDelegate onBeginGenerateUniqueLoginDelegate;
        
        private EndOperationDelegate onEndGenerateUniqueLoginDelegate;
        
        private System.Threading.SendOrPostCallback onGenerateUniqueLoginCompletedDelegate;
        
        private BeginOperationDelegate onBeginIsLoginAvailableDelegate;
        
        private EndOperationDelegate onEndIsLoginAvailableDelegate;
        
        private System.Threading.SendOrPostCallback onIsLoginAvailableCompletedDelegate;
        
        private BeginOperationDelegate onBeginRegisterDelegate;
        
        private EndOperationDelegate onEndRegisterDelegate;
        
        private System.Threading.SendOrPostCallback onRegisterCompletedDelegate;
        
        private BeginOperationDelegate onBeginResetPasswordDelegate;
        
        private EndOperationDelegate onEndResetPasswordDelegate;
        
        private System.Threading.SendOrPostCallback onResetPasswordCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public RegistrationServiceClient() {
        }
        
        public RegistrationServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public RegistrationServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RegistrationServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RegistrationServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<GenerateUniqueLoginCompletedEventArgs> GenerateUniqueLoginCompleted;
        
        public event System.EventHandler<IsLoginAvailableCompletedEventArgs> IsLoginAvailableCompleted;
        
        public event System.EventHandler<RegisterCompletedEventArgs> RegisterCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> ResetPasswordCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Fab.Client.RegistrationServiceReference.IRegistrationService.BeginGenerateUniqueLogin(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGenerateUniqueLogin(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        string Fab.Client.RegistrationServiceReference.IRegistrationService.EndGenerateUniqueLogin(System.IAsyncResult result) {
            return base.Channel.EndGenerateUniqueLogin(result);
        }
        
        private System.IAsyncResult OnBeginGenerateUniqueLogin(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((Fab.Client.RegistrationServiceReference.IRegistrationService)(this)).BeginGenerateUniqueLogin(callback, asyncState);
        }
        
        private object[] OnEndGenerateUniqueLogin(System.IAsyncResult result) {
            string retVal = ((Fab.Client.RegistrationServiceReference.IRegistrationService)(this)).EndGenerateUniqueLogin(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGenerateUniqueLoginCompleted(object state) {
            if ((this.GenerateUniqueLoginCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GenerateUniqueLoginCompleted(this, new GenerateUniqueLoginCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GenerateUniqueLoginAsync() {
            this.GenerateUniqueLoginAsync(null);
        }
        
        public void GenerateUniqueLoginAsync(object userState) {
            if ((this.onBeginGenerateUniqueLoginDelegate == null)) {
                this.onBeginGenerateUniqueLoginDelegate = new BeginOperationDelegate(this.OnBeginGenerateUniqueLogin);
            }
            if ((this.onEndGenerateUniqueLoginDelegate == null)) {
                this.onEndGenerateUniqueLoginDelegate = new EndOperationDelegate(this.OnEndGenerateUniqueLogin);
            }
            if ((this.onGenerateUniqueLoginCompletedDelegate == null)) {
                this.onGenerateUniqueLoginCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGenerateUniqueLoginCompleted);
            }
            base.InvokeAsync(this.onBeginGenerateUniqueLoginDelegate, null, this.onEndGenerateUniqueLoginDelegate, this.onGenerateUniqueLoginCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Fab.Client.RegistrationServiceReference.IRegistrationService.BeginIsLoginAvailable(string login, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginIsLoginAvailable(login, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        bool Fab.Client.RegistrationServiceReference.IRegistrationService.EndIsLoginAvailable(System.IAsyncResult result) {
            return base.Channel.EndIsLoginAvailable(result);
        }
        
        private System.IAsyncResult OnBeginIsLoginAvailable(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string login = ((string)(inValues[0]));
            return ((Fab.Client.RegistrationServiceReference.IRegistrationService)(this)).BeginIsLoginAvailable(login, callback, asyncState);
        }
        
        private object[] OnEndIsLoginAvailable(System.IAsyncResult result) {
            bool retVal = ((Fab.Client.RegistrationServiceReference.IRegistrationService)(this)).EndIsLoginAvailable(result);
            return new object[] {
                    retVal};
        }
        
        private void OnIsLoginAvailableCompleted(object state) {
            if ((this.IsLoginAvailableCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.IsLoginAvailableCompleted(this, new IsLoginAvailableCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void IsLoginAvailableAsync(string login) {
            this.IsLoginAvailableAsync(login, null);
        }
        
        public void IsLoginAvailableAsync(string login, object userState) {
            if ((this.onBeginIsLoginAvailableDelegate == null)) {
                this.onBeginIsLoginAvailableDelegate = new BeginOperationDelegate(this.OnBeginIsLoginAvailable);
            }
            if ((this.onEndIsLoginAvailableDelegate == null)) {
                this.onEndIsLoginAvailableDelegate = new EndOperationDelegate(this.OnEndIsLoginAvailable);
            }
            if ((this.onIsLoginAvailableCompletedDelegate == null)) {
                this.onIsLoginAvailableCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnIsLoginAvailableCompleted);
            }
            base.InvokeAsync(this.onBeginIsLoginAvailableDelegate, new object[] {
                        login}, this.onEndIsLoginAvailableDelegate, this.onIsLoginAvailableCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Fab.Client.RegistrationServiceReference.IRegistrationService.BeginRegister(string login, string password, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginRegister(login, password, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Fab.Client.RegistrationServiceReference.UserDTO Fab.Client.RegistrationServiceReference.IRegistrationService.EndRegister(System.IAsyncResult result) {
            return base.Channel.EndRegister(result);
        }
        
        private System.IAsyncResult OnBeginRegister(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string login = ((string)(inValues[0]));
            string password = ((string)(inValues[1]));
            return ((Fab.Client.RegistrationServiceReference.IRegistrationService)(this)).BeginRegister(login, password, callback, asyncState);
        }
        
        private object[] OnEndRegister(System.IAsyncResult result) {
            Fab.Client.RegistrationServiceReference.UserDTO retVal = ((Fab.Client.RegistrationServiceReference.IRegistrationService)(this)).EndRegister(result);
            return new object[] {
                    retVal};
        }
        
        private void OnRegisterCompleted(object state) {
            if ((this.RegisterCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.RegisterCompleted(this, new RegisterCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void RegisterAsync(string login, string password) {
            this.RegisterAsync(login, password, null);
        }
        
        public void RegisterAsync(string login, string password, object userState) {
            if ((this.onBeginRegisterDelegate == null)) {
                this.onBeginRegisterDelegate = new BeginOperationDelegate(this.OnBeginRegister);
            }
            if ((this.onEndRegisterDelegate == null)) {
                this.onEndRegisterDelegate = new EndOperationDelegate(this.OnEndRegister);
            }
            if ((this.onRegisterCompletedDelegate == null)) {
                this.onRegisterCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnRegisterCompleted);
            }
            base.InvokeAsync(this.onBeginRegisterDelegate, new object[] {
                        login,
                        password}, this.onEndRegisterDelegate, this.onRegisterCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Fab.Client.RegistrationServiceReference.IRegistrationService.BeginResetPassword(string login, string email, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginResetPassword(login, email, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void Fab.Client.RegistrationServiceReference.IRegistrationService.EndResetPassword(System.IAsyncResult result) {
            base.Channel.EndResetPassword(result);
        }
        
        private System.IAsyncResult OnBeginResetPassword(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string login = ((string)(inValues[0]));
            string email = ((string)(inValues[1]));
            return ((Fab.Client.RegistrationServiceReference.IRegistrationService)(this)).BeginResetPassword(login, email, callback, asyncState);
        }
        
        private object[] OnEndResetPassword(System.IAsyncResult result) {
            ((Fab.Client.RegistrationServiceReference.IRegistrationService)(this)).EndResetPassword(result);
            return null;
        }
        
        private void OnResetPasswordCompleted(object state) {
            if ((this.ResetPasswordCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.ResetPasswordCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void ResetPasswordAsync(string login, string email) {
            this.ResetPasswordAsync(login, email, null);
        }
        
        public void ResetPasswordAsync(string login, string email, object userState) {
            if ((this.onBeginResetPasswordDelegate == null)) {
                this.onBeginResetPasswordDelegate = new BeginOperationDelegate(this.OnBeginResetPassword);
            }
            if ((this.onEndResetPasswordDelegate == null)) {
                this.onEndResetPasswordDelegate = new EndOperationDelegate(this.OnEndResetPassword);
            }
            if ((this.onResetPasswordCompletedDelegate == null)) {
                this.onResetPasswordCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnResetPasswordCompleted);
            }
            base.InvokeAsync(this.onBeginResetPasswordDelegate, new object[] {
                        login,
                        email}, this.onEndResetPasswordDelegate, this.onResetPasswordCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override Fab.Client.RegistrationServiceReference.IRegistrationService CreateChannel() {
            return new RegistrationServiceClientChannel(this);
        }
        
        private class RegistrationServiceClientChannel : ChannelBase<Fab.Client.RegistrationServiceReference.IRegistrationService>, Fab.Client.RegistrationServiceReference.IRegistrationService {
            
            public RegistrationServiceClientChannel(System.ServiceModel.ClientBase<Fab.Client.RegistrationServiceReference.IRegistrationService> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginGenerateUniqueLogin(System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[0];
                System.IAsyncResult _result = base.BeginInvoke("GenerateUniqueLogin", _args, callback, asyncState);
                return _result;
            }
            
            public string EndGenerateUniqueLogin(System.IAsyncResult result) {
                object[] _args = new object[0];
                string _result = ((string)(base.EndInvoke("GenerateUniqueLogin", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginIsLoginAvailable(string login, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = login;
                System.IAsyncResult _result = base.BeginInvoke("IsLoginAvailable", _args, callback, asyncState);
                return _result;
            }
            
            public bool EndIsLoginAvailable(System.IAsyncResult result) {
                object[] _args = new object[0];
                bool _result = ((bool)(base.EndInvoke("IsLoginAvailable", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginRegister(string login, string password, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[2];
                _args[0] = login;
                _args[1] = password;
                System.IAsyncResult _result = base.BeginInvoke("Register", _args, callback, asyncState);
                return _result;
            }
            
            public Fab.Client.RegistrationServiceReference.UserDTO EndRegister(System.IAsyncResult result) {
                object[] _args = new object[0];
                Fab.Client.RegistrationServiceReference.UserDTO _result = ((Fab.Client.RegistrationServiceReference.UserDTO)(base.EndInvoke("Register", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginResetPassword(string login, string email, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[2];
                _args[0] = login;
                _args[1] = email;
                System.IAsyncResult _result = base.BeginInvoke("ResetPassword", _args, callback, asyncState);
                return _result;
            }
            
            public void EndResetPassword(System.IAsyncResult result) {
                object[] _args = new object[0];
                base.EndInvoke("ResetPassword", _args, result);
            }
        }
    }
}
