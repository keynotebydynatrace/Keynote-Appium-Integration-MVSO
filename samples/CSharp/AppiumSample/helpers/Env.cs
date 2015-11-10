using System;
using System.Collections.Generic;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.MultiTouch;
using OpenQA.Selenium.Appium.Interfaces;
using System.IO;
using System.Reflection;
//using System.Web.Script.Serialization;

namespace AppiumSample.helpers
{
	public class Env
	{
		public static TimeSpan INIT_TIMEOUT_SEC = TimeSpan.FromSeconds(180);
		public static TimeSpan IMPLICIT_TIMEOUT_SEC = TimeSpan.FromSeconds(50);
	}
}

