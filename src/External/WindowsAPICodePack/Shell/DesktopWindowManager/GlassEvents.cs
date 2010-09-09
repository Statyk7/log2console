using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    /// Event argument for The GlassAvailabilityChanged event
    /// </summary>
    public class AeroGlassCompositionChangedEvenArgs : EventArgs
    {
        private bool availability;

        internal AeroGlassCompositionChangedEvenArgs( bool avilability )
        {
            this.availability = avilability;
        }

        /// <summary>
        /// The new GlassAvailable state
        /// </summary>
        public bool GlassAvailable
        {
            get
            {
                return availability;
            }
        }
    }

    /// <summary>
    /// Sent when the availability of the desktop Glass effect is changed
    /// </summary>
    /// <param name="sender">The AeroGlassWindow that is affected by this change</param>
    /// <param name="e">The new state of the glass availability</param>
    public delegate void AeroGlassCompositionChangedEvent( object sender, AeroGlassCompositionChangedEvenArgs e );
}
