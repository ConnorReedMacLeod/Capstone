using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class LibText {

	public static Dictionary<string, char> dictTextConversions =
		new Dictionary<string, char>(){
		{"P", (char)176},
		{"M", (char)177},
		{"E", (char)178},
		{"B", (char)179},
		{"O", (char)180}};

	//Convert a single Symbol to it's ascii char representation
	public static char PrepSymbol(string sSym){
		if (dictTextConversions.ContainsKey (sSym)) {
			return dictTextConversions [sSym];
		} else {
			Debug.LogError ("ERROR!  " + sSym + " cannot be converted to a ascii character");
			return (char)0;
		}
	}

	//Convert any escaped words (using |xxx|) with their assigned ascii value
	public static string PrepText(string sText){

		StringBuilder test = new StringBuilder (sText);

		string[] arsComponents = sText.Split ('|');

		//Skip over the first section of text which isn't escaped
		int i = 1;
		Debug.Log ("Length of first is " + arsComponents [0].Length);


		while (i < arsComponents.Length) {

			if (!dictTextConversions.ContainsKey (arsComponents [i])) {
				//Then no dictionary translation exists
				Debug.LogError ("ERROR! NO TRANSLATION EXISTS FOR: " + arsComponents [i]);
			} else {

				//Replace this string with it's translation in the dictionary
				arsComponents [i] = (dictTextConversions [arsComponents [i]]).ToString();
			}

			i += 2; //advance to the next escaped string
		}

		return string.Join(string.Empty, arsComponents);
	}
}
