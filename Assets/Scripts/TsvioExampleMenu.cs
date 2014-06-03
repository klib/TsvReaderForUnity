//
//  TsvExampleMenu
//
// Copyright (c) 2014 Keita Nakazawa. All rights reserved.
//
// MIT Lisence
//#####################################################################
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//
// 以下に定める条件に従い、本ソフトウェアおよび関連文書のファイル（以下「ソフトウェア」）の複製を取得する
// すべての人に対し、ソフトウェアを無制限に扱うことを無償で許可します。
// これには、ソフトウェアの複製を使用、複写、変更、結合、掲載、頒布、サブライセンス、
// および/または販売する権利、およびソフトウェアを提供する相手に同じことを許可する権利も無制限に含まれます。
// 上記の著作権表示および本許諾表示を、ソフトウェアのすべての複製または重要な部分に記載するものとします。
// ソフトウェアは「現状のまま」で、明示であるか暗黙であるかを問わず、何らの保証もなく提供されます。
// ここでいう保証とは、商品性、特定の目的への適合性、および権利非侵害についての保証も含みますが、
// それに限定されるものではありません。
// 作者または著作権者は、契約行為、不法行為、またはそれ以外であろうと、ソフトウェアに起因または関連し、
// あるいはソフトウェアの使用またはその他の扱いによって生じる一切の請求、損害、その他の義務について
// 何らの責任も負わないものとします。
//
//#####################################################################
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections;
using System.Collections.Generic;


public class TsvioExampleMenu : MonoBehaviour {
	
	StringBuilder	parseResult	= null;						///< The log buffer for results.
	TsvElementDebug	debugTree	= new TsvElementDebug ();	///< The Elements tree debug object.
	
	//=============================================================
	/**
	 *	@brief	Example GUI
	 */
	//=============================================================
	void OnGUI()
	{
		Rect rect = new Rect(10, 10, 400, 64);
		
		// PlainText Example
		//------------------------------------------------------
		if ( GUI.Button (rect, "PlainText Load & Save test") )
		{
			PlainExample ();
		}
		
		rect.y += 80;
		
		// KeyPairText Example
		//------------------------------------------------------		
		if ( GUI.Button (rect, "KeyPairText Load & Save test") )
		{
			
			KeyPairExample ();
			
		}
		
		rect.y += 80;
		
		// TreeText Example
		//------------------------------------------------------
		if ( GUI.Button (rect, "TreeText Load & Save test") )
		{
			
			TreeTextExample ();
			
		}
		
		rect.y += 80;
		rect.height = 132;
		
		if ( parseResult != null )
		{
		
			GUI.TextArea ( rect, parseResult.ToString () );
			
		}
		
	}
	

