using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AppOpener
{
    class AppsSection : ConfigurationSection
    {
        [ConfigurationProperty("apps", IsRequired = true)]
        public ConfigElementsCollection Apps
        {
            get
            {
                return base["apps"] as ConfigElementsCollection;
            }
        }


    }

    [ConfigurationCollection(typeof(AppToOpen), AddItemName = "app")]
    class ConfigElementsCollection : ConfigurationElementCollection, IEnumerable<AppToOpen>
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new AppToOpen();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var app = element as AppToOpen;
            if (app != null)
                return app.Path;
            else
                return null;
        }

        public AppToOpen this[int index]
        {
            get
            {
                return BaseGet(index) as AppToOpen;
            }
        }

        #region IEnumerable<ConfigElement> Members

        IEnumerator<AppToOpen> IEnumerable<AppToOpen>.GetEnumerator()
        {
            return (from i in Enumerable.Range(0, this.Count)
                    select this[i])
                    .GetEnumerator();
        }

        #endregion
    }

    public class AppOpenerInstanceCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AppToOpen();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AppToOpen)element).Path;
        }

        public new AppToOpen this[string elementName]
        {
            get
            {
                return this.OfType<AppToOpen>().FirstOrDefault(item => item.Path == elementName);
            }
        }
    }

    public class AppToOpen : ConfigurationElement
    {
        //Make sure to set IsKey=true for property exposed as the GetElementKey above
        [ConfigurationProperty("path", IsKey = true, IsRequired = true)]
        public string Path
        {
            get { return (string)base["path"]; }
            set { base["path"] = value; }
        }

        [ConfigurationProperty("parameters", IsRequired = false, DefaultValue = "")]
        public string Parameters
        {
            get { return (string)base["parameters"]; }
            set { base["parameters"] = value; }
        }

        [ConfigurationProperty("desktop", IsRequired = false, DefaultValue = "1")]
        public int Desktop
        {
            get { return (int)base["desktop"]; }
            set { base["desktop"] = value; }
        }
    }
}