//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;

namespace Microsoft.WindowsAPICodePack.ApplicationServices
{
    internal static class PersonalityGuids
    {
        internal static readonly Guid HighPerformance = new Guid(0x8c5e7fda, 0xe8bf, 0x4a96, 0x9a, 0x85, 0xa6, 0xe2, 0x3a, 0x8c, 0x63, 0x5c);
        internal static readonly Guid PowerSaver = new Guid(0xa1841308, 0x3541, 0x4fab, 0xbc, 0x81, 0xf7, 0x15, 0x56, 0xf2, 0x0b, 0x4a);
        internal static readonly Guid Automatic = new Guid(0x381b4222, 0xf694, 0x41f0, 0x96, 0x85, 0xff, 0x5b, 0xb2, 0x60, 0xdf, 0x2e);

        internal static PowerPersonality GuidToEnum(Guid convertee)
        {
            if (convertee == HighPerformance)
                return PowerPersonality.HighPerformance;
            else if (convertee == PowerSaver)
                return PowerPersonality.PowerSaver;
            else if (convertee == Automatic)
                return PowerPersonality.Automatic;

            throw new ArgumentOutOfRangeException("convertee");
        }
    }
}
