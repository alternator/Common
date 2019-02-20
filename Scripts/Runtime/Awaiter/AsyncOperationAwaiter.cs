using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ICKX {

	public static class AsyncOperationAwaitable {

		public static AsyncOperationAwaiter GetAwaiter (this AsyncOperation request) {
			return new AsyncOperationAwaiter (request);
		}
	}

	public class AsyncOperationAwaiter : INotifyCompletion {

		public AsyncOperation operation { get; private set; }
		private System.Action continuation;

		public AsyncOperationAwaiter (AsyncOperation request) {
			this.operation = request;
		}

		public void OnCompleted (System.Action continuation) {
			this.continuation = continuation;
			CoroutineManager.Start (WrappedCoroutine ());
		}

		IEnumerator WrappedCoroutine () {
			yield return operation;
			continuation ();
		}

		public bool IsCompleted {
			get {
				return operation.isDone;
			}
		}

		public void GetResult () { }
	}
}
