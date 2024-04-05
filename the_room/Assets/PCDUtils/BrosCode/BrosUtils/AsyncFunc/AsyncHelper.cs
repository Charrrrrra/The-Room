using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class AsyncHelper
{
    static public async Task WaitForChecker(Func<bool> checker) {
		while (!checker.Invoke())
			await Task.Yield();
	}
}