	//=============================================================
	/**
	 *	@brief	KeyPairText i/o example 
	 */
	//=============================================================
	void PlainExample () {
	    
		// Please check the "Tsv/KeyPairTextExample.tsv".
		Tsvio		tsvio		= new Tsvio ( Tsvio.Mode.PlainText );
		string		path		= "Tsv/PlainTextExample";
		TextAsset	txt			= Resources.Load ("Tsv/PlainTextExample", typeof(TextAsset) ) as TextAsset;
		string		tsv_text	= Encoding.UTF8.GetString ( txt.bytes );
		
		parseResult = new StringBuilder ();
		
		// If you want to see the console messages, then set to true below.
		tsvio.debugMode		= false;
		
		// Set log function callback.
		tsvio.DelegateLog = AppendLog;
		
		AppendLog (
			"***************************\n" +
			"The loaded elements tree made for debugging.\n" +
			"Please open the example tsv file from "+ path +".\n" +
			"and, Compare tree in the hierarchy tab.\n" + 
			"***************************\n");
		
		// For example project.
		if ( debugTree != null )
		{
			
			debugTree.Clear ();
			
		}
		else
		{
			
			debugTree = new TsvElementDebug ();
			
		}
		
		// Parse the tree structured tsv string.
		if ( tsvio.Parse ( tsv_text ) )
		{
			
			int len = tsvio.plain.Length;
			
			// Set Element tree visualize debug by the GameObjects tree.
			debugTree.Parse ( tsvio.plain, "[Tsvio] Root" );
			
			//#######################################################
			//
			// GetAttributes example
			//
			//#######################################################
			for ( int i=0; i<len; ++i )
			{
				
				AppendLog ( "PlainExample["+ i +"]:"+ string.Join (", ", tsvio.plain[i]) );
				
			}
			
			//#######################################################
			//
			// Ways of add elements, and SaveTo example
			//
			//#######################################################
			int			n		= 10;
			string[][]	newtsv	= new string[n][];
			
			newtsv[0] = new string[1]{"# Plain text can not use comment line."};
			
			// Make data
			for ( int i=1; i<n; ++i )
			{
				
				newtsv[i] = new string[5]{ ""+(i*5), ""+(i*5+1), ""+(i*5+2), ""+(i*5+3), ""+(i*5+4) };
				
			}
			
			tsvio.plain = newtsv;
			
			path = Application.persistentDataPath + "/PlainWriteExample.tsv";
			tsvio.SaveTo ( path );
			
			AppendLog ("Example TSV saved to " + path + "\nPlease check the written tsv!");
			
			// If you want get only text from the KeypairText,
			// then call ToString () and be use the returned value.
			//string plainStr = tsvio.ToString (); // or tsvio.PlainToString ()
			
			// Your customize (encrypt etc...) and your save method add to here.
			//plainStr = YourEncrypt (plainStr);
			//YourSave (plainStr);

			
		}
		
		
		
	}
	
	
	//=============================================================
	/**
	 *	@brief	KeyPairText i/o example 
	 */
	//=============================================================
	void KeyPairExample () {
	    
		// Please check the "Tsv/KeyPairTextExample.tsv".
		Tsvio		tsvio		= new Tsvio ( Tsvio.Mode.KeyPairText );
		string		path		= "Tsv/KeyPairTextExample";
		TextAsset	txt			= Resources.Load (path, typeof(TextAsset) ) as TextAsset;
		string		tsv_text	= Encoding.UTF8.GetString ( txt.bytes );
		
		
		
		parseResult = new StringBuilder ();
		
		// If you want to see the console messages, then set to true below.
		tsvio.debugMode		= false;
		
		// Set log function callback.
		tsvio.DelegateLog = AppendLog;
		
		AppendLog (
			"***************************\n" +
			"The loaded elements tree made for debugging.\n" +
			"Please open the example tsv file from "+ path +".\n" +
			"and, Compare tree in the hierarchy tab.\n" + 
			"***************************\n");
		
		// For example project.
		if ( debugTree != null )
		{
			
			debugTree.Clear ();
			
		}
		else
		{
			
			debugTree = new TsvElementDebug ();
			
		}
		
		// Parse the tree structured tsv string.
		if ( tsvio.Parse ( tsv_text ) )
		{
			
			Dictionary <string, string> keypair = tsvio.keypair;
			string blstr = keypair["boolExample"];
				
			if ( bool.Parse (blstr) )
			{
				// Set Element tree visualize debug by the GameObjects tree.
				debugTree.Parse ( tsvio.keypair, "[Tsvio] Root" );
			
				
				//#######################################################
				//
				// GetAttributes example
				//
				//#######################################################
				AppendLog ( "KeyPairExample :" + ( int.Parse(keypair["intExample"]) + (double.Parse (keypair["doubleExample"])) ) );
				AppendLog ( "KeyPairExample2:" + keypair["stringExample"]);
				
				//#######################################################
				//
				// Ways of add elements, and SaveTo example
				//
				//#######################################################
				keypair["AddValue"] = "Added example.";
				
				path = Application.persistentDataPath + "/KeypairWriteExample.tsv";
				tsvio.SaveTo ( path );
				
				AppendLog ("Example TSV saved to " + path + "\nPlease check the written tsv!");
				
				// If you want get only text from the KeypairText,
				// then call ToString () and be use the returned value.
				//string keypairStr = tsvio.ToString (); // or tsvio.KeypairToString ()
				
				// Your customize (encrypt etc...) and your save method add to here.
				//keypairStr = YourEncrypt (keypairStr);
				//YourSave (keypairStr);
			}
			
		}
		
	}
	
