using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PasswordGeneratorNamespace.Generators
{
	public static class IntGenerator
	{
		/// <summary>
		/// Returns a cryptographic random number >= MinAllowed, and <= MaxAllowed
		/// </summary>
		public static int GetInt(int MinAllowed, int MaxAllowed)
		{
			int difference;
			try {
				difference = checked(MaxAllowed - MinAllowed);
			}
			catch (OverflowException e) {
				throw new Exception("The difference between MinAllowed and MaxAllowed is too large", innerException: e);
			}
			if (MinAllowed >= MaxAllowed) {
				throw new ArgumentException("MinAllowed must be less than MaxAllowed");
			}
			// difference is guaranteed by the above logic to be >= 1 and <= int.MaxValue

			// Guaranteed to be ok by logic above. Also guaranteed to be >= 2. Imagine we're rolling an n-sided die.
			uint numSides = (uint)(difference + 1);

			// If I generate a random 32-bit uint, there are uint.MaxValue / numSides fair rolls in the uint; 
			// the last set would be incomplete and unfair, so if we get a random uint larger than that value, we
			// need to roll the uint again to get the fair roll of numSides.
			// firstUnfair is the first unfair value. A fair value must be less than firstUnfair
			uint? firstUnfair;
			if (uint.MaxValue % numSides == numSides - 1) {
				firstUnfair = null;  // All values are fair
			}
			else {
				firstUnfair = numSides * (uint.MaxValue / numSides);
			}

			using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider()) {
				while (true) {
					var bytes = new byte[sizeof(uint)];
					rng.GetBytes(bytes);
					uint byteRoll = BitConverter.ToUInt32(bytes, 0); // don't care about endianness, because random.
					if (firstUnfair == null || byteRoll < firstUnfair) {
						// byteRoll % numSides will be an int from 0 to numSides - 1
						return (int)(byteRoll % numSides) + MinAllowed;
					}
				}
			}
		}

#if DEBUG
		/// <summary>
		/// It would obviously be dumb to get random bytes this way. This exists solely for the purpose of validating
		/// the logic used in GetInt()
		/// </summary>
		public static byte GetByte(byte MinAllowed, byte MaxAllowed)
		{
			if (MinAllowed >= MaxAllowed) { throw new ArgumentException("MaxAllowed must be > MinAllowed"); }
			byte difference;
			difference = (byte)(MaxAllowed - MinAllowed);

			if (difference > 127) { throw new ArgumentOutOfRangeException(); }

			// Guaranteed to be ok by logic above. Also guaranteed to be >= 2. Imagine we're rolling an n-sided die.
			byte numSides = (byte)(difference + 1);

			// If I generate a random 32-bit uint, there are uint.MaxValue / numSides fair rolls in the uint; 
			// the last set would be incomplete and unfair, so if we get a random uint larger than that value, we
			// need to roll the uint again to get the fair roll of numSides.
			// firstUnfair is the first unfair value. A fair value must be less than firstUnfair
			byte? firstUnfair;
			if (byte.MaxValue % numSides == numSides - 1) {
				firstUnfair = null;  // All values are fair
			}
			else {
				firstUnfair = (byte)(numSides * (byte.MaxValue / numSides));
			}

			using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider()) {
				while (true) {
					var bytes = new byte[1];
					rng.GetBytes(bytes);
					byte byteRoll = bytes[0];
					if (firstUnfair == null || byteRoll < firstUnfair) {
						// byteRoll % numSides will be an int from 0 to numSides - 1
						return (byte)((byteRoll % numSides) + MinAllowed);
					}
				}
			}
		}
#endif
	}
}
