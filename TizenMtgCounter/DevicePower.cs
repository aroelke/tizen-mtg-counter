using System.Runtime.InteropServices;

namespace TizenMtgCounter
{
	abstract class DevicePower
	{
		[DllImport("libcapi-system-device.so.0", EntryPoint = "device_power_request_lock", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int DevicePowerRequestLock(int type, int timeout_ms);

		[DllImport("libcapi-system-device.so.0", EntryPoint = "device_power_release_lock", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int DevicePowerReleaseLock(int type);

		public const int CPU = 0;
		public const int DISPLAY = 1;
		public const int DISPLAY_DIM = 2;

		public static int RequestLock(int type, int timeout)
		{
			return DevicePowerRequestLock(type, timeout);
		}

		public static int ReleaseLock(int type)
		{
			return DevicePowerReleaseLock(type);
		}
	}
}
