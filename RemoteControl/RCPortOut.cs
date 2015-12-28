//  RCPortOut [Remote Control Port Output]
//  ------------------------------------
//
// 	Creates a sending port for RemoteControl
//  Version 0.9
//
//  Remote Control for Unity - part of Digital Puppet Tools, A digital ecosystem for Virtual Marionette Project
//	Copyright (c) 2015 Luis Leite (Grifu)
//
// 	Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// 	documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// 	the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
// 	and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// 	of the Software.
//
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// 	TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// 	THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// 	CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// 	IN THE SOFTWARE.
//


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityOSC;
using System.Net;

[System.Serializable]
public class RCPortOut : MonoBehaviour {
	public int port = 0;								// port number
	public string address = "127.0.0.1";
	public Dictionary<string, ClientData> clients;		// mapping server
	private RCPortOut[] instancesRCPortOut;			// instance of this


	// initialization
	void Start () {
		OSCManager.Instance.Init(); //init OSC
		clients = new Dictionary<string, ClientData>();
		
		// if port does not exists we can create this new port -- I should check for existing ports!!!
		if(port > 0)
		{
			string clientName = "RemoteClient-" + port;
			OSCManager.Instance.CreateClient (clientName, IPAddress.Parse(address), port);
		}
	}


	// Check for existing ports
	void OnValidate() {
		instancesRCPortOut = FindObjectsOfType (typeof(RCPortOut)) as RCPortOut[];
		if (instancesRCPortOut.Length > 1)
		{
			
			foreach(RCPortOut item in instancesRCPortOut)
			{
				
				if(port == item.port && item != this) 
				{
					Debug.Log ("Port already exists! please insert a different port number");
					port = 0;
					
				}
			}
			
			
		}
		
	}

}
