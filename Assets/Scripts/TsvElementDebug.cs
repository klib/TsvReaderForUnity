//
//  TsvElementDebug
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
using System.Collections.Generic;

//===================================================
/**
 * 	@brief	Class of the TsvElement debug for serialize attributes.
 */
//===================================================
[System.Serializable]
public class TsvDebugTree : MonoBehaviour {
	
	public string[]	attributes;
	public string	text;
	
}

//===================================================
/**
 * 	@brief	Class of the Keypair debug for serialize attributes.
 */
//===================================================
[System.Serializable]
public class TsvDebugKeypair : MonoBehaviour {
	
	public string value;
	
}

//===================================================
/**
 * 	@brief	Class of the plain tsv debug for serialize attributes.
 */
//===================================================
[System.Serializable]
public class TsvDebugPlain : MonoBehaviour {
	
	public string[] values;
	
}

//===================================================
/**
 * 	@brief	Class of the TsvElement tree for debug at the Hierarchy tree.
 */
//===================================================
public class TsvElementDebug {
	
	/// Root element
	GameObject	rootObj	= null;
	
	//===================================================
	/**
	 *	@brief	Parse TsvElement structure and convert to GameObject components.
	 *
	 *	@param	target		Parse target element.
	 *	@param	targetName	Target name for hierarchy.
	 *	@param	parentObj	Parent TsvElement. If target is root object then to set to null.
	 */
	//===================================================
	public void Parse ( TsvElement target, string targetName, GameObject parentObj ) {
		
		Elements.KeyCollection	keys		= target.childs.Keys;
		int						len			= 0;
		
		// Make object
		//- - - - - - - - - - - - - - - - - - - - -
		GameObject		obj		= new GameObject (targetName);
		TsvDebugTree	elem	= obj.AddComponent <TsvDebugTree> ();
		
		// Link parent and childs
		if ( parentObj == null )
		{
		
			rootObj = obj;
			
		}
		else
		{
			
			obj.transform.parent	= parentObj.transform;
			
		}
		
		// Set attributes.
		elem.attributes	= target.attr;
		elem.text		= target.multistr;
		
		// Recursion loop for the Child elements.
		foreach ( string key in keys )
		{
			
			len = target.childs[key].Count;
			
			for ( int i=0; i<len; ++i )
			{
				
				Parse ( target.childs[key][i], key, obj );
				
			}
			
		}
		
	}
	
	
	//===================================================
	/**
	 *	@brief	Parse Keypair and convert to GameObject components.
	 *
	 *	@param	data		Keypair dictionary. (Type of Dictionary <string, string>)
	 *	@param	rootName	The root GameObject name.
	 */
	//===================================================
	public void Parse ( Dictionary <string, string> data, string rootName ) {
		
		Dictionary <string, string>.KeyCollection keys = data.Keys;
		
		// Make the root object.
		//- - - - - - - - - - - - - - - - - - - - -
		GameObject obj = new GameObject (rootName);
		rootObj = obj;
		
		foreach ( string key in keys )
		{
		
			// Make object
			//- - - - - - - - - - - - - - - - - - - - -
			obj						= new GameObject (key);
			TsvDebugKeypair	elem	= obj.AddComponent <TsvDebugKeypair> ();
			elem.value				= data[key];
			obj.transform.parent	= rootObj.transform;
			
		}
		
	}

	//===================================================
	/**
	 *	@brief	Parse plain tsv table and convert to GameObject components.
	 *
	 *	@param	data	Plain tsv table array. (Type of Dictionary <string, string>)
	 *	@param	rootName	The root GameObject name.
	 */
	//===================================================
	public void Parse ( string[][] data, string rootName ) {
		
		int len = data.Length;
		
		// Make the root object.
		//- - - - - - - - - - - - - - - - - - - - -
		GameObject obj = new GameObject (rootName);
		rootObj = obj;
		
		for ( int i=0; i<len; ++i )
		{
		
			// Make object
			//- - - - - - - - - - - - - - - - - - - - -
			obj		= new GameObject (""+i);
			TsvDebugPlain	elem	= obj.AddComponent <TsvDebugPlain> ();
			
			elem.values = data[i];
			
			obj.transform.parent	= rootObj.transform;
			
		}
		
	}
	
	
	//===================================================
	/**
	 *	@brief	Clear on hierarchy tree.
	 */
	//===================================================
	public void Clear () {
		
		if ( rootObj != null )
		{
			Object.Destroy (rootObj);
		}
		
	}
	
}
