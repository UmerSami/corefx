// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    public sealed partial class SafeRegistryHandle : SafeHandle
    {
        protected override bool ReleaseHandle() =>
            Interop.mincore.RegCloseKey(handle) == Interop.mincore.Errors.ERROR_SUCCESS;
    }
}
