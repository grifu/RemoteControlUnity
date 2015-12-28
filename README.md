Remote Control for Unity (RCu)
-----------------------------
* authors: Grifu (Luis Leite)
* forum: http://www.grifu.com/vm/?forum=remote-control-for-unity
* source code:
* date: 26.12.2015
* version: 0.9 Beta


DEPENDENCIES
------------
RCu focus on the user interface, thus, it depends upon UnityOSC developed by Jorge Garcia Martin for the communication layer.
The UnityOSC is available for download at https://github.com/jorgegarcia/UnityOSC
Compatibility with Unity versions 4 and 5 (Windows / Mac)


ABSTRACT
--------
A remote control plugin for Unity3D that allows the controlling of objects inside Unity from the outside. 
A simple, flexible and scalable plugin that exposes any property that exists in the objects to the outside making it possible to control other applications or to be used as a debugging tool.
The plugin is very easy to use, with just two steps: create a network port and add OSC send or receive objects. This plugin was developed as a part of a digital EcoSystem to connect and orchestrate digital data in real-time for performance animation, a digital puppetry environment. A set of Digital Puppet Tools (DPT) developed during a PhD research (UTAustin|Portugal / UPorto) on Digital Puppetry. The first release was in July 2015.


OBJECTIVE
---------
The goal of this plugin is to facilitate the way you map or connect objects. There is no need for coding, just attach the behavior to any object, camera, or light.

There are just 4 behaviors
- RCPortOut: port Out (open OSC port for sending messages)
- RCPortIn: port In (open OSC port for receive menages)
- RCSender: send messages to the outside exposing properties
- RCReceiver: receive messages from the outside that control the objects
- OSCmanager: manages the OSC

[![RemoteControlForUnity](https://vimeo.com/135032229)](https://vimeo.com/grifu/remotecontrol0)


INSTALLATION
------------
Copy UnityOSC to Assets Folder or Download it from https://github.com/jorgegarcia/UnityOSC
Copy the RemoteControl to your Assets Folder
Copy Editor to Assets Folder


HOW TO USE
----------
1-> Initialize Remote Control by dragging RCInit to any GameObject

2-> Setup your in or out port by dragging the RCPortIn or RCPortOut to any Gameobject on your scene and define a valid port (i.e. 7010)

3-> Establish the mapping between the action/object and the message by dragging the RCReceiver or the RCSender (RCReceiver: to control the object with a OSC message such as a TouchOSC fader from your Ipad) 

Open OSC Helper window (Window->RemoteControl) to trace OSC in/out ports


CONTACT / +INFO
---------------
*video: https://vimeo.com/grifu/remotecontrol0
*mail: virtual.marionette@grifu.com
*web: http://www.virtualmarionette.grifu.com


LICENSE
-------
GNU GENERAL PUBLIC LICENSE Version 3
See License.txt for more details.


ISSUES
------
This is still a beta version with many issues
- only the animated parameters are sent
- methods are limited to blend shapes
- fields are not implemented yet!
- some issues within the inspector of the OSCReceiver and OSCSender

TODO
----
- Scale the mapping (include fields to scale values)
- Identify incoming OSC messages (a learn button to capture the OSC address)
- RCReceiver solve the offset problem
- Optimize performance
- Complete methods and fields