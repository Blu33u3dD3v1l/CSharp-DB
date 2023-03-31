using Footballers.Data.Models.Enums;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ExportDto
{
    [XmlType("Footballer")]
    public class ExportFootballersDto
    {

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Position")]
        public string PositionType { get; set; }
    }
}
