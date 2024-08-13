using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSpace
{
    public class ResourceLoadingConfiguration
    {
        public string GameDataPath { get; set; }
        public string GameMapsPath { get; set; }
    }
    public class MapResourceFileLoader
    {
        private readonly ResourceLoadingConfiguration _config;

        public MapResourceFileLoader(ResourceLoadingConfiguration config) => _config = config;

        public async Task<IReadOnlyList<MapDataDTO>> LoadAsync()
        {
            string filePath = Path.Combine(_config.GameDataPath, "MapIDData.dat");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} should be present");
            }

            var maps = new List<MapDataDTO>();
            var dictionaryId = new Dictionary<int, string>();
            int i = 0;

            using (var mapIdStream = new StreamReader(filePath, Encoding.GetEncoding(1252)))
            {
                string line;
                while ((line = mapIdStream.ReadLine()) != null)
                {
                    string[] values = line.Split(' ');
                    if (values.Length <= 1)
                    {
                        continue;
                    }
                    if (values[0] == "DATA") continue;
                    if (!int.TryParse(values[0], out int mapId))
                    {
                        continue;
                    }
                    if (!dictionaryId.ContainsKey(mapId))
                    {
                        dictionaryId.Add(mapId, values[4]);
                    }
                }
            }

            try
            {
                foreach (FileInfo file in new DirectoryInfo(_config.GameMapsPath).GetFiles())
                {
                    string name = string.Empty;
                    if (dictionaryId.TryGetValue(int.Parse(file.Name), out string value))
                    {
                        name = value;
                    }
                    byte[] data = File.ReadAllBytes(file.FullName);
                    short width = BitConverter.ToInt16(data, 0);
                    short height = BitConverter.ToInt16(data, 2);
                    maps.Add(new MapDataDTO
                    {
                        Id = short.Parse(file.Name),
                        Name = name,
                        Width = width,
                        Height = height,
                        Grid = data.Skip(4).ToArray()
                    });
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return maps;
        }
    }
}
