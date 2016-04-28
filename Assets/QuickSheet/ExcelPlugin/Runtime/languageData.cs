using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class languageData
{
	[SerializeField]
	int number;
	
	[ExposeProperty]
	public int Number { get {return number; } set { number = value;} }
	
	[SerializeField]
	string korean;
	
	[ExposeProperty]
	public string Korean { get {return korean; } set { korean = value;} }
	
	[SerializeField]
	string english;
	
	[ExposeProperty]
	public string English { get {return english; } set { english = value;} }
	
	[SerializeField]
	string chinese;
	
	[ExposeProperty]
	public string Chinese { get {return chinese; } set { chinese = value;} }
	
	[SerializeField]
	string divide_line;
	
	[ExposeProperty]
	public string Divide_line { get {return divide_line; } set { divide_line = value;} }
	
}