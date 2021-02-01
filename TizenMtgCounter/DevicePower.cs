using System.Runtime.InteropServices;

namespace TizenMtgCounter
{
	/// <summary>
	/// Binds methods from the Tizen C API for handling power states.
	/// <seealso href="https://docs.tizen.org/application/native/api/iot-headless/6.0/group__CAPI__SYSTEM__DEVICE__POWER__MODULE.html"/>
	/// </summary>
	abstract class DevicePower
	{
		/// <summary>
		/// Request a power lock.  Valid types of locks are:
		/// <list type="bullet">
		/// <item><see cref="CPU"/></item>
		/// <item><see cref="DISPLAY"/></item>
		/// <item><see cref="DISPLAY_DIM"/></item>
		/// </list>
		/// </summary>
		/// <param name="type">Type of lock to request.</param>
		/// <param name="timeout_ms">Amount of time to wait for the request to be granted. A value of 0 waits indefinitely.</param>
		/// <returns>0 on success, or a negative value on failure.</returns>
		[DllImport("libcapi-system-device.so.0", EntryPoint = "device_power_request_lock", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int DevicePowerRequestLock(int type, int timeout_ms);

		/// <summary>
		/// Release a power lock.  Valid types of locks are:
		/// <list type="bullet">
		/// <item><see cref="CPU"/></item>
		/// <item><see cref="DISPLAY"/></item>
		/// <item><see cref="DISPLAY_DIM"/></item>
		/// </summary>
		/// <param name="type">Type of lock to release.</param>
		/// <returns>0 on success, or a negative value on failure.</returns>
		[DllImport("libcapi-system-device.so.0", EntryPoint = "device_power_release_lock", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int DevicePowerReleaseLock(int type);

		/// <summary>CPU lock.</summary>
		public const int CPU = 0;
		/// <summary>Display normal lock.</summary>
		public const int DISPLAY = 1;
		/// <summary>Display dim lock.</summary>
		public const int DISPLAY_DIM = 2;

		/// <summary>
		/// Request a power lock.  Valid types of locks are:
		/// <list type="bullet">
		/// <item><see cref="CPU"/></item>
		/// <item><see cref="DISPLAY"/></item>
		/// <item><see cref="DISPLAY_DIM"/></item>
		/// </list>
		/// </summary>
		/// <param name="type">Type of lock to request.</param>
		/// <param name="timeout_ms">Amount of time to wait for the request to be granted. A value of 0 waits indefinitely.</param>
		/// <returns>0 on success, or a negative value on failure.</returns>
		public static int RequestLock(int type, int timeout) => DevicePowerRequestLock(type, timeout);

		/// <summary>
		/// Release a power lock.  Valid types of locks are:
		/// <list type="bullet">
		/// <item><see cref="CPU"/></item>
		/// <item><see cref="DISPLAY"/></item>
		/// <item><see cref="DISPLAY_DIM"/></item>
		/// </summary>
		/// <param name="type">Type of lock to release.</param>
		/// <returns>0 on success, or a negative value on failure.</returns>
		public static int ReleaseLock(int type) => DevicePowerReleaseLock(type);
	}
}
