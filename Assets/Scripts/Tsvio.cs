//
//  Tsvio
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
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

//=====================================================
/**
 *	@brief	Tsv i/o class
 */
//=====================================================
public class Tsvio {
	
	/// I/O mode
	public enum Mode {
		
		PlainText,		///< Load to string[][] buffer. Not skip empty line and comment lines.
		KeyPairText,	///< Load to Dictionary<string,TsvElement> buffer. Skip empty line and comment lines. Same key is overwrote by last elements.
		TreeText,		///< Load to TsvElements tree structure. Same key is not overwrote. that is added to same keyname array.
		
	};
	
	
	
	// P A R S E   B U F F E R
	//=====================================================
	
	/// The buffer for the PlainText.
	string[][] plainTable;
	
	/// The list for the KeypairText.
	Dictionary <string, string> keypairObj;
	
	/// The root element for the TreeText.
	TsvElement root;

	/// Cache object
	Dictionary <string, TsvElement> cache;
	
	
	
	// S E T T I N G S
	//=====================================================
	 
	/// Debug output flag.
	static bool debug = false;
	
	/// Debug at parsing.
	static bool debugParse = false;
	
	/// Use cache flag.
	bool useCache	= true;
	
	/// Escape tab and newlines flag.
	bool escape		= true;
	
	/// Unescape tab and newlines flag.
	bool unescape	= true;
	
	/// Selector separator.
	static char sepChar = ">".ToCharArray()[0];
	
	/// Selector trim characters.( null is will not trimming. )
	static string strTrimming = " ";
	
	/// Comment line identifier.
	static string strCommentBegin = "#";
		
	/// I/O parse mode.
	Mode mode = Mode.PlainText;
	
	/// Newline num in header.
	static int nNewLineHeader = 1;
	
	/// Newline num in footer.
	static int nNewLineFooter = 1;
	
	/// Newline num in same indent levels.
	static int nNewLineSameLine = 0;
	
	/// Tab escape/unescape identifier.
	static string strEscapeTab = "^*t";

	/// Newline escape/unescape identifier.
	static string strEscapeNewline = "^*n";
	
	/// Multi line is added new line on each lines.
	static bool multilineNewLine = true;
	
	
	/// Logging callback delegate define.
	public delegate void LogDelegate ( string str );
	
	/// Logging callback delegate.
	static public LogDelegate logFunc = null;
	
	
	
	// G E T T E R   /   S E T T E R
	//=====================================================
	
	//=====================================================
	/**
	 *	@brief	Set/Get log delegate.
	 *
	 *	@param	DelegateLog	void FuncName (string str) type function.
	 */
	//=====================================================
	public LogDelegate DelegateLog {
		
		get { return logFunc; }
		set { logFunc = value; }
		
	}
	
	//=====================================================
	/**
	 *	@brief	Total debug out flag settings.
	 *
	 *	@param	isDebugOut	true is output the debug strings.
	 */
	//=====================================================
	public bool debugMode {
		
		get { return debug; }
		set { debug = value; }
		
	}
	
	//=====================================================
	/**
	 *	@brief	Debug out flag settings for parsing.
	 *
	 *	@param	isDebugOut	true is output the debug strings.
	 */
	//=====================================================
	public bool debugParsing {
		
		get { return debugParse; }
		set { debugParse = value;}
		
	}
		
	//=====================================================
	/**
	 *	@brief	Use cache for TreeText settings.
	 *
	 *	@param	useCache	If you use cache in TreeTsv access, then set true.
	 */
	//=====================================================
	public bool cachedTree {
		
		get { return useCache; }
		set { useCache = value; }
		
	}
	
	//=====================================================
	/**
	 *	@brief	Tsvio mode.
	 *
	 *	@param	mode	Tsvio.Mode
	 */
	//=====================================================
	public Mode iomode {
		
		get { return mode; }
		set { mode = value; }
		
	}
	
	//=====================================================
	/**
	 *	@brief	Use escape tab and newlines settings.
	 *
	 *	@param	useEscape	If you want tab and newline escape, then set true.
	 */
	//=====================================================
	public bool useEscape {
		
		get { return escape; }
		set { escape = value; }
		
	}
	
	//=====================================================
	/**
	 *	@brief	Use unescape tab and newlines settings.
	 *
	 *	@param	useUnescape	If you want tab and newline unescape, then set true.
	 */
	//=====================================================
	public bool useUnescape {
		
		get { return unescape; }
		set { unescape = value; }
		
	}
	
	//=====================================================
	/**
	 *	@brief	The tag separator character settings for GetAttributes, GetMultiLine, and GetElement.
	 *
	 *	@param	sepchar	Separate character of the tag.
	 */
	//=====================================================
	static public char tagSeparator {
		
		get { return sepChar; }
		set { sepChar = value; }
		
	}
	
	//=====================================================
	/**
	 *	@brief	The selector trimming string settings.
	 *
	 *	@param	Trim string from the separated selector.
	 */
	//=====================================================
	static public string selectorTrimString {
		
		get { return strTrimming; }
		set { strTrimming = value; }
		
	}
	
	//=====================================================
	/**
	 *	@brief	The comment line identifier settings.
	 *
	 *	@param	Identifier string for comment line detection.
	 */
	//=====================================================
	static public string commentLineBeginFrom {
		
		get { return strCommentBegin; }
		set { strCommentBegin = value; }
		
	}	
	
	//=====================================================
	/**
	 *	@brief	Number of newlines setting for writing elements.(Different indent levels.)
	 *
	 *	@param	num	NewLine num.
	 */
	//=====================================================
	static public int newlineNum {
		
		get { return nNewLineHeader; }
		set { nNewLineHeader = value; }
		
	}
	
	//=====================================================
	/**
	 *	@brief	Number of newlines setting for writing elements.(Same indent levels.)
	 *
	 *	@param	num	NewLine num.
	 */
	//=====================================================
	static public int newlineNumSameLevel {
		
		get { return nNewLineSameLine; }
		set { nNewLineSameLine = value; }
		
	}	
	
