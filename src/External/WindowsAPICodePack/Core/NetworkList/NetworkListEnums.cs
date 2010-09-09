//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
namespace Microsoft.WindowsAPICodePack.Net
{
    /// <summary>
    /// Specifies types of network connectivity.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Same name as the native API"), Flags]
    public enum Connectivity
    {
        /// <summary>
        /// The underlying network interfaces have no 
        /// connectivity to any network.
        /// </summary>
        Disconnected = 0,
        /// <summary>
        /// There is connectivity to the Internet 
        /// using the IPv4 protocol.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv")]
        IPv4Internet = 0x40,
        /// <summary>
        /// There is connectivity to a routed network
        /// using the IPv4 protocol.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv")]
        IPv4LocalNetwork = 0x20,
        /// <summary>
        /// There is connectivity to a network, but 
        /// the service cannot detect any IPv4 
        /// network traffic.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv")]
        IPv4NoTraffic = 1,
        /// <summary>
        /// There is connectivity to the local 
        /// subnet using the IPv4 protocol.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv")]
        IPv4Subnet = 0x10,
        /// <summary>
        /// There is connectivity to the Internet 
        /// using the IPv4 protocol.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv")]
        IPv6Internet = 0x400,
        /// <summary>
        /// There is connectivity to a local 
        /// network using the IPv6 protocol.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv")]
        IPv6LocalNetwork = 0x200,
        /// <summary>
        /// There is connectivity to a network, 
        /// but the service cannot detect any 
        /// IPv6 network traffic
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv")]
        IPv6NoTraffic = 2,
        /// <summary>
        /// There is connectivity to the local 
        /// subnet using the IPv6 protocol.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv")]
        IPv6Subnet = 0x100
    }

    /// <summary>
    /// Specifies the domain type of a network.
    /// </summary>
    public enum DomainType
    {
        /// <summary>
        /// The network is not an Active Directory network.
        /// </summary>
        NonDomainNetwork = 0,
        /// <summary>
        /// The network is an Active Directory network, but this machine is not authenticated against it.
        /// </summary>
        DomainNetwork = 1,
        /// <summary>
        /// The network is an Active Directory network, and this machine is authenticated against it.
        /// </summary>
        DomainAuthenticated = 2,
    }

    /// <summary>
    /// Specifies the trust level for a 
    /// network.
    /// </summary>
    public enum NetworkCategory
    {
        /// <summary>
        /// The network is a public (untrusted) network. 
        /// </summary>
        Public,
        /// <summary>
        /// The network is a private (trusted) network. 
        /// </summary>
        Private,
        /// <summary>
        /// The network is authenticated against an Active Directory domain.
        /// </summary>
        Authenticated
    }

    /// <summary>
    /// Specifies the level of connectivity for 
    /// networks returned by the 
    /// <see cref="NetworkListManager"/> 
    /// class.
    /// </summary>
    [Flags]
    public enum NetworkConnectivityLevels
    {
        /// <summary>
        /// Networks that the machine is connected to.
        /// </summary>
        Connected = 1,
        /// <summary>
        /// Networks that the machine is not connected to.
        /// </summary>
        Disconnected = 2,
        /// <summary>
        /// All networks.
        /// </summary>
        All = 3,
    }


}
