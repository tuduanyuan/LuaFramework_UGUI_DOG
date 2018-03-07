using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility{
    /// <summary>
    /// 时间类，非计时器，就只单单是时间
    /// </summary>
    public static class TimeUtil {
		private static readonly string TimeFormat = "yyyy-MM-dd HH:mm:ss";		//时间格式化字符串
        private static readonly DateTime BaseTime = new DateTime(1970, 1, 1);	//时间起始时间
		public static long TimeOffset = 28800000;								//时间偏移  默认是GMT+8			
		private static long mServerTimestamp = 0;								//服务器开始时间(unix时间戳)
		private static long mStartTime = BaseTime.Ticks;						//开始时间
		public static void Initialize(long timestamp) {
			mServerTimestamp = timestamp;
			mStartTime = DateTime.UtcNow.Ticks;
		}
		/**获得当前unix时间戳*/
		public static long GetDateTime() {
			return mServerTimestamp + Decimal.ToInt64(Decimal.Divide(DateTime.UtcNow.Ticks - mStartTime, 10000));
        }
		/**获得日历类*/
		public static DateTime GetCalendar() {
			return GetCalendar (GetDateTime ());
		}
        /**获得日历类*/
        public static DateTime GetCalendar(long timestamp) {
            return new DateTime((timestamp + TimeOffset) * 10000 + BaseTime.Ticks, DateTimeKind.Utc);
        }
		/**获得当天0点的时间戳*/
		public static long GetZeroTime() {
			DateTime now = GetCalendar();
			DateTime zero = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Utc);
			return zero.Ticks / 10000;
		}
		/**获得当前时间字符串*/
		public static String GetDateString() {
			return GetDateString (GetDateTime (), TimeFormat);
		}
		/**获得当前时间字符串*/
		public static String GetDateString(String format) {
			return GetDateString(GetDateTime(), format);
		}
		/**获得时间字符串*/
		public static String GetDateString(long timestamp) {
			return GetDateString(timestamp, TimeFormat);
		}
		/**获得时间字符串*/
		public static String GetDateString(long timestamp, String format) {
			return GetCalendar (timestamp).ToString (format);
		}
		/** 判断是不是同一年 */
		public static bool IsSameYear(long time1,long time2) {
			var calendar1 = GetCalendar(time1);
			var calendar2 = GetCalendar(time2);
			return (calendar1.Year == calendar2.Year);
		}
		/** 判断是不是同一个月 */
		public static bool IsSameMonth(long time1,long time2) {
			var calendar1 = GetCalendar(time1);
			var calendar2 = GetCalendar(time2);
			return (calendar1.Year == calendar2.Year && calendar1.Month == calendar2.Month);
		}
        /** 判断两个时间戳是不是同一天 */
        public static bool IsSameDay(long time1, long time2) {
            var calendar1 = GetCalendar(time1);
            var calendar2 = GetCalendar(time2);
            return (calendar1.Year == calendar2.Year && calendar1.DayOfYear == calendar2.DayOfYear);
        }
		/** 判断两个时间戳是不是同一小时 */
		public static bool IsSameHour(long time1,long time2) {
			var calendar1 = GetCalendar(time1);
			var calendar2 = GetCalendar(time2);
			return (calendar1.Year == calendar2.Year && 
			        calendar1.DayOfYear == calendar2.DayOfYear &&
			        calendar1.Hour == calendar2.Hour);
		}
		/** 判断两个时间戳是不是同一分钟 */
		public static bool IsSameMinute(long time1,long time2) {
			var calendar1 = GetCalendar(time1);
			var calendar2 = GetCalendar(time2);
			return (calendar1.Year == calendar2.Year && 
			        calendar1.DayOfYear == calendar2.DayOfYear &&
			        calendar1.Hour == calendar2.Hour &&
			        calendar1.Minute == calendar2.Minute);
		}
		/** 判断两个时间戳是不是同一秒 */
		public static bool IsSameSecond(long time1,long time2) {
			var calendar1 = GetCalendar(time1);
			var calendar2 = GetCalendar(time2);
			return (calendar1.Year == calendar2.Year && 
			        calendar1.DayOfYear == calendar2.DayOfYear &&
			        calendar1.Hour == calendar2.Hour &&
			        calendar1.Minute == calendar2.Minute &&
			        calendar1.Second == calendar2.Second);
		}
    }
}
