# Voice Command Interface v3.4.0

![VCILogo](https://github.com/zxeltor/voice_command_interface/blob/main/OrcYell.png) 

## Overview
This application takes spoken phrases, translates them into a keystroke, and sends the keystroke to a targeted Windows application. 

## Details 
I originally created this application to interact with World of Warcraft.  I was having trouble completing the Guardian Druid mage tower challenge in the Burning Legion expansion.  I was used to playing with the standard WOW interface, and a certain set of hotkeys tied to my keyboard and mouse.  The Guardian Druid mage tower challenge required me to use more abilities than I could setup with my hotkeys.  So ... in comes the Voice Command Interface.  It gave me the opportunity to bark out voice commands tied to some of my less commonly used abilities.

## Requirements
* Microsoft Windows 10 or higher
* Microsoft .NET Framework 4.8

## Application Interface
The application has evolved quite a bit over the past few years. It started out as harcoded app with a few voice regonition commands tied to World of Warcraft. Over time the application was updated to support other windows applications, along with user profiles and other various configurable options.

## Startup
On startup the application attempts to find user saved settings. If none are found, default settings are used.

### Log Tab
![LogTab](https://github.com/zxeltor/voice_command_interface/blob/main/ScreenShots/LogTab.png)

This tab logs recognized voice commands which were procesed and sent to our target application, along with other settings changes and similiar events.

### Profiles Tab
![ProfileTab](https://github.com/zxeltor/voice_command_interface/blob/main/ScreenShots/ProfilesTab.png)
This tab displays the currently selected voice recognition profile. The main grid display the various speech recognition commands, along with the keystroke which is sent to our target application.
#### Application
This is the currently targeted windows application. You can add new application which are currently running, or removing an existing one
#### Profile
Each targeted application can have multiple voice recognition profiles. When this is changed, the main grid is updated to reflect the voice commands saved under the currently selected profile.
Note: I thought this would come in handy when trying to support muiltiple characters in WOW, along with their multiple specs.
#### Create Profile
Used to create a new voice recognition profile for the currently selected Application.

### Key Translations Tab
![KeyTab](https://github.com/zxeltor/voice_command_interface/blob/main/ScreenShots/KeyTranslationTab.png)

This tab maintains a map of windows keyboard keys which require translation, so they'll be recognized by the targeted windows application. A seperate map of these keys is saved for each target application. New applications are given a default list, which can be modified.
Note: It's not likely you'll ever have to change these keys.

### Recognition Tab
![RecogTab](https://github.com/zxeltor/voice_command_interface/blob/main/ScreenShots/RecognitionTab.png)
This tab can be used to change various windows defaults.
#### Microphone
The application uses the windows default microphone. As a convience, it's displayed here.
#### Voice Recognition Engine
By default the application uses the default recognition engine available to windows. If for some reason you have another option, you can give it a try.
Note: This has only been tested with the following recognition engine "Microsoft Speech Recognizer 8.0 for Windows (English - US)". I can't gaurentee it will work with others.
#### Voice Command Acknowledgement: Enable
When a voice command is recognized and processed, the application can use a TTS (Text-to-speech) message of acknowlgement to the user. You can enable/disable this feature.
#### Voice Command Acknowledgement: Test
Use this to test your Voice Command Acknowledgement settings.
#### Voice Command Acknowledgement: Operating System Voice
Use can choose any windows installed voice to be used with Voice Command Acknowledgement.
#### Voice Command Acknowledgement: Volume
You can set the colume level for Voice Command Acknowledgement.
### Options Tab
![OptionsTab](https://github.com/zxeltor/voice_command_interface/blob/main/ScreenShots/OptionsTab.png) 




## Settings 
