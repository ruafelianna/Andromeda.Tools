﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Andromeda.Tools.PublishPackages.Assets {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Andromeda.Tools.PublishPackages.Assets.Strings", typeof(Strings).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select containing NuGet package folder.
        /// </summary>
        public static string Dialog_SelectPackageFolder {
            get {
                return ResourceManager.GetString("Dialog_SelectPackageFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot create NuGet client: {0}.
        /// </summary>
        public static string Error_CannotCreateNuGetClient {
            get {
                return ResourceManager.GetString("Error_CannotCreateNuGetClient", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot create a new process.
        /// </summary>
        public static string Error_CannotCreateProcess {
            get {
                return ResourceManager.GetString("Error_CannotCreateProcess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot push packages.
        /// </summary>
        public static string Error_CannotPushPackages {
            get {
                return ResourceManager.GetString("Error_CannotPushPackages", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error while choosing folder.
        /// </summary>
        public static string Error_ChooseFolder {
            get {
                return ResourceManager.GetString("Error_ChooseFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No NuGet API key provided.
        /// </summary>
        public static string Error_NoNuGetAPIKey {
            get {
                return ResourceManager.GetString("Error_NoNuGetAPIKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Process returned exit code = {0}.
        /// </summary>
        public static string Error_ProcessReturnedCode {
            get {
                return ResourceManager.GetString("Error_ProcessReturnedCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot remove package: {0}.
        /// </summary>
        public static string Error_RemovePackage {
            get {
                return ResourceManager.GetString("Error_RemovePackage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server name is null.
        /// </summary>
        public static string Error_ServerNameIsNull {
            get {
                return ResourceManager.GetString("Error_ServerNameIsNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No answer.
        /// </summary>
        public static string Error_TimeoutException {
            get {
                return ResourceManager.GetString("Error_TimeoutException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot update packages list.
        /// </summary>
        public static string Error_UpdatePackagesList {
            get {
                return ResourceManager.GetString("Error_UpdatePackagesList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Directory is chosen: {0}.
        /// </summary>
        public static string Info_DirectoryChosen {
            get {
                return ResourceManager.GetString("Info_DirectoryChosen", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Folder is added: {0}.
        /// </summary>
        public static string Info_FolderAdded {
            get {
                return ResourceManager.GetString("Info_FolderAdded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Directory already exists: {0}.
        /// </summary>
        public static string Info_FolderExists {
            get {
                return ResourceManager.GetString("Info_FolderExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Folders are removed: {0}.
        /// </summary>
        public static string Info_FoldersRemoved {
            get {
                return ResourceManager.GetString("Info_FoldersRemoved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to NuGet package files were listed for {0} folders.
        /// </summary>
        public static string Info_PackageFilesListed {
            get {
                return ResourceManager.GetString("Info_PackageFilesListed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Package is removed: {0}.
        /// </summary>
        public static string Info_PackageRemoved {
            get {
                return ResourceManager.GetString("Info_PackageRemoved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Packages list is updated, count = {0}.
        /// </summary>
        public static string Info_PackagesListUpdated {
            get {
                return ResourceManager.GetString("Info_PackagesListUpdated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Packages pushed: {0}.
        /// </summary>
        public static string Info_PackagesPushed {
            get {
                return ResourceManager.GetString("Info_PackagesPushed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server is added: {0}.
        /// </summary>
        public static string Info_ServerAdded {
            get {
                return ResourceManager.GetString("Info_ServerAdded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server is removed: {0}.
        /// </summary>
        public static string Info_ServerRemoved {
            get {
                return ResourceManager.GetString("Info_ServerRemoved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Add.
        /// </summary>
        public static string View_Btn_Add {
            get {
                return ResourceManager.GetString("View_Btn_Add", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ....
        /// </summary>
        public static string View_Btn_ChooseFolder {
            get {
                return ResourceManager.GetString("View_Btn_ChooseFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Push selected packages.
        /// </summary>
        public static string View_Btn_PushSelectedPackages {
            get {
                return ResourceManager.GetString("View_Btn_PushSelectedPackages", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Refresh.
        /// </summary>
        public static string View_Btn_Refresh {
            get {
                return ResourceManager.GetString("View_Btn_Refresh", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Remove.
        /// </summary>
        public static string View_Btn_Remove {
            get {
                return ResourceManager.GetString("View_Btn_Remove", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Remove selected folders.
        /// </summary>
        public static string View_Btn_RemoveSelectedFolders {
            get {
                return ResourceManager.GetString("View_Btn_RemoveSelectedFolders", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Update.
        /// </summary>
        public static string View_Btn_Update {
            get {
                return ResourceManager.GetString("View_Btn_Update", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Choose or enter a folder.
        /// </summary>
        public static string View_ChooseOrEnterFolder {
            get {
                return ResourceManager.GetString("View_ChooseOrEnterFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Choose server.
        /// </summary>
        public static string View_ChooseServer {
            get {
                return ResourceManager.GetString("View_ChooseServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Count: {0}.
        /// </summary>
        public static string View_Count {
            get {
                return ResourceManager.GetString("View_Count", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter new server.
        /// </summary>
        public static string View_EnterNewServer {
            get {
                return ResourceManager.GetString("View_EnterNewServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Push.
        /// </summary>
        public static string View_Tab_Push {
            get {
                return ResourceManager.GetString("View_Tab_Push", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Search.
        /// </summary>
        public static string View_Tab_Search {
            get {
                return ResourceManager.GetString("View_Tab_Search", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Packages Helper.
        /// </summary>
        public static string View_Title {
            get {
                return ResourceManager.GetString("View_Title", resourceCulture);
            }
        }
    }
}
