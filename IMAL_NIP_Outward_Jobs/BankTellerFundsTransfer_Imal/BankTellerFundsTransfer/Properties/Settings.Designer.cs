﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BankTellerFundsTransfer_imal.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost/smsservice/smsSender.asmx")]
        public string BankTellerFundsTransfer_lagos_SMS_smsSender {
            get {
                return ((string)(this["BankTellerFundsTransfer_lagos_SMS_smsSender"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://10.0.0.217:814/banks.asmx")]
        public string BankTellerFundsTransfer_lagos_EACBS_banks {
            get {
                return ((string)(this["BankTellerFundsTransfer_lagos_EACBS_banks"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://10.0.41.102:8061/services.asmx")]
        public string BankTellerFundsTransfer_lagos3_imalVteller_Services {
            get {
                return ((string)(this["BankTellerFundsTransfer_lagos3_imalVteller_Services"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost:9081/bankteller.asmx")]
        public string BankTellerFundsTransfer_lagos_BankTeller_bankTeller {
            get {
                return ((string)(this["BankTellerFundsTransfer_lagos_BankTeller_bankTeller"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://10.0.41.102/salesmanagerws/service.asmx")]
        public string BankTellerFundsTransfer_lagos3_ImalAcctinfo_Service {
            get {
                return ((string)(this["BankTellerFundsTransfer_lagos3_ImalAcctinfo_Service"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost:8084/bankTeller.asmx")]
        public string BankTellerFundsTransfer_imal_BankTellerV2_bankTeller {
            get {
                return ((string)(this["BankTellerFundsTransfer_imal_BankTellerV2_bankTeller"]));
            }
        }
    }
}
