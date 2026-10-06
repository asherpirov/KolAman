using CommandCenter.Models;
namespace CommandCenter.Services
{
    public class ValidatorService
    {
        private const double MinLat = -90;
        private const double MaxLat = 90;
        private const double MinLon = -180;
        private const double MaxLon = 180;

        private readonly List<string> ValidPriorities = new() { "CRITICAL", "HIGH", "MEDIUM", "LOW" };
        private readonly List<string> ValidClassifications = new() { "UNCLASSIFIED", "RESTRICTED", "SECRET", "TOP_SECRET" };
        private readonly List<string> ValidSources = new() { "aman", "mossad", "pikud-haoref", "shabak" };
        private const string ExpectedStatus = "WAITING";

        public  bool Validate(Alert? alert)
        {
            if (alert is null)
            {   
                return false;
            }

            if (string.IsNullOrWhiteSpace(alert.AlertId))
            {
                return false;
            } 
            if (string.IsNullOrWhiteSpace(alert.Source))
            {
                 return false;
            }
            if (string.IsNullOrWhiteSpace(alert.Title))
            {
                return false;
            } 
            if (string.IsNullOrWhiteSpace(alert.Content))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(alert.Priority))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(alert.Classification))
            {
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(alert.Status))
            {
                return false;
            }

            if (alert.Lat < MinLat || alert.Lat > MaxLat)
            {
                return false;
            }
            
            if (alert.Lon < MinLon || alert.Lon > MaxLon)
            {
                return false;
            }

            if (!ValidPriorities.Contains(alert.Priority))
            {
                return false;
            }
            if (!ValidClassifications.Contains(alert.Classification))
            {
                return false;
            }
            if (!ValidSources.Contains(alert.Source))
            {
                return false;
            }

            if (!string.Equals(alert.Status, ExpectedStatus, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}