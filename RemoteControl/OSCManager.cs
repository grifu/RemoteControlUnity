//  OSCManager [Open Sound Control Manager]
//  ------------------------------------
// 	manages Server and Client data 
//  Version 0.9
//
//	www.virtualmarionette.grifu.com
//
//	OSCManager - Open Sound Control Manager to be used with the Jorge Garcia Martin UnityOSC
//
// 	This file was adapted from OSCHandler from https://github.com/jorgegarcia/UnityOSC.git
//	  

using System;
using System.Net;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public struct ServerData
{
	public OSCServer server;
	public List<OSCPacket> packets;
	public List<string> log;
}

public struct ClientData
{
	public OSCClient client;
	public List<OSCMessage> messages;
	public List<string> log;
}


public class OSCManager : MonoBehaviour
{

	#region Singleton Constructors
	static OSCManager()
	{
	}

	OSCManager()
	{
	}
	
	public static OSCManager Instance 
	{
	    get 
		{
	        if (_instance == null) 
			{
				_instance = new GameObject ("OSCManager").AddComponent<OSCManager>();
	        }    
	        return _instance;
	    }
	}
	#endregion
	
	#region Member Variables -------------------------------------
	private static OSCManager _instance = null;
	private Dictionary<string, ClientData> _clients = new Dictionary<string, ClientData>();
	private Dictionary<string, ServerData> _servers = new Dictionary<string, ServerData>();
	private RCReceiver[] OSCinputObjects;
	private string[] OSCaddresses;
	private const int _loglength = 25;
	#endregion
	

	
	#region Properties -------------------------------------
	public Dictionary<string, ClientData> Clients
	{
		get
		{
			return _clients;
		}
	}
	
	public Dictionary<string, ServerData> Servers
	{
		get
		{
			return _servers;
		}
	}
	#endregion
	
	#region Methods -------------------------------------
	// Setup procedure - Create a vector with all the addresses 
	public void Init()
	{
		// find control input objects with all their addresses to be easy to track
		OSCinputObjects = FindObjectsOfType (typeof(RCReceiver)) as RCReceiver[];
		OSCaddresses = new string[OSCinputObjects.Length];
		if (OSCinputObjects.Length > 0)
		{
			int i = 0;
			foreach(RCReceiver item in OSCinputObjects)
			{
				OSCaddresses[i] = item.address;
				i++;
			}
		}
	}

	void OnApplicationQuit() 
	{
		foreach(KeyValuePair<string,ClientData> pair in _clients)
		{
			pair.Value.client.Close();
		}
		
		foreach(KeyValuePair<string,ServerData> pair in _servers)
		{
			pair.Value.server.Close();
		}
			
		_instance = null;
	}
	

	public void CreateClient(string clientId, IPAddress destination, int port)
	{
		ClientData clientitem = new ClientData();
		clientitem.client = new OSCClient(destination, port);
		clientitem.log = new List<string>();
		clientitem.messages = new List<OSCMessage>();
		
		_clients.Add(clientId, clientitem);
		
		// Send test message
		string testaddress = "/RemoteControl/alive/";
		OSCMessage message = new OSCMessage(testaddress, destination.ToString());
		message.Append(port); message.Append("OK");
		_clients[clientId].log.Add(String.Concat(DateTime.UtcNow.ToString(),".",
		                                         FormatMilliseconds(DateTime.Now.Millisecond), " : ",
		                                         testaddress," ", DataToString(message.Data)));
		_clients[clientId].messages.Add(message);
		
		_clients[clientId].client.Send(message);
	}
	

	public void CreateServer(string serverId, int port)
	{
        OSCServer server = new OSCServer(port);
        server.PacketReceivedEvent += OnPacketReceived;

        ServerData serveritem = new ServerData();
        serveritem.server = server;
		serveritem.log = new List<string>();
		serveritem.packets = new List<OSCPacket>();
		
		_servers.Add(serverId, serveritem);
	}

    void OnPacketReceived(OSCServer server, OSCPacket packet)
    {

    }
		
	public void SendMessageToClient<T>(string clientId, string address, T value)
	{
		List<object> temp = new List<object>();
		temp.Add(value);
		SendMessageToClient(clientId, address, temp);
	}
	

