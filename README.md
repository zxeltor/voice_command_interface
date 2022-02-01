# Voice Command Interface v4.2.0

![VCILogo](OrcYell.png?v17-10-2021) 

* [Overview](#overview)
* [Details](#details)
* [Requirements](#requirements)
* [Application Interface](#application-interface)
* [Use with World of Warcraft](#use-with-world-of-warcraft)
* [Disclaimer](#disclaimer)

## Overview
VCI takes spoken phrases, translates them into a keystroke, and sends the keystroke to a targeted Windows application.  

VCI isn't an exploitation tool for World of Warcraft, or any other windows application. I figured if you could buy a mouse to send programmable keystrokes, it would be ok to have an application that could do the same thing. As a precuation, I limited the application to sending single keystrokes with a voice command.

Have fun and enjoy this application.

__Note:__ Most of my testing has been done with World of Warcraft and Star Trek Online. In theory, this should work with MOST windows applications which launch from an actual window. This won't work with console applications.
## Details
I originally created this application to interact with World of Warcraft. I was having trouble completing the Guardian Druid mage tower challenge in the Burning Legion expansion. I was used to playing with the standard WOW interface, and a certain set of hotkeys tied to my keyboard and mouse. The Guardian Druid mage tower challenge required me to use more abilities than I could setup with my hotkeys. So ... in comes the Voice Command Interface. It gave me the opportunity to bark out voice commands tied to some of my less commonly used abilities. 
## Requirements
* Microsoft Windows 10 or higher 
* Microsoft .NET Framework 4.8 runtime (It will not work with the .NET 5 runtime or higher) 
* Optional: A decent headset with a microphone. Background talking from a person or TV can trigger a voice recognition event. 
## Application Interface
The application has evolved quite a bit over the past few years. It started out as a hardcoded app with a few voice recognition commands tied to World of Warcraft. Over time the application was updated to support other windows applications, along with user profiles and other various configurable options. On startup the application attempts to find user saved settings. If none are found, default settings are used. 
### Status Bar 
![StatusBar](ScreenShots/StatusBar.png?v17-10-2021) 
#### Settings: 
- __Targeted Application (Circled in green)__ -
This shows the targeted application, along with status of the target application. (Running or Not Running) 
- __Selected Profile (Circled in red)__ -
This shows the currently selected voice recognition profile. 
- __Speech Recognition__ -
You can enable/disable speech recognition used by the application. You can use this to temporarily turn off speech recognition instead of closing the application. 
__Note:__ Speech Recognition is automatically disabled when you change certain settings. You will be forced to save your changes before you can enable voice recognition again. 
- __Save Settings__ -
This allows you to save your settings changes. 
__Note:__ You will receive a notification in the status bar when a profile change is made. If you don’t save your profile changes, they will be lost when you close the application. 
- __Application Link (Circled in blue)__ -
This is just a link to the applications web site. 
### Log Tab 
![LogTab](ScreenShots/LogTab.png?v17-10-2021) 
This tab logs recognized voice commands which were processed and sent to our target application, along with other settings changes and similar events. 
### Profiles Tab 
![ProfileTab](ScreenShots/ProfilesTab.png?v17-10-2021) 
This tab displays the currently selected voice recognition profile. The main grid displays the various speech recognition commands, along with the keystroke to send to your target application. 
#### Settings: 
- __Application__ -
This is the currently targeted windows application. You can add new applications which are currently running, or remove an existing one. 
__Note:__ This will only work with Windowed applications. Not console applications. 
- __Profile__ -
Each targeted application can have multiple voice recognition profiles. When this is changed, the main grid is updated to reflect the voice commands saved under the currently selected profile. 
__Note:__ I thought this would come in handy when trying to support multiple characters in WOW, along with their multiple specs. 
- __Create Profile__ -
Used to create a new voice recognition profile for the currently selected Application. 
### Options Tab 
![RecogTab](ScreenShots/OptionsTab.png?v17-10-2021) 
This tab can be used to change various settings for the app. 
#### Settings: 
- __Voice Recognition Engine__ -
By default, the application uses the default recognition engine available to windows. If for some reason you have another option, you can give it a try. 
__Note:__ This has only been tested with the following recognition engine "Microsoft Speech Recognizer 8.0 for Windows (English - US)". I can't guarantee it will work with others. 
- __Pause Keys__ -
Pause Keys are used to temporarily suppress speech recognition. For example, if you have discord open and a push-to-talk key set, you can add your push-to-talk key to the Pause Keys list. This will pause speech recognition until you’re done talking to your teammates.
- __Voice Command Acknowledgement: Enable__ - 
When a voice command is recognized and processed, the application can play a TTS (Text-to-speech) acknowledgement to the user. You can enable/disable this feature. 
- __Voice Command Acknowledgement: Test__ - 
Use this to test your Voice Command Acknowledgement settings. 
- __Voice Command Acknowledgement: Operating System Voice__ -
Use can choose any windows installed voice to be used with Voice Command Acknowledgement. 
- __Voice Command Acknowledgement: Volume__ -
You can set the volume level for Voice Command Acknowledgement. 
- __Microphone__ -
The application uses the windows default microphone. As a convenience it's displayed here. 
- __Keystroke Settings__ -
When sending multiple keystrokes to the target application, it’s necessary to have a short delay between keystrokes. Slower computers may require a longer delay.
- __Import / Export__ -
You can use this feature to save/export your application settings to a XML file as a backup. 
- __Software Update Check__ -
With this enabled, the software will check for the latest version of Voice Command Interface online and verify your running the latest version. 
## Use with World of Warcraft
I started by adding spells and abilities to my actions bars, then setting keybinds to those spells and abilities. I used SHIFT+F1, CTRL+F1, and etc. I then created a voice recognition profile for WOW, and added voice commands to send keystrokes like SHIFT+F1, CTRL+F1, and etc to WOW.

![ProfileTab](ScreenShots/ProfilesTab_WOW.png?v17-10-2021)
As an example from the above profile tab. When I say "engage bull rush", the SHIFT+F1 key combination is sent to WOW, and WOW responds by clicking the actionbar button I keybinded with SHIFT-F1.
## Disclaimer
This software and any related documentation is provided “as is” without warranty of any kind, either express or implied, including, without limitation, the implied warranties of merchantability, fitness for a particular purpose, or non-infringement. Licensee accepts any and all risk arising out of use or performance of Software.
