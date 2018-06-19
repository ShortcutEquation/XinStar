using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ServiceCenter
{
    public class InitializationSection : ConfigurationSection
    {
        [ConfigurationProperty("Functions")]
        public FunctionsSection FunctionsSetting { get { return (FunctionsSection)base["Functions"]; } }
    }

    [ConfigurationCollection(typeof(ItemSection), AddItemName = "Item")]
    public class FunctionsSection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ItemSection();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ItemSection)element).Name;
        }
    }

    public class ItemSection : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { Name = value; }
        }

        [ConfigurationProperty("Assembly", IsRequired = true)]
        public string Assembly
        {
            get { return (string)this["Assembly"]; }
            set { Assembly = value; }
        }

        [ConfigurationProperty("Class", IsRequired = true)]
        public string Class
        {
            get { return (string)this["Class"]; }
            set { Class = value; }
        }

        [ConfigurationProperty("Method", IsRequired = true)]
        public string Method
        {
            get { return (string)this["Method"]; }
            set { Method = value; }
        }

        [ConfigurationProperty("Param", IsRequired = true)]
        public string Param
        {
            get { return (string)this["Param"]; }
            set { Param = value; }
        }
    }
}
