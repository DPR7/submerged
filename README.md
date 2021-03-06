
# submerged

Submerged is an opensource .NET based framework for connecting your aquarium to the IoT. The full solution consists of:

* An Azure based IoT back-end which collects data and hosts an API
* Arduino devices connected to a Raspberry Pi running Windows IoT Core via Bluetooth
* An Apache Cordova mobile app for mobile phones

# What does it do?

Submerged is meant to help you monitor your aquarium in every way you want to. At the moment, I use it to monitor things like temperature, pH and leakage. Based on those readings you can set triggers so your phone notifies you as soon as things go south for whatever reason. It's also possible to control hardware such as relays to switch power sockets on / off. And I'm planning on adding a lot more functionality such as an intelligent log which alerts you when your stock solutions or CO2 bottle need replacing.

# Supported functionality
The current list of functionality includes:
* **Monitoring Sensors**: 
 * Temperature (DS18B20)
 * pH (Atlas EZO pH)
 * Flow (hall effect flow sensors) 
 * Leakage (generic moisture sensors)
 * Stock solution level (level switch sensors) 
* **Getting Notifications**: all of the sensors which output data can be monitored against min/max thresholds. A notification is sent to your mobile device as soon as one of the sensor reports an out of bound value. 
* **Controlling Relays**: using your mobile app, you can control relays inside of your cabinet remotely. Maintenance mode allows to easily switch multiple relays on/off when performing tank maintenance. 
* **Live data**: the measured data can be monitored live using the mobile app for smartphone. Internet connection required. 
* **Analytics**: all data is aggregated and stored. This allows you to view long-term statics of your tank per hour, day, week or month. 

# How do I use it?

At the moment, the sources are here for you to use under the Apache 2.0 License. To run the entire solution, you'll need both the hardware ([here](hardware.md)) as well as an Azure subscription to run the back-end. If you don't want to run the Azure back-end yourself, please get in touch so I can add you yo my subscription. The end goal is to create a multi-tenant back-end so that you only need to plug in the hardware that you use.

More information on how I built this and how you configure the back-end parts can be found in this blog series: <http://blog.repsaj.nl/index.php/series/azure-aquarium-monitor/>.

# Solution Overview

The solution architecture of submerged looks like this:

![Solution Overview](http://www.repsaj.nl/Uploads/submerged/submerged_overview.png)

Here's roughly what each component does:

* Each arduino device is called a **module** in order to standardize a bit. The "Sensor module" is capable of reading sensors (temperature, pH, flow, etc.). The "cabinet module" will control relays and connect to water sensors for leak detection. More module types might follow in the future.
* A Raspberry Pi running Windows 10 IoT serves as a **gateway**. It connects to the different modules you have and communicates with Azure in a secure way. At the moment the modules are connected via Bluetooth, but I'm planning on maybe replacing that with Alljoyn.
* **Azure IoT hub** takes care of communications from and to the gateway. Your gateway needs to be registered as a device with IoT hub.
* The incoming telemetry data is then processed by **Azure Stream Analytics**. One task ASA has is to seperate notifications and device data from normal telemetry data.
* All data is stored in **Azure blob storage** where it can reside for a long time.
* Notifications and device data are sent to a **Event Hub**. The items in the event hub are then processed by **Azure functions**.
* Events are forwarded to a **Notification hub**. This hub takes care of forwarding the information to a registered device (such as your smartphone).
* A **WebAPI** running in Azure serves as API. The API is secured using Azure AD.
* **Azure DocumentDB** is used to store information about the subscriptions, tanks, sensors and all stuff we need to run the service.
* Lastly, a Cordova mobile app allows you to read out telemetry data, control relays and do all kinds of other stuff.

# Can I contribute? 

Yes, please! I'm very much looking for enthusiasts who'd like to help build this out more. There's plenty of stuff to do. Here's a todo list:

* Implement additional sensors / modules
* Implement some of the missing features, such as the intelligent journal
* Test the mobile app for iPhone, currently only Android is being used / tested
* Build a web based entry point for management / extensive resporting / etc
* Implement additional unit tests
* Any other awesome additions I didn't think of
* There could probably be a little additional documentation, do check out the [blog series](http://blog.repsaj.nl/index.php/series/azure-aquarium-monitor/) for that too

So please get in touch if you want to contribute in any way! :)

# What do I get?

Pictures say more than a thousands words!

Screendumps of the mobile app:

<img src="http://www.repsaj.nl/Uploads/submerged/submerged_live.png" alt="Screenshot Live" width="400"/>
<img src="http://www.repsaj.nl/Uploads/submerged/submerged_relays.png" alt="Screenshot Relays" width="400"/>

Screendump of the gateway dashboard:

<img src="http://www.repsaj.nl/Uploads/submerged/submerged_dashboard.png" alt="Screenshot Dashboard" width="800"/>
