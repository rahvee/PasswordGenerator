using NUnit.Framework;
using System;
using System.Collections.Generic;
using PasswordGeneratorNamespace.Constants;
using PasswordGeneratorNamespace.Generators;

namespace PasswordGeneratorNamespace.Test
{
	[TestFixture()]
	public class Test
	{
		/*
		[SetUp()]
		public void Init()
		{
		}
		*/

		[Test()]
		public void IntGeneratorTest()
		{
#if DEBUG
			/* This code block exhaustively tests GetByte(), which was modeled after GetInt()
			 * for the sake of validating the logic process on a much smaller scale, which can be
			 * exhaustively tested in a reasonable amount of time.  (under a minute, see 49sec comment
			 * below).
			 */
			int maxNumMax = 0;
			int minNumMax = byte.MaxValue;
			int maxNumMin = 0;
			int minNumMin = byte.MaxValue;
			var startTime = DateTime.Now;
			for (byte min = 0; min < 126; min++) {
				for (byte max = (byte)(min + 1); max < 127; max++) {
					// I'm going to roll the die a whole bunch of times, I'm going to count the number of times I got minvalue
					// and maxvalue, and assume a fair distribution between.  As long as I get a fair distribution on the endpoints,
					// then I assume it's good. So how many times do I need to roll the die?  How about...  Till we get a total of
					// 128 results that we count, where we expect approx 64 to be the min value, and approx 64 to be the max.
					int numMin = 0;
					int numMax = 0;
					while (numMax + numMin < 128) {
						byte b = IntGenerator.GetByte(min, max);
						Assert.GreaterOrEqual(b, min, "should not have gotten result less than min");
						Assert.LessOrEqual(b, max, "should not have gotten result greater than max");
						if (b == min) numMin++;
						if (b == max) numMax++;
					}
					if (numMin > maxNumMin) maxNumMin = numMin;
					if (numMin < minNumMin) minNumMin = numMin;
					if (numMax > maxNumMax) maxNumMax = numMax;
					if (numMax < minNumMax) minNumMax = numMax;
				}
			}
			var endTime = DateTime.Now;
			var elapsedTime = endTime - startTime;
			/* I don't know statistics well enough to analyze how often we expect failure based on normal
			 * random distribution. But when I run this test, it takes about 49 sec, and it's normal for
			 * maxNumMax and maxNumMin to be around 88 (24 deviation from the 64 center)
			 * minNumMax and minNumMin to be around 40 (24 deviation from the 64 center)
			 * So I'm confident in a few things:
			 *     1.  If I had a blatant logic error in the random number generator, I'd get immediate
			 *         failures to ever produce the min value or max value, which I'd immediately detect
			 *         as 0's or 128's here.
			 *     2.  If I've done everything right in the random number generator, it will take thousands
			 *         of repetitions of this test before we expect to get any result > 112 or < 16, which
			 *         is 48 deviation from the center 64.
			 * So I'm using those thresholds as my criteria for success/failure.
			 */
			Assert.LessOrEqual(maxNumMax, 112, "maxNumMax exceeded threshold");
			Assert.LessOrEqual(maxNumMin, 112, "maxNumMin exceeded threshold");
			Assert.GreaterOrEqual(minNumMax, 16, "minNumMax exceeded threshold");
			Assert.GreaterOrEqual(minNumMin, 16, "minNumMin exceeded threshold");
#endif
			// Now outside the #if DEBUG code block, we test a few corner cases of GetInt()
			int oneCount = 0;
			for (int i = 0; i < 128; i++) {
				int result = IntGenerator.GetInt(0, 1);
				if (result == 1) {
					oneCount++;
				}
				else if (result == 0) {
					// do nothing; we imply zeroCount from oneCount
				}
				else {
					throw new Exception("Got int not in acceptable range");
				}
			}
			Assert.LessOrEqual(oneCount, 112, "oneCount > 112");
			Assert.GreaterOrEqual(oneCount, 16, "oneCount < 16");
		}

		[Test()]
		public void PasswordGeneratorTest()
		{
			string word = PasswordGenerator.GetWord();
			Assert.IsNotNull(word, "word should not be null");
			// There are a bunch of 2-letter words.  We have removed 1-letter words from our WordList.
			Assert.GreaterOrEqual(word.Length, 2, "word length should be 2 or greater");
			// The longest word is 15 chars long:  dissatisfaction
			Assert.LessOrEqual(word.Length, 15, "word length exceeds maximum word length from WordList");

			for (int i = 1; i < 10; i++) {
				string words = PasswordGenerator.GetWords(i);
				Assert.IsNotNull(words, "words should not be null");
				// There are a bunch of 2-letter words.  We have removed 1-letter words from our WordList.
				Assert.GreaterOrEqual(words.Length, 2*i, "words length should be 2 or greater per word");
				// The longest word is 15 chars long:  dissatisfaction
				Assert.LessOrEqual(words.Length, 15*i, "words length exceeds maximum possible words length");
			}
		}

		[Test()]
		public void WordListTest()
		{
			// For the time being, PasswordGenerator.GetWord() is hard-coded to expect 2282 words in the list
			Assert.AreEqual(WordList.WordListArray.Length, 2282, "WordList.WordListArray is expected to be 2282 items long");
			Assert.AreEqual(WordList.WordListString.Length, 15622, "WordList.WordListString is expected to be 15622 chars long");
		}
	}
}
