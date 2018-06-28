
/************************************************************************
* Copyright 2012  Pigobo Limited
* Author: Simon Keating email: support@pigobo.com
*
*The Unity Notification bar is a simple utility to display notification messages at the top
*of the Unity window for a short time.
*Just call this class from anywhere in your project
* ************************************************************************/
////////////////////////////////////////////////////////////////////
//Version:1.1  August 24th 2012
////////////////////////////////////////////////////////////////////
#pragma strict
#pragma implicit
#pragma downcast


 class UnityNotificationBar extends MonoBehaviour
{
	
	
  	private var numberOfMessagesCurrentlyDisplayed:int;
	private var timeDelay:int;//after this time the message is forever hidden from view
	public static var unbScript : UnityNotificationBar;
	public static var unbGameObject :GameObject;
	//Skin
	public var unbSkin:GUISkin;
	
	//vars
//	public static var box_width:float=Screen.width;
//	public static var box_height:float=30;
//	public static var position_x:float=0;
//	public static var position_y:float=-box_height;
	
	public static var showingNewMessage :boolean;
	public static var numberOfExistingMessages :int=-1;//Strange math means this must start at -1
	public static var numberOfCreatedMessages :int;
	
	
	////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////
	//Constructor
	//The first time the class is called the constructor is run
	static private function UnityNotificationBar()
	{

	  	Debug.Log("Unity Notification Bar Constructor Initialized..");
	  	CreateNotificationBarGameObject();//Creates the gamebject and assigns component/skin etc
	}
	////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////
	
	////////////////////////////////////////////////////////////////////
	//PUBLIC METHODS
	////////////////////////////////////////////////////////////////////
//	public static function Unotify(message :String)
//	{
//		//Unotify(message,30);
//	}
	
	public static function UNotify(message :String)
	{
		Debug.Log("numberOfExistingMessages="+numberOfExistingMessages);
		numberOfExistingMessages+=1;
		numberOfCreatedMessages+=1;
		//create a new game object under the main Unity Notification Bar object
		var notification :GameObject = new GameObject();
		notification.name="Notification"+numberOfCreatedMessages;
		notification.transform.parent=unbGameObject.transform;
		var notificationScript : Notification=notification.AddComponent.<Notification>();
		notificationScript.id=numberOfCreatedMessages;
		notificationScript.notificationSkin=unbScript.unbSkin;
		notificationScript.myMessage=message;
		

	  	showingNewMessage=true;
		
		Debug.Log("numberOfExistingMessages="+numberOfExistingMessages);
	}
	
	
	
	////////////////////////////////////////////////////////////////////
	//PRIVATE METHODS
	//accessable from this class but not from outside
	////////////////////////////////////////////////////////////////////
	 private static function CreateNotificationBarGameObject()
	{
		
		//Creates a game object inside the game the first time it is called and also attaches THIS
		//script to it so we can run the ONGUI function in this script (but outside of the class)
		unbGameObject  = new GameObject();
		DontDestroyOnLoad(unbGameObject);//allows the bar to exist through scenes
		unbGameObject.name="Unity Notification Bar";
		unbScript=unbGameObject.AddComponent.<UnityNotificationBar>();//this is the clever bit :)

		//Assign the skin
		unbScript.unbSkin = Resources.Load("UNB_Skin");
		
	}
	
	
  
};



