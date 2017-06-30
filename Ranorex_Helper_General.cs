using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

//using Microsoft.Office.Interop.Word;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationSupplements
{
	/// <summary>
	/// A collection of usefull general helper methods.
	/// </summary>
	[UserCodeCollection]
	public class Ranorex_Helper_General
	{

		// Popup Watcher
		public static PopupWatcher watcher = new PopupWatcher();
		public static Dictionary<string, System.DateTime>  timers = new Dictionary<string, System.DateTime>();

		
		public Ranorex_Helper_General()
		{
			Mouse.DefaultMoveTime = 300;
			Keyboard.DefaultKeyPressTime = 100;
			Delay.SpeedFactor = 1.0;
		}
		
		/// <summary>
		/// Compares two values.
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		[UserCodeMethod]
		public static void CompareValues (string value1, string value2)
		{
			Validate.AreEqual(value1,value2);
		}

		/// <summary>
		/// Concatenates two strings and returns the new string.
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <returns>Concatenated string</returns>
		[UserCodeMethod]
		public static string ConcatStrings (string value1, string value2)
		{
			return String.Concat(value1, value2);
		}

		
		/// <summary>
		/// Kills a process
		/// </summary>
		/// <param name="processname">Name of the process to kill</param>
		public static void KillProcess(string processname)
		{
			Process[] processes = Process.GetProcessesByName(processname);
			
			foreach (Process p in processes)
			{
				try
				{
					p.Kill();
					Ranorex.Report.Info("Process killed: " + p.ProcessName);
				}
				catch (Exception ex)
				{
					Ranorex.Report.Error(ex.Message);
				}
			}
		}	

				
		/// <summary>
		/// Waits for a popup window to appear and clicks an element to close the window.
		/// </summary>
		/// <param name="findElement">Element to be waited for.</param>
		/// <param name="clickElement">Elment which should be clicked after the popup appeard.</param>
		[UserCodeMethod]
		public static void AddPopupWatcher(Ranorex.Core.Repository.RepoItemInfo findElement, Ranorex.Core.Repository.RepoItemInfo clickElement)
		{
			watcher.WatchAndClick(findElement, clickElement);
			watcher.Start();
			Ranorex.Report.Info("Popup watcher started.");
		}
		
		/// <summary>
		/// Stops the popup watcher.
		/// </summary>
		[UserCodeMethod]
		public static void StopPopupWatcher()
		{
			watcher.Stop();
			Ranorex.Report.Info("Popup watcher stopped.");
		}
		
		/// <summary>
		/// Starts a new timer.
		/// </summary>
		/// <param name="timerName">Name of the timer</param>
		[UserCodeMethod]
		public static void StartTimer(string timerName)
		{
			if (timers.ContainsKey(timerName)) {
				timers.Remove(timerName);
			}
			
			timers.Add(timerName, System.DateTime.Now);
			Ranorex.Report.Log(ReportLevel.Info, "Timer", "Started: '" + timerName + "'");
			
		}
		
		/// <summary>
		/// Stops a timer.
		/// </summary>
		/// <param name="timerName">Name of the timer to stop.</param>
		[UserCodeMethod]
		public static TimeSpan StopTimer(string timerName)
		{
			if (timers.ContainsKey(timerName))
			{
				System.DateTime end= System.DateTime.Now;
				System.DateTime start= timers[timerName];
				System.TimeSpan duration= end-start;
				
				timers.Remove(timerName);
								
				Ranorex.Report.Log(ReportLevel.Info, "Timer", "Stopped: '" + timerName + "' (duration: " + (duration.TotalMilliseconds /1000).ToString() + " seconds)");
				
				return duration;
			}
			throw new Exception("Timer '"+timerName+"' does not exist.");			
		}
		
	}
		
}
	
	#region ************************************ Helper classes **************************************
	public class Measurement {
		
		public System.DateTime time;
		public TimeSpan span;
		
		public Measurement(System.DateTime time, TimeSpan span) {
			this.span=span;
			this.time=time;
		}
	}
	#endregion

