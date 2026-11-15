using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;


namespace Configuration.Configurations
{
    public abstract class AbstractConfiguration<T>
    {
        private readonly string _resourcePath, _subsectionName;


        protected AbstractConfiguration(string jsonName, string subsectionName)
        {
            _resourcePath = "Configurations/" + jsonName;
            _subsectionName = subsectionName;
        }
        
        
        public T Load()
        {
            TextAsset config = Resources.Load<TextAsset>(_resourcePath);
            if (config == null)
                throw new FileNotFoundException($"Config resource not found: {_resourcePath}");

            JToken subsection = JObject.Parse(config.text)[_subsectionName];
            if (subsection == null)
                throw new InvalidDataException($"Config {_resourcePath} has no section '{_subsectionName}'");

            return subsection.ToObject<T>();
        }
    }
}
