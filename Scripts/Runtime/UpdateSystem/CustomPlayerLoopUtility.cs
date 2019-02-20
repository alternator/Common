using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;

namespace ICKX {

	public static class CustomPlayerLoopUtility {

		static bool isInit = false;
		static PlayerLoopSystem playerLoopSystem;

		public static void InsertLoop (Type playerLoopType, int insertIndex, PlayerLoopSystem system) {
			Insert (playerLoopType, (subSystemList) => {
				subSystemList.Insert (0, system);
				return true;
			});
		}

		public static void InsertLoopFirst (Type playerLoopType, PlayerLoopSystem system) {
			InsertLoop (playerLoopType, 0, system);
		}

		public static void InsertLoopLast (Type playerLoopType, PlayerLoopSystem system) {
			Insert (playerLoopType, (subSystemList) => {
				subSystemList.Add (system);
				return true;
			});
		}

		public static void InsertPrevious (Type playerLoopType, Type subLoopType, PlayerLoopSystem system) {
			Insert (playerLoopType, (subSystemList) => {
				for (int j = 0; j < subSystemList.Count; j++) {
					if (subSystemList[j].type == subLoopType) {
						subSystemList.Insert (j, system);
						return true;
					}
				}
				return false;
			});
		}

		public static void InsertNext (Type playerLoopType, Type subLoopType, PlayerLoopSystem system) {
			Insert (playerLoopType, (subSystemList) => {
				for (int j = 0; j < subSystemList.Count; j++) {
					if (subSystemList[j].type == subLoopType) {
						subSystemList.Insert (j + 1, system);
						return true;
					}
				}
				return false;
			});
		}

		static void Insert (Type playerLoopType, Func<List<PlayerLoopSystem>, bool> function) {
			if (!isInit) {
				isInit = true;
				playerLoopSystem = PlayerLoop.GetDefaultPlayerLoop ();
			}

			for (int i = 0; i < playerLoopSystem.subSystemList.Length; i++) {
				var mainSystem = playerLoopSystem.subSystemList[i];
				if (mainSystem.type == playerLoopType) {
					var subSystemList = new List<PlayerLoopSystem> (mainSystem.subSystemList);
					if (function (subSystemList)) {
						mainSystem.subSystemList = subSystemList.ToArray ();
						playerLoopSystem.subSystemList[i] = mainSystem;
						PlayerLoop.SetPlayerLoop (playerLoopSystem);
						return;
					}
				}
			}
		}
	}
}