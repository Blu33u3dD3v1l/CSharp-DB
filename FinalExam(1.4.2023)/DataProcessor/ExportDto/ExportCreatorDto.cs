using System.Reflection.Metadata.Ecma335;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ExportDto
{
    [XmlType("Creator")]
    public class ExportCreatorDto
    {

        [XmlAttribute("BoardgamesCount")]
        public int BoardgamesCount { get; set; }


        [XmlElement("CreatorName")]
        public string CreatorName { get; set; }

        [XmlArray("Boardgames")]
        public virtual ExportBoardGamesDto[] Boardgames { get; set; }
    }
}

