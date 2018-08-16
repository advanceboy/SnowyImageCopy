﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowyImageCopy.Helper
{
	/// <summary>
	/// Manages date and time based on FAT file system.
	/// </summary>
	public static class FatDateTime
	{
		/// <summary>
		/// Converts int representing date and int representing time to <see cref="DateTime"/>.
		/// </summary>
		/// <param name="sourceDate">Source int representing date</param>
		/// <param name="sourceTime">Source int representing time</param>
		/// <returns>Outcome DateTime</returns>
		/// <remarks>
		/// Date format:
		/// Bits 0–4: Day
		/// Bits 5–8: Month
		/// Bits 9–15: Count of years from 1980
		/// Time format:
		/// Bits 0–4: Count of 2 seconds
		/// Bits 5–10: Minute
		/// Bits 11–15: Hour
		/// </remarks>
		public static DateTime ConvertFromDateIntAndTimeIntToDateTime(int sourceDate, int sourceTime)
		{
			if ((sourceDate <= 0) || (sourceTime < 0))
				return default(DateTime);

			var baDate = new BitArray(new[] { sourceDate }).Cast<bool>().ToArray();
			var baTime = new BitArray(new[] { sourceTime }).Cast<bool>().ToArray();

			var year = ConvertFromBitsToInt(baDate.Skip(9)) + 1980;
			var month = ConvertFromBitsToInt(baDate.Skip(5).Take(4));
			var day = ConvertFromBitsToInt(baDate.Take(5));
			var hour = ConvertFromBitsToInt(baTime.Skip(11));
			var minute = ConvertFromBitsToInt(baTime.Skip(5).Take(6));
			var second = ConvertFromBitsToInt(baTime.Take(5)) * 2;

			try
			{
				return new DateTime(year, month, day, hour, minute, second);
			}
			catch (ArgumentOutOfRangeException)
			{
				// If time stamp of source folder or file is weird, date or time parameters may become invalid
				// and so this exception will be thrown.
				return default(DateTime);
			}
		}

		/// <summary>
		/// Converts <see cref="DateTime"/> to int representing date.
		/// </summary>
		/// <param name="source">Source DateTime</param>
		/// <returns>Outcome int representing date</returns>
		public static int ConvertFromDateTimeToDateInt(DateTime source)
		{
			var baDate = new BitArray(16);

			var baYear = new BitArray(new[] { source.Year - 1980 });
			var baMonth = new BitArray(new[] { source.Month });
			var baDay = new BitArray(new[] { source.Day });

			CopyBits(baYear, ref baDate, 9, 15);
			CopyBits(baMonth, ref baDate, 5, 8);
			CopyBits(baDay, ref baDate, 0, 4);

			return ConvertFromBitsToInt(baDate);
		}

		/// <summary>
		/// Converts <see cref="DateTime"/> to int representing time.
		/// </summary>
		/// <param name="source">Source DateTime</param>
		/// <returns>Outcome int representing time</returns>
		public static int ConvertFromDateTimeToTimeInt(DateTime source)
		{
			var baTime = new BitArray(16);

			var baHour = new BitArray(new[] { source.Hour });
			var baMinute = new BitArray(new[] { source.Minute });
			var baSecond = new BitArray(new[] { source.Second / 2 });

			CopyBits(baHour, ref baTime, 11, 15);
			CopyBits(baMinute, ref baTime, 5, 10);
			CopyBits(baSecond, ref baTime, 0, 4);

			return ConvertFromBitsToInt(baTime);
		}

		#region Helper

		/// <summary>
		/// Converts bits to int.
		/// </summary>
		/// <param name="source">Source BitArray</param>
		/// <returns>Outcome int</returns>
		private static int ConvertFromBitsToInt(BitArray source)
		{
			return ConvertFromBitsToInt(source.Cast<bool>());
		}

		/// <summary>
		/// Converts bits to int.
		/// </summary>
		/// <param name="source">Source bits</param>
		/// <returns>Outcome int</returns>
		private static int ConvertFromBitsToInt(IEnumerable<bool> source)
		{
			var target = new int[1];
			new BitArray(source.ToArray()).CopyTo(target, 0);
			return target[0];
		}

		/// <summary>
		/// Copies bits from BitArray to BitArray.
		/// </summary>
		/// <param name="source">Source BitArray</param>
		/// <param name="target">Target BitArray</param>
		/// <param name="startTargetIndex">Start index of target BitArray</param>
		/// <param name="endTargetIndex">End index of target BitArray</param>
		private static void CopyBits(BitArray source, ref BitArray target, int startTargetIndex, int endTargetIndex)
		{
			for (int i = 0, j = startTargetIndex;
				(i < source.Length) && (j < target.Length) && (j <= endTargetIndex);
				i++, j++)
			{
				target[j] = source[i];
			}
		}

		#endregion
	}
}