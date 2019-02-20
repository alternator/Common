using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ICKX {

	public static class WaitUtil {

		public static async Task WaitIsTrue (System.Func<bool> func
		, SynchronizationContext context = null, int interval = 5) {
			while (!func ()) {
				if (context != null) SynchronizationContext.SetSynchronizationContext (context);
				await Task.Delay (interval);
			}
		}

		public static async Task WaitIsNull (object obj, SynchronizationContext context = null, int interval = 5) {
			while (obj == null) {
				if (context != null) SynchronizationContext.SetSynchronizationContext (context);
				await Task.Delay (interval);
			}
		}
	}
}
