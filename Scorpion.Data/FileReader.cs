using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Scorpion.Data
{
    public class FileReader
    {
        public List<Model.Scorpion> GetScorpionFromDirectory(string directory)
        {
            List<Model.Scorpion> scorpions = new List<Model.Scorpion>();
            string[] files = Directory.GetFiles(directory, "*.txt");
            foreach (var file in files)
            {
                var s = TransformDocument(file);
                scorpions.Add(s);
            }

            return scorpions;
        }

        private string GetFileContent(string filePath)
        {
            string fileContent = string.Empty;
            using (StreamReader sr = new StreamReader(filePath))
            {
                fileContent = sr.ReadToEnd();
            }

            return fileContent;
        }

        private Model.Scorpion TransformDocument(string filePath)
        {
            Model.Scorpion scorpion = new Model.Scorpion();
            string fileContent = GetFileContent(filePath);
            string[] sections = fileContent.Split('[', ']');
            FillScorpion(scorpion, sections);


            return scorpion;

        }

        private string[] GetUrl(string urlContent)
        {
            var urls = urlContent.Split('\n', '\r').Where(u => u != string.Empty).ToArray();
            return urls;
        }

        private string TrimNewLine(string line)
        {
            return line.Trim('\r', '\n');
        }

        private void FillScorpion(Model.Scorpion scorpion, string[] sections)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                switch (sections[i])
                {
                    case "TAG":
                        scorpion.TagId = TrimNewLine(sections[i + 1]);
                        break;
                    case "GENERAL_NAME":
                        scorpion.GeneralName = TrimNewLine(sections[i + 1]);
                        break;
                    case "SCIENTIFIC_NAME":
                        scorpion.ScientificName = TrimNewLine(sections[i + 1]);
                        break;
                    case "DESCRIPTION":
                        scorpion.Description = TrimNewLine(sections[i + 1]);
                        break;
                    case "DEADLY":
                        scorpion.Deadly = TrimNewLine(sections[i + 1]);
                        break;
                    case "FIRST_AID":
                        scorpion.FirstAid = TrimNewLine(sections[i + 1]);
                        break;
                    case "URL":
                        scorpion.Url = GetUrl(TrimNewLine(sections[i + 1]));
                        break;
                    case "IMAGE":
                        scorpion.ProfileImage = TrimNewLine(sections[i + 1]);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