	//=====================================================
	/**
	 *	@brief	Multi line each add new line character. (Enable on TreeText)
	 *
	 *	@param	bool	add new line to each multi lines.
	 */
	//=====================================================
	static public bool multilineEachAddNewlines {
		
		get { return multilineNewLine; }
		set { multilineNewLine = value; }
		
	}
	

	// I N I T I A L I Z E
	//=====================================================
		
	//=====================================================
	/**
	 *	@brief	Constructor 
	 */
	//=====================================================
	public Tsvio ( Mode iomode ) {
		
		Init (iomode, true);
		
	}
		
	//=====================================================
	/**
	 *	@brief	Constructor 
	 */
	//=====================================================
	public Tsvio ( Mode iomode, bool useDebugOut ) {
		
		Init ( iomode, useDebugOut );
		
	}

	//=====================================================
	/**
	 *	@brief	Common initialize rutin.
	 */
	//=====================================================
	void Init ( Mode iomode, bool useDebugOut ) {
		
		mode		= iomode;
		debug		= useDebugOut;
		
		/*
		if ( sha1 == null  ||  sha1managed == null )
		{
			
			sha1		= new SHA1CryptoServiceProvider();
			sha1managed = new SHA1Managed();
	
		}
		*/
		
		if ( mode == Mode.TreeText )
		{
		
			root	= new TsvElement ();
			cache	= new Dictionary<string, TsvElement> ();
		
		}
		else if ( mode == Mode.KeyPairText )
		{
			
			keypairObj = new Dictionary<string, string> ();
			
		}
		else
		{
			
			// Plain は Parse 時に行数がわかるのでそこで確保
			
		}
		
	}
	
	

