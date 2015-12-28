//  RCSender [Remote Control Sender]
//  ------------------------------------
//
// 	Outputs OSC packadges from selected parameters of the Gameobject to the network (expose parameters)
//  Version 0.9
//
//  Remote Control for Unity - part of Digital Puppet Tools, A digital ecosystem for Virtual Marionette Project
//
//	Copyright (c) 2015 Luis Leite (Grifu)
//	www.virtualmarionette.grifu.com
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityOSC;

public class RCSender : MonoBehaviour {


	[SerializeField]
	public Component objectComponent;
	public int _componentIndex = 0;
	public int _generalIndex = 0;
	public int _portIndex = 0;			// network port
	public int _controlIndex = 0;
	public int _extra = 0;

	[SerializeField]
	public PropertyInfo propertyObject;
	public MethodInfo methodObject;
	public string address;
	public RCPortOut OSCtransmitPort;
	public bool sendEveryFrame = false;
	private ObjectRequirements requirements;	// requirments arguments (number and type)
	private object relativeValue;				// keep the original value for relative option
	public OSCManager handler;

	private object oldPropertyObject;
	// Check required arguments for this property	
	Type CheckArgumentType(PropertyInfo info)
	{
		
		Type type = info.PropertyType;
		//	requirements.requiredArgumentstype = type;
		if (type == typeof(int) || type == typeof(float) || type == typeof(bool) || type == typeof(string)) 
			requirements.requiredArgumentsAmount = 1;
		else if (type == typeof(Vector2))
			requirements.requiredArgumentsAmount = 2;
		else if (type == typeof(Vector3))
			requirements.requiredArgumentsAmount = 3;
		else if (type == typeof(Vector4))
			requirements.requiredArgumentsAmount = 4;
		else if (type == typeof(Enum))
			return type; // error
		else return type; // error
		
		return type;
		
	}


	
	
	// Initialization
	void Start () {
		if (propertyObject == null)
		{


			Type typeComponent = objectComponent.GetType();
			const BindingFlags flags = /*BindingFlags.NonPublic | */ BindingFlags.DeclaredOnly  | BindingFlags.Public | 
				BindingFlags.Instance | BindingFlags.Static;
			PropertyInfo[] properties = typeComponent.GetProperties(flags);


			if(properties.Length > _generalIndex) 
			{
				propertyObject = properties[_generalIndex];
				oldPropertyObject = propertyObject.GetValue(objectComponent,null);

			}
		}

		
		if (methodObject == null)
		{
			
			Type typeComponent = objectComponent.GetType();
			const BindingFlags flags = /*BindingFlags.NonPublic | */ BindingFlags.DeclaredOnly  | BindingFlags.Public | 
				BindingFlags.Instance | BindingFlags.Static;
			
			MethodInfo[] methods = typeComponent.GetMethods(flags);
			
			if(methods.Length > _generalIndex) methodObject = methods[_generalIndex];
			
			//		print (" This GameObject="+this.name+" | component=" + objectComponent+" | type="+typeComponent+" | prop="+propertyObject);
			
		}



		handler = FindObjectOfType (typeof(OSCManager)) as OSCManager;
		requirements = new ObjectRequirements();
	}




	void Update () {

		if(handler == null) handler = FindObjectOfType (typeof(OSCManager)) as OSCManager;

		// properties
		if(_controlIndex == 0)
		{
			// check if we have different incoming values to send or if we want to send every frames
			if(propertyObject.GetValue(objectComponent,null).Equals(oldPropertyObject) && !sendEveryFrame)
			{
				//print("equal");
			} else
			{
				oldPropertyObject = propertyObject.GetValue(objectComponent,null);
				List<object> objects = new List<object>();
				objects = listConvertedOSC(propertyObject);

				//		print ("handler = "+propertyObject.GetValue(objectComponent,null)+" type="+propertyObject.GetValue(objectComponent,null).GetType()+" name="+propertyObject);
				string clientName = "RemoteClient-"+OSCtransmitPort.port;

				if(objects != null) handler.SendMessageToClient(clientName, address, objects);

			}






		} else if(_controlIndex == 1)	// method sending not include yet
		{
			if(methodObject != null)
			{
				// for now its just for blendhshapes
				// TODO: create a generic method for sending all the data that derives from methodObject
				if(methodObject.Name == "GetBlendShapeWeight")
				{
					GameObject objectTemp = this.gameObject;
					SkinnedMeshRenderer meshTemp = objectTemp.GetComponent<SkinnedMeshRenderer>();
//					print (" blendshape ---------- " +  meshTemp.GetBlendShapeWeight(_extra));		// Extra is used as the blendshape index
					string clientName = "RemoteClient-"+OSCtransmitPort.port;
					List<object> objects = new List<object>();
					objects.Add(meshTemp.GetBlendShapeWeight(_extra)); // Extra is used as the blendshape index
					handler.SendMessageToClient(clientName, address, objects);		// send OSC message to client
				}

			}
		}
	}


