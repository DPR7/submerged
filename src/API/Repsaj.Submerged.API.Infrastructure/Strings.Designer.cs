﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Repsaj.Submerged.Infrastructure {
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
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Repsaj.Submerged.Infrastructure.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to A physical hardware device..
        /// </summary>
        internal static string CustomDeviceDescription {
            get {
                return ResourceManager.GetString("CustomDeviceDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://azure.microsoft.com/documentation/articles/iot-suite-connecting-devices/.
        /// </summary>
        internal static string CustomDeviceInstructionsUrl {
            get {
                return ResourceManager.GetString("CustomDeviceInstructionsUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Custom Device.
        /// </summary>
        internal static string CustomDeviceName {
            get {
                return ResourceManager.GetString("CustomDeviceName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The device is already in maintenance mode..
        /// </summary>
        internal static string DeviceAlreadyInMaintenanceMessage {
            get {
                return ResourceManager.GetString("DeviceAlreadyInMaintenanceMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Device {0} is already registered..
        /// </summary>
        internal static string DeviceAlreadyRegisteredExceptionMessage {
            get {
                return ResourceManager.GetString("DeviceAlreadyRegisteredExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This device is currently not in maintenance mode..
        /// </summary>
        internal static string DeviceNotInMaintenanceMessage {
            get {
                return ResourceManager.GetString("DeviceNotInMaintenanceMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Device {0} is not registered..
        /// </summary>
        internal static string DeviceNotRegisteredExceptionMessage {
            get {
                return ResourceManager.GetString("DeviceNotRegisteredExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was an error while registering device {0}..
        /// </summary>
        internal static string DeviceRegistrationExceptionMessage {
            get {
                return ResourceManager.GetString("DeviceRegistrationExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Enabled State must be set. Please select Enabled or Disabled..
        /// </summary>
        internal static string EnabledStateNotSetError {
            get {
                return ResourceManager.GetString("EnabledStateNotSetError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Software to simulate a device. Easily extensible for arbitrary events and commands; can run in a Windows Azure worker role. To create a simulated device, please follow the cooler sample instructions..
        /// </summary>
        internal static string SimulatedDeviceDescription {
            get {
                return ResourceManager.GetString("SimulatedDeviceDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Simulated Device.
        /// </summary>
        internal static string SimulatedDeviceName {
            get {
                return ResourceManager.GetString("SimulatedDeviceName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unexpected error occurred..
        /// </summary>
        internal static string UnexpectedErrorOccurred {
            get {
                return ResourceManager.GetString("UnexpectedErrorOccurred", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Device {0} does not support the command {1}..
        /// </summary>
        internal static string UnsupportedCommandExceptionMessage {
            get {
                return ResourceManager.GetString("UnsupportedCommandExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value for Created cannot be changed on updating..
        /// </summary>
        internal static string ValidationCreatedChanged {
            get {
                return ResourceManager.GetString("ValidationCreatedChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Description cannot be empty..
        /// </summary>
        internal static string ValidationDescriptionEmpty {
            get {
                return ResourceManager.GetString("ValidationDescriptionEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Device already exists in the device registry.
        /// </summary>
        internal static string ValidationDeviceExists {
            get {
                return ResourceManager.GetString("ValidationDeviceExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DeviceID cannot be empty or null.
        /// </summary>
        internal static string ValidationDeviceIdMissing {
            get {
                return ResourceManager.GetString("ValidationDeviceIdMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You cannot have two tanks with exactly the same name, pick a different name..
        /// </summary>
        internal static string ValidationDuplicateTanks {
            get {
                return ResourceManager.GetString("ValidationDuplicateTanks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to One or more validation errors occured for device {0}..
        /// </summary>
        internal static string ValidationExceptionMessage {
            get {
                return ResourceManager.GetString("ValidationExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module type {0} is not a valid module type..
        /// </summary>
        internal static string ValidationInvalidModuleType {
            get {
                return ResourceManager.GetString("ValidationInvalidModuleType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sensor type {0} is not a valid sensor type. .
        /// </summary>
        internal static string ValidationInvalidSensorType {
            get {
                return ResourceManager.GetString("ValidationInvalidSensorType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The LogType cannot be empty..
        /// </summary>
        internal static string ValidationLogTypeEmpty {
            get {
                return ResourceManager.GetString("ValidationLogTypeEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is already a module registered for this device with the same name.
        /// </summary>
        internal static string ValidationModuleExists {
            get {
                return ResourceManager.GetString("ValidationModuleExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is no module registered with name {0}..
        /// </summary>
        internal static string ValidationModuleUnknown {
            get {
                return ResourceManager.GetString("ValidationModuleUnknown", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Relay already exists, the relay number needs to be unique..
        /// </summary>
        internal static string ValidationRelayExists {
            get {
                return ResourceManager.GetString("ValidationRelayExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a relay with number {1} for this device..
        /// </summary>
        internal static string ValidationRelayUnknown {
            get {
                return ResourceManager.GetString("ValidationRelayUnknown", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sensor already exists, the sensor name needs to be unique.
        /// </summary>
        internal static string ValidationSensorExists {
            get {
                return ResourceManager.GetString("ValidationSensorExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a sensor named {0} with this device..
        /// </summary>
        internal static string ValidationSensorUnknown {
            get {
                return ResourceManager.GetString("ValidationSensorUnknown", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Subscription already exists in the subscription registry..
        /// </summary>
        internal static string ValidationSubscriptionExists {
            get {
                return ResourceManager.GetString("ValidationSubscriptionExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SubscriptionID cannot be empty or null.
        /// </summary>
        internal static string ValidationSubscriptionIdMissing {
            get {
                return ResourceManager.GetString("ValidationSubscriptionIdMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a subscription..
        /// </summary>
        internal static string ValidationSubscriptionUnknown {
            get {
                return ResourceManager.GetString("ValidationSubscriptionUnknown", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a tank with that Id in the registry..
        /// </summary>
        internal static string ValidationTankNotFound {
            get {
                return ResourceManager.GetString("ValidationTankNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Title cannot be empty..
        /// </summary>
        internal static string ValidationTitleEmpty {
            get {
                return ResourceManager.GetString("ValidationTitleEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value for User cannot be changed on updating..
        /// </summary>
        internal static string ValidationUserChanged {
            get {
                return ResourceManager.GetString("ValidationUserChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The passed in User ID does not match the registered user. Cannot continue..
        /// </summary>
        internal static string ValidationWrongUser {
            get {
                return ResourceManager.GetString("ValidationWrongUser", resourceCulture);
            }
        }
    }
}
