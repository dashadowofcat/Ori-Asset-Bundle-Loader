using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ConverterManager
{
    public Dictionary<string, ElementConverter> Converters = new Dictionary<string, ElementConverter>();

    public void SetupConverters()
    {
        Converters.Add("Bash", new BashConverter());
        Converters.Add("Leash", new LeashConverter());
        Converters.Add("Spring", new SpringConverter());
    }
}

