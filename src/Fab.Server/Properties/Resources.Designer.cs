﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17379
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fab.Server.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Fab.Server.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE [Users] (
        ///  [Id] uniqueidentifier NOT NULL DEFAULT (newid())
        ///, [Login] nvarchar(50) NOT NULL
        ///, [Password] nvarchar(256) NOT NULL
        ///, [Email] nvarchar(256) NULL
        ///, [Registered] datetime NOT NULL DEFAULT (GETDATE())
        ///, [LastAccess] datetime NULL
        ///, [DatabasePath] nvarchar(2048) NULL
        ///, [ServiceUrl] nvarchar(2048) NULL
        ///, [IsDisabled] bit NOT NULL DEFAULT ((0))
        ///, [DisabledChanged] datetime NULL
        ///);
        ///GO
        ///ALTER TABLE [Users] ADD CONSTRAINT [PK_Users] PRIMARY KEY ([Id]);
        ///GO.
        /// </summary>
        internal static string master_001_setup {
            get {
                return ResourceManager.GetString("master_001_setup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- Script Date: 29.09.2011 1:23  - Generated by Export2SqlCe version 3.5.1.7
        ///CREATE TABLE [Accounts] (
        ///  [Id] int NOT NULL  IDENTITY (1,1)
        ///, [Name] nvarchar(50) NOT NULL
        ///, [Created] datetime NOT NULL DEFAULT (GETDATE())
        ///, [Balance] money NOT NULL DEFAULT ((0))
        ///, [IsSystem] bit NOT NULL DEFAULT ((0))
        ///, [IsClosed] bit NOT NULL DEFAULT ((0))
        ///, [ClosedChanged] datetime NULL
        ///, [PostingsCount] int NOT NULL DEFAULT ((0))
        ///, [FirstPostingDate] datetime NULL
        ///, [LastPostingDate] datetime NULL
        ///, [AssetType_ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string personal_001_setup {
            get {
                return ResourceManager.GetString("personal_001_setup", resourceCulture);
            }
        }
    }
}
