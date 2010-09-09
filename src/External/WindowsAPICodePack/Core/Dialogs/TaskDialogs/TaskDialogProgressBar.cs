//Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.WindowsAPICodePack.Dialogs
{
    /// <summary>
    /// Provides a visual representation of the progress of a long running operation.
    /// </summary>
    public class TaskDialogProgressBar : TaskDialogBar
    {
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public TaskDialogProgressBar() 
        {
        }
        /// <summary>
        /// Creates a new instance of this class with the specified name.
        /// And using the default values: Min = 0, Max = 100, Current = 0
        /// </summary>
        /// <param name="name">The name of the control.</param>
        public TaskDialogProgressBar(string name) : base(name) { }
        /// <summary>
        /// Creates a new instance of this class with the specified 
        /// minimum, maximum and current values.
        /// </summary>
        /// <param name="minimum">The minimum value for this control.</param>
        /// <param name="maximum">The maximum value for this control.</param>
        /// <param name="value">The current value for this control.</param>
        public TaskDialogProgressBar(int minimum, int maximum, int value)
        {
            Minimum = minimum;
            Maximum = maximum;
            Value = value;
        }

        private int minimum = TaskDialogDefaults.ProgressBarMinimumValue;
        private int value = TaskDialogDefaults.ProgressBarMinimumValue;
        private int maximum = TaskDialogDefaults.ProgressBarMaximumValue;

        /// <summary>
        /// Gets or sets the minimum value for the control.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value",
                        Justification="Value is standard for progressbar's current value property")]
        public int Minimum
        {
            get { return minimum; }
            set 
            {
                CheckPropertyChangeAllowed("Minimum");
                // Check for positive numbers
                if (value < 0)
                    throw new System.ArgumentException("Minimum value provided must be a positive number", "value");
                // Check if min / max differ
                if (value >= Maximum)
                    throw new System.ArgumentException("Minimum value provided must less than the maximum value", "value");
                minimum = value;
                ApplyPropertyChange("Minimum");
            }
        }
        /// <summary>
        /// Gets or sets the maximum value for the control.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value",
            Justification="Value is standard for progressbar's current value property")]
        public int Maximum
        {
            get { return maximum; }
            set 
            {
                CheckPropertyChangeAllowed("Maximum");
                // Check if min / max differ
                if (value < Minimum)
                    throw new System.ArgumentException("Maximum value provided must be greater than the minimum value", "value");
                maximum = value;
                ApplyPropertyChange("Maximum");
            }
        }
        /// <summary>
        /// Gets or sets the current value for the control.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value",
                        Justification="Value is standard for progressbar's current value property")]
        public int Value
        {
            get { return this.value; }
            set 
            {
                CheckPropertyChangeAllowed("Value");
                // Check for positive numbers
                if (value < Minimum)
                    throw new System.ArgumentException("Value provided must be greater than or equal to minimum value", "value");
                if (value > Maximum)
                    throw new System.ArgumentException("Value provided must be less than or equal to the maximum value", "value");
                this.value = value;
                ApplyPropertyChange("Value");
            }
        }

        internal bool HasValidValues
        {
            get { return (minimum <= value && value <= maximum); }
        }
        /// <summary>
        /// Resets the control to its minimum value.
        /// </summary>
        protected internal override void Reset()
        {
            base.Reset();
            value = minimum;
        }
    }
}
