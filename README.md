# Voice Command Interface 
![VCILogo](https://github.com/zxeltor/voice_command_interface/blob/main/OrcYell.png)  
## Overview
This application takes spoken phrases, translates them into a keystroke, and sends the keystroke to a targeted Windows application.  
## Details
I originally created this application to interact with World of Warcraft. I was having trouble completing the Guardian Druid mage tower challenge in the Burning Legion expansion. I was used to playing with the standard WOW interface, and a certain set of hotkeys tied to my keyboard and mouse. The Guardian Druid mage tower challenge required me to use more abilities than I could setup with my hotkeys. So ... in comes the Voice Command Interface. It gave me the opportunity to bark out voice commands tied to some of my less commonly used abilities. 
## Requirements
* Microsoft Windows 10 or higher 
* Microsoft .NET Framework 4.8 runtime (It will not work with the .NET 5 runtime or higher) 
* Optional: A decent headset with a microphone. Background talking from a person or TV can trigger a voice recognition event. 
## Application Interface
The application has evolved quite a bit over the past few years. It started out as a hardcoded app with a few voice recognition commands tied to World of Warcraft. Over time the application was updated to support other windows applications, along with user profiles and other various configurable options. On startup the application attempts to find user saved settings. If none are found, default settings are used. 
### Status Bar 
![StatusBar](https://github.com/zxeltor/voice_command_interface/blob/main/ScreenShots/StatusBar.png) 
#### Settings: 
##### Targeted Application (Circled in green) 
This shows the targeted application, along with status of the target application. (Running or Not Running) 
##### Selected Profile (Circled in red) 
This shows the currently selected voice recognition profile. 
##### Speech Recognition 
You can enable/disable speech recognition used by the application. You can use this to temporarily turn off speech recognition instead of closing the application. 
Note: Speech Recognition is automatically disabled when you change certain settings. You will be forced to save your changes before you can enable voice recognition again. 
##### Save Settings 
This allows you to save your settings changes. 
Note: You will receive a notification in the status bar of when a profile change was made. If you don’t save your profile changes, they will be lost when you close the application. 
##### Application Link (Circled in blue) 
This is just a link to the applications web site. 
### Log Tab 
![LogTab](https://github.com/zxeltor/voice_command_interface/blob/main/ScreenShots/LogTab.png) 
This tab logs recognized voice commands which were processed and sent to our target application, along with other settings changes and similar events. 
### Profiles Tab 
![ProfileTab](https://github.com/zxeltor/voice_command_interface/blob/main/ScreenShots/ProfilesTab.png) 
This tab displays the currently selected voice recognition profile. The main grid displays the various speech recognition commands, along with the keystroke to send to your target application. 
#### Settings: 
##### Application 
This is the currently targeted windows application. You can add new applications which are currently running, or remove an existing one. 
Note: This will only work with Windowed applications. Not console applications. 
##### Profile 
Each targeted application can have multiple voice recognition profiles. When this is changed, the main grid is updated to reflect the voice commands saved under the currently selected profile. 
Note: I thought this would come in handy when trying to support multiple characters in WOW, along with their multiple specs. 
##### Create Profile 
Used to create a new voice recognition profile for the currently selected Application. 
### Key Translations Tab 
![KeyTab](https://github.com/zxeltor/voice_command_interface/blob/main/ScreenShots/KeyTranslationTab.png) 
This tab maintains a map of windows keyboard keys which require translation, so they'll be recognized by the targeted windows application. A separate map of these keys is saved for each target application. New applications are given a default list, which can be modified. 
Note: It's not likely you'll ever have to change these keys. 
### Recognition Tab 
![RecogTab](https://github.com/zxeltor/voice_command_interface/blob/main/ScreenShots/RecognitionTab.png) 
This tab can be used to change various windows defaults. 
#### Settings: 
##### Microphone 
The application uses the windows default microphone. As a convenience it's displayed here. 
##### Voice Recognition Engine 
By default, the application uses the default recognition engine available to windows. If for some reason you have another option, you can give it a try. 
Note: This has only been tested with the following recognition engine "Microsoft Speech Recognizer 8.0 for Windows (English - US)". I can't guarantee it will work with others. 
##### Voice Command Acknowledgement: Enable 
When a voice command is recognized and processed, the application can play a TTS (Text-to-speech) acknowledgement to the user. You can enable/disable this feature. 
##### Voice Command Acknowledgement: Test 
Use this to test your Voice Command Acknowledgement settings. 
##### Voice Command Acknowledgement: Operating System Voice 
Use can choose any windows installed voice to be used with Voice Command Acknowledgement. 
##### Voice Command Acknowledgement: Volume 
You can set the volume level for Voice Command Acknowledgement. 
### Options Tab 
![OptionsTab](https://github.com/zxeltor/voice_command_interface/blob/main/ScreenShots/OptionsTab.png)  
#### Settings: 
##### Hotkeys 
Hotkeys are used to suppress speech recognition. For example, if you have discord open and a push-to-talk key set, you can add your push-to-talk key to the Hotkeys list. This will pause speech recognition until you’re done talking to your teammates. 
##### Import / Export 
You can use this feature to save/export your application settings to a XML file as a backup. 
##### Software Update Check 
With this enabled, the software will check for the latest version of Voice Command Interface online and verify your running the latest version. 
## Use with World of Warcraft
