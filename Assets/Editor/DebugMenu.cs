//
//  DebugMenu
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
using UnityEditor;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

//================================================================
/**
 *	@brief	Tsvio debug menu 
 */
//================================================================
public class DebugMenu : EditorWindow {
	
	//================================================================
	/**
	 *	@brief	Parse test 
	 */
	//================================================================
	[MenuItem("Debug/Test/TsvioParse")]
	public static void ParseTest()
	{
	    
		Tsvio		tsvio		= new Tsvio ( Tsvio.Mode.TreeText );
		TextAsset	txt			= Resources.Load ("Tsv/TreeTextSample", typeof(TextAsset) ) as TextAsset;
		string		tsv_text	= Encoding.UTF8.GetString ( txt.bytes );
		
		tsvio.debugMode		= false;
		tsvio.debugParsing	= true;
		
		// Parse the tree structured tsv.
		if ( tsvio.Parse ( tsv_text ) )
		{
			//#######################################################
			//
			// GetAttributes sample
			//
			//#######################################################
			
			// Get the tsv element's attribute by selector string.
			string[] res = tsvio.GetAttributes ("TreeTextSample > String sample > Single line");
			
			if ( res != null )
			{
			
				// Attributes was given as string[].
				Debug.Log ("attributes: " + string.Join (", ", res) );
			
			}
			
			// If element have the same name more than one target, you can select one by index.
			res = tsvio.GetAttributes ("TreeTextSample > String sample > Single line[1]");
			
			if ( res != null )
			{
			
				// Attributes was given as string[].
				Debug.Log ("attributes: " + string.Join (", ", res) );
			
			}
			
			
			
			//#######################################################
			//
			// GetElement sample
			//
			//#######################################################
			
			// This case is to get the root element.
			TsvElement elem = tsvio.GetElement("TreeTextSample");
			
			if ( elem != null )
			{
				
				// Access attribute in the element.
				Debug.Log ("Element attributes: " + string.Join (", ", elem.attr) );
				
			}
			
			
			// Get the elements having multi line string.
			elem = tsvio.GetElement("TreeTextSample > String sample > Multiple");
			
			if ( elem != null )
			{
				
				// Multi-line string is set on other buffer "multistr".
				Debug.Log ("Element multiline: " + elem.multistr);
				
			}
			
			
			// Be able to get the child elements.
			elem = tsvio.GetElement("TreeTextSample > String sample");
			
			if ( elem != null )
			{
				
				// Caution! If you accessed elements directly, Then must be added to index of the List. like [0]			
				// This element is same as selector of "TreeTextSample > String sample > Single line[0]"
				Debug.Log ("Element array access: " + string.Join (", ", elem.childs["Single line"][0].attr) );
				
				// This element is same as selector of "TreeTextSample > String sample > Single line[1]"
				Debug.Log ("Element array access: " + string.Join (", ", elem.childs["Single line"][1].attr) );
			
			}
			
			
			// And, You can access directly by the indexer.
			// This sample is same to GetElement("TreeTextSample > String sample")
			elem = tsvio.rootObject["TreeTextSample > String sample"];
			
			if ( elem != null )
			{
			
				// This element is same as selector of "TreeTextSample > String sample > Single line[0]"
				Debug.Log ("Direct access: " + string.Join (", ", elem.childs["Single line"][0].attr) );
				
				// This element is same as selector of "TreeTextSample > String sample > Single line[1]"
				Debug.Log ("Direct access: " + string.Join (", ", elem.childs["Single line"][1].attr) );
			
			}
			
			//#######################################################
			//
			// Ways of add elements, and SaveTo sample
			//
			//#######################################################
			
			// Add element to current tree.
			string[]	attributes	= new string[]{"This", "is", "added", "element."};
			TsvElement	addElem		= Tsvio.CreateElement( attributes );
			
			
			// Child element add to the parent element.
			addElem.AddTo ("AddToSample", tsvio.GetElement("TreeTextSample"));
			
			// Child element add from the parent element.
			tsvio.GetElement("TreeTextSample").AddChild ( "AddChildSample", addElem );
			
			
			// Get root object. ( Elements list dictionary. )
			TsvElement root = tsvio.rootObject;
			
			// Make new tree in root.
			root["NewHeader(only header)"] = Tsvio.CreateElement ();	// Only header
			root["NewHeader(with attributes)"] = Tsvio.CreateElement (new string[]{"Attribute", "Sample"}); // With attributes
			
			// Make new tree with new levels.
			root["AAAAA>BBBBB>CCCCC>DDDDD>EEEEE"] = Tsvio.CreateElement ("This is multi line string.\nNew line test.\nNew level making sample.");
			
			string path = Application.persistentDataPath + "/test.tsv";
			tsvio.SaveTo ( path );
			
			Debug.Log ("Sample TSV saved to " + path);
			Debug.Log ("Check out the written tsv!");
			
			// If you want get only text from the TreeText,
			// then call ToString () and be use the returned value.
			string treeToStr = tsvio.ToString (); // or tsvio.TreeToString ()
			
			// Your customize (encrypt etc...) and your save method add to here.
			
		}
		
	}
	
}