	// P A R S E D   D A T A   A C C E S S 
	//=====================================================
		
		
	//=====================================================
	/**
	 *	@brief	Trimming spacers in the selector.
	 *
	 *	@param	selector	selector string.
	 *
	 *	@return	trimmed selector.
	 */
	//=====================================================	
	static string TrimSelector ( string selector ) {
		
		string[]						names			= selector.Split ( new char[]{sepChar} );
		int								nNames			= names.Length;
		char							trimChar		= strTrimming.ToCharArray ()[0];

		if ( strTrimming != null && strTrimming.Length>0 )
		{
		
			for ( int i=0; i<nNames; ++i )
			{
				
				names[i] = names[i].Trim (trimChar);
				
			}
			
			selector = string.Join ( sepChar.ToString (), names );
		
		}
		
		return selector;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Get attributes string array from the selector tag string.
	 *
	 *	@param	selector	Selector string. ex:"Root>A>B>C".
	 *			If you want select in same name array, then write to "Root>A[2]>B>C". added "[index]".
	 *			Separator can change ">" to other character by Change Separator().
	 *
	 *	@return	null	... Invalid selector or Fired exceptions.
	 *			string[]... Got attributes.
	 */
	//=====================================================	
	public string[] GetAttributes ( string selector ) {
		
		TsvElement	cur	= root;
		TsvElement	ret	= null;
		
		if ( debugParse )
		{
			Print ("GetAttributes\n\tSelector:" + selector + "\n");
		}
		
		//selector = TrimSelector ( selector );
		//selector = GetHash ( selector );
		
		// キャッシュがあればキャッシュ利用、なければ都度解析
		if ( !cache.ContainsKey (selector) )
		{
			
			ret = ParseSelector ( selector, cur );
			
		}
		else
		{
			
			ret = cache[selector];
			
		}
		
		if ( useCache )
		{
			
			cache[selector] = ret;
			
		}
		
		return ret!=null ? ret.attr : null;
			
	}
			
	//=====================================================
	/**
	 *	@brief	Get child elements count.
	 *
	 *	@param	selector	Selector string. ex:"Root>A>B>C".
	 *			If you want select in same name array, then write to "Root>A[2]>B>C". added "[index]".
	 *			Separator can change ">" to other character by Change Separator().
	 *
	 *	@return	0	... Invalid selector or Fired exceptions. or not found element childs.
	 *			int ... Got childs count.
	 */
	//=====================================================	
	public int GetChildsCount ( string selector ) {
		
		TsvElement elem = GetElement ( selector );
	
		return elem!=null && elem.childs!=null ? elem.childs.Count : -1;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Get/Set the root element list dictionary.
	 *
	 *	@return	TsvElement object	... The root element.
	 */
	//=====================================================	
	public TsvElement rootObject {
		
		get { return root; }
		set { root = value; }
		
	}
	
	//=====================================================
	/**
	 *	@brief	Get/Set the keypair dictionary.
	 *
	 *	@return	Dictionary <string, string> object	... The keypair object.
	 */
	//=====================================================	
	public Dictionary <string, string> keypair {
		
		get { return keypairObj; }
		set { keypairObj = value; }
		
	}
	
	//=====================================================
	/**
	 *	@brief	Get/Set the plain text arrays.
	 *
	 *	@return	string[][] object	... The plain text object.
	 */
	//=====================================================	
	public string[][] plain {
		
		get { return plainTable; }
		set { plainTable = value; }
		
	}
	
	//=====================================================
	/**
	 *	@brief	Get child elements count.
	 *
	 *	@param	selector	Selector string. ex:"Root>A>B>C".
	 *			If you want select in same name array, then write to "Root>A[2]>B>C". added "[index]".
	 *			Separator can change ">" to other character by Change Separator().
	 *
	 *	@return	null		... Invalid selector or Fired exceptions. or not found element.
	 *			TsvElement	... Got element.
	 */
	//=====================================================	
	public TsvElement GetElement ( string selector ) {
		
		TsvElement	cur	= root;
		TsvElement	ret	= null;
		
		selector = TrimSelector ( selector );
		
		// キャッシュがあればキャッシュ利用・なければ都度解析
		if ( !cache.ContainsKey (selector) )
		{
			
			ret = ParseSelector ( selector, cur );
			
		}
		else
		{
			
			ret = cache[selector];
			
		}
		
		if ( useCache )
		{
			
			cache[selector] = ret;
			
		}
	
		return ret;
		
	}	
	
	// S A V E
	//=====================================================
	
	public bool SaveTo ( string path ) {
		
		// TODO: 暗号化等、ユーザー事前処理用のコールバックを挟む必要あり
		
		string data = null;
		
		if ( mode == Mode.TreeText )
		{
			
			data = TreeToString ( root, 0 );
			
		}
		else if ( mode == Mode.KeyPairText )
		{
			
			data = KeyPairToString ( keypairObj );
			
		}
		else if ( mode == Mode.PlainText )
		{
			
			data = PlainToString ( plainTable );
			
		}
		
		if ( data != null )
		{
			
			System.IO.StreamWriter fileWriter = System.IO.File.CreateText(path);
			fileWriter.Write (data);
			fileWriter.Close();
			
			return true;
			
		}
		
		return false;
		
	}

	//=====================================================
	/**
	 *	@brief	Elements to string by current text mode.
	 *
	 *	@return	Tsv string.
	 */
	//=====================================================	
	public override string ToString () {
		
		if ( mode == Mode.TreeText )
		{
			
			return TreeToString ( root, 0 );
			
		}
		else if ( mode == Mode.KeyPairText )
		{
			
			return KeyPairToString ( keypairObj );
		
		}
		else if ( mode == Mode.PlainText )
		{
			
			return PlainToString ( plainTable );
			
		}
		
		return null;
		
	}
	

	//=====================================================
	/**
	 *	@brief	KeyPair to string.
	 *
	 *	@param	keyPaiValue	KeyPairValue
	 *
	 *	@return	Tsv string.
	 */
	//=====================================================	
	StringBuilder KeyPairToStringBuffer ( System.Text.StringBuilder buf, string key, string value ) {
		
		char	newlineChar	= '\n';
		string	newline		= newlineChar.ToString ();
		char	tabChar		= '\t';
		string	tab			= tabChar.ToString ();
		
		buf.Append ( key + tab + value + newline );
		
		return buf;
		
	}

	//=====================================================
	/**
	 *	@brief	Escape tab from source string.
	 *
	 *	@param	src	Source string.
	 *
	 *	@return	Escape flag is enable, then return escaped string. the flag is disable , then return source string.
	 */
	//=====================================================	
	string EscapeTab ( string src ) {
		
		return escape ? src.Replace( "\t", strEscapeTab ) : src;
		
	}

	//=====================================================
	/**
	 *	@brief	Unescape tab from source string.
	 *
	 *	@param	src	Source string.
	 *
	 *	@return	Unscape flag is enable, then return unescaped string. the flag is disable , then return source string.
	 */
	//=====================================================	
	string UnescapeTab ( string src ) {
		
		return unescape ? src.Replace( strEscapeTab, "\t" ) : src;
		
	}
	

	//=====================================================
	/**
	 *	@brief	Escape newline from source string.
	 *
	 *	@param	src	Source string.
	 *
	 *	@return	Escape flag is enable, then return escaped string. the flag is disable , then return source string.
	 */
	//=====================================================	
	string EscapeNewline ( string src ) {
		
		return escape ? src.Replace( "\n", strEscapeNewline ) : src;
		
	}

	//=====================================================
	/**
	 *	@brief	Unescape newline from source string.
	 *
	 *	@param	src	Source string.
	 *
	 *	@return	Unscape flag is enable, then return unescaped string. the flag is disable , then return source string.
	 */
	//=====================================================	
	string UnescapeNewline ( string src ) {
		
		return unescape ? src.Replace( strEscapeNewline, "\n" ) : src;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Plain text to string.
	 *
	 *	@param	text	string[][]
	 *
	 *	@return	Tsv string.
	 */
	//=====================================================	
	public string PlainToString ( string[][] text ) {
		
		StringBuilder	ret		= new StringBuilder ();
		int				nLine	= text.Length;
		int				nCol	= 0;
		
		// EscapeTab 内で二重チェックになってしまうのをどうにかしたい
		if ( escape )
		{
			for ( int i=0; i<nLine; ++i )
			{
				
				nCol = text[i].Length;
				
				// タブエスケープ
				for ( int c=0; c<nCol; ++c )
				{
					
					text[i][c] = EscapeTab( text[i][c] );
					
				}
				
				ret.Append ( string.Join ( "\t", text[i] ) + "\n" );
				
			}
		}
		else
		{
			
			for ( int i=0; i<nLine; ++i )
			{
				
				ret.Append ( string.Join ( "\t", text[i] ) + "\n" );
				
			}
			
		}
		
		return ret.ToString ();
		
	}
	
	//=====================================================
	/**
	 *	@brief	KeyPair to string.
	 *
	 *	@param	keyPaiValue	KeyPairValue
	 *
	 *	@return	Tsv string.
	 */
	//=====================================================	
	public string KeyPairToString ( Dictionary <string, string> keypairValue ) {
		
		StringBuilder								ret		= new StringBuilder ();
		Dictionary <string, string>.KeyCollection	keys	= keypairValue.Keys;
		
		foreach (string key in keys)
		{
			
			ret = KeyPairToStringBuffer ( ret, key, keypairValue[key] );
			
		}
		
		return ret.ToString ();
		
	}

	//=====================================================
	/**
	 *	@brief	KeyPair to string.
	 *
	 *	@param	keyPaiValue	KeyPairValue
	 *
	 *	@return	Tsv string.
	 */
	//=====================================================	
	public string KeyPairToString ( Dictionary <string, int> keypairValue ) {
		
		StringBuilder							ret		= new StringBuilder ();
		Dictionary <string, int>.KeyCollection	keys	= keypairValue.Keys;
		
		foreach (string key in keys)
		{
			
			ret = KeyPairToStringBuffer ( ret, key, "" + keypairValue[key] );
			
		}
		
		return ret.ToString ();
		
	}
	
	//=====================================================
	/**
	 *	@brief	KeyPair to string.
	 *
	 *	@param	keyPaiValue	KeyPairValue
	 *
	 *	@return	Tsv string.
	 */
	//=====================================================	
	public string KeyPairToString ( Dictionary <string, double> keypairValue ) {
		
		StringBuilder								ret		= new StringBuilder ();
		Dictionary <string, double>.KeyCollection	keys	= keypairValue.Keys;
		
		foreach (string key in keys)
		{
			
			ret = KeyPairToStringBuffer ( ret, key, "" + keypairValue[key] );
			
		}
		
		return ret.ToString ();
		
	}

	//=====================================================
	/**
	 *	@brief	KeyPair to string.
	 *
	 *	@param	keyPaiValue	KeyPairValue
	 *
	 *	@return	Tsv string.
	 */
	//=====================================================	
	public string KeyPairToString ( Dictionary <string, string[]> keypairValue ) {
		
		StringBuilder								ret		= new StringBuilder ();
		Dictionary <string, string[]>.KeyCollection	keys	= keypairValue.Keys;
		
		foreach (string key in keys)
		{
			
			ret = KeyPairToStringBuffer ( ret, key, string.Join ("\t", keypairValue[key]) );
			
		}
		
		return ret.ToString ();
		
	}

	//=====================================================
	/**
	 *	@brief	KeyPair to string.
	 *
	 *	@param	keyPaiValue	KeyPairValue
	 *
	 *	@return	Tsv string.
	 */
	//=====================================================	
	public string KeyPairToString ( Dictionary <string, int[]> keypairValue ) {
		
		StringBuilder								ret		= new StringBuilder ();
		Dictionary <string, int[]>.KeyCollection	keys	= keypairValue.Keys;
		
		foreach (string key in keys)
		{
			
			ret = KeyPairToStringBuffer ( ret, key, string.Join ("\t", System.Array.ConvertAll < int, string >( keypairValue[key], System.Convert.ToString )) );
			
		}
		
		return ret.ToString ();
		
	}
	
	//=====================================================
	/**
	 *	@brief	KeyPair to string.
	 *
	 *	@param	keyPaiValue	KeyPairValue
	 *
	 *	@return	Tsv string.
	 */
	//=====================================================	
	public string KeyPairToString ( Dictionary <string, double[]> keypairValue ) {
		
		StringBuilder								ret		= new StringBuilder ();
		Dictionary <string, double[]>.KeyCollection	keys	= keypairValue.Keys;
		
		foreach (string key in keys)
		{
			
			ret = KeyPairToStringBuffer ( ret, key, string.Join ("\t", System.Array.ConvertAll < double, string >( keypairValue[key], System.Convert.ToString )) );
			
		}
		
		return ret.ToString ();
		
	}
	
	//=====================================================
	/**
	 *	@brief	Elements tree to string.
	 *
	 *	@param	rootElement	Root element.
	 *	@param	indentOffset	Indent offset for root.
	 *
	 *	@return	Tsv string.
	 */
	//=====================================================	
	public string TreeToString ( TsvElement rootElement, int indentOffset ) {
		
		StringBuilder			ret			= new StringBuilder ();
		char					newlineChar	= '\n';
		string					newline		= newlineChar.ToString ();
		char					tabChar		= '\t';
		string					tab			= tabChar.ToString ();
		string[]				attr_str	= null;
		string[]				mlines		= null;
		int						alen		= 0;
		Elements.KeyCollection	keys		= rootElement.childs.Keys;
		bool					hasAttr		= false;
		TsvElement				elem		= null;
		
		foreach (string key in keys)
		{
			
			int nKeyList = rootElement.childs[key].Count;
			
			for ( int el=0; el<nKeyList; ++el )
			{
			
				hasAttr		= false;
				attr_str	= null;
				
				elem = rootElement.childs[ key ][ el ];
				
				// アトリビュートの設定
				if ( elem.attr != null )
				{
				
					alen = elem.attr.Length;
					
					if ( alen > 0 && elem.attr[0] != null )
					{
						
						attr_str = new string[alen];
					
						for ( int a=0; a<alen; ++a )
						{
							
							attr_str[a] = EscapeTab (elem.attr[a]);
							attr_str[a] = EscapeNewline (attr_str[a]);
							
						}
						
						// キーはタブエスケープしない仕様
						ret.Append ( (indentOffset>0 ? tab.PadLeft(indentOffset, tabChar) : "") + key + tab + string.Join ( tab, attr_str ) + newline.PadLeft (nNewLineSameLine, newlineChar) );
						
						hasAttr = true;
						
					}
					
				}
				
				// アトリビュートがない場合
				if ( !hasAttr )
				{
					
					// キーはタブエスケープしない仕様
					ret.Append ( (indentOffset>0 ? tab.PadLeft(indentOffset, tabChar) : "") + key + newline.PadLeft (nNewLineSameLine, newlineChar) );
					
				}
				
				// マルチラインストリング
				if ( elem.multistr != null && elem.multistr.Length>0 ) {
					
					ret.Append ( newline.PadLeft (nNewLineHeader, newlineChar) );
					
					// Trim \r
					elem.multistr	= elem.multistr.Replace ( "\r", "" );
					
					mlines			= elem.multistr.Split ("\n".ToCharArray ());
					
					for ( int ml=0; ml<mlines.Length; ++ml )
					{
						
						ret.Append ( tab.PadLeft(indentOffset+2, tabChar) + mlines[ml] + (multilineNewLine ? newline: "") );
						
					}
					
					
				}
				
				// 子がある場合は再帰する
				if ( elem.childs != null && elem.childs.Count>0 )
				{
					
					ret.Append ( newline.PadLeft (nNewLineHeader, newlineChar) );
					ret.Append ( TreeToString ( elem, indentOffset+1 ) );
					ret.Append ( newline.PadLeft (nNewLineFooter, newlineChar) );
					
				}
							
			}
			
		}
			
		return ret.ToString ();
		
	}
	
	
	
	// P A R S E
	//=====================================================
	
	//=====================================================
	/**
	 *	@brief	Parse and update cache from selector.
	 *
	 *	@param	selector	Selector string.
	 *	@param	rootElem	Root element dictionary.
	 *
	 *	@return	null		... Invalid result or Fired exceptions or not found a key in the selector.
	 *			otherwise	... Got cache.
	 */
	//=====================================================	
	static public TsvElement ParseSelector ( string selector, TsvElement rootElem ) {
		
		return ParseSelector ( selector, rootElem, false );
		
	}
	
	//=====================================================
	/**
	 *	@brief	Parse and update cache from selector.
	 *
	 *	@param	selector	Selector string.
	 *	@param	rootElem	Root element dictionary.
	 *	@param	createLostLevels	If not found a key in the selector, then create an element of the key.
	 *
	 *	@return	null		... Invalid result or Fired exceptions.
	 *			otherwise	... Got cache.
	 */
	//=====================================================	
	static public TsvElement ParseSelector ( string selector, TsvElement rootElem, bool createLostLevels ) {
		
		int			nList			= 0;
		string[]	aryIndex		= null;
		string[]	names			= selector.Split ( new char[]{sepChar} );
		string[]	withoutIndexName= null;
		int			nNames			= names.Length;
		char		indexerChar		= "[".ToCharArray()[0];
		char		indexerCloseChar= "]".ToCharArray()[0];
		TsvElement	curElem			= new TsvElement();
		TsvElement	ret				= null;
		
		selector = TrimSelector ( selector );
		
		for ( int i=0; i<nNames; ++i )
		{
			int index = 0;
			
			names[i] = names[i].Trim ();
			
			withoutIndexName = names[i].Split (indexerChar);
			
			if ( ( createLostLevels && !rootElem.childs.ContainsKey ( withoutIndexName[0] ) ) || rootElem.childs.ContainsKey ( withoutIndexName[0] ) )
			{
				// 不足分を補完する場合
				// ( 後で追加した箇所ゆえ以下に重複の可能性あり、修正候補とする )
				if ( createLostLevels && !rootElem.childs.ContainsKey ( withoutIndexName[0] ) )
				{
					
					rootElem.childs[ withoutIndexName[0] ] = CreateTree ();
					
				}
		
				nList = rootElem.childs[ withoutIndexName[0] ].Count;
				
				// 同名要素がない場合はインデクス指定なし==[0]とする
				if ( nList==1 )
				{
					
					aryIndex = names[i].Split (indexerChar);
					
					// インデクサ文字が複数あると指定エラー
					if ( aryIndex.Length>2 )
					{
						
						Print ("ParseSelector Error :" + " Too many index selector." + "\n\t" + "Selector:" + selector );
						
						return null;
						
					}
					// インデクサ指定なしは[0]を使用
					else
					{
						
						// 不足分を補完する場合
						if ( createLostLevels && !rootElem.childs.ContainsKey ( withoutIndexName[0] ) )
						{
							
							rootElem.childs[ withoutIndexName[0] ] = CreateTree ();
							
						}
						
						// インデクサ負数は配列末から逆方向扱いだが、要素がひとつしかないので全部[0]扱い
						curElem		= rootElem.childs[ withoutIndexName[0] ][0];
						rootElem	= curElem;
						
					}
					
				}
				// 同名要素が2つ以上ある場合
				else
				{
					
					aryIndex = names[i].Split (indexerChar);
					
					// インデクサ文字が複数あると指定エラー
					if ( aryIndex.Length>2 )
					{
						
						Print ("ParseSelector Error :" + " Too many index selector." + "\n\t" + "Selector:" + selector );
						
						return null;
						
					}
					// インデクサ指定なしは[0]を使用
					else if ( aryIndex.Length<=1 )
					{
					
						// 不足分を補完する場合
						if ( createLostLevels && !rootElem.childs.ContainsKey ( withoutIndexName[0] ) )
						{
							
							rootElem.childs[ withoutIndexName[0] ] = CreateTree ();
							
						}
						
						// インデクサ負数は配列末から逆方向扱いだが、要素がひとつしかないので全部[0]扱い
						curElem		= rootElem.childs[ withoutIndexName[0] ][0];
						rootElem	= curElem;
						
					}
					// インデクサあり
					else
					{
						
						string[] aryStrIndex = aryIndex[1].Split (indexerCloseChar);
						
						try
						{
							
							index = int.Parse (aryStrIndex[0]);
							
							// インデクサ負数は配列末から逆方向扱い
							if ( index<0 )
							{
								
								try
								{
									
									// 不足分を補完する場合
									if ( createLostLevels && !rootElem.childs.ContainsKey ( withoutIndexName[0] ) )
									{
										
										rootElem.childs[ withoutIndexName[0] ] = CreateTree ();
										
										// 配列不足の時は現状エラー
										
									}
									
									int n		= rootElem.childs[ withoutIndexName[0] ].Count;
									curElem		= rootElem.childs[ withoutIndexName[0] ][ n+index ];
									rootElem	= curElem;
								
								}
								catch ( System.Exception e )
								{
									
									Print ("ParseSelector Error :" + "Index underflow." + "\n\t" + "Selector:" + selector + "\n\t" + e.Message );
									
									return null;
									
								}
								
							}
							else
							{
								
								try
								{
								
									// 不足分を補完する場合
									if ( createLostLevels && !rootElem.childs.ContainsKey ( withoutIndexName[0] ) )
									{
										
										rootElem.childs[ withoutIndexName[0] ] = CreateTree ();
										
										// 配列不足の時は現状エラー
										
									}
									
									curElem		= rootElem.childs[ withoutIndexName[0] ][ index ];
									rootElem	= curElem;
									
								}
								catch ( System.Exception e )
								{
									
									Print ("ParseSelector Error :" + "Index overflow." + "\n\t" + "Selector:" + selector + "\n\t" + e.Message );
						
									return null;
									
								}
								
							}
							
						}
						catch ( System.Exception e )
						{
							
							Print ("ParseSelector Error :" + "Non number characters in index brackets." + "\n\t" + "Selector:" + selector + "\n\t" + e.Message );
							
							return null;
							
						}
						
					}
					
				}
				
			}
			else
			{
				
				return null;
				
			}
						
		}
		
		ret	= curElem;
		
		return ret;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Parse Tsv
	 */
	//=====================================================
	public bool Parse ( string strTsv ) {
		
		if ( mode==Mode.TreeText )
		{
			
			return ParseTreeText ( strTsv );
			
		}
		else if ( mode==Mode.KeyPairText )
		{
			
			return ParseKeyPairText ( strTsv );
			
		}
		else if ( mode==Mode.PlainText )
		{
			
			return ParsePlainTsvText ( strTsv );
			
		}
		
		return false;
		
	}

	//=====================================================
	/**
	 *	@brief	Parse plain Tsv
	 */
	//=====================================================
	bool ParsePlainTsvText ( string strTsv ) {
			
		try
		{
			
			int				npl		= 0;
			char			tab		= "\t".ToCharArray ()[0];
			string[]		lines	= null;
			
			strTsv	= strTsv.Replace ( "\r", "" );
			
			lines	= strTsv.Split ("\n".ToCharArray ());
			
			if ( lines != null )
			{
				int n = lines.Length;
				
				plainTable = new string[n][];
				
				// 各行の処理
				for ( int i=0; i<n; ++i )
				{
					
					plainTable[i] = lines[i].Split (tab);
					
					npl = plainTable[i].Length;
					
					for ( int pl=0; pl<npl; ++pl )
					{
						
						plainTable[i][pl] = UnescapeTab (plainTable[i][pl]);
						plainTable[i][pl] = UnescapeNewline (plainTable[i][pl]);
						
					}
					
				}
				
			}
			
		}
		catch ( System.Exception e )
		{
			
			Print ( e.ToString () + e.StackTrace );
			return false;
			
		}
		
		return true;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Parse KeyPair text Tsv
	 */
	//=====================================================
	bool ParseKeyPairText ( string strTsv ) {
			
		try
		{
		
			char			tab		= "\t".ToCharArray ()[0];
			string[]		lines	= null;
			string[]		cols	= null;
			
			strTsv	= strTsv.Replace ( "\r", "" );
			
			lines	= strTsv.Split ("\n".ToCharArray ());
			
			if ( lines != null )
			{
				int n = lines.Length;
				
				// 各行の処理
				for ( int i=0; i<n; ++i )
				{
					
					cols = lines[i].Split (tab);
					
					// 空行
					if ( cols.Length == 0 || lines[i].Trim().StartsWith ("#") )
					{
						
					}
					// なにかしら値が入力されている
					else
					{
						
						string key = cols[0];
						
						// アトリビュートの設定
						if ( cols.Length > 1 )
						{
							
							for ( int o=1; o<cols.Length; ++o )
							{
								
								if ( o>1 )
								{
									
									keypairObj[key] += tab;
									
								}
								
								if ( keypairObj.ContainsKey (key) )
								{
									keypairObj[key] += cols[o];
								}
								else
								{
									keypairObj[key] = cols[o];
								}
								
							}
							
							if ( debugParse )
							{
						
								Print ("\tVALUE:" + keypairObj[key]);
								
							}
							
						}
						
					}
					
				}
				
			}
			
		}
		catch ( System.Exception e )
		{
			
			Print ( e.ToString () + e.StackTrace );
			return false;
			
		}
		
		return true;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Parse Tree text Tsv
	 */
	//=====================================================
	bool ParseTreeText ( string strTsv ) {
			
		try
		{
		
			// 現在の参照
			TsvElement cur = root;
			
			// 階層が変わった直後、同じ階層を指定している場合の参照変更を行う
			TsvElement nextCur = null;
		
			// タブ区切り文字
			char			tab				= "\t".ToCharArray ()[0];
			
			// 行バッファ
			string[]		lines			= null;
			
			// カラムバッファ
			string[]		cols			= null;
			
			// 処理中の階層名
			List <string>	nestName		= new List <string> ();
			
			// 現在の階層数
			int				currentLv		= 0;
			
			// 最後に処理した名前
			string			lastCurrentName	= "";
			
			strTsv	= strTsv.Replace ( "\r", "" );
			
			lines	= strTsv.Split ("\n".ToCharArray ());
			
			if ( lines != null )
			{
				int n = lines.Length;
				
				// 各行の処理
				for ( int i=0; i<n; ++i )
				{
					
					cols = lines[i].Split (tab);
					
					// 空行
					if ( cols.Length == 0 )
					{
						
					}
					// なにかしら値が入力されている
					else
					{
						int offset = -1;
						
						// 何番目から有効値があるか
						for ( int o=0; o<cols.Length; ++o )
						{
							int l = cols[o].Length;
							
							if ( l > 0 )
							{
								
								// コメント行か
								if ( cols[o].StartsWith (strCommentBegin) )
								{
									break;
								}
								
								offset = o;
								break;
							}
							
						}
						
						// 有効なカラムがない場合は処理しない
						if ( offset > -1 )
						{
							string key = cols[offset];
							
							// 同じ階層
							if ( offset == currentLv )
							{
								
								if ( nextCur != null )
								{
									
									cur		= nextCur;
									nextCur	= null;
									
								}
								
								lastCurrentName	= AddElementList ( cur, key );
								
								int index = cur.childs[ key ].Count-1;
							
								// 名前スタックを再生成
								if ( offset <= nestName.Count )
								{
									
									nestName.RemoveRange ( offset, nestName.Count-offset );
									
								}
								
								// アトリビュートの設定
								if ( cols.Length - offset - 1 > 0 )
								{
									
									cur.childs[ key ][ index ].attr = new string[cols.Length - offset - 1];
									for ( int aa=0; aa<cols.Length - offset - 1; ++aa )
									{
										
										cur.childs[ key ][ index ].attr[aa] = UnescapeTab ( cols[ offset+1+aa ] );
										cur.childs[ key ][ index ].attr[aa] = UnescapeNewline ( cur.childs[ key ][ index ].attr[aa] );
										
										if ( debugParse )
										{
							
											if ( aa==0 )
											{
											
												Print ("KEY:" + string.Join(" "+sepChar.ToString ()+" ", nestName.ToArray ()) + " " + sepChar + " " + key + "[" + index + "]");
											
											}
											Print ("\tATTR:" + cur.childs[ key ][ index ].attr[aa]);
											
										}
										
									}
									
								}
								
								nestName.Add ( key );
								
								// キャッシュの作成
								UpdateCache ( cur, key, index, nestName );
							
								nextCur = null;
								
							}
							// 次の階層
							else if ( offset == currentLv+1 && lastCurrentName.Length>0 )
							{
								
								int index = 0;
								int nList = 0;
								
								nList = cur.childs [ lastCurrentName ].Count;
								
								if ( nList>0 )
								{
									
									index = nList - 1;
										
								}
								else
								{
									
									index = 0;
										
								}
								/*
								if ( nextCur != null )
								{
									
									cur		= nextCur;
									nextCur	= null;
									lastCurrentName = key;
									
								}
								*/
								TsvElement elem = cur.childs [ lastCurrentName ][index];
								lastCurrentName = AddElementList ( elem, key );
								cur = elem;
								nextCur = elem;
								
								// アトリビュートの設定
								if ( cols.Length - offset - 1 > 0 )
								{
									
									index = elem.childs[ key ].Count-1;
									
									elem.childs[ key ][ index ].attr = new string[cols.Length - offset - 1];
									for ( int aa=0; aa<cols.Length - offset - 1; ++aa )
									{
										
										elem.childs[ key ][ index ].attr[aa] = UnescapeTab ( cols[ offset+1+aa ] );
										elem.childs[ key ][ index ].attr[aa] = UnescapeNewline ( elem.childs[ key ][ index ].attr[aa] );
										
										if ( debugParse )
										{
							
											if ( aa==0 )
											{
												
												Print ("KEY:" + string.Join(" "+sepChar.ToString ()+" ", nestName.ToArray ()) + " " + sepChar + " " + key + "[" + index + "]");
											
											}
											Print ("\tATTR:" + elem.childs[ key ][ index ].attr[aa]);
											
										}
										
									}
									
								}
								
								// 名前スタックを再生成
								if ( offset < nestName.Count )
								{
									
									nestName.RemoveRange ( offset, nestName.Count-offset );
									
								}
							
								nestName.Add ( key );
								
								// キャッシュの作成
								UpdateCache ( elem, key, index, nestName );
							
								currentLv++;
								
							}
							// MultiLine
							else if ( offset == currentLv+2 && lastCurrentName.Length>0)
							{
								
								string[]	mstr	= null;
								int			msl		= cols.Length; 
								
								mstr	= new string[msl - offset];
								msl		= mstr.Length;
								
								for ( int ms=0; ms<msl; ++ms )
								{
									
									mstr[ms] = cols[offset+ms];
									
								}
								
								// マルチラインはタブをエスケープしないでそのままバッファに乗せる
								cur.childs[ lastCurrentName ][ cur.childs[ lastCurrentName ].Count-1 ].multistr += string.Join( tab.ToString (), mstr ) + "\n";
																
								// キャッシュの作成
								UpdateCache ( cur, lastCurrentName, cur.childs[ lastCurrentName ].Count-1, nestName );
								
								nextCur = null;
								
							}
							// 現状より浅いレベル
							else if ( offset < currentLv )
							{
								
								string		name	= "";
								int			index	= 0;
								int			nNest	= nestName.Count;
								TsvElement	ary		= root;
								TsvElement	list;
								
								// どこの配列の下に所属するか
								for ( int a=0; a<nNest && a<offset; ++a )
								{
									name	= nestName[a];
									list	= ary.childs[name][ ary.childs[name].Count-1 ];
									ary		= list;
								}
								
								cur = ary;
								
								lastCurrentName	= AddElementList ( cur, key );
								
								// 名前スタックを再生成
								if ( offset < nestName.Count )
								{
									
									nestName.RemoveRange ( offset, nestName.Count-offset );
									
								}
									
								// アトリビュートの設定
								if ( cols.Length - offset - 1 > 0 )
								{
									
									index = cur.childs[ key ].Count-1;
									
									cur.childs[ key ][ index ].attr = new string[cols.Length - offset - 1];
									for ( int aa=0; aa<cols.Length - offset - 1; ++aa )
									{
										
										cur.childs[ key ][ index ].attr[aa] = UnescapeTab ( cols[ offset+1+aa ] );
										cur.childs[ key ][ index ].attr[aa] = UnescapeNewline ( cur.childs[ key ][ index ].attr[aa] );
										
										if ( debugParse )
										{
							
											if ( aa==0 )
											{
												Print ("KEY:" + string.Join(" "+sepChar.ToString ()+" ", nestName.ToArray ()) + " " + sepChar + " " + key + "[" + index + "]");
											}
											
											Print ("\tATTR:" + cur.childs[ key ][ index ].attr[aa]);
											
										}
										
									}
									
								}
									
								nestName.Add ( key );
								
								currentLv = offset;
								
								// キャッシュの作成
								UpdateCache ( cur, key, index, nestName );
								
								nextCur = null;
								
							}
						
						}
						
					}
					
				}
				
			}
			
		}
		catch ( System.Exception e )
		{
			
			Print ( e.ToString () + e.StackTrace );
			return false;
			
		}
		
		return true;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Update tag caches.
	 */
	//=====================================================
	void UpdateCache ( TsvElement rootElem, string key, int index, List <string> nestName ) {
		
		if ( useCache )
		{
			string		newkey	= string.Join ( sepChar.ToString (), nestName.ToArray ());
			
			cache[ newkey + "[" + index + "]" ] = rootElem.childs[ key ][ index ];
		
			// [0]の時は[0]とインデクサなしキーを作る
			if ( index==0 )
			{
				
				cache[ newkey ]	= rootElem.childs[ key ][ index ];
				
			}
			
		}
		
	}
	
	string GetHash ( string strSrc ) {
		
		/*
		// UTF-8でバイト化
    	byte[] bytes	= Encoding.UTF8.GetBytes (strSrc);
		
		byte[] results	= sha1.ComputeHash ( bytes );
		
		shaToStrBuilder = new StringBuilder();
	    
		for (int i = 0; i < results.Length; i++)
	    {
			
			shaToStrBuilder.AppendFormat("{0:X2}", results[i]);
			
	    }
		
		return shaToStrBuilder.ToString ();
		*/
		return strSrc;
	}
	
	//=====================================================
	/**
	 *	@brief	Add element list or new list.
	 *
	 *	@param	parent	Element of destination level.
	 *	@param	key		Element key.
	 */
	//=====================================================
	static public string AddElementList ( TsvElement parent, string key ) {
		
		// まだ未定義
		if ( parent.childs == null )
		{
			
			//parent.childs =
			
		}
		if ( !parent.childs.ContainsKey (key) )
		{
		
			parent.childs[ key ] = CreateTree ();
			
		}
		// すでにあるなら追加
		else
		{
			
			parent.childs[ key ].Add ( CreateElement () );
			
		}
				
		return key;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Return the new elements array.
	 */
	//=====================================================
	internal static ElementList CreateTree () {
		
		ElementList ret = new ElementList ();
		
		ret.Add ( CreateElement() );
		
		return ret;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Return the new elements array.
	 */
	//=====================================================
	internal static ElementList CreateTree ( TsvElement element ) {
		
		ElementList ret = new ElementList ();
		
		ret.Add ( element );
		
		return ret;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Return the new element.
	 */
	//=====================================================
	static public TsvElement CreateElement () {
		
		return new TsvElement ();
		
	}
	
	//=====================================================
	/**
	 *	@brief	Return the new element with setup attributes.
	 */
	//=====================================================
	static public TsvElement CreateElement ( string[] attributes ) {
		
		return new TsvElement (attributes);
		
	}
	
	//=====================================================
	/**
	 *	@brief	Return the new element with setup the multi-line string.
	 */
	//=====================================================
	static public TsvElement CreateElement ( string multilineStr ) {
		
		return new TsvElement (multilineStr);
		
	}
	
	//=====================================================
	/**
	 *	@brief	Return the new element with setup attribute and the multi-line string.
	 */
	//=====================================================
	static public TsvElement CreateElement ( string[] attributes, string multilineStr ) {
		
		return new TsvElement (attributes, multilineStr);
		
	}
	
	
	// D E B U G
	//========================================================
		
	//=====================================================
	/**
	 *	@brief	Debug output
	 */
	//=====================================================
	static void Print ( string str ) {
		
		if ( debug )
		{
			
			Debug.Log ("# Tsvio # " + str );
			
			if ( logFunc != null )
			{
				
				logFunc (str);
				
			}
			
		}
		
	}
		
}



// E L E M E N T S   D E F I N E
//=====================================================

//[System.Serializable]
public class ElementList : List <TsvElement> {}

//[System.Serializable]
public class Elements : Dictionary <string, ElementList> {}

/// Elements
//[System.Serializable]
public class TsvElement {
	
	static bool debug = false;
		
	public string[]		attr;		///< Attributes buffer
	public string		multistr;	///< Multi-line string buffer
	public Elements		childs;		///< The childs of this element
	
	// Selector access by relative element position.
	public TsvElement this[string key] {
	
		get
		{
			
			return Tsvio.ParseSelector ( key, this );
			
		}
		
		set
		{
			
			TsvElement elem = Tsvio.ParseSelector ( key, this );
			
			if ( elem != null )
			{
				
				elem = value;
				
			}
			// 要素がない場合はそこまでの要素を作成する
			else
			{
				
				this.CreateLevels ( key, value );
				
			}
			
		}
	
	}
	
	//=====================================================
	/**
	 *	@brief	Create the new element.
	 */
	//=====================================================
	public TsvElement () {
		
		childs		= new Elements ();
		
		attr		= new string[1];
	
		multistr	= "";
		
	}
	
	//=====================================================
	/**
	 *	@brief	Create the new element with setup attributes.
	 */
	//=====================================================
	public TsvElement ( string[] attributes ) {
		
		childs		= new Elements ();
		
		attr		= new string[ attributes.Length ];
		
		attributes.CopyTo ( attr, 0 );
	
		multistr	= "";
		
	}
	
	//=====================================================
	/**
	 *	@brief	Create the new element with setup the multi-line string.
	 */
	//=====================================================
	public TsvElement ( string multilineStr ) {
		
		childs		= new Elements ();
		
		attr		= new string[1];
	
		multistr	= multilineStr;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Create the new element with setup the attribute and multi-line string.
	 */
	//=====================================================
	public TsvElement ( string[] attributes, string multilineStr ) {
		
		childs		= new Elements ();
		
		attr		= new string[ attributes.Length ];
	
		attributes.CopyTo ( attr, 0 );
		
		multistr	= multilineStr;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Create all elements tree in the selector levels.
	 *
	 *	@param	key				The key of the element to be added.
	 *	@param	terminalElem	The terminal element.
	 */
	//=====================================================
	public bool CreateLevels ( string selector, TsvElement terminalElem ) {
		
		TsvElement elem = Tsvio.ParseSelector ( selector, this, true );
	
		if ( elem == null )
		{
			
			return false;
			
		}
		
		elem.attr		= terminalElem.attr;
		elem.multistr	= terminalElem.multistr;
		elem.childs		= terminalElem.childs;
		
		return true;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Element add to parent target.
	 *
	 *	@param	key			The key of the element to be added.
	 *	@param	parentElem	Parent target.
	 */
	//=====================================================
	public bool AddTo ( string key, TsvElement parentElem ) {
		
		try
		{
			
			if ( !parentElem.childs.ContainsKey (key) )
			{
				parentElem.childs[key] = Tsvio.CreateTree ( this );
			}
			else
			{
				parentElem.childs[key].Add ( this );
			}
			
		}
		catch ( System.Exception e )
		{
			
			if ( debug )
			{
				
				Debug.Log ("# TsvElement # Failed AddTo:\n\t" + e.Message );
				
			}
			
			return false;
			
		}
		
		return true;
		
	}
	
	//=====================================================
	/**
	 *	@brief	Add to child target.
	 *
	 *	@param	key			The key of the child element.
	 *	@param	parentElem	The child element.
	 */
	//=====================================================
	public bool AddChild ( string key, TsvElement childElem ) {
		
		try
		{
			
			if ( !this.childs.ContainsKey (key) )
			{
				this.childs[key] = Tsvio.CreateTree ( childElem );
			}
			else
			{
				this.childs[key].Add ( childElem );
			}
			
		}
		catch ( System.Exception e )
		{
			
			if ( debug )
			{
				
				Debug.Log ("# TsvElement # Failed AddChild:\n\t" + e.Message );
				
			}
			
			return false;
			
		}
		
		return true;
		
	}		
	
	
};
