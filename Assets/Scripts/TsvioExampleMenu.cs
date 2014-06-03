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
// �ȉ��ɒ�߂�����ɏ]���A�{�\�t�g�E�F�A����ъ֘A�����̃t�@�C���i�ȉ��u�\�t�g�E�F�A�v�j�̕������擾����
// ���ׂĂ̐l�ɑ΂��A�\�t�g�E�F�A�𖳐����Ɉ������Ƃ𖳏��ŋ����܂��B
// ����ɂ́A�\�t�g�E�F�A�̕������g�p�A���ʁA�ύX�A�����A�f�ځA�Еz�A�T�u���C�Z���X�A
// �����/�܂��͔̔����錠���A����у\�t�g�E�F�A��񋟂��鑊��ɓ������Ƃ������錠�����������Ɋ܂܂�܂��B
// ��L�̒��쌠�\������і{�����\�����A�\�t�g�E�F�A�̂��ׂĂ̕����܂��͏d�v�ȕ����ɋL�ڂ�����̂Ƃ��܂��B
// �\�t�g�E�F�A�́u����̂܂܁v�ŁA�����ł��邩�Öقł��邩���킸�A����̕ۏ؂��Ȃ��񋟂���܂��B
// �����ł����ۏ؂Ƃ́A���i���A����̖ړI�ւ̓K�����A����ь�����N�Q�ɂ��Ă̕ۏ؂��܂݂܂����A
// ����Ɍ��肳�����̂ł͂���܂���B
// ��҂܂��͒��쌠�҂́A�_��s�ׁA�s�@�s�ׁA�܂��͂���ȊO�ł��낤�ƁA�\�t�g�E�F�A�ɋN���܂��͊֘A���A
// ���邢�̓\�t�g�E�F�A�̎g�p�܂��͂��̑��̈����ɂ���Đ������؂̐����A���Q�A���̑��̋`���ɂ���
// ����̐ӔC������Ȃ����̂Ƃ��܂��B
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

