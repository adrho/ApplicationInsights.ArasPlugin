using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ApplicationInsights.ArasPlugin
{
	public static class Telemetry
	{

		private static TelemetryClient telemetryClient;
		public static TelemetryClient TelemetryClient
		{
			get
			{
				if (telemetryClient == null)
					SetTelemetryClient();

				return telemetryClient;
			}
		}
		private static void SetTelemetryClient()
		{
			telemetryClient = new TelemetryClient()
			{
				InstrumentationKey = InnovatorConfig.GetOperatingParamValue("AppInsightKey")
			};
		}
		private static bool? isEnable = null;
		public static bool IsEnable
		{
			get
			{
				if (!isEnable.HasValue)
					SetIsTelemetryEnable();

				return isEnable.Value;
			}
		}
		private static void SetIsTelemetryEnable()
		{
			var enable = InnovatorConfig.GetOperatingParamValue("IsTelemetryEnable");
			if (bool.TryParse(enable, out bool result))
				isEnable = result;
			else
				isEnable = false;
		}
	}
}
