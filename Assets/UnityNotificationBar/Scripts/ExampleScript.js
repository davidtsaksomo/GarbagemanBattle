/************************************************************************
* Copyright 2012  Pigobo Limited
* Author: Simon Keating email: support@pigobo.com
*
*Example of UNotify usage.
* ************************************************************************/

#pragma strict
var notificationSkin:GUISkin;
var messageNumber:int;
var testBackgroundObject:GameObject;
var testBackgroundGuiTexture:GUITexture;

function Start () {
	
	//create a background gameObject
	testBackgroundObject=GameObject.Find("TestBackground");
	testBackgroundGuiTexture=testBackgroundObject.GetComponent (GUITexture);
}

function Update () {

	testBackgroundGuiTexture.pixelInset.width=Screen.width;
	testBackgroundGuiTexture.pixelInset.height=Screen.height;
}

function OnGUI()
{
	
	//debug buttons//////////////////////////////////////////////////////////
	if (GUI.Button(Rect((Screen.width/2)-60,Screen.height-50,120,25),"Basic Message"))
    	{
    		
    		UnityNotificationBar.UNotify(GetMessage());
    		messageNumber++;
    		if(messageNumber>6){messageNumber=0;}
    	}
    	
    if (GUI.Button(Rect((Screen.width/2)-60,Screen.height-80,120,25),"Achievement"))
    	{
    		UnityNotificationBar.UNotify("Achievement Unlocked!  500 Experience Points!");
    	}
    ////////////////////////////////////////////////////////////////////////
 }
 
function GetMessage()
{
	var message:String;
	switch (messageNumber)
	{
	case 0:
	message="The Unity Notification Bar is easy to use with just one line of code..";
	break;
	case 1:
	message="UnityNotificationBar.UNotify('Your message'); is all you need! The bar does the rest!";
	break;
	case 2:
	message="Use it for Achievements, Chat, Debugging, Help Tips or anything else...";
	break;
	case 3:
	message="The Unity Notification bar is easy to modify..";
	break;
	case 4:
	message="Change the background and font just my modifying the GUI skin for the bar in the inspector.";
	break;
	case 5:
	message="You can also customize the size and position of the bar, the on-screen time and fade speed!";
	break;
	case 6:
	message="It's so simple! And it's time and $$$ saved for you!";
	break;
	
	}
	return message;
}