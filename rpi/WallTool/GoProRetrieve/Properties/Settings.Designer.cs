﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GoProRetrieve.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.3.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("convert")]
        public string LensCorrectionProgram {
            get {
                return ((string)(this["LensCorrectionProgram"]));
            }
            set {
                this["LensCorrectionProgram"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"-extent ""125%x125%"" -gravity center -filter Lagrange -quality 100 {0} -distort Barrel ""-0.92578 1.3845 -2.09958"" -distort Perspective ""1072,1512 0,0 3404,1524 2300,0 3100,2220 2300,1000 1484,2344 0,1000"" -gravity northwest -extent ""2300x1000"" -adaptive-sharpen ""1"" {1}")]
        public string LensCorrectionArgs {
            get {
                return ((string)(this["LensCorrectionArgs"]));
            }
            set {
                this["LensCorrectionArgs"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://10.5.5.9/gp/gpControl/command/wireless/ap/ssid?ssid=GARAGEGOPRO")]
        public string CameraKeepAlive {
            get {
                return ((string)(this["CameraKeepAlive"]));
            }
            set {
                this["CameraKeepAlive"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://10.5.5.9/gp/gpMediaList")]
        public string CameraListMedia {
            get {
                return ((string)(this["CameraListMedia"]));
            }
            set {
                this["CameraListMedia"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://10.5.5.9:8080/videos/DCIM/{0}")]
        public string CameraGetMedia {
            get {
                return ((string)(this["CameraGetMedia"]));
            }
            set {
                this["CameraGetMedia"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://10.5.5.9/gp/gpControl/command/storage/delete?p={0}")]
        public string CameraDeleteMedia {
            get {
                return ((string)(this["CameraDeleteMedia"]));
            }
            set {
                this["CameraDeleteMedia"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://10.5.5.9/gp/gpControl/command/shutter?p=1")]
        public string CameraCapturePhoto {
            get {
                return ((string)(this["CameraCapturePhoto"]));
            }
            set {
                this["CameraCapturePhoto"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public int CaptureEveryMinutes {
            get {
                return ((int)(this["CaptureEveryMinutes"]));
            }
            set {
                this["CaptureEveryMinutes"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int CaptureTotalPhotos {
            get {
                return ((int)(this["CaptureTotalPhotos"]));
            }
            set {
                this["CaptureTotalPhotos"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public int CaptureWaitBetweenPhotoSeconds {
            get {
                return ((int)(this["CaptureWaitBetweenPhotoSeconds"]));
            }
            set {
                this["CaptureWaitBetweenPhotoSeconds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("120")]
        public int CameraKeepAliveEverySeconds {
            get {
                return ((int)(this["CameraKeepAliveEverySeconds"]));
            }
            set {
                this["CameraKeepAliveEverySeconds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int CameraCommandsTimeoutSeconds {
            get {
                return ((int)(this["CameraCommandsTimeoutSeconds"]));
            }
            set {
                this["CameraCommandsTimeoutSeconds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://10.5.5.9/gp/gpControl/command/storage/delete/all")]
        public string CameraDeleteMediaAll {
            get {
                return ((string)(this["CameraDeleteMediaAll"]));
            }
            set {
                this["CameraDeleteMediaAll"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int CameraCommandsRetriesTotal {
            get {
                return ((int)(this["CameraCommandsRetriesTotal"]));
            }
            set {
                this["CameraCommandsRetriesTotal"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int CameraCommandsRetryTimeoutSeconds {
            get {
                return ((int)(this["CameraCommandsRetryTimeoutSeconds"]));
            }
            set {
                this["CameraCommandsRetryTimeoutSeconds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("from_camera")]
        public string DirectoryCaptures {
            get {
                return ((string)(this["DirectoryCaptures"]));
            }
            set {
                this["DirectoryCaptures"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("convert")]
        public string MergePhotosProgram {
            get {
                return ((string)(this["MergePhotosProgram"]));
            }
            set {
                this["MergePhotosProgram"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(" {0} -evaluate-sequence mean {1}")]
        public string MergePhotosArgs {
            get {
                return ((string)(this["MergePhotosArgs"]));
            }
            set {
                this["MergePhotosArgs"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("compare")]
        public string CompareDifferencesProgram {
            get {
                return ((string)(this["CompareDifferencesProgram"]));
            }
            set {
                this["CompareDifferencesProgram"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-metric MAE {0} {1} null:")]
        public string CompareDifferencesArgs {
            get {
                return ((string)(this["CompareDifferencesArgs"]));
            }
            set {
                this["CompareDifferencesArgs"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://10.5.5.9/gp/gpControl/setting/98/1")]
        public string CameraSetPhotoMode {
            get {
                return ((string)(this["CameraSetPhotoMode"]));
            }
            set {
                this["CameraSetPhotoMode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://10.5.5.9/gp/gpControl/setting/17/0")]
        public string CameraSetPhotoQuality {
            get {
                return ((string)(this["CameraSetPhotoQuality"]));
            }
            set {
                this["CameraSetPhotoQuality"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("tmp_averaged.png")]
        public string CompareDifferencesTemporalFilename {
            get {
                return ((string)(this["CompareDifferencesTemporalFilename"]));
            }
            set {
                this["CompareDifferencesTemporalFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("wall_final.png")]
        public string FinalImageBeforeEnhancementFilename {
            get {
                return ((string)(this["FinalImageBeforeEnhancementFilename"]));
            }
            set {
                this["FinalImageBeforeEnhancementFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("python3")]
        public string ImageEnhancementProgram {
            get {
                return ((string)(this["ImageEnhancementProgram"]));
            }
            set {
                this["ImageEnhancementProgram"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("rebuild_wall.py {0} {1}")]
        public string ImageEnhancementArgs {
            get {
                return ((string)(this["ImageEnhancementArgs"]));
            }
            set {
                this["ImageEnhancementArgs"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("wall_enhanced.png")]
        public string FinalImageFilename {
            get {
                return ((string)(this["FinalImageFilename"]));
            }
            set {
                this["FinalImageFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://wall.innovationgarage.no/upload.php")]
        public string FinalImageUploadServer {
            get {
                return ((string)(this["FinalImageUploadServer"]));
            }
            set {
                this["FinalImageUploadServer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("rebuild_postits.py 1.2")]
        public string ImagePreEnhancementArgs {
            get {
                return ((string)(this["ImagePreEnhancementArgs"]));
            }
            set {
                this["ImagePreEnhancementArgs"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("python3")]
        public string ImagePreEnhancementProgram {
            get {
                return ((string)(this["ImagePreEnhancementProgram"]));
            }
            set {
                this["ImagePreEnhancementProgram"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("06:41:69:2d:3b:14")]
        public string CameraPhysicalAddress {
            get {
                return ((string)(this["CameraPhysicalAddress"]));
            }
            set {
                this["CameraPhysicalAddress"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10.5.5.9")]
        public string CameraIPAddress {
            get {
                return ((string)(this["CameraIPAddress"]));
            }
            set {
                this["CameraIPAddress"] = value;
            }
        }
    }
}
