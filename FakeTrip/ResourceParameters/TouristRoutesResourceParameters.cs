using System.Text.RegularExpressions;

namespace FakeTrip.ResourceParameters;

public class TouristRoutesResourceParameters
{
    public string? Keyword { get; set; }
    public string? RatingOperator { get; set; }
    public int? RatingValue { get; set; }
    private string? rating;
    public string? Rating
    {
        get { return rating; }
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
                Match match = regex.Match(value);
                if (match.Success)
                {
                    RatingOperator = match.Groups[1].Value;
                    RatingValue = Int32.Parse(match.Groups[2].Value);
                }
            }
            rating = value;
        }
    }

}
