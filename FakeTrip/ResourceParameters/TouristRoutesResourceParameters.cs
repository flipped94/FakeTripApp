using System.Text.RegularExpressions;

namespace FakeTrip.ResourceParameters;

public class TouristRoutesResourceParameters
{
    private int pageNumber = 1;
    public int PageNumber
    {
        get => pageNumber;
        set
        {
            if (value >= 1)
            {
                pageNumber = value;
            }
        }
    }

    private const int maxPageSize = 50;
    private int pageSize = 10;
    public int PageSize
    {
        get => pageSize;
        set
        {
            if (value >= 1)
            {
                pageSize = value > maxPageSize ? maxPageSize : value;
            }

        }
    }

    public string? Keyword { get; set; }

    public string? RatingOperator { get; set; }
    public int? RatingValue { get; set; }

    private string? rating;
    public string? Rating
    {
        get => rating;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Regex regex = new(@"([A-Za-z0-0\-]+)(\d+)");
                Match match = regex.Match(value);
                if (match.Success)
                {
                    RatingOperator = match.Groups[1].Value;
                    RatingValue = int.Parse(match.Groups[2].Value);
                }
            }

            rating = value;
        }
    }

}
