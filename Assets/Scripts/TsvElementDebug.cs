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
