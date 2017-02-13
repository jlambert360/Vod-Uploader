# Vod-Uploader

Vod Uploader Instructions:

- Step 1. Create a folder on your desktop and name it VodUploader

- Step 2. Inside that folder make a text file and name it Path.txt

     NOTE: If you want a DESCRIPTION and TAGS to go along with your videos you need to make text files in the same folder called "Description.txt" and "Tags.txt". 
		   Make sure You edit in Notepad++ and all the text is on the first line of the file
		   GAME TAGS are already set up. If you want extra tags they would be tags specific to your YouTube channel. Here is an example from the AON channel:
		   Set them up like this: "AON", "AON Gaming", "AON Smash", "Long Island", "Shady Penguin", "AON Long Island", "AON Gaming Long Island", "esports"
		   
- Step 3. Specific lines mean specific things in Path.txt so make sure it's set up like this

EX: Line 1: VOD Folder <br />
	Line 2: Put 1 to post vods as public 2 to post as unlisted <br />
	Line 3: Path to PM Uploader.exe <br />
	Line 4: Path to Melee Uploader.exe <br />
	Line 5: Path to Smash4 Uploader.exe <br />
	Line 6: Path to FileRegisterPM.exe <br />
	Line 7: Path to FileRegisterMelee.exe <br />
	Line 8: Path to FileRegisterSmash4.exe <br />
	Line 9: Doubles xml file <br />
	Line 10: Singles xml file <br />
	
	My Path.txt looks like this for example:
	
	D:/Users/Jordan/Desktop/AON VODS/TestVods/
    2
    D:/Users/Jordan/Documents/Visual Studio 2015/Projects/VodUploader/PM Uploader/bin/Release/PM Uploader.exe
    D:/Users/Jordan/Documents/Visual Studio 2015/Projects/VodUploader/Melee Uploader/bin/Release/Melee Uploader.exe
    D:/Users/Jordan/Documents/Visual Studio 2015/Projects/VodUploader/Smash4 Uploader/bin/Release/Smash4 Uploader.exe
    D:/Users/Jordan/Documents/Visual Studio 2015/Projects/VodUploader/FileRegisterPM/bin/Release/FileRegisterPM.exe
    D:/Users/Jordan/Documents/Visual Studio 2015/Projects/VodUploader/FileRegisterMelee/bin/Release/FileRegisterMelee.exe
    D:/Users/Jordan/Documents/Visual Studio 2015/Projects/VodUploader/FileRegisterSmash4/bin/Release/FileRegisterSmash4.exe
    D:/Users/Jordan/Downloads/Scoreboard-Assistant-v1.1.5/Scoreboard Assistant/output/playertest.xml
    D:/Users/Jordan/Downloads/Scoreboard-Assistant-v1.1.5/Scoreboard Assistant/output/versus.xml
	
- Step 4. Run FileRegister.exe BEFORE RUNNING OBS to check when OBS is recording

- Step 5. When you run FileRegister.exe it should say "Are you uploading for Project M, Melee, or Smash 4? (Can be entered as pm, m, s4)"
          Here you can type Project M or pm, Melee or m, Smash 4 or s4 to choose what game you're streaming

- Step 6. When you start recording some numbers and information about the recording should show up. This means the file was detected and you are recording successfully.

- Step 7. When you stop recording two new windows should show up. These windows will upload the recording you just stopped, or wait until you start recording again.

WARNING! DO NOT CHANGE THE NAMES IN SCOREBOARD ASSISTANT UNTIL YOU SEE THE TWO WINDOWS OPEN!

- Step 8. Remember to monitize your videos



Requirements: <br />
BouncyCastle 1.7.0 <br />
Google.Apis 1.9.0 <br />
Google.Apis.Auth 1.9.0 <br />
Google.Apis.Core 1.9.0 <br />
Google.Apis.YouTube.v3 1.8.1.1160 <br />
log4net 2.0.3 <br />
Newtonsoft.Json 9.01 <br />
Microsoft.Bcl 1.1.9 <br />
Microsoft.Bcl.Async 1.0.168 <br />
Microsoft.Bcl.Build 1.0.14 <br />
Microsoft.Net.Http 2.2.22 <br />
Zlib.Portable 1.10.0 <br />
Zlib.Portable.Signed 1.11.0 <br />