	// Os OSC-out enviam os parametros para um função no OSCmanager que envia para fora
	// tenho de implementar uma lista de selecção de Ports criados no OSCmanager
	// Sempre que o parametro mudar ? ou sempre ?


	// method to convert types to the correct OSC format
	List<object> listConvertedOSC(PropertyInfo propObject)
	{
		
		// Add to the list the correct sequence of values
		// TODO: This should be optimized to convert.changetype
		List<object> objectValuesToSend = new List<object>();

		
		if(propObject.GetValue(objectComponent,null) != null) 
		{
//			 print (" TYPE = "+propObject.GetValue(objectComponent,null).GetType());



		//	if (propObject.GetValue (objectComponent, null).GetType != null)
		//					print ("jdhsfk");
			
			if(propObject.GetValue(objectComponent,null).GetType() == typeof(Vector2))
			{
				for(int x=0;x<2;x++)
					objectValuesToSend.Add(((Vector2)propObject.GetValue(objectComponent,null))[x]);
				
			} else if(propObject.GetValue(objectComponent,null).GetType() == typeof(Vector3))
			{	
				for(int x=0;x<3;x++)
					objectValuesToSend.Add(((Vector3)propObject.GetValue(objectComponent,null))[x]);
				
			} else if(propObject.GetValue(objectComponent,null).GetType() == typeof(Vector4))
			{
				for(int x=0;x<4;x++)
					objectValuesToSend.Add(((Vector4)propObject.GetValue(objectComponent,null))[x]);	
			} else if(propObject.GetValue(objectComponent,null).GetType() == typeof(Boolean))
			{
				objectValuesToSend.Add( ((bool)propObject.GetValue(objectComponent,null) == true) ? 1 : 0);//
			} else if(propObject.GetValue(objectComponent,null).GetType() == typeof(Single))
			{
				objectValuesToSend.Add( ((float)propObject.GetValue(objectComponent,null)));
			} else if(propObject.GetValue(objectComponent,null).GetType() == typeof(Transform))	// send all data from transform
			{
				Transform tempTransform;
				tempTransform = (Transform)propObject.GetValue(objectComponent,null);
				
				for(int x=0;x<3;x++)
					objectValuesToSend.Add((float)tempTransform.position[x]);
				for(int x=0;x<3;x++)
					objectValuesToSend.Add((float)tempTransform.localScale[x]);
				for(int x=0;x<4;x++)
					objectValuesToSend.Add((float)tempTransform.rotation[x]);
			} else if(propObject.GetValue(objectComponent,null).GetType() == typeof(Matrix4x4))
			{
				for(int y=0;y<3;y++)
				{
					for(int x=0;x<3;x++)
					{
						float tempFloat = ((Matrix4x4)propObject.GetValue(objectComponent,null))[x,y];
						objectValuesToSend.Add(tempFloat);
					}
				}
				
			} else if(propObject.GetValue(objectComponent,null).GetType() == typeof(int))
			{
				objectValuesToSend.Add((int)propObject.GetValue(objectComponent,null));

			} else if(propObject.GetValue(objectComponent,null).GetType() == typeof(Quaternion))
			{
				for(int x=0;x<4;x++)
					objectValuesToSend.Add(((Quaternion)propObject.GetValue(objectComponent,null))[x]);
				
			} else
			{
				
				objectValuesToSend.Add((string)propObject.GetValue(objectComponent,null));
				
			}
			

		} 
		return objectValuesToSend;
	}





}




