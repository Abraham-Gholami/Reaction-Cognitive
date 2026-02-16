using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parent    {
	public int fatherAge { get; set; } 
	public int motherAge { get; set; }
	public string motherEducation { get; set; } = "";
	public string fatherEducation { get; set; } = "";
	public string motherJob { get; set; } = "";
	public string fatherJob { get; set; } = "";
}

public class Profile
{
	public string phone { get; set; } = "";
	public string physicalDisease { get; set; } = "";
	public string mentalDisease { get; set; } = "";
	public int age { get; set; }
	public string birthday { get; set; } = "";
	public string education { get; set; } = "";
	public bool rightHand { get; set; } = true;
	public bool man { get; set; } = true;
}


public class ApiUser
{
	public string id { get; set; } = "";
	public string appId { get; set; }  = "";
	public string userName { get; set; }  = "";
	public string firstName { get; set; }  = "";
	public string lastName { get; set; }  = "";
	public string password { get; set; }  = "";
	public string email { get; set; }  = "";
	public Parent parent { get; set; } = new();
	public Profile profile { get; set; } = new();
}