	public void SendMessageToClient<T>(string clientId, string address, List<T> values)
	{	
		if(_clients.ContainsKey(clientId))
		{

			OSCMessage message = new OSCMessage(address);
		
			foreach(T msgvalue in values)
			{
				message.Append(msgvalue);
			}
			
			if(_clients[clientId].log.Count < _loglength)
			{
				_clients[clientId].log.Add(String.Concat(DateTime.UtcNow.ToString(),".",
				                                         FormatMilliseconds(DateTime.Now.Millisecond),
				                                         " : ", address, " ", DataToString(message.Data)));
				_clients[clientId].messages.Add(message);
			}
			else
			{
				_clients[clientId].log.RemoveAt(0);
				_clients[clientId].messages.RemoveAt(0);
				
				_clients[clientId].log.Add(String.Concat(DateTime.UtcNow.ToString(),".",
				                                         FormatMilliseconds(DateTime.Now.Millisecond),
				                                         " : ", address, " ", DataToString(message.Data)));
				_clients[clientId].messages.Add(message);
			}
			// TODO: optimize this connection
			_clients[clientId].client.Connect();
			_clients[clientId].client.Send(message);

		}
		else
		{
			Debug.LogError(string.Format("Can't send OSC messages to {0}. Client doesn't exist.", clientId));
		}
	}
	



	/// Updates clients and servers Data.
	public void UpdateLogs()
	{
		foreach(KeyValuePair<string,ServerData> pair in _servers)
		{
			if(_servers[pair.Key].server.LastReceivedPacket != null)
			{
				//Initialization for the first packet received
				if(_servers[pair.Key].log.Count == 0)
				{	
					_servers[pair.Key].packets.Add(_servers[pair.Key].server.LastReceivedPacket);
						
					_servers[pair.Key].log.Add(String.Concat(DateTime.UtcNow.ToString(), ".",
					                                         FormatMilliseconds(DateTime.Now.Millisecond)," : ",
					                                         _servers[pair.Key].server.LastReceivedPacket.Address," ",
					                                         DataToString(_servers[pair.Key].server.LastReceivedPacket.Data)));
					break;
				}
						
				if(_servers[pair.Key].server.LastReceivedPacket.TimeStamp
				   != _servers[pair.Key].packets[_servers[pair.Key].packets.Count - 1].TimeStamp)
				{	
					if(_servers[pair.Key].log.Count > _loglength - 1)
					{
						_servers[pair.Key].log.RemoveAt(0);
						_servers[pair.Key].packets.RemoveAt(0);
					}
		

					_servers[pair.Key].packets.Add(_servers[pair.Key].server.LastReceivedPacket);
						
					_servers[pair.Key].log.Add(String.Concat(DateTime.UtcNow.ToString(), ".",
					                                         FormatMilliseconds(DateTime.Now.Millisecond)," : ",
					                                         _servers[pair.Key].server.LastReceivedPacket.Address," ",
					                                         DataToString(_servers[pair.Key].server.LastReceivedPacket.Data)));
					/// found a corresponding message
					if (System.Array.IndexOf (OSCaddresses, _servers[pair.Key].server.LastReceivedPacket.Address) != -1) 
					{
						// send to all addresses in the address vector, run through all vector and compare with the incoming address
						// TODO: optimize this please
						for(int i = 0; i < OSCinputObjects.Length; i++)
						{
							if(_servers[pair.Key].server.LastReceivedPacket.Address == OSCaddresses[i])
							{
								if(OSCinputObjects[i].enabled)
									OSCinputObjects[i].ProcessOSC( _servers[pair.Key].server.LastReceivedPacket);

							}
						}


					}

				}
			}
		}
	}
	

	/// Converts a collection of object values to a concatenated string.
	private string DataToString(List<object> data)
	{
		string buffer = "";
		
		for(int i = 0; i < data.Count; i++)
		{
			buffer += data[i].ToString() + " ";
		}
		
		buffer += "\n";
		
		return buffer;
	}

	/// Formats a milliseconds number to a 000 format. E.g. given 50, it outputs 050. Given 5, it outputs 005
	private string FormatMilliseconds(int milliseconds)
	{	
		if(milliseconds < 100)
		{
			if(milliseconds < 10)
				return String.Concat("00",milliseconds.ToString());
			
			return String.Concat("0",milliseconds.ToString());
		}
		
		return milliseconds.ToString();
	}
			
	#endregion
}	

