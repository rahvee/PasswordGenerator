using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PasswordGeneratorNamespace.Constants;
using PasswordGeneratorNamespace.Generators;

namespace PasswordGeneratorNamespace
{
	public static class PasswordGenerator
	{
		/// <summary>
		/// Returns the lowercase word, cryptographic randomly selected from WordList
		/// </summary>
		public static string GetWord()
		{
			// The WordListTest asserts that the WordList has 2282 words in it, and a comment about me hard-coding that expectation here.
			// So if it changes, we'll know about it, and this will not end up broken.
			const int numWords = 2282;

			return WordList.WordListArray[IntGenerator.GetInt(0, numWords - 1)];
		}
		/// <summary>
		/// Returns a string of concatenated and camelized words, cryptographic randomly selected from WordList
		/// </summary>
		public static string GetWords(int wordCount)
		{
			if (wordCount < 1) {
				throw new ArgumentException("wordCount must be >= 1");
			}

			var sb = new StringBuilder();
			for (int i = 0; i < wordCount; i++) {
				string word = GetWord();
				string camelWord = word.First().ToString().ToUpper() + word.Substring(1);
				sb.Append(camelWord);
			}
			return sb.ToString();
		}
	}
}
