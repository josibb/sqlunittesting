using System.Configuration;

namespace Data.Tools.UnitTesting.Configuration
{
    public class ConnectionElementCollection: ConfigurationElementCollection
    {
        public ConnectionElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as ConnectionElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new ConnectionElement this[string key]
        {
            get { return (ConnectionElement)BaseGet(key); }
            set
            {
                if (BaseGet(key) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(key)));
                }
                BaseAdd(value);
            }
        }

        protected override System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new ConnectionElement();
        }

        protected override object GetElementKey(System.Configuration.ConfigurationElement element)
        {
            return ((ConnectionElement)element).Name;
        }
    }


    /*
     * 
     * 
     * <!-- Configuration section settings area. -->
  <pageAppearanceGroup>
    <pageAppearance remoteOnly="true">
      <font name="TimesNewRoman" size="18"/>
      <color background="000000" foreground="FFFFFF"/>
    </pageAppearance>
  </pageAppearanceGroup>

    */
}
