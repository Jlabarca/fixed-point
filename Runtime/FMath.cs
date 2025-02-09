﻿using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Mathematics.Fixed
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static partial class FMath
	{
		/// <summary>
		/// Returns a number indicating the sign of a Fix64 number.
		/// Returns 1 if the value is positive or 0, and -1 if it is negative.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(FP value)
		{
			return (int)(1 | (value.RawValue & FP.SignMask));
		}

		/// <summary>
		/// Returns a number indicating the sign of a Fix64 number.
		/// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SignWithZero(FP value)
		{
			return value.RawValue < 0 ? -1 :
				value.RawValue > 0 ? 1 :
				0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP CopySign(FP to, FP from)
		{
			return FP.FromRaw(CopySign(to.RawValue, from.RawValue));
		}

		/// <summary>
		/// Returns the absolute value of a Fix64 number.
		/// FastAbs(Fix64.MinValue) is undefined.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Abs(FP value)
		{
			var mask = value.RawValue >> FP.AllBitsWithoutSign;
			return FP.FromRaw((value.RawValue + mask) ^ mask);
		}

		/// <summary>
		/// Returns the absolute value of a Fix64 number.
		/// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP SafeAbs(FP value)
		{
			if (value.RawValue == FP.MinValueRaw)
			{
				return FP.MaxValue;
			}

			var mask = value.RawValue >> FP.AllBitsWithoutSign;
			return FP.FromRaw((value.RawValue + mask) ^ mask);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Max(FP x, FP y)
		{
			var rawResult = x.RawValue;
			if (y.RawValue > x.RawValue)
			{
				rawResult = y.RawValue;
			}
			return FP.FromRaw(rawResult);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Min(FP x, FP y)
		{
			var rawResult = x.RawValue;
			if (y.RawValue < x.RawValue)
			{
				rawResult = y.RawValue;
			}
			return FP.FromRaw(rawResult);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Lerp(FP start, FP end, FP t)
		{
			if (t.RawValue < 0)
			{
				t.RawValue = 0L;
			}
			if (t.RawValue > FP.OneRaw)
			{
				t.RawValue = FP.OneRaw;
			}
			return start + t * (end - start);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP LerpUnclamped(FP start, FP end, FP t)
		{
			return start + t * (end - start);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP MoveTowards(FP current, FP target, FP maxDelta)
		{
			return Abs(target - current) <= maxDelta ? target : current + Sign(target - current) * maxDelta;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Clamp01(FP value)
		{
			if (value.RawValue < 0)
			{
				return FP.Zero;
			}
			if (value.RawValue > FP.OneRaw)
			{
				return FP.One;
			}
			return value;
		}

		/// <summary>
		/// Compares two values with some epsilon and returns true if they are similar.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ApproximatelyEqual(FP x, FP y)
		{
			return ApproximatelyEqual(x, y, FP.CalculationsEpsilonSqr);
		}

		/// <summary>
		/// Compares two values with some epsilon and returns true if they are similar.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ApproximatelyEqual(FP x, FP y, FP epsilon)
		{
			var difference = Abs(x - y);
			return difference <= epsilon;
		}

		/// <summary>
		/// Returns the largest integer less than or equal to the specified number.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Floor(FP value)
		{
			return FP.FromRaw(value.RawValue & FP.IntegerSignMask);
		}

		/// <summary>
		/// Returns the largest integer less than or equal to the specified number.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FloorToInt(FP value)
		{
			return value.ToInt();
		}

		/// <summary>
		/// Returns the smallest integral value that is greater than or equal to the specified number.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Ceil(FP value)
		{
			var hasFractionalPart = (value.RawValue & FP.FractionalMask) != 0;
			return hasFractionalPart ? Floor(value) + FP.One : value;
		}

		/// <summary>
		/// Returns the smallest integral value that is greater than or equal to the specified number.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CeilToInt(FP value)
		{
			var hasFractionalPart = (value.RawValue & FP.FractionalMask) != 0;
			return hasFractionalPart ? value.ToInt() + 1 : value.ToInt();
		}

		/// <summary>
		/// Rounds a value to the nearest integral value.
		/// If the value is halfway between an even and an uneven value, returns the even value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Round(FP value)
		{
			var fractionalPart = value.RawValue & FP.FractionalMask;
			var integralPart = Floor(value);
			if (fractionalPart < FP.HalfRaw)
			{
				return integralPart;
			}

			if (fractionalPart > FP.HalfRaw)
			{
				return integralPart + FP.One;
			}

			// If number is halfway between two values, round to the nearest even number.
			// This is the method used by System.Math.Round().
			return (integralPart.RawValue & FP.OneRaw) == 0
				? integralPart
				: integralPart + FP.One;
		}

		/// <summary>
		/// Rounds a value to the nearest integral value.
		/// If the value is halfway between an even and an uneven value, returns the even value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int RoundToInt(FP value)
		{
			return Round(value).ToInt();
		}

		/// <summary>
		/// Calculates the square root of a fixed-point number.
		/// Has absolute precision when <see cref="FP.FractionalBits"/> &lt;= 31. Otherwise
		/// may have some rare minor inaccuracies, that are tied to absolute precision.<br/>
		/// If any, inaccuracies are in range [0, Epsilon * 1000).
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the input is negative.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Sqrt(FP x)
		{
			if (x.RawValue < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(x), "Negative value passed to Sqrt.");
			}

			var value = (ulong)x.RawValue;
			var result = 0UL;

			const int correctionForOdd = FP.FractionalBits & 1;

			// Find highest power of 4 <= num.
			var bit = 1UL << (FP.AllBits - 2 + correctionForOdd);
			while (bit > value)
			{
				bit >>= 2;
			}

			while (bit != 0)
			{
				var t = result + bit;
				result >>= 1;
				if (value >= t)
				{
					value -= t;
					result += bit;
				}
				bit >>= 2;
			}

			// & (FP.AllBits - 1) is a correction when FractionalBits == 0.
			bit = 1UL << ((FP.FractionalBits - 2 + correctionForOdd) & (FP.AllBits - 1));

#pragma warning disable CS0162 // Unreachable code detected
			if (FP.FractionalBits < FP.AllBits / 2) // Faster case for FP.FractionalBits <= 31.
			{
				value <<= FP.FractionalBits;
				result <<= FP.FractionalBits;
			}
			else
			{
				LeftShift128(out var valueHigh, ref value, FP.FractionalBits);
				LeftShift128(out var resultHigh, ref result, FP.FractionalBits);

				var t = result + bit;

				// Exit early if we can continue with a standart 64-bit version.
				while (bit != 0 && (valueHigh != 0 || resultHigh != 0 || t < result))
				{
					AddToNew128(out var tHigh, out t, ref resultHigh, ref result, bit);
					RightShift128(ref resultHigh, ref result, 1);
					if (valueHigh > tHigh || (valueHigh == tHigh && value >= t))
					{
						Sub128(ref valueHigh, ref value, ref tHigh, ref t);
						Add128(ref resultHigh, ref result, bit);
					}
					bit >>= 2;
				}
			}
#pragma warning restore CS0162 // Unreachable code detected

			while (bit != 0)
			{
				var t = result + bit;
				result >>= 1;
				if (value >= t)
				{
					value -= t;
					result += bit;
				}
				bit >>= 2;
			}

			// Rounding up.
			if (value > result)
			{
				result++;
			}

			return FP.FromRaw((long)result);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void LeftShift128(out ulong high, ref ulong low, int shift)
			{
				high = low >> (FP.AllBits - shift);
				low <<= shift;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void RightShift128(ref ulong high, ref ulong low, int shift)
			{
				low = (high << (FP.AllBits - shift)) | (low >> shift);
				high >>= shift;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Add128(ref ulong highA, ref ulong lowA, ulong b)
			{
				var sum = lowA + b;
				if (sum < lowA)
				{
					++highA;
				}
				lowA = sum;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void AddToNew128(out ulong highC, out ulong lowC, ref ulong highA, ref ulong lowA, ulong b)
			{
				lowC = lowA + b;
				highC = highA;
				if (lowC < lowA)
				{
					++highC;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Sub128(ref ulong highA, ref ulong lowA, ref ulong highB, ref ulong lowB)
			{
				if (lowA < lowB)
				{
					--highA;
				}
				lowA -= lowB;
				highA -= highB;
			}
		}

		/// <summary>
		/// Returns 2 raised to the specified power.
		/// Provides at least 6 decimals of accuracy.
		/// </summary>
		public static FP Pow2(FP x)
		{
			if (x.RawValue == 0)
			{
				return FP.One;
			}

			// Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
			var neg = x.RawValue < 0;
			if (neg)
			{
				x = -x;
			}

			if (x == FP.One)
			{
				return neg ? FP.One / 2 : FP.Two;
			}

			if (x >= FP.Log2Max)
			{
				return neg ? FP.One / FP.MaxValue : FP.MaxValue;
			}

			if (x <= FP.Log2Min)
			{
				return neg ? FP.MaxValue : FP.Zero;
			}

			/* The algorithm is based on the power series for exp(x):
			* http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
			*
			* From term n, we get term n+1 by multiplying with x/n.
			* When the sum term drops to zero, we can stop summing.
			*/

			var integerPart = Floor(x).ToInt();
			// Take fractional part of exponent
			x = FP.FromRaw(x.RawValue & FP.FractionalMask);

			var result = FP.One;
			var term = FP.One;
			var i = 1;
			while (term.RawValue != 0)
			{
				term = x * term * FP.Ln2 / i;
				result += term;
				i++;
			}

			result = FP.FromRaw(result.RawValue << integerPart);
			if (neg)
			{
				result = FP.One / result;
			}

			return result;
		}

		/// <summary>
		/// Returns the base-2 logarithm of a specified number.
		/// Provides at least 9 decimals of accuracy.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The argument was non-positive
		/// </exception>
		internal static FP Log2(FP x)
		{
			if (x.RawValue <= 0)
			{
				throw new ArgumentOutOfRangeException("Non-positive value passed to Ln", "x");
			}

			// This implementation is based on Clay. S. Turner's fast binary logarithm
			// algorithm (C. S. Turner,  "A Fast Binary Logarithm Algorithm", IEEE Signal
			//     Processing Mag., pp. 124,140, Sep. 2010.)

			long b = 1U << (FP.FractionalBits - 1);
			long y = 0;

			var rawX = x.RawValue;
			while (rawX < FP.OneRaw)
			{
				rawX <<= 1;
				y -= FP.OneRaw;
			}

			while (rawX >= (FP.OneRaw << 1))
			{
				rawX >>= 1;
				y += FP.OneRaw;
			}

			var z = FP.FromRaw(rawX);

			for (var i = 0; i < FP.FractionalBits; i++)
			{
				z = z * z;
				if (z.RawValue >= (FP.OneRaw << 1))
				{
					z = FP.FromRaw(z.RawValue >> 1);
					y += b;
				}

				b >>= 1;
			}

			return FP.FromRaw(y);
		}

		/// <summary>
		/// Returns the natural logarithm of a specified number.
		/// Provides at least 7 decimals of accuracy.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The argument was non-positive
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Ln(FP x)
		{
			return Log2(x) * FP.Ln2;
		}

		/// <summary>
		/// Returns a specified number raised to the specified power.
		/// Provides about 5 digits of accuracy for the result.
		/// </summary>
		/// <exception cref="DivideByZeroException">
		/// The base was zero, with a negative exponent
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The base was negative, with a non-zero exponent
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Pow(FP b, FP exp)
		{
			if (b == FP.One)
			{
				return FP.One;
			}

			if (exp.RawValue == 0)
			{
				return FP.One;
			}

			if (b.RawValue == 0)
			{
				if (exp.RawValue < 0)
				{
					throw new DivideByZeroException();
				}

				return FP.Zero;
			}

			var log2 = Log2(b);
			return Pow2(exp * log2);
		}
	}
}
