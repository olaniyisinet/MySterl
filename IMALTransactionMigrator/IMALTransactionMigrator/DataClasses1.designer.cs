﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IMALTransactionMigrator
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="proxy")]
	public partial class DataClasses1DataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void Insertproxytable(proxytable instance);
    partial void Updateproxytable(proxytable instance);
    partial void Deleteproxytable(proxytable instance);
    #endregion
		
		public DataClasses1DataContext() : 
				base(global::IMALTransactionMigrator.Properties.Settings.Default.proxyConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public DataClasses1DataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataClasses1DataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataClasses1DataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataClasses1DataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<proxytable> proxytables
		{
			get
			{
				return this.GetTable<proxytable>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.proxytable")]
	public partial class proxytable : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _id;
		
		private string _phone;
		
		private string _sender;
		
		private string _text;
		
		private System.Nullable<System.DateTime> _inserted_at;
		
		private System.Nullable<System.DateTime> _processed_at;
		
		private System.Nullable<System.DateTime> _sent_at;
		
		private System.Nullable<System.DateTime> _dlr_timestamp;
		
		private System.Nullable<byte> _dlr_status;
		
		private string _dlr_description;
		
		private string _nuban;
		
		private string _ledgercode;
		
		private string _currency;
		
		private System.Nullable<bool> _aans_status;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(int value);
    partial void OnidChanged();
    partial void OnphoneChanging(string value);
    partial void OnphoneChanged();
    partial void OnsenderChanging(string value);
    partial void OnsenderChanged();
    partial void OntextChanging(string value);
    partial void OntextChanged();
    partial void Oninserted_atChanging(System.Nullable<System.DateTime> value);
    partial void Oninserted_atChanged();
    partial void Onprocessed_atChanging(System.Nullable<System.DateTime> value);
    partial void Onprocessed_atChanged();
    partial void Onsent_atChanging(System.Nullable<System.DateTime> value);
    partial void Onsent_atChanged();
    partial void Ondlr_timestampChanging(System.Nullable<System.DateTime> value);
    partial void Ondlr_timestampChanged();
    partial void Ondlr_statusChanging(System.Nullable<byte> value);
    partial void Ondlr_statusChanged();
    partial void Ondlr_descriptionChanging(string value);
    partial void Ondlr_descriptionChanged();
    partial void OnnubanChanging(string value);
    partial void OnnubanChanged();
    partial void OnledgercodeChanging(string value);
    partial void OnledgercodeChanged();
    partial void OncurrencyChanging(string value);
    partial void OncurrencyChanged();
    partial void Onaans_statusChanging(System.Nullable<bool> value);
    partial void Onaans_statusChanged();
    #endregion
		
		public proxytable()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_phone", DbType="NVarChar(15) NOT NULL", CanBeNull=false)]
		public string phone
		{
			get
			{
				return this._phone;
			}
			set
			{
				if ((this._phone != value))
				{
					this.OnphoneChanging(value);
					this.SendPropertyChanging();
					this._phone = value;
					this.SendPropertyChanged("phone");
					this.OnphoneChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sender", DbType="NVarChar(15) NOT NULL", CanBeNull=false)]
		public string sender
		{
			get
			{
				return this._sender;
			}
			set
			{
				if ((this._sender != value))
				{
					this.OnsenderChanging(value);
					this.SendPropertyChanging();
					this._sender = value;
					this.SendPropertyChanged("sender");
					this.OnsenderChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_text", DbType="NVarChar(459) NOT NULL", CanBeNull=false)]
		public string text
		{
			get
			{
				return this._text;
			}
			set
			{
				if ((this._text != value))
				{
					this.OntextChanging(value);
					this.SendPropertyChanging();
					this._text = value;
					this.SendPropertyChanged("text");
					this.OntextChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_inserted_at", DbType="DateTime")]
		public System.Nullable<System.DateTime> inserted_at
		{
			get
			{
				return this._inserted_at;
			}
			set
			{
				if ((this._inserted_at != value))
				{
					this.Oninserted_atChanging(value);
					this.SendPropertyChanging();
					this._inserted_at = value;
					this.SendPropertyChanged("inserted_at");
					this.Oninserted_atChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_processed_at", DbType="DateTime")]
		public System.Nullable<System.DateTime> processed_at
		{
			get
			{
				return this._processed_at;
			}
			set
			{
				if ((this._processed_at != value))
				{
					this.Onprocessed_atChanging(value);
					this.SendPropertyChanging();
					this._processed_at = value;
					this.SendPropertyChanged("processed_at");
					this.Onprocessed_atChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sent_at", DbType="DateTime")]
		public System.Nullable<System.DateTime> sent_at
		{
			get
			{
				return this._sent_at;
			}
			set
			{
				if ((this._sent_at != value))
				{
					this.Onsent_atChanging(value);
					this.SendPropertyChanging();
					this._sent_at = value;
					this.SendPropertyChanged("sent_at");
					this.Onsent_atChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_dlr_timestamp", DbType="DateTime")]
		public System.Nullable<System.DateTime> dlr_timestamp
		{
			get
			{
				return this._dlr_timestamp;
			}
			set
			{
				if ((this._dlr_timestamp != value))
				{
					this.Ondlr_timestampChanging(value);
					this.SendPropertyChanging();
					this._dlr_timestamp = value;
					this.SendPropertyChanged("dlr_timestamp");
					this.Ondlr_timestampChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_dlr_status", DbType="TinyInt")]
		public System.Nullable<byte> dlr_status
		{
			get
			{
				return this._dlr_status;
			}
			set
			{
				if ((this._dlr_status != value))
				{
					this.Ondlr_statusChanging(value);
					this.SendPropertyChanging();
					this._dlr_status = value;
					this.SendPropertyChanged("dlr_status");
					this.Ondlr_statusChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_dlr_description", DbType="NVarChar(10)")]
		public string dlr_description
		{
			get
			{
				return this._dlr_description;
			}
			set
			{
				if ((this._dlr_description != value))
				{
					this.Ondlr_descriptionChanging(value);
					this.SendPropertyChanging();
					this._dlr_description = value;
					this.SendPropertyChanged("dlr_description");
					this.Ondlr_descriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_nuban", DbType="VarChar(20)")]
		public string nuban
		{
			get
			{
				return this._nuban;
			}
			set
			{
				if ((this._nuban != value))
				{
					this.OnnubanChanging(value);
					this.SendPropertyChanging();
					this._nuban = value;
					this.SendPropertyChanged("nuban");
					this.OnnubanChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ledgercode", DbType="VarChar(10)")]
		public string ledgercode
		{
			get
			{
				return this._ledgercode;
			}
			set
			{
				if ((this._ledgercode != value))
				{
					this.OnledgercodeChanging(value);
					this.SendPropertyChanging();
					this._ledgercode = value;
					this.SendPropertyChanged("ledgercode");
					this.OnledgercodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_currency", DbType="VarChar(10)")]
		public string currency
		{
			get
			{
				return this._currency;
			}
			set
			{
				if ((this._currency != value))
				{
					this.OncurrencyChanging(value);
					this.SendPropertyChanging();
					this._currency = value;
					this.SendPropertyChanged("currency");
					this.OncurrencyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_aans_status", DbType="Bit")]
		public System.Nullable<bool> aans_status
		{
			get
			{
				return this._aans_status;
			}
			set
			{
				if ((this._aans_status != value))
				{
					this.Onaans_statusChanging(value);
					this.SendPropertyChanging();
					this._aans_status = value;
					this.SendPropertyChanged("aans_status");
					this.Onaans_statusChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
