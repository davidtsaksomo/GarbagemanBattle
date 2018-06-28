/************************************************************************
* Copyright 2012  Pigobo Limited
* Author: Simon Keating email: support@pigobo.com
*
*This script gets applied to each new notification. You can make size and speed modifications here.
* ************************************************************************/
#pragma strict
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////

//////////////////////////ADJUSTABLE VARIABLES////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////

var onScreenTime:float=4;//Default 4 //Change this to modify the number of seconds each notification remains on screen
//Increase the following variable to speed up thye fade out. Set to 1 to snap the notification from view.
var fadeOutSpeed:float=0.02;//Default 0.02
//You can modify these variables to adjust the size an position of the notification bar
var box_height:float=30;//Default 30. All notifications will be this hight in pixels. 
var position_x:float=0;//Default 0. By default the bar goes all the way across the screen but you can make it smaller!
var box_width:float=Screen.width;//By default the bar goes all the way across the screen but you can make it smaller!

//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////


//DO NOT ADJUST THE FOLLOWING VARS..YOU HAVE BEEN WARNED!
var position_y:float;//The Y position is set automatically (DO NOT ADJUST!!)
var notificationSkin:GUISkin;//If you want to mess with the skin, make a copy of the UNB_Skin resource.
var myMessage:String;
var numberOfExistingMessages:int;
var myTimer:float;
var myAlpha:float=1;
var isFading:boolean;
var previousNotification:Notification;

public static var id:int;

function Start () {
	//make sure the start position is correctly alligned above the previous one no matter how quickly you click!
	//get the prev notification object and its script
	var previousGameObject:GameObject=GameObject.Find("Notification"+(id-1));
	if(previousGameObject)
	{
		previousNotification=previousGameObject.GetComponent(Notification);
		position_y=previousNotification.position_y-box_height;
	}
	else
	{
		//if its the first one place it above the screen
		position_y=-box_height;
	}
}

function Update () {
	myTimer+=1*Time.deltaTime;
	
	if(myTimer>onScreenTime)
	{
		
		if(isFading==false)
		{
			isFading=true;
			FadeOut();
		}
		
	}
}

function OnGUI()
{
	GUI.skin=notificationSkin;
	
    
    if(UnityNotificationBar.showingNewMessage)//move everything down! (all messages)
    {        
		position_y++;	
	}
	if(position_y>=box_height*UnityNotificationBar.numberOfExistingMessages)
	{
		UnityNotificationBar.showingNewMessage=false;
	}
	
	GUI.color.a=myAlpha;
	GUI.Box(new Rect(position_x,position_y,box_width,box_height), myMessage);

	
}

function FadeOut()
{
	while(myAlpha>0)
	{
		myAlpha=myAlpha-fadeOutSpeed;
		yield;
	}
	//once fade, destroy me
	UnityNotificationBar.numberOfExistingMessages-=1;
	Destroy(this.gameObject);
}