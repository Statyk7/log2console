// Decompiled with JetBrains decompiler
// Type: Log2Console.Settings.FieldType
// Assembly: Log2Console, Version=9.9.9.9, Culture=neutral, PublicKeyToken=null
// MVID: 44D35A25-C349-4FB9-B272-3AC90AA136EE
// Assembly location: C:\Users\rwahl\Desktop\Log2Console.exe

using Log2Console.Log;
using System;
using System.ComponentModel;

namespace Log2Console.Settings
{
    [Serializable]
    public class FieldType
    {
        /// <summary>
        /// Gets or sets the type of field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        [Category("Field Configuration")]
        [DisplayName("Field Type")]
        [Description("The Type of the Field")]
        public LogMessageField Field { get; set; }

        /// <summary>
        /// If the Field is of type Property, specify the name of the Property
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        [Category("Field Configuration")]
        [DisplayName("Property")]
        [Description("The Name of the Property")]
        public string Property { get; set; }

        /// <summary>
        /// The Display / Column name of the Field
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        [Category("Field Configuration")]
        [DisplayName("Name")]
        [Description("The Name of the Column")]
        public string Name { get; set; }

        public FieldType()
        {
        }

        public FieldType(LogMessageField field, string name, string property = null)
        {
            Field = field;
            Name = name;
            Property = property;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}",Name,Property);
        }
    }
}