	//=============================================================
	/**
	 *	@brief	TreeText i/o example 
	 */
	//=============================================================
	void TreeTextExample () {
	    
		// Please check the "Tsv/TreeTextExample.tsv".
		Tsvio		tsvio		= new Tsvio ( Tsvio.Mode.TreeText );
		string		path		= "Tsv/TreeTextExample";
		TextAsset	txt			= Resources.Load (path, typeof(TextAsset) ) as TextAsset;
		string		tsv_text	= Encoding.UTF8.GetString ( txt.bytes );
		
		parseResult = new StringBuilder ();
		
		// If you want to see the console messages, then set to true below.
		tsvio.debugMode		= false;
		
		// Set log function callback.
		tsvio.DelegateLog = AppendLog;
		
		AppendLog (
			"***************************\n" +
			"The loaded elements tree made for debugging.\n" +
			"Please open the example tsv file from "+ path +".\n" +
			"and, Compare tree in the hierarchy tab.\n" + 
			"***************************\n");
		
		// For example project.
		if ( debugTree != null )
		{
			
			debugTree.Clear ();
			
		}
		else
		{
			
			debugTree = new TsvElementDebug ();
			
		}
		
		// Parse the tree structured tsv string.
		if ( tsvio.Parse ( tsv_text ) )
		{
			// Set Element tree visualize debug by the GameObjects tree.
			debugTree.Parse ( tsvio.rootObject, "[Tsvio] Root", null );
			
			
			//#######################################################
			//
			// GetAttributes example
			//
			//#######################################################
			
			// Get the tsv element's attribute by selector string.
			string[] res = tsvio.GetAttributes ("TreeTextExample > String example > Single line");
			
			if ( res != null )
			{
			
				// Attributes was given as string[].
				AppendLog ("attributes: " + string.Join (", ", res) );
			
			}
			
			// If element have the same name more than one target, you can select one by index.
			res = tsvio.GetAttributes ("TreeTextExample > String example > Single line[1]");
			
			if ( res != null )
			{
			
				// Attributes was given as string[].
				AppendLog ("attributes: " + string.Join (", ", res) );
			
			}
			
			
			
			//#######################################################
			//
			// GetElement example
			//
			//#######################################################
			
			// This case is to get the first level element.
			TsvElement elem = tsvio.GetElement("TreeTextExample");
			
			if ( elem != null )
			{
				
				// Access attribute in the element.
				AppendLog ("Element attributes: " + string.Join (", ", elem.attr) );
				
			}
			
			
			// Get the elements having multi line string.
			elem = tsvio.GetElement("TreeTextExample > String example > Multiple");
			
			if ( elem != null )
			{
				
				// Multi-line string is set on other buffer "multistr".
				AppendLog ("Element multiline: " + elem.multistr);
				
			}
			
			
			// Be able to get the child elements.
			elem = tsvio.GetElement("TreeTextExample > String example");
			
			if ( elem != null )
			{
				
				// Caution! If you accessed elements directly, Then must be added to index of the List. like [0]			
				// This element is same as selector of "TreeTextExample > String example > Single line[0]"
				AppendLog ("Element array access: " + string.Join (", ", elem.childs["Single line"][0].attr) );
				
				// This element is same as selector of "TreeTextExample > String example > Single line[1]"
				AppendLog ("Element array access: " + string.Join (", ", elem.childs["Single line"][1].attr) );
			
			}
			
			
			// And, You can access directly by the TsvElements indexer.
			// This example is same to GetElement("TreeTextExample > String example")
			elem = tsvio.rootObject["TreeTextExample > String example"];
			
			if ( elem != null )
			{
			
				// This element is same as selector of "TreeTextExample > String example > Single line[0]"
				AppendLog ("Direct access: " + string.Join (", ", elem.childs["Single line"][0].attr) );
				
				// This element is same as selector of "TreeTextExample > String example > Single line[1]"
				AppendLog ("Direct access: " + string.Join (", ", elem.childs["Single line"][1].attr) );
			
			}
			
			//#######################################################
			//
			// Ways of add elements, and SaveTo example
			//
			//#######################################################
			
			// Add element to current tree.
			string[]	attributes	= new string[]{"This", "is", "added", "element."};
			TsvElement	addElem		= Tsvio.CreateElement( attributes );
			
			
			// Child element add to the parent element. ( Call from the child element to the parent element. )
			// This time use the loaded tsv continued.
			addElem.AddTo ("AddToExample", tsvio.GetElement("TreeTextExample"));
			//
			// [Root]
			// |
			// +-- TreeTextExample
			//     |
			//     +-- AddToExample
			//
			
			// Child element add from the parent element. ( Call from the parent element to the child element. )
			tsvio.GetElement("TreeTextExample").AddChild ( "AddChildExample", addElem );
			// 
			// [Root]
			// |
			// +-- TreeTextExample
			//     |
			//     +-- AddToExample
			//     |
			//     +-- AddChildExample
			//
			
			// Get root object. ( Elements list dictionary. )
			TsvElement root = tsvio.rootObject;
			
			
			// Make new tree in root.
			root["NewHeader(only header)"] = Tsvio.CreateElement ();	// Only header
			root["NewHeader(with attributes)"] = Tsvio.CreateElement (new string[]{"Attribute\t\n", "Example"}); // With attributes
			
			
			// Make new tree levels.
			// If not found an element of a key in the selector, Tsvio will create there keys.
			root["AAAAA > BBBBB > CCCCC > DDDDD > EEEEE"] = Tsvio.CreateElement ("This is multi line string.\nNew level making example.");
			
			path = Application.persistentDataPath + "/TreeWriteExample.tsv";
			tsvio.SaveTo ( path );
			
			AppendLog ("Example TSV saved to " + path + "\nPlease check the written tsv!");
			
			
			// If you want get only text from the TreeText,
			// then call ToString () and be use the returned value.
			//string treeToStr = tsvio.ToString (); // or tsvio.TreeToString ()
			
			// Your customize (encrypt etc...) and your save method add to here.
			//treeToStr = YourEncrypt (treeToStr);
			//YourSave (treeToStr);
			
		}
		
	}
	
	//====================================================
	/**
	 *	@brief	Output string to log buffer.
	 */
	//====================================================
	void AppendLog ( string str ) {
		
		Debug.Log ( str  );
		parseResult.Append ( str + "\n" );
		
	}
	
}

