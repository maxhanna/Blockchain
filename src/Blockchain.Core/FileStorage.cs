using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Blockchain.Core
{
    public class FileStorage
    {
        private readonly string _path;
        public FileStorage(string path) => _path = path;

        public List<Block> Load()
        {
            if (!File.Exists(_path)) return new List<Block>();
            var json = File.ReadAllText(_path);
            return JsonConvert.DeserializeObject<List<Block>>(json);
        }

        public void Save(List<Block> chain)
        {
            var json = JsonConvert.SerializeObject(chain, Formatting.Indented);
            File.WriteAllText(_path, json);
        }
    }
}