using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ComponentsHub.Models
{
	public class ComponentModel
	{
		public string name { get; set; }
		public string id { get; set; }
	}

	public class ComponentModelCollection
    {
		public List<ComponentModel> components { get; set; }

		public static ComponentModelCollection ParseJSON(string path)
		{
			using StreamReader streamReader = new(path);
			var json = streamReader.ReadToEnd();
			ComponentModelCollection components = JsonConvert.DeserializeObject<ComponentModelCollection>(json);

			return components;
		}
	}

	public class ParsedComponent
	{
		public string PartNumber { get; set; }
        public string Price { get; set; }
        public string Amount { get; set; }

    }
}